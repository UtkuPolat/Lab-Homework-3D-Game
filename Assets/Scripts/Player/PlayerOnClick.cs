using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnClick : MonoBehaviour
{
    public float speed = 5f;                                        //Default speed for Player.
    public float turnSpeed = 10f;                                   //Default turn speed for Player. It relevant to mouse clicks.
    public float attackRange = 2f;                                  //Default attack range for Enemy and Player.

    private Animator anim;                                          //Player animations referance.
    private CharacterController controller;                         //Animator Controller referance.
    private CollisionFlags collisionFlag = CollisionFlags.None;     //Between Enemy and Player collisions for taking damage.
    private Vector3 playerMove = Vector3.zero;                      //It uses for Player movements points coordinates when clicking somewhere.
    private Vector3 targetMovePoint = Vector3.zero;                 //When clicking enemy, it take the coordinates.
    private float currentSpeed;                                     //It uses for speed changes.
    private float playerToPointDistance;                            //It hold the differences of clicked point and Player position coordinates.
    private float gravity = 9.8f;                                   
    private float height;                                           //It uses for gravity. If player move the higher points.
    private bool canMove;                                           
    private bool canAttackMove;
    private bool finishedMovement = true;
    private Vector3 newMovePoint;               
    private Vector3 newAttackPoint;
    private Vector3 targetAttackPoint = Vector3.zero;
    private GameObject enemy;
/////////////////////////////////////////////
    private float boostSpeed = 15f;
    private float boostTime;
    private bool isBoost = false;

////////////////////////////////////////////

    private void Awake() 
    {
        anim = GetComponent<Animator>(); 
        controller = GetComponent<CharacterController>();
        currentSpeed = speed;
        boostTime = 0;
    }


    void Update()
    {
        CalculateHeight();

        CheckIfFinishedMovement();

        AttackMove();

        if (isBoost) {                  // For the Speed boost time 

            boostTime += Time.deltaTime;

            if (boostTime >= 3) 
            {
                currentSpeed = speed;
                boostTime = 0;
                anim.SetBool("SpeedBoost",false);
                isBoost = false;
            }
        }
    }

    bool isGrounded() 
    {
        return collisionFlag == CollisionFlags.CollidedBelow ? true : false;  //For the Player movement and gravity.
    }

    void AttackMove() 
    {
        if (canAttackMove) 
        {
            targetAttackPoint = enemy.gameObject.transform.position; // If click on the Enemy get the target enemy position
            newAttackPoint = new Vector3(targetAttackPoint.x, transform.position.y, targetAttackPoint.z);
        }

        if(!anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Punching")) // For the while punching the Enemy, Player can turn the enemy slowly.
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(newAttackPoint - transform.position),turnSpeed * 2 * (Time.deltaTime));
        }
    }

    void CalculateHeight() 
    {
        if (isGrounded())
        {
            height = 0f;
        }
        else 
        {
            height -= gravity * Time.deltaTime;
        }
    }

    void CheckIfFinishedMovement() 
    {
        if(!finishedMovement)
        {
            if(!anim.IsInTransition(0) && !anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f) 
            {
                finishedMovement = true;  //If there are not any mouse click event, the player change its animation to the Idle Animation.
            }
        }
        else 
        {
            MovePlayer();
            playerMove.y = height * Time.deltaTime;      // For else, there is a mouse click or target position.
            collisionFlag = controller.Move(playerMove);
        }
    }

    void MovePlayer() 
    {
        if(Input.GetMouseButtonDown(1)) 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //Take the coordinate between camera and clicked position.
            RaycastHit hit;                                              //It uses for using took coordinates on the up comment. 

            if (Physics.Raycast(ray, out hit)) 
            {
                playerToPointDistance = Vector3.Distance(transform.position, hit.point); //It takes the difference of Player poisition and Clicked Point.

                if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))    //If the clicked object is the ground which is the tag of plane, clicked event is acceptable.
                {
                    if(playerToPointDistance >= 1.0f)                                   //If the clicked 
                    {
                        canMove = true;
                        canAttackMove = false;
                        targetMovePoint = hit.point;
                    }
                }
                else if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Target"))
                {    
                    enemy= hit.collider.gameObject.GetComponentInParent<EnemyWaypointTracker>().gameObject;  //For the Skeleton Enemy propertie.
                    canMove = true;
                    canAttackMove = true;                                                                    //The both of them is true because if different scenerios, must enter below statement.  
                }
            }
        }
        if(canMove) 
        {
            anim.SetFloat("Speed", 1f); //Turn the Walk Animation

            if (!canAttackMove) // Take the coordinates of enemy becuse its coordinate will be change.
            { 
                newMovePoint = new Vector3(targetMovePoint.x, transform.position.y, targetMovePoint.z);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(newMovePoint - transform.position),turnSpeed * (Time.deltaTime));
            }
            else 
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(newAttackPoint - transform.position),turnSpeed * (Time.deltaTime));
            }

            playerMove = transform.forward * currentSpeed * Time.deltaTime;

            if(Vector3.Distance(transform.position, newMovePoint) <= 0.6f && !canAttackMove) 
            {
                canMove = false;
                canAttackMove = false;
            }
            else if (canAttackMove) 
            {
                if (Vector3.Distance(transform.position, newAttackPoint) <= attackRange) // For the Player attack to enemy.
                {
                    playerMove.Set(0f,0f,0f);
                    anim.SetFloat("Speed", 0f);         //Turn the ıdle position
                    targetAttackPoint = Vector3.zero;   //Reach target attack point
                    anim.SetTrigger("AttackMove");      // For the punch Animation
                    canAttackMove = false; 
                    canMove = false;
                }
            }
        }
        else 
        {
            playerMove.Set(0f,0f,0f);
            anim.SetFloat("Speed", 0f); // Turn the Idle animation
        }
    }
    
    void OnTriggerEnter(Collider other)             //This Trigger Method for the speed boost object.
    {
        if (other.gameObject.tag == "SpeedBoost") 
        {
            Destroy(other.gameObject);
            currentSpeed = boostSpeed;
            isBoost = true;
            anim.SetBool("SpeedBoost",true);
        }
    }
}
