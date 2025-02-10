using UnityEngine;
using System.Collections;

public class ZombieSpawner : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public GameObject zombiePrefab; // The prefab of the zombie to spawn
    public float spawnDistance = 10.0f; // The distance from the player to spawn zombies
    public float spawnHeight = 2.0f; // The desired Y-coordinate where zombies should be spawned
    public LayerMask terrainLayer; // Layer mask for the terrain or ground
    public float spawnInterval = 5.0f; // Initial spawn interval
    public float spawnRateIncrease = 0.1f; // Rate of spawn interval decrease per spawn

    private float timePassed = 0.0f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(SpawnZombiesAfterDelay(10.0f));
    }

    private IEnumerator SpawnZombiesAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        while (true)
        {
            if (player != null)
            {
                // Calculate a random offset within spawnDistance
                Vector2 randomOffset = Random.insideUnitCircle * spawnDistance;
                Vector3 spawnPosition = player.position + new Vector3(randomOffset.x, 0, randomOffset.y);

                // Check the terrain height at the spawn position
                if (Physics.Raycast(spawnPosition + Vector3.up * 1000, Vector3.down, out RaycastHit hit, Mathf.Infinity, terrainLayer))
                {
                    // Set the Y-coordinate to the terrain height plus spawnHeight
                    spawnPosition.y = hit.point.y + spawnHeight;

                    // Check if there are no obstacles between the player and the spawn position
                    Vector3 direction = spawnPosition - player.position;
                    if (!Physics.Raycast(player.position, direction, direction.magnitude))
                    {
                        // Spawn a zombie at the valid spawn position
                        SpawnZombie(spawnPosition);
                    }
                }
            }

            // Decrease the spawn interval over time
            timePassed += spawnInterval;
            spawnInterval = Mathf.Max(1.0f, spawnInterval - spawnRateIncrease);

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnZombie(Vector3 spawnPosition)
    {
        // Create a new zombie at the specified position
        GameObject zombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);

        // Modify zombie's health and speed based on timePassed
        Target targetScript = zombie.GetComponent<Target>();
        EnemyController zombieController = zombie.GetComponent<EnemyController>();
        if (targetScript != null && zombieController != null)
        {
            float healthMultiplier = 1.0f + (timePassed / 120.0f); // Increase health every 2 minutes

            float speedMultiplier = 1.0f + (timePassed / 60.0f); // Increase speed over time
            targetScript.health = Mathf.Ceil(targetScript.health * healthMultiplier);

            // Update the zombie's speed directly in the EnemyController script
            zombieController.initialMovementSpeed *= speedMultiplier;
            zombieController.maxMovementSpeed = zombieController.initialMovementSpeed * 1.5f; // Increase the max speed over time
        }
    }
}