using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private float maxHealth; 
    public float currentHealth;

    private float timeBetweenHits; 

    [SerializeField] private float takeDamageCooldown; 

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (timeBetweenHits < takeDamageCooldown)
        {
            timeBetweenHits += Time.deltaTime;
        }
    }

    private void TakeDamage(float damage)
    {
        currentHealth -= damage;
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
