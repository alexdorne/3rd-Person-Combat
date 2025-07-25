using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private float maxHealth; 
    public float currentHealth;

    private float timeBetweenHits; 

    [SerializeField] private float takeDamageCooldown; 

    NavMeshAgent navMeshAgent;

    [SerializeField] Animator animator; 

    [SerializeField] GameObject player;

    [SerializeField] BoxCollider attackCollider; 

    [SerializeField] private float attackRange; 

    [SerializeField] private float attackDamage;

    [SerializeField] private float attackCooldown;

    [SerializeField] private float walkSpeed; 

    private float lastAttackTime;

    public enum EnemyState
    {
        Seeking,
        Attack,
        Dead
    }

    private EnemyState currentState;

    private void Start()
    {
        currentHealth = maxHealth;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.stoppingDistance = attackRange; // Set the stopping distance to the attack range
    }

    private void Update()
    {
        if (timeBetweenHits < takeDamageCooldown)
        {
            timeBetweenHits += Time.deltaTime;
        }

        switch (currentState)
        {
            case EnemyState.Seeking:
                animator.SetFloat("Speed", navMeshAgent.velocity.magnitude); 
                navMeshAgent.SetDestination(player.transform.position);
                navMeshAgent.isStopped = false; 
                navMeshAgent.speed = walkSpeed; 
                if (IsInAttackRange()) 
                {
                    SwitchState(EnemyState.Attack);
                    navMeshAgent.isStopped = true; // Stop the enemy from moving when in attack range
                    navMeshAgent.speed = 0f; // Set speed to 0 while attacking
                    lastAttackTime = 0f; // Reset the attack cooldown timer
                    animator.SetTrigger("Attack");
                }
                break; 
            case EnemyState.Attack:
                lastAttackTime += Time.deltaTime;
                if (lastAttackTime >= attackCooldown)
                {
                    if (IsInAttackRange())
                    {
                        // Perform attack logic
                        navMeshAgent.isStopped = true; // Stop the enemy from moving while attacking
                        navMeshAgent.speed = 0f; // Set speed to 0 while attacking
                        animator.SetTrigger("Attack"); // Trigger the attack animation
                        lastAttackTime = 0f; // Reset the attack cooldown timer
                    }
                    else
                    {
                        SwitchState(EnemyState.Seeking); // Switch back to seeking if out of range
                    }
                }
                break;
            case EnemyState.Dead:
                
                break;


        }
    }

    private void SwitchState(EnemyState state)
    {
        currentState = state;
    }

    private void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            SwitchState(EnemyState.Dead);
            animator.SetTrigger("Dead"); // Trigger the death animation
            navMeshAgent.isStopped = true; // Stop the enemy from moving
        }
    }

    private bool IsInAttackRange()
    {
        return Vector3.Distance(transform.position, player.transform.position) <= attackRange;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon"))
        {
            if (timeBetweenHits >= takeDamageCooldown)
            {
                timeBetweenHits = 0f; // Reset the cooldown timer
                TakeDamage(1); // Assuming the weapon deals 1 damage
            }
        }
    }

    
}
