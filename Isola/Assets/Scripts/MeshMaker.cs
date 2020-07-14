using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshMaker
{
    Mesh mesh;
    int size;


    bool useFallOff;
    float[] fallOffMap;
    
    public MeshMaker(Mesh mesh, int size, bool useFallOff, float a, float b)
    {
        this.mesh = mesh;
        this.size = size;
        this.useFallOff = useFallOff;
        fallOffMap = new float[size + 1 * size + 1];
        fallOffMap = FallOffMap.FallOffMapMaker(size,a, b);
    }

    public void MeshBuilder()
    {
        Vector3[] vertices = new Vector3[(size + 1) * (size + 1)];
        int[] triangles = new int[size * size * 6];
        for(int i = 0, z = 0; z <= size; z++)
        {
            for(int x = 0; x <= size; x++)
            {
                vertices[i] = new Vector3(x, 0, z);
                if(useFallOff)
                {
                    vertices[i].y = fallOffMap[i];
                }
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
        mesh.RecalculateNormals();
    }
}

