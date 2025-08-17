using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField] BoxCollider swordCollider;
    [SerializeField] PlayerMovement playerMovement; // Reference to the PlayerMovement script
    [SerializeField] AttackScript attackScript; // Reference to the Animator component

    [SerializeField] AudioSource audioSource;

    [SerializeField] AudioClip swordSwingSound; 

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

    public void DisallowAttack()
    {
        attackScript.canAttack = false; // Disallow the player from attacking
    }

    public void PlaySwordSwingSound()
    {
        if (audioSource != null && swordSwingSound != null)
        {
            audioSource.pitch = Random.Range(0.8f, 1.2f); // Randomize pitch for variety
            audioSource.PlayOneShot(swordSwingSound); // Play the sword swing sound
        }

    }
}

