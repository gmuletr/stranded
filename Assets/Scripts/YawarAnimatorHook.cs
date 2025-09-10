using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YawarAnimatorHook : MonoBehaviour
{
    public GameObject yawar;
    private void YawarDeathParticles()
    {
        yawar.GetComponent<EnemyController>().DeathParticle();
    }

    private void YawarDeathDisappear()
    {
        yawar.GetComponent<EnemyController>().Death();
    }

    private void ActiveYawarAttack()
    {
        yawar.GetComponent<EnemyController>().ActiveYawarAttack();
    }
    
    private void CloseYawarAttack()
    {
        yawar.GetComponent<EnemyController>().CloseYawarAttack();
    }

}
