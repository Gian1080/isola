using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class MoonShapeSettings : ScriptableObject
{

    public float planetRadius = 1;
    public MoonNoiseLayer[] noiseLayers;

    [System.Serializable]
    public class MoonNoiseLayer
    {
        public bool enabled = true;
        public bool useFirstLayerAsMask;
        public MoonNoiseSettings noiseSettings;
    }
}