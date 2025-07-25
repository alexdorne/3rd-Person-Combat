using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField] BoxCollider swordCollider;

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
}
