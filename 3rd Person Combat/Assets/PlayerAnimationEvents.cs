using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField] BoxCollider swordCollider;
    [SerializeField] PlayerMovement playerMovement; // Reference to the PlayerMovement script
    [SerializeField] AttackScript attackScript; // Reference to the Animator component


    private void Start()
    {
        swordCollider.enabled = false;
    }
    public void EnableSwordCollider()
    {
        if (swordCollider != null)
        {
            swordCollider.enabled = true; // Enable the sword collider
        }
    }

    public void DisableSwordCollider()
    {
        if (swordCollider != null)
        {
            swordCollider.enabled = false; // Disable the sword collider
        }
    }

    public void EnableMovement()
    {
        if (playerMovement != null)
        {
            playerMovement.EnableMovement(); // Enable player movement
            playerMovement.EnableBodyRotation();
        }
    }
    public void DisableMovement()
    {
        if (playerMovement != null)
        {
            playerMovement.DisableMovement(); // Disable player movement
            playerMovement.DisableBodyRotation();
        }
    }

    public void AllowAttack()
    {
        attackScript.canAttack = true; // Allow the player to attack
    }

}
