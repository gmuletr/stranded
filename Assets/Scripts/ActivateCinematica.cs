using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateCinematica : MonoBehaviour
{
    public Collider cinematicaCollider;

    private void Update()
    {
        if(gameObject.GetComponent<EnemyController>().isDead == true)
        {
            cinematicaCollider.isTrigger = true; 
        }
    }

}
