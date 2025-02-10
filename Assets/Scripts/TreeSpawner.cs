using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{

    public TerrainGenerator terrainGenerator;

    public GameObject treePrefab;  // The tree prefab you want to spawn
    public float spawnRadius = 10f;  // Radius in which trees will be spawned
    public int numberOfTrees = 50;  // Number of trees to spawn
    public float minHeight = 5f;    // Minimum height for trees
    public float maxHeight = 10f;   // Maximum height for trees

    void Start()
    {
        SpawnTrees();
    }

    void SpawnTrees()
    {
        for (int i = 0; i < numberOfTrees; i++)
        {
            Vector3 randomPosition = transform.position + Random.insideUnitSphere * spawnRadius;

            // Get the terrain height at the random position
            float terrainHeight = terrainGenerator.GetTerrainHeightAtPosition(randomPosition);

            // Check if the terrain height is below a certain threshold (e.g., to avoid mountains)
            if (terrainHeight <= maxHeight)
            {
                float randomHeight = Random.Range(minHeight, maxHeight);
                randomPosition.y = randomHeight; // Set Y position to the random height

                GameObject newTree = Instantiate(treePrefab, randomPosition, Quaternion.identity);
                newTree.transform.localScale = new Vector3(randomHeight, randomHeight, randomHeight);
            }
        }
    }


    public float GetTerrainHeightAtPosition(Vector3 position)
    {
        Ray ray = new Ray(new Vector3(position.x, 1000, position.z), Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            return hit.point.y;
        }

        return 0f; // Return a default value if the raycast doesn't hit the terrain
    }




    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}