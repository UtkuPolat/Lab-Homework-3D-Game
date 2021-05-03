using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyWaypointTracker : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] walkPoints;



    [Header("Movement Settings")]
    public float turnSpeed = 5f; 
    public float patrolTime = 10f;
    public float walkDistance = 8f;


    [Header("Attack Distance")]
    public float attackDistance = 1.4f;
    public float attackRate = 1f;



    private Transform playerTarget;
    private Animator animator;
    private NavMeshAgent agent;

    private float currentAttackTime;
    private Vector3 nextDestination;
    private int index; 
    EnemyHealth enemyHealth;

    private void Awake() {

        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();
        agent = GetComponent<NavMeshAgent>();
        index = Random.Range(0, walkPoints.Length);
        if (walkPoints.Length > 0) {
            InvokeRepeating("Patrol", Random.Range(0, patrolTime), patrolTime); //Random.Range(0, patrolTime) saniye bekler ve çalışır. 10 sn de bi patrol bölgesi belirler.
        }
    }

    private void Start() {
        agent.avoidancePriority = Random.Range(1,51); //Prevent the coollision of the enemies.
    }

    void Update()
    {
        if(enemyHealth.currentHealth > 0) {
            MoveAndAttack();
        }
        else {
            animator.ResetTrigger("Hit");  // When  enemy die, hit animation do not work. 
            animator.SetBool("Death", true);
            agent.enabled = false;         
        }
        
        
    }

    void MoveAndAttack() {

        float distance = Vector3.Distance(transform.position, playerTarget.position);

        if (distance > walkDistance) {

            if(agent.remainingDistance >= agent.stoppingDistance) {

                agent.isStopped = false;
                agent.speed = 2f;
                animator.SetBool("Walk", true);

                nextDestination = walkPoints[index].position;
                agent.SetDestination(nextDestination); // Enemy walkpoints calcuşation if the enemy hit any obstacle.
            }
            else {
                agent.isStopped = true;
                agent.speed = 0f;
                animator.SetBool("Walk", false);

                nextDestination = walkPoints[index].position;
                agent.SetDestination(nextDestination);
            }
        }
        else {
            if(distance > attackDistance + 0.15f && playerTarget.GetComponent<PlayerHealth>().currentHealth > 0) {  // Because, Enemy follow slowly.

                if (!animator.IsInTransition(0) && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")){      //When enemy attack at the same time it does not walk.
                    animator.ResetTrigger("Attack");                               
                    agent.isStopped = false;
                    agent.speed = 3f;
                    animator.SetBool("Walk", true);
                    agent.SetDestination(playerTarget.position);                                                    // Enemy follows the Player if the player seem.
                } 

            }
            else if (distance <= attackDistance && playerTarget.GetComponent<PlayerHealth>().currentHealth > 0) {
                agent.isStopped = true;
                animator.SetBool("Walk", false);
                agent.speed = 0;

                Vector3 targetPosition = new Vector3(playerTarget.position.x,
                    transform.position.y, playerTarget.position.z);
                
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetPosition - transform.position),
                    turnSpeed * Time.deltaTime);
                
                if (currentAttackTime >= attackRate) {
                    animator.SetTrigger("Attack");
                    currentAttackTime = 0;
                }
                else {
                    currentAttackTime += Time.deltaTime;
                }
            }
        }
    }
    void Patrol() {
        index = index == walkPoints.Length - 1 ? 0 : index + 1;                                                     // For New walkpoints
    }
}
