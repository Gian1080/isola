using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public static class PerlinNoise
{
    public static float[] PerlinMapMaker(int islandSize, float scale, int seed, Vector2 noiseStep, int octaves, float persistance, float lacunarity)
    {
        islandSize += 1;
        System.Random randomNumber = new System.Random(seed);
        float xOffSet = randomNumber.Next(-1000, 1000) + noiseStep.x;
        float yOffSet = randomNumber.Next(-1000, 1000) + noiseStep.y;
        float[] perlinMap = new float[islandSize * islandSize];
        float halfMap = islandSize * 0.5f;
        if(scale == 0)
        {
            scale = 1.0f;
        }
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;
        for (int i = 0, perlinIndex = 0; i < islandSize; i++)
        {
            for (int j = 0; j < islandSize; j++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                for(int o = 0; o < octaves; o++)
                {
                    float x = ((j - halfMap) / scale  + xOffSet) * frequency;
                    float y = ((i - halfMap) / scale  + yOffSet) * frequency;
                    float perlinValue = Mathf.PerlinNoise(x, y) * 2 -1;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                if(noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if(noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                perlinMap[perlinIndex] = noiseHeight;
                perlinIndex++;
            }
        }
        for(int i = 0, perlinIndex = 0; i < islandSize; i++)
        {
            for(int j = 0; j < islandSize; j++)
            {
                perlinMap[perlinIndex] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, perlinMap[perlinIndex]);
                perlinIndex++;
            }
        }

        return perlinMap;
    }
}
