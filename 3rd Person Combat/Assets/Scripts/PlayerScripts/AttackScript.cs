using System.Collections;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    InputSystem_Actions InputActions;

    PlayerMovement playerMovement;

    [SerializeField] BoxCollider swordCollider; 

    private bool canAttack;

    [SerializeField] Animator animator; 




    [SerializeField] private float attackCooldown;

    [SerializeField] private float maxComboInterval; 

    private float lastAttackTime;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        InputActions = new InputSystem_Actions();
        InputActions.Player.Enable();
        InputActions.Player.Attack.performed += ctx => PerformAttack();
        swordCollider.enabled = false; // Ensure the sword collider is initially disabled
    }

    private void Update()
    {
        if (lastAttackTime < maxComboInterval)
        {
            lastAttackTime += Time.deltaTime;
        }
    }

    private void PerformAttack()
    {
        if (lastAttackTime >= attackCooldown)
        {
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        playerMovement.DisableMovement(); 
        animator.SetTrigger("Attack"); // Trigger the attack animation
        
        lastAttackTime = 0; // Reset the last attack time
        swordCollider.enabled = true; // Enable the sword collider to detect hits

        //AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        //float animationLength = stateInfo.length; // Get the length of the current animation

        yield return new WaitForSeconds(1);

        swordCollider.enabled = false; // Disable the sword collider after the attack animation duration

        yield return new WaitForSeconds(0.5f);

        playerMovement.EnableMovement(); // Re-enable movement after the attack animation

    }

}
