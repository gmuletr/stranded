using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bonfire : MonoBehaviour
{
    public float maxFireLife = 10f;
    public float currentFireLife;
    public float branchValue = 25f;
    public bool fireIsAlive = true;
    public ParticleSystem fireParticle;
    public Image fireBar;

   
    
    // Start is called before the first frame update
    void Start()
    {
        currentFireLife = maxFireLife;
        fireIsAlive = true;
    }
    // Update is called once per frame
    void Update()
    {
        if(currentFireLife <= 0)
        {
            currentFireLife = 0;
            fireIsAlive = false;
            fireParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
        else
        {
            if (fireParticle.isStopped && !fireIsAlive)
            {
                fireParticle.Play(true);
                fireIsAlive = true;
            }
            DecreaseFire();
        }
        UpdateFireBar();
    }
    void DecreaseFire()
    {
        currentFireLife -= Time.deltaTime;
    }
    public void IncreaseFire()
    {
        if (currentFireLife < maxFireLife)
        {
            currentFireLife += branchValue;
        }
        else if (currentFireLife >= (maxFireLife - branchValue))
        {
            currentFireLife += (maxFireLife - currentFireLife);
        }
    }
    public void MaxFire()
    {
        currentFireLife = maxFireLife;
    }
     void UpdateFireBar()
    {
        fireBar.fillAmount = Mathf.Clamp(currentFireLife / maxFireLife, 0.0f, 1.0f);
    }
}
