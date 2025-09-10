using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class ControlPlayer : MonoBehaviour
{
    float v;
    float h;
    private CharacterController playerController;
    public Animator playerAnimator;

    private Vector3 moveVector;
    private float speed;
    private Quaternion newRotation;
    private Vector3 finalMovement;
    public Transform respawnPos;
    public GameObject bonfire;

    [Header("MOVEMENT")]
    //MOVEMENT
    public float walkSpeed;
    public float runSpeed;
    public float turnSpeed = 30;
    private Vector3 startPos;

    [Header("JUMP")]
    //JUMP
    private float gravity = -9.8f;
    private float groundGravity = -0.05f;
    private float initialJumpVelocity;
    private float maxJumpHeight = 4.0f;
    private float maxJumpTime = 0.75f;
    private float timeToMaxHeight;

    public bool grounded;
    public float groundRayDistance = 1.0f;
   public Transform groundChecker;

  
    //private bool isJumping = false;
    private bool jumpPressed = false;
    private bool isFalling = false;
    private float newVelocity;
    private float fallMultiplier = 2.0f;

    [Header("ATTACK")]
    //ATTACK
    
    public bool isPowerUpActive;
    private Rigidbody attackRb;
    public bool isKratosMode; //he lanzado el bate de beisbol
    public float kratosForce = 200;
    public ParticleSystem powerUpFX;
    public float recoveryWeaponSpeed = 20;
    public float minRecoveryDistance = 0.5f;
    public Transform handTr;
    private Vector3 attackHandPos;
    private Vector3 attackHandRot;
    public TrailRenderer hitFX;
    public AudioSource hitSound;

    [Header("PICKUP")]
    //PICKUP 
    public GameObject pickedObject;
    private Rigidbody pickedRB;
    public Transform pickedHook;
    public Collider pickedCollider;

    
    [Header("STATS")]
    //STATS
    private float maxHealth = 100f;
    public float currentHealth;
    private float maxEnergy = 100;
    public float currentEnergy;
    public float lostEnergyPerSec;
    private bool noEnergy = false;
    public int chickenValue;


    [Header("MISC")]
    
    public GameObject blackScreen;
    private bool isOnBonfire = false;

    public GameObject pauseUI;
    public AudioSource pauseAudio;

    public Image healthBar;
    public Image energyBar;
    

    // Start is called before the first frame update
    void Start()
    {
        timeToMaxHeight = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToMaxHeight, 2);
        gravity *= 4.5f;
        initialJumpVelocity = 2 * maxJumpHeight / timeToMaxHeight;
        playerController = GetComponent<CharacterController>();
      
        speed = walkSpeed;
        startPos = transform.position;

        /*pickedCollider.enabled = false;
        attackRb = attackCollider.GetComponent<Rigidbody>();
        attackHandPos = attackRb.gameObject.transform.localPosition;
        attackHandRot = attackRb.gameObject.transform.localEulerAngles;
        */

        currentEnergy = maxEnergy;
        currentHealth = maxHealth;

        blackScreen.SetActive(false);

    }

    /*void Jumping()

    {
        jumpPressed = Input.GetButton("Jump");
        if (!isJumping && grounded && jumpPressed)
        {
            isJumping = true;
            finalMovement.y = initialJumpVelocity;

        }
        else if (isJumping && grounded && !jumpPressed)
        {
            isJumping = false;
        }
        playerAnimator.SetBool("jumping", isJumping);
    }
    */

    // Update is called once per frame
    void Update()
    {
        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");
        grounded = Physics.Raycast(groundChecker.position, Vector3.down, groundRayDistance);
        //Jumping();
        Movement();
        Falling();
        Attack();
        
        UpdateEnergyBar();
        //PowerUp();

        /*if (isKratosMode && isPowerUpActive)
        {
            attackRb.isKinematic = true;
            float distance = Vector3.Distance(attackRb.gameObject.transform.position, handTr.position);
            attackRb.gameObject.transform.position = Vector3.Lerp(attackRb.gameObject.transform.position, handTr.position, recoveryWeaponSpeed * Time.deltaTime / distance);
            if (distance <= minRecoveryDistance)
            {
                isKratosMode = false;
                isPowerUpActive = false;
                attackRb.gameObject.transform.parent = handTr;
                attackRb.gameObject.transform.localPosition = attackHandPos;
                attackRb.gameObject.transform.localEulerAngles = attackHandRot;
                playerAnimator.CrossFade("catch", 0.25f);
            }
        }*/

        if (Input.GetButtonDown("Cancel"))
        {
            if (!pauseUI.activeInHierarchy)
            {
                pauseUI.SetActive(true);
                Time.timeScale = 0;
                pauseAudio.Play();
            }
            else if(pauseUI.activeInHierarchy )
            {
                pauseUI.SetActive(false);
                Time.timeScale = 1;
                pauseAudio.Play();
            }
        }

        if (currentEnergy <= 0)
        {
            currentEnergy = 0;
            speed *= 0.75f;
            noEnergy = true;
        }
        else
        {
            noEnergy = false;
            DecreaseEnergy();
        }
        if (Input.GetButtonDown("PickUp"))
        {
            if (pickedObject != null && pickedRB != null)
            {
                Debug.Log("dropped object");
                DropPickedObject();

            }
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            playerAnimator.SetBool("die", true);

        }
        if(currentEnergy >= maxEnergy)
        {
            currentEnergy = maxEnergy;
        }

        
    }
    void Attack()
    {
        if (Input.GetButtonDown("Attack"))
        {
            if (grounded)
            {
                if (pickedObject.CompareTag("Chicken"))
                {
                    if (pickedObject.GetComponent<ChickenController>().isCooked)
                    {
                        playerAnimator.CrossFade("Ryan Eat", 0.25f);
                    }
                }
                else
                {
                    playerAnimator.SetTrigger("hit");
                    
                    hitFX.enabled = true;
                }
            }
            
        }
    }
    public void ActiveAttack()
    {
        pickedCollider.enabled = true;
        hitSound.Play();
    }
    public void CloseAttack()
    {
        pickedCollider.enabled = false;
        hitFX.enabled = false;


    }
    
    /*void PowerUp()
    {

        if (Input.GetButtonDown("Fire2"))
        {
            //SI NO ESTÁ HACIENDO EL POWERUP
            if (!isPowerUpActive)
            {
                isPowerUpActive = true;
                powerUpFX.Play();

            }
        }
        if (Input.GetButtonUp("Fire2"))
        {
            if (isPowerUpActive)
            {
                isPowerUpActive = false;
                powerUpFX.Stop();

            }
        }
        playerAnimator.SetBool("powerup", isPowerUpActive);
    }

    public void ClosePowerUp()
    {
        if (!isKratosMode)
        {
            playerAnimator.CrossFade("throw", 0.25f);
            isPowerUpActive = false;
            playerAnimator.SetBool("powerup", isPowerUpActive);
        }

    }

    public void CloseThrow()
    {
        attackCollider.enabled = true;
        attackRb.isKinematic = false;
        attackRb.gameObject.transform.parent = null;
        attackRb.AddForce(transform.forward * kratosForce, ForceMode.Impulse);
        attackRb.AddTorque(transform.right * kratosForce * 20);
        isKratosMode = true;
    }
    */
    void Movement()
    {
        if (playerAnimator.GetBool("die") == false)
        {
            if ((Input.GetButton("Run")) && noEnergy == false)
            {
                speed = runSpeed;
            }
            else
            {
                speed = walkSpeed;

            }

            playerAnimator.SetFloat("Speed", speed * moveVector.sqrMagnitude / runSpeed);
            //playerAnimator.SetFloat("Speed", Mathf.Clamp(speed, 0, runSpeed));

            moveVector = new Vector3(h, 0, v) * speed;

            if (moveVector.sqrMagnitude != 0)
            {

                moveVector = Camera.main.transform.right * moveVector.x + Camera.main.transform.forward * moveVector.z;
                newRotation = Quaternion.LookRotation(new Vector3(moveVector.x, 0, moveVector.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, turnSpeed * Time.deltaTime);

            }
            finalMovement = new Vector3(moveVector.x, finalMovement.y, moveVector.z);
            //-(transform.position.z - startPos.z


            playerController.Move(finalMovement * Time.deltaTime);
        }
    }

    void Falling()
    {
        isFalling = finalMovement.y <= 0.0f || !jumpPressed;
        playerAnimator.SetBool("falling", isFalling && !grounded);

        if (grounded)
        {
            finalMovement.y = groundGravity;
        }
        else if (isFalling)
        {
            //finalMovement.y = finalMovement.y + (gravity * Time.deltaTime);
            newVelocity = finalMovement.y + (gravity * fallMultiplier * Time.deltaTime);
            finalMovement.y = Mathf.Max(Mathf.Lerp(finalMovement.y, newVelocity, 0.5f), -20.0f);
        }
        else
        {
            newVelocity = finalMovement.y + (gravity * Time.deltaTime);
            finalMovement.y = Mathf.Lerp(finalMovement.y, newVelocity, 0.5f);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Cinematica"))
        {
            
            LoadScene("WatchEndCinematique");
        }
    }
    private void LoadScene(string sceneToLoad)
    {
        Debug.Log("loadscene initiated");
        Singleton.instance.sceneToLoad = sceneToLoad;
        SceneManager.LoadScene(sceneToLoad);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Branch") || other.CompareTag("Bone") || other.CompareTag("Chicken"))
        {
            if (Input.GetButtonDown("PickUp"))
            {
                if(pickedObject == null)
                {
                    if (other.transform.parent == null || !other.transform.parent.CompareTag("Enemy"))
                    {
                        playerAnimator.CrossFade("Pick Up", 0.15f);
                        pickedObject = other.gameObject;
                        other.gameObject.GetComponent<AnimatorHook>().enabled = true;
                    }
                }
            }
        }

        if (other.CompareTag("Bonfire"))
        {
            isOnBonfire = true;
            if(pickedObject != null && pickedRB != null)
            {
                if (Input.GetButtonDown("PickUp"))
                {
                    if (pickedObject.CompareTag("Branch"))
                    {
                        pickedRB.isKinematic = false;
                        pickedObject.transform.parent = null;
                        Destroy(pickedObject);
                        bonfire.GetComponent<Bonfire>().IncreaseFire();
                        pickedObject = null;
                        pickedRB = null;
                        pickedCollider = null;
                    }
                    else if (pickedObject.CompareTag("Chicken"))
                    {
                        if (bonfire.GetComponent<Bonfire>().fireIsAlive)
                         {
                            pickedObject.GetComponent<ChickenController>().Die();
                            playerAnimator.CrossFade("Ryan Cook", 0.25f);
                        }
                    }
                    
                   
                }
                
                
            }

        }

        if (other.CompareTag("Hut"))
        {
            if (Input.GetButton("Sleep"))
            {
                
                if (bonfire.GetComponent<Bonfire>().fireIsAlive)
                {
                
                    Sleep();
                }
                else
                {
                    Debug.Log("No puedes dormir con la hoguera apagada");
                }
            }
            
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bonfire"))
        {
            isOnBonfire = false;
        }
    }

    public void PickUp()
    {
       if (pickedObject.CompareTag("Chicken"))
        {
            pickedObject.GetComponent<NavMeshAgent>().enabled = false;
        }
        pickedRB = pickedObject.GetComponent<Rigidbody>();
        pickedRB.isKinematic = true;
        pickedCollider = pickedObject.GetComponent<Collider>();
        pickedCollider.enabled = false;
        pickedObject.transform.position = pickedHook.position;
        pickedObject.transform.parent = pickedHook;
        pickedObject.transform.localRotation = Quaternion.Euler(15.6f, 8.9f, 63.7f);
    }

    public void DropPickedObject()
    {
        if (!isOnBonfire)
        {
            if (pickedObject.CompareTag("Chicken") && !pickedObject.GetComponent<ChickenController>().isCooked)
            {
                pickedObject.GetComponent<NavMeshAgent>().enabled = true;
            }
            pickedCollider.enabled = true;
            pickedCollider = null;
            pickedRB.isKinematic = false;
            pickedObject.transform.parent = null;
            pickedObject = null;
            pickedRB = null;
        }
    }

    void DecreaseEnergy()
    {
        currentEnergy -= Time.deltaTime*lostEnergyPerSec;
    }

    public void UpdateEnergyBar()
    {
        energyBar.fillAmount = Mathf.Clamp(currentEnergy / maxEnergy, 0, 1);
        
    }
    public void Sleep()
    {
        blackScreen.SetActive(true);
        blackScreen.GetComponent<Animator>().Play("fade in out");
    }

    public void RestoreHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void IncreaseEnergy()
    {
        currentEnergy += chickenValue;
    }

    public void DisappearChicken()
    {
        pickedRB.isKinematic = false;
        pickedObject.transform.parent = null;
        Destroy(pickedObject);
        IncreaseEnergy();
        pickedObject = null;
        pickedRB = null;

    }
    public void ReceiveDamage(int damageDealt)
    {
        if (playerAnimator.GetBool("die") == false)
        {
            playerAnimator.CrossFade("ReceiveAttack", 0.25f);
            currentHealth -= damageDealt;
            UpdateHealthBar();
        }
    }
    public void UpdateHealthBar()
    {
       
        healthBar.fillAmount = Mathf.Clamp(currentHealth / maxHealth, 0.0f, 1.0f);
        


    }

        public void Die()
    {
        blackScreen.SetActive(true);
        blackScreen.GetComponent<Animator>().Play("death fade in out");
        
    }

    public void RespawnPlayer()
    {
        transform.position = respawnPos.position;
        currentEnergy = maxEnergy / 2;
        currentHealth = maxHealth;
        UpdateHealthBar();
        playerAnimator.SetBool("die", false);
        bonfire.GetComponent<Bonfire>().MaxFire();
    }

}
