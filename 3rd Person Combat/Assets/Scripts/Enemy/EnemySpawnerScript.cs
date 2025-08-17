using UnityEngine;

public class EnemySpawnerScript : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab; // Prefab of the enemy to spawn

    [SerializeField] private float spawnInterval = 5f; // Time interval between spawns

    float timer;

    private void Start()
    {
        SpawnEnemy(); // Spawn the first enemy immediately
    }
    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f; // Reset the timer after spawning
        }
    }

    private void SpawnEnemy()
    {
        // Instantiate the enemy at the spawner's position and rotation
        GameObject enemy = Instantiate(enemyPrefab, transform.position, transform.rotation);
        // Optionally, you can set the parent of the spawned enemy to this spawner
        enemy.transform.SetParent(transform);
    }
}
