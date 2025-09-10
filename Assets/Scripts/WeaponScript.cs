using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public int damageToDeal = 25;
    public void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Enemy") && transform.root.CompareTag("Player"))
        {

            other.gameObject.GetComponent<EnemyController>().ReceiveDamage(damageToDeal);
            Debug.Log("has hit enemy");
        }

        if (other.gameObject.CompareTag("Player") && transform.root.CompareTag("Enemy"))
        {
            if (other.isTrigger)
            {
                other.gameObject.GetComponent<ControlPlayer>().ReceiveDamage(damageToDeal);
                Debug.Log("has hit player");
            }
        }
    }
}
