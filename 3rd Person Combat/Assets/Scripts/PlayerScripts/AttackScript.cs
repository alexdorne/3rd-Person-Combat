using System.Collections;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    InputSystem_Actions InputActions;

    PlayerMovement playerMovement;


    public bool canAttack = true;

    [SerializeField] Animator animator;

    [SerializeField] int maxComboCount = 3; 

    int currentCombo = 1; 


    [SerializeField] private float attackCooldown;

    [SerializeField] private float comboWindow; 


    private float lastAttackTime;



    // --------------------------------------------------

    bool attackQueued = false; 

    float attackQueueTimer;

    [SerializeField] float attackQueueMaxTime; 



    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        InputActions = new InputSystem_Actions();
        InputActions.Player.Enable();
        InputActions.Player.Attack.performed += ctx => PerformAttack();
    }

    private void Update()
    {
        if (lastAttackTime < comboWindow)
        {
            lastAttackTime += Time.deltaTime;
        }
        else
        {
            currentCombo = 1; 
            animator.SetInteger("ComboInt", currentCombo);

        }

        if (attackQueueTimer < attackQueueMaxTime)
        {
            if (attackQueued)
            {
                attackQueueTimer += Time.deltaTime; 
            }
        }
        else
        {
            attackQueued = false;
            attackQueueTimer = 0;
        }


        if (canAttack && attackQueued)
        {
            PerformAttack();
        }

        animator.SetBool("AttackQueued", attackQueued);

    }

    private void PerformAttack()
    {

        Debug.Log($"Attempting attack, can attack? {canAttack} while attack queued is {attackQueued}"); 
        if (canAttack)
        {
            lastAttackTime = 0; // Reset the last attack time
            canAttack = false; 

            animator.SetTrigger("Attack"); // Trigger the attack animation

            if (currentCombo < 3)
            {
                currentCombo++;
            }
            else
            {
                currentCombo = 1; 
            }

            StartCoroutine(Attack()); 

            //Debug.Log(currentCombo);



        }
        else if (attackQueued == false)
        {
            attackQueued = true; 
            attackQueueTimer = 0; 
        }
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.5f);

        animator.SetInteger("ComboInt", currentCombo);

    }


}
