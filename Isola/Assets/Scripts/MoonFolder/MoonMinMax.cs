using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonMinMax
{
    public float Min { get; private set; }
    public float Max { get; private set; }

    public MoonMinMax()
    {
        Min = float.MaxValue;
        Max = float.MinValue;
    }

    public void AddValue(float v)
    {
        if (v > Max)
        {
            Max = v; ;
        }
        if (v < Min)
        {
            Min = v; ;
        }
    }
}
