using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WaterMaker
{
    int size;
    Mesh mesh;
    public WaterMaker(Mesh mesh, int size)
    {
        this.size = size;
        this.mesh = mesh;
    }

    public void GenerateWater()
    {
        Vector3[] vertices = new Vector3[(size + 1) * (size + 1)];
        int[] triangles = new int[size * size * 6];
        Vector2[] uvs = new Vector2[vertices.Length];
        float halfMap = size * 0.5f;
        for(int z = 0, i = 0; z <= size; z++)
        {
            for(int x = 0; x <= size; x++)
            {
                vertices[i] = new Vector3(x - halfMap, 1, z - halfMap);
                i++;
            }
        }
        for(int z = 0, vertcounter = 0, triangleIndex = 0; z < size; z++, vertcounter++)
        {
            for(int x = 0; x < size; x++, vertcounter++)
            {
                triangles[triangleIndex + 0] = vertcounter + 0;
                triangles[triangleIndex + 1] = vertcounter + size +1;
                triangles[triangleIndex + 2] = vertcounter + 1;

                triangles[triangleIndex + 3] = vertcounter + 1;
                triangles[triangleIndex + 4] = vertcounter + size + 1;
                triangles[triangleIndex + 5] = vertcounter + size + 2;
                triangleIndex += 6;
            }
        }
        for (int z = 0, i = 0; z <= size; z++)
        {
            for (int x = 0; x <= size; x++)
            {
                uvs[i] = new Vector2((float)x / size, (float)z / size);
                i++;
            }
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }

}
