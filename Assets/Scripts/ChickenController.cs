using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChickenController : MonoBehaviour
{
    [Header("Cooking")]
    public ParticleSystem fire;
    public bool isCooked = false;
    private float fireTimer;
    private float fireTimeToWait = 0.5f;
    public Material cookedMaterial;

    [Header("Movement")]

    private NavMeshAgent agent;
    //public Animator chAnim;
    public float speed;
    public float fleeSpeed;

    [Header("Patrol")]
    public Transform[] destinies;
    public int index;
    private Vector3 actualDestiny;

    public enum CSTATES { PATROL, WAIT, FLEE, CAUGHT};
    [Header("States")]
    public CSTATES states;
    public bool isWaiting;
    public bool isPatroling;
    public bool isFleeing;
    public bool isCaught;


    [Header("Wait")]
    //WAIT
    public float timeToWait;
    private float timer;

    [Header("FLEE")]
    //FLEE
    public float detectRadius;
    public float fleeRadius;
    public Collider[] detectCollider = new Collider[1];
    public LayerMask detectLayer;
    public float detectAngle = 360;
    private Transform fleeStartTransform;
    public float multiplyBy;
    private Vector3 fleePos;
   

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        actualDestiny = destinies[index].position;
        agent.SetDestination(actualDestiny);
    }

    void CheckDestiny()
    {
        if (!isPatroling)
        {
            isPatroling = true;
            agent.SetDestination(actualDestiny);
            //anim.CrossFade("Walk", 0.25f);
        }

        agent.speed = speed;
        agent.isStopped = false;

        if (Vector3.Distance(transform.position, actualDestiny) < 0.5f)
        {
            index++;
            if (index >= destinies.Length)
            {
                index = 0;
            }
            actualDestiny = destinies[index].position;
            agent.SetDestination(actualDestiny);
            states = CSTATES.WAIT;
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
        if (isFleeing)
        {
            isFleeing = false;
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
            //anim.CrossFade("Idle", 0.25f);
        }
        ResetOtherStates();
        agent.speed = 0;
        agent.isStopped = true;
        timer += Time.deltaTime;
        if (timer >= timeToWait)
        {
            timer = 0;
            states = CSTATES.PATROL;
        }
    }

    public void Flee()
    {

        if (!isFleeing)
        {
            isFleeing = true;
            fleePos = transform.position - detectCollider[0].transform.position;
            fleePos.y = 0;
            fleePos = fleePos.normalized;
            fleePos *= multiplyBy;
            fleePos += transform.position;
            agent.SetDestination(fleePos);
            agent.isStopped = false;
            agent.speed = fleeSpeed;
        }
        else
        {
            if (Vector3.Distance(transform.position, fleePos) < 2.0f)
            {
                states = CSTATES.WAIT;
            }
        }

    }

    void DetectPlayer()
    {
        //MIRO SI HAY UN PLAYER DENTRO DE UN RANGO
        Physics.OverlapSphereNonAlloc(transform.position, detectRadius, detectCollider, detectLayer);
        if (detectCollider[0] != null)
        {
            Vector3 playerDirection = detectCollider[0].transform.position - transform.position;
            playerDirection.y = 1.0f;
            //Debug.DrawRay(transform.position, playerDirection, Color.red, 1);
            if (Physics.Raycast(transform.position, playerDirection.normalized, detectRadius, detectLayer))
            {

                if (Vector3.Angle(transform.forward, playerDirection) < detectAngle)
                {
                   
                    if (Vector3.Distance(transform.position, detectCollider[0].transform.position) <= fleeRadius)
                    {
                       
                        ResetOtherStates();
                        states = CSTATES.FLEE;
                        return;
                    }
                }


            }

        }
        switch (states)
        {
            
            case CSTATES.FLEE:
              
                //states = CSTATES.PATROL;
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
            case CSTATES.PATROL:
                ResetWaitingState();
                CheckDestiny();
                break;
            case CSTATES.FLEE:
                ResetWaitingState();
                Flee();
                break;
            case CSTATES.WAIT:
                Wait();
                break;
            case CSTATES.CAUGHT:
                break;
        }
        

        if (isCooked)
        {
            fireTimer += Time.deltaTime;
            if (fireTimer >= fireTimeToWait)
            {
                timer = 0;
                GetComponentInChildren<MeshRenderer>().material = cookedMaterial;
            }
            
        }
    }

    public void Die()
    {
        fire.transform.rotation = Quaternion.Euler(0, 0, 0);
        fire.Play();
        isCooked = true;
    }

}
