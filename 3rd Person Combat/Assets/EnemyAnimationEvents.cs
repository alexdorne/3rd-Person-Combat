using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    [SerializeField] private BoxCollider attackCollider; // Collider for detecting hits during the attack

    private void Start()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = false; // Ensure the attack collider is disabled at the start
        }
    }

    public void SetAttackColliderActive()
    {
        attackCollider.enabled = true; // Enable the attack collider to detect hits
    }

    public void SetAttackColliderInactive()
    {
        attackCollider.enabled = false; // Disable the attack collider after the attack
    }
}
