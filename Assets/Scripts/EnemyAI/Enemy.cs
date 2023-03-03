using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public GameObject drop;

    public GameObject player;
    public AudioClip[] deathSounds;
    [Header("Stats")]
    public float maxHealth = 100.0f;
    public float health = 100.0f;
    public float baseSpeed = 8.0f;
    private float Speed = 8.0f;
    public float speed
    {
        get { return Speed; }
        set
        {
            Speed = value;
            if (agent != null)
                agent.speed = Speed;
        }
    }
    public float attackRate = 1.0f; // attacks per second
    public float attackRange = 10.0f; // distance from player to attack
    public float sightRange = 20.0f; // distance from player to spot player
    public int damage = 1;
    public int dropValue = 1;
    public float invunerableTime = .5f;

    [Header("Element")]
    public Element element = Element.None;
    public float elementDuration = 3.0f;
    public float stunDuration = .5f;
    public float fireDamageMultiplier = 1.0f;
    public float iceSlowMultiplier = .1f;
    public float maxIceSlow = .9f;
    public int elementLevel = 0;

    // protected variables
    protected float attackcooldown = 0.0f;
    protected float invunerableTimer = 0.0f;
    protected float elementTimer = 0.0f;


    public enum Element { None, Fire, Ice, Electric };

    protected Slider slider;
    protected NavMeshAgent agent;
    protected Rigidbody rb;
    protected Animator spriteAnimator;
    protected SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("No NavMeshAgent on " + gameObject.name);
        }
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody on " + gameObject.name);
        }
        spriteAnimator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        slider = GetComponentInChildren<Slider>();
        if (slider == null)
        {
            Debug.LogError("No Slider on " + gameObject.name);
        }
        slider.value = healthBarUpdate();
        health = maxHealth;
        speed = baseSpeed;

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //updates slider value
        slider.value = healthBarUpdate();

        // update invunerable timer
        if (invunerableTimer > 0.0f)
        {
            invunerableTimer -= Time.deltaTime;
            if (invunerableTimer <= 0.0f)
            {
                spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            }
        }

        // walk animation
        if (agent.velocity.magnitude > .5 && spriteAnimator != null)
        {
            // start walk animation
            spriteAnimator.SetFloat("WalkSpeed", speed);
        }
        else if (spriteAnimator != null)
        {
            // stop walk animation
            spriteAnimator.SetFloat("WalkSpeed", 0);
        }

        // apply element effects
        ElementalDamage();

        // death check
        Death();
    }

    //updates healthbar based on health/maxhealth
    float healthBarUpdate()
    {
        return health / maxHealth;
    }

    public void Death()
    {
        if (health <= 0)
        {
            // drop loot
            if (dropValue > 0)
            {
                var dropInstance = Instantiate(drop, transform.position, Quaternion.identity);
                dropInstance.GetComponent<Drop>().value = dropValue;
                dropInstance.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-1f, 1f), 2, Random.Range(-1f, 1f));
            }
            // play random death sound
            if (deathSounds.Length > 0)
            {
                var deathSound = deathSounds[Random.Range(0, deathSounds.Length)];
                AudioSource.PlayClipAtPoint(deathSound, transform.position);
            }
            //play death animation
            if (spriteAnimator != null)
            {
                spriteAnimator.SetBool("Dead", true);
            }
            // destroy enemy
            agent.enabled = false;
            Destroy(gameObject, 1.0f);
            this.enabled = false;
        }
    }

    public void TakeDamage(int damage, Element e = Element.None, int el = 0)
    {
        if (invunerableTimer <= 0.0f)
        {
            invunerableTimer = invunerableTime;
            health -= damage;
            spriteRenderer.color = new Color(1f, 1f, 1f, .5f);
        }

        // apply elemental damage
        // only apply if the enemy currenty has no element or the new element is the same as the current element
        if (e != Element.None && (element == Element.None || element == e))
        {
            element = e;
            elementLevel = el;
        }
    }

    private void ElementalDamage()
    {
        // set element timer if theres an element
        if (element != Element.None && elementTimer <= 0.0f)
        {
            elementTimer = elementDuration;
            if (element == Element.Electric)
            {
                elementTimer = stunDuration * elementLevel;
            }
        }

        // update element timer
        if (elementTimer > 0.0f)
        {
            elementTimer -= Time.deltaTime;
            if (elementTimer < 0.0f)
            {
                element = Element.None;
                elementLevel = 0;
                speed = baseSpeed;
                agent.enabled = true;
            }
        }

        // apply elemental effect
        if (element == Element.Fire)
        {
            // deal level damage * multiplier every second
            if (elementTimer % 1.0f < Time.deltaTime)
            {
                health -= elementLevel * fireDamageMultiplier;
            }
        }
        else if (element == Element.Ice)
        {
            // slow enemy down by iceSlowMultiplier per level (maxIceSlow)
            speed = baseSpeed * (1.0f - Mathf.Min(elementLevel * iceSlowMultiplier, maxIceSlow));
        }
        else if (element == Element.Electric)
        {
            // stun enemy
            agent.enabled = false;
        }

        // apply elemental effect
        // temp sprite color change
        if (element == Element.Fire)
        {
            // redish orange tint
            spriteRenderer.color = new Color(1f, 0.5f, 0f, 1f);
        }
        else if (element == Element.Ice)
        {
            // cold blue tint
            spriteRenderer.color = new Color(0f, 0.5f, 1f, 1f);
        }
        else if (element == Element.Electric)
        {
            // lightning yellow tint
            spriteRenderer.color = new Color(1f, 1f, 0f, 1f);
        }
        else
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        }
    }

    // update agent speed when speed is changed in the inspector
    private void OnValidate()
    {
        if (agent != null)
        {
            speed = baseSpeed;
            agent.speed = speed;
        }
    }
}
