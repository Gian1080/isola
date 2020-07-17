using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshMaker
{
    Mesh mesh;
    int size;
    float meshHeight;
    AnimationCurve curve;
    Gradient gradient;

    bool useFallOff;
    float[] fallOffMap;

    bool usePerlin;
    float[] perlinMap;
    bool useColor;
    Color[] colors;
    
    public MeshMaker(Mesh mesh, int size, bool useFallOff, bool usePerlin, bool useColor, float a, float b,float scale,Vector2 noiseStep,int seed, float meshHeight, AnimationCurve curve, Gradient gradient)
    {
        this.mesh = mesh;
        this.size = size;
        this.curve = curve;
        this.meshHeight = meshHeight;
        this.useFallOff = useFallOff;
        this.usePerlin = usePerlin;
        this.useColor = useColor;
        this.gradient = gradient;
        fallOffMap = FallOffMap.FallOffMapMaker(size,a, b);
        perlinMap = PerlinNoise.PerlinMapMaker(size, scale, seed, noiseStep);
    }

    public void MeshBuilder()
    {
        Vector3[] vertices = new Vector3[(size + 1) * (size + 1)];
        int[] triangles = new int[size * size * 6];
        float halfMap = size * 0.5f;
        colors = new Color[vertices.Length];
        for(int i = 0, z = 0; z <= size; z++)
        {
            for (int x = 0; x <= size; x++)
            {
                vertices[i] = new Vector3(x - halfMap, 0, z - halfMap);
                if (useFallOff)
                {
                    perlinMap[i] -= fallOffMap[i];
                }
                if (usePerlin)
                {
                    vertices[i].y = perlinMap[i];
                }
                if (x == 0 || x == size || z == 0 || z == size || vertices[i].y < 0.001f)
                {
                    //vertices[i].y = 0.01f;
                }
                colors[i] = gradient.Evaluate(vertices[i].y);
                vertices[i].y = curve.Evaluate(vertices[i].y);
                vertices[i].y *= meshHeight;
                i++;
            }
        }
        for(int z = 0, triangleIndex = 0, vertexIndex = 0; z < size; z++, vertexIndex++)
        {
            for(int x = 0; x < size; x++, triangleIndex += 6, vertexIndex++)
            {
                triangles[triangleIndex + 0] = vertexIndex;
                triangles[triangleIndex + 1] = vertexIndex + size + 1;
                triangles[triangleIndex + 2] = vertexIndex + 1;

                triangles[triangleIndex + 3] = vertexIndex + 1;
                triangles[triangleIndex + 4] = vertexIndex + size + 1;
                triangles[triangleIndex + 5] = vertexIndex + size + 2;
            }
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        if(useColor)
        {
        mesh.colors = colors;
        }
        mesh.RecalculateNormals();
    }
}

