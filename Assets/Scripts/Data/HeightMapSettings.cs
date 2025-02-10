using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class HeightMapSettings : UpdatableData
{
    public NoiseSettings noiseSettings;
    public bool useFalloff;
    public float heightMultiplier;
    public AnimationCurve heightCurve;
    public bool useFlatShading;

    public void RandomizeSeed()
    {
        // Generate a new random seed each time this method is called
        noiseSettings.seed = Random.Range(0, 1000000); // You can adjust the range as needed
    }

    // Call this method when you want to update height settings
    public void UpdateHeightSettings(float newHeightMultiplier, AnimationCurve newHeightCurve)
    {
        heightMultiplier = newHeightMultiplier;
        heightCurve = newHeightCurve;
    }

    public float minHeight
    {
        get { return heightMultiplier * heightCurve.Evaluate(0); }
    }

    public float maxHeight
    {
        get { return heightMultiplier * heightCurve.Evaluate(1); }
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
       
        noiseSettings.ValidateValues();
        RandomizeSeed();
        base.OnValidate();
    }
#endif
}
