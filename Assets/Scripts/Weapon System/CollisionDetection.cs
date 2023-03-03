using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public WeaponController wc;
    private bool isDamaged = false;
    /*    private void OnTriggerEnter(Collider other)
        {
            Debug.Log("hello");
            if (other.gameObject.CompareTag("Enemy") && wc.isAttacking == true)
            {
                Debug.Log(other.gameObject.name);
                //other.gameObject.GetComponent<Enemy>().TakeDamage(50);
            }
        }*/

    private void OnTriggerStay(Collider other)
    {
        // check if object is an enemy and if the player is attacking
        if (other.gameObject.CompareTag("Enemy") && wc.isAttacking == true)
        {
            var weapon = (Equipment)wc.em.currentEquipment[0];
            other.gameObject.GetComponent<Enemy>().TakeDamage((int)(weapon.damageStat * wc.damageMultiplier), wc.element, wc.elementLevel);
        }
    }
}