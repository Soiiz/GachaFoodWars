using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBossAI : Enemy
{
    [Header("Stats")]
    public float attackRate = 1.0f; // attacks per second
    public float attackRange = 10.0f; // distance from player to attack
    public float sightRange = 20.0f; // distance from player to spot player
    public float chargeForce = 100.0f; // force to charge at player
    public float damageTimeout = 1f; // time between damage ticks
    private float attackcooldown = 0.0f;
    private float damageCooldown = 0.0f;
    // Start is called before the first frame update
    protected override void Start()
    {
        // run parent script start function
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        // check if player is in line of sight
        var rayDirection = player.transform.position - transform.position;
        if (Physics.Raycast(transform.position, rayDirection, out var hit, sightRange))
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                if (rayDirection.magnitude <= attackRange && attackcooldown <= 0.0f)
                {
                    agent.ResetPath();
                    rb.AddForce(rayDirection.normalized * chargeForce, ForceMode.Impulse);
                    attackcooldown = 1 / attackRate;
                }
                else if (agent.isActiveAndEnabled && attackcooldown <= 0.0f)
                {
                    // chase player if too far away
                    agent.SetDestination(player.transform.position - rayDirection.normalized * attackRange * .9f);
                }


            }
        }

        // decrement attack cooldown
        if (attackcooldown > 0.0f)
        {
            attackcooldown -= Time.deltaTime;
        }

        // decrement damage cooldown
        if (damageCooldown > 0.0f)
        {
            damageCooldown -= Time.deltaTime;
        }

    }

    void OnDrawGizmosSelected()
    {
        // draw wireframe for attack range
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // draw wireframe for sight range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (damageCooldown <= 0.0f)
            {
                Debug.Log("Player hit by boss");
                collision.gameObject.GetComponent<Player>().takeDamage(damage);
                damageCooldown = damageTimeout;
            }
        }
    }
}