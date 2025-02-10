using UnityEngine;

public class SeedRandomizer : MonoBehaviour
{
    public HeightMapSettings heightMapSettings; // Reference to your HeightMapSettings script

    void Start()
    {
        if (heightMapSettings != null)
        {
            // Randomize the seed in HeightMapSettings when the game starts
            heightMapSettings.RandomizeSeed();
        }
        else
        {
            Debug.LogWarning("HeightMapSettings reference is missing!");
        }
    }
}
