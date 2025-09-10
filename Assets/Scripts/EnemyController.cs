using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    
    private NavMeshAgent agent;
    public Animator anim;
    public float speed;
    public float seekSpeed;

    [Header("Patrol")]
    public Transform[] destinies;
    public int index;
    private Vector3 actualDestiny;
   
    public enum ESTATES { PATROL, WAIT, ATTACK, SEEK, DAMAGE, DEAD };
    [Header("States")]
    public ESTATES states;
    public bool isAttacking;
    public bool isSeeking;
    public bool isWaiting;
    public bool isPatroling;
    public bool isDead;
   


    [Header("Wait")]
    //WAIT
    public float timeToWait;
    private float timer;
    [Header("SEEK")]
    //SEEK
    public float detectRadius;
    public Collider[] detectCollider = new Collider[1];
    public LayerMask detectLayer;
    public float detectAngle = 30; //la mitad de mi cono de vision
    [Header("ATTACK")]
    //ATTACK
    public float attackRadius;
    public float attackTime;
    public float damageTime;

    [Header("STATS")]
    //STATS
    public float maxHealth = 100;
    public float currentHealth;
    public Image healthBar;

    [Header("WEAPON")]
    public GameObject yawarWeapon;
    private Collider weaponCollider;
    private Rigidbody weaponRB;
    public ParticleSystem deathParticle;
    

    // Start is called before the first frame update
    void Start()
    {
        
        agent = GetComponent<NavMeshAgent>();
        actualDestiny = destinies[index].position;
        agent.SetDestination(actualDestiny);
        currentHealth = maxHealth;
        weaponRB = yawarWeapon.GetComponent<Rigidbody>();
        weaponRB.isKinematic = true;
        weaponCollider = yawarWeapon.GetComponent<Collider>();
        weaponCollider.enabled = false;
    }

    void CheckDestiny()
    {
        if (!isPatroling)
        {
            isPatroling = true;
            agent.SetDestination(actualDestiny);
            anim.CrossFade("Walk", 0.1f);
        }
        
        agent.speed = speed;
        agent.isStopped = false;

        if (Vector3.Distance(transform.position, actualDestiny) < 0.5f)
        {
            index++;
            if(index>= destinies.Length)
            {
                index = 0;
            }
            actualDestiny = destinies[index].position;
            agent.SetDestination(actualDestiny);
            states = ESTATES.WAIT;
        }
    }
    void ResetWaitingState()
    {
        if (isWaiting)
        {
            isWaiting = false;
        }
    }
    void ResetOtherStates()
    {
        if (isAttacking)
        {
            isAttacking = false;
        }
        if (isSeeking)
        {
            isSeeking = false;
        }
        if (isPatroling)
        {
            isPatroling = false;
        }
       
    }

    void Wait()
    {
        if (!isWaiting)
        {
            isWaiting = true;
            anim.CrossFade("Idle", 0.4f);
        }
        
        if (isAttacking)
        {
            StopCoroutine("AttackAndWait");
           
        }
        
        agent.speed = 0;
        agent.isStopped = true;
        agent.SetDestination(transform.position);
        timer += Time.deltaTime;
        if(timer >= timeToWait)
        {
            timer = 0;
            states = ESTATES.PATROL;
        }
        ResetOtherStates();
    }

    void SeekPlayer()
    {
        
        if (!isSeeking)
        {
            isSeeking = true;
            anim.Play("Run");
        }

        if (detectCollider[0] == null) //HE PERDIDO LA REFERENCIA AL PLAYER
        {
            states = ESTATES.WAIT;
            return; 
        }
        agent.isStopped = false;
        agent.speed = seekSpeed;
        agent.SetDestination(detectCollider[0].transform.position);
       
   }

    void Attack()
    {
        if (!isAttacking)
        {
            anim.CrossFade("Attack", 0.25f);
            isAttacking = true;
            StartCoroutine("AttackAndWait");
        }
    }

    IEnumerator AttackAndWait()
    {
        anim.Play("Attack");
        yield return new WaitForSeconds(attackTime);
        StartCoroutine("AttackAndWait");
    }

    void DetectPlayer()
    {
        if (states == ESTATES.DEAD)
        {
            return;
        }
        
        //MIRO SI HAY UN PLAYER DENTRO DE UN RANGO
        Physics.OverlapSphereNonAlloc(transform.position, detectRadius, detectCollider, detectLayer) ;
        if(detectCollider[0] != null)
        {
            Vector3 playerDirection = detectCollider[0].transform.position - transform.position;
            playerDirection.y = 1.0f;
            //Debug.DrawRay(transform.position, playerDirection, Color.red, 1);
            if (Physics.Raycast(transform.position, playerDirection.normalized, detectRadius, detectLayer))
            {
                
                if (Vector3.Angle(transform.forward, playerDirection) < detectAngle)
                {
                    
                    if (Vector3.Distance(transform.position, detectCollider[0].transform.position) <= attackRadius)
                    {
                        agent.isStopped = true;
                        agent.speed = 0;
                        agent.velocity = Vector3.zero;
                        states = ESTATES.ATTACK;
                    }
                    else
                    {
                        states = ESTATES.SEEK;
                        ResetOtherStates();
                        StopAllCoroutines();
                    }
                    return;
                }
                
            }
            
        }
        
        switch (states)
            {
                case ESTATES.SEEK:
                case ESTATES.ATTACK:
                    states = ESTATES.WAIT;
              
                    break;
                default:
                    break;
         }
        
    }

    // Update is called once per frame
    void Update()
    {

        DetectPlayer();
        switch (states)
        {
            case ESTATES.PATROL:
                ResetWaitingState();
                CheckDestiny();
                break;
            case ESTATES.ATTACK:
                ResetWaitingState();
                Attack();
                break;
            case ESTATES.SEEK:
                ResetWaitingState();
                SeekPlayer();
                break;
            case ESTATES.WAIT:
                 Wait();
                break;
        }
        
        if(currentHealth <= 0 && states != ESTATES.DEAD)
        {
            states = ESTATES.DEAD;
            isDead = true;
            ResetOtherStates();
            agent.isStopped = true;
            agent.speed = 0;
            anim.Play("Death");

            weaponCollider.gameObject.transform.parent = null;
            weaponCollider.isTrigger = false;
            weaponCollider.enabled = true;
            weaponRB.useGravity = true;
            weaponRB.isKinematic = false;
            StopAllCoroutines();
        
            //this.enabled = false;
        }
        
    }

    public void ReceiveDamage(int damageDealt)
    {
        anim.CrossFade("ReceiveAttack", 0.25f);
        currentHealth -= damageDealt;
        UpdateEnergyBar();
        states = ESTATES.DAMAGE;
        StartCoroutine("DamageAndWait");
    }

    IEnumerator DamageAndWait()
    {
        yield return new WaitForSeconds(damageTime);
        states = ESTATES.WAIT;
    }

    public void UpdateEnergyBar()
    {
        healthBar.fillAmount = Mathf.Clamp(currentHealth / maxHealth, 0, 1);

    }
    public void ActiveYawarAttack()
    {
        yawarWeapon.GetComponent<Collider>().enabled = true;
        //hitFX.enabled = false;
    }
    public void CloseYawarAttack()
    {
        yawarWeapon.GetComponent<Collider>().enabled = false;
        //hitFX.enabled = false;
    }

    public void DeathParticle()
    {
        Debug.Log("play particles here");
        deathParticle.Play();
    }
    public void Death()
    {
        Destroy(gameObject);
    }
}
