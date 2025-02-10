using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn; // The prefab to be spawned.
    public LayerMask groundLayer; // The layer(s) where the ground is placed.
    public float minHeight = 5f; // The minimum height for a valid collision.
    public float maxHeight = 15f; // The maximum height for a valid collision;
    public float spawnRadius = 666f; // The radius within which to spawn the trees.

    void Start()
    {
        // Spawn trees when the game starts.
        SpawnRandomPrefabs();
    }

    void SpawnRandomPrefabs()
    {
        // Generate random positions within the specified spawn radius.
        for (int i = 0; i < 10; i++) // Change the number of trees to spawn as needed.
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(0f, spawnRadius);
            Vector3 spawnPosition = transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

            // Perform a raycast to check if the position is valid.
            Ray ray = new Ray(spawnPosition + Vector3.up * 1000f, Vector3.down);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                if (hit.point.y >= minHeight && hit.point.y <= maxHeight)
                {
                    SpawnPrefab(hit.point);
                }
            }
        }
    }

    private void SpawnPrefab(Vector3 spawnPosition)
    {
        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }
}
