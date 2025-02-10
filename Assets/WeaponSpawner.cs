using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PrefabSpawnInfo
{
    public GameObject prefab; // The prefab to spawn
    public float spawnRate = 2f; // The base spawn rate for this
    public AudioClip spawnSound;                           
}

public class WeaponSpawner : MonoBehaviour
{
    public PrefabSpawnInfo[] prefabSpawnInfos; // Array of prefabs and their spawn rates
    public float spawnRadius = 10f; // Radius within which the objects will spawn
    public int maxObjects = 10; // Maximum number of spawned objects
    public float density = 1f; // Density of objects around the player
    public float fallSpeed = 2f; // Speed at which the spawned objects fall
    public float destroyYThreshold = -10f; // Y-value threshold for destroying objects
    public float spawnHeight = 3f; // Height above ground for spawning objects
    public float spawnDistanceThreshold = 50f; // Distance threshold for spawning objects

    private Transform playerTransform; // Reference to the player's transform
    private Vector3 initialPlayerPosition; // Initial player position for distance comparison
    private List<GameObject> spawnedObjects = new List<GameObject>(); // Track spawned objects

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        initialPlayerPosition = playerTransform.position;
    }

    void Update()
    {
        float distanceTraveled = Vector3.Distance(initialPlayerPosition, playerTransform.position);

        if (distanceTraveled >= spawnDistanceThreshold)
        {
            SpawnObject();
            initialPlayerPosition = playerTransform.position; // Update initial position after spawning
        }

        CheckObjectPositions();
    }

    void SpawnObject()
    {
        int objectsToSpawn = Mathf.Min(maxObjects - spawnedObjects.Count, 5); // Spawn only 5 objects per iteration
        if (spawnedObjects.Count >= maxObjects)
        {
            int objectsToRemove = Mathf.Min(100, spawnedObjects.Count); // Remove the oldest 100 objects
            for (int i = 0; i < objectsToRemove; i++)
            {
                GameObject objToRemove = spawnedObjects[i];
                Destroy(objToRemove);
            }
            spawnedObjects.RemoveRange(0, objectsToRemove);
        }
        for (int i = 0; i < objectsToSpawn; i++)
        {
            // Choose a prefab to spawn based on spawn rates
            float randomValue = Random.Range(0f, CalculateTotalSpawnRate());
            float accumulatedSpawnRate = 0f;
            GameObject selectedPrefab = null;

            foreach (var info in prefabSpawnInfos)
            {
                accumulatedSpawnRate += info.spawnRate;
                if (randomValue <= accumulatedSpawnRate)
                {
                    selectedPrefab = info.prefab;
                    break;
                }
            }

            if (selectedPrefab == null)
            {
                Debug.LogError("No prefab selected!");
                return;
            }

            // Calculate spawn position slightly above the ground
            Vector3 randomPos = Random.insideUnitSphere * spawnRadius;
            randomPos.y = 0;

            // Limit spawn position's distance from the player to avoid clustering
            randomPos = Vector3.ClampMagnitude(randomPos, spawnRadius);

            Vector3 spawnPos = playerTransform.position + randomPos + Vector3.up * spawnHeight;

            RaycastHit hit;
            if (Physics.Raycast(spawnPos, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                spawnPos = hit.point + Vector3.up * 0.1f; // Slightly above the ground
            }

            Collider[] colliders = Physics.OverlapSphere(spawnPos, density);
            if (colliders.Length == 0)
            {
                GameObject newObject = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
                AudioSource audioSource = newObject.AddComponent<AudioSource>();
                audioSource.clip = GetAudioClip(selectedPrefab); // Get corresponding audio clip
                audioSource.loop = true;
                audioSource.volume = 0.5f; 
                audioSource.spatialBlend = 1f; // Set 3D spatial blend
                audioSource.rolloffMode = AudioRolloffMode.Linear; // Try different rolloff modes
                audioSource.minDistance = 5f; // Set a minimum distance for the sound to start fading
                audioSource.maxDistance = 50f; // Set the maximum distance to hear the sound

                audioSource.Play(); // Start playing the sound
                // Add a light component to the spawned object
                Light lightComponent = newObject.AddComponent<Light>();
                lightComponent.color = Color.white; // Set the color of the light (you can modify this)
                lightComponent.intensity = 2f; // Set the intensity of the light (you can modify this)
                lightComponent.range = 5f; // Set the range of the light (you can modify this)

                spawnedObjects.Add(newObject); // Add the spawned object to the list
                Rigidbody rb = newObject.AddComponent<Rigidbody>();
                rb.useGravity = false;
                rb.velocity = Vector3.down * fallSpeed;
                rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

                spawnedObjects.Add(newObject); // Add the spawned object to the list
            }
        }
    }

    AudioClip GetAudioClip(GameObject prefab)
    {
        foreach (var info in prefabSpawnInfos)
        {
            if (info.prefab == prefab)
            {
                return info.spawnSound;
            }
        }
        return null;
    }

    float CalculateTotalSpawnRate()
    {
        float totalSpawnRate = 0f;
        foreach (var info in prefabSpawnInfos)
        {
            totalSpawnRate += info.spawnRate;
        }
        return totalSpawnRate;
    }

    void CheckObjectPositions()
    {
        for (int i = spawnedObjects.Count - 1; i >= 0; i--)
        {
            GameObject spawnedObject = spawnedObjects[i];
            if (spawnedObject.transform.position.y < destroyYThreshold)
            {
                AudioSource audioSource = spawnedObject.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    audioSource.Stop(); // Stop the sound
                }
                Destroy(spawnedObject);
                spawnedObjects.RemoveAt(i);
            }
        }
    }
}
