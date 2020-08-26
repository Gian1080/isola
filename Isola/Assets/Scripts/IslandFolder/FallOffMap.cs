using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public static class FallOffMap
{
    public static float[] FallOffMapMaker(int islandSize, float a, float b)
    {
        islandSize += 1;
        float[] map = new float[islandSize * islandSize];
        for(int i = 0, vertCounter = 0; i < islandSize; i++)
        {
            for(int j = 0; j < islandSize; j++, vertCounter++)
            {
                float x = i / (float)islandSize * 2 - 1;
                float y = j / (float)islandSize * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[vertCounter] = Evaluate(value, a, b);
            }
        }
        return map;
    }

    static float Evaluate(float value, float a, float b)
    {
        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }
}
