using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHook : MonoBehaviour
{
    public ControlPlayer controller;
    
    public void ActiveAttack()
    {
        controller.ActiveAttack();
    }
    public void CloseAttack()
    {
        controller.CloseAttack();
    }

    /*public void ClosePowerUp()
    {
        controller.ClosePowerUp();
    }

    public void CloseThrow()
    {
        controller.CloseThrow();
    }
    */
    public void PickUp()
    {
        Debug.Log("pickup event now");
        controller.PickUp();
    }

    public void RestoreHealth()
    {
        controller.RestoreHealth();
    }
    public void RyanDie()
    {
        controller.Die();
    }

    public void RyanRespawn()
    {
        controller.RespawnPlayer();
    }
        
    public void DisappearChicken()
    {

        controller.DisappearChicken();
    }
    private void DisableBlackScreen()
    {
        gameObject.SetActive(false);
    }
}
