using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PerlinNoise
{
    public static float[] PerlinMapMaker(int islandSize, float scale, int seed, Vector2 noiseStep)
    {
        islandSize += 1;
        System.Random randomNumber = new System.Random(seed);
        float xOffSet = randomNumber.Next(-1000, 1000) + noiseStep.x;
        float yOffSet = randomNumber.Next(-1000, 1000) + noiseStep.y;
        float[] perlinMap = new float[islandSize * islandSize];
        float halfMap = islandSize * 0.5f;
        if(scale == 0)
        {
            scale = 1.111f;
        }
        for (int i = 0, perlinIndex = 0; i < islandSize; i++)
        {
            for (int j = 0; j < islandSize; j++)
            {
                float x = (j - halfMap) / scale + xOffSet;
                float y = (i - halfMap) / scale + yOffSet;
                float perlinValue = Mathf.PerlinNoise(x, y);
                perlinMap[perlinIndex] = perlinValue;
                perlinIndex++;
            }
        }
        return perlinMap;
    }
}
