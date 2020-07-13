using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshMaker
{
    Mesh mesh;
    int resolution;

    public MeshMaker(Mesh mesh, int resolution)
    {
        this.mesh = mesh;
        this.resolution = resolution;
    }

    public void MeshBuilder()
    {
        Vector3[] vertices = new Vector3[(resolution + 1) * (resolution + 1)];
        int[] triangles = new int[resolution * resolution * 6];
        for(int i = 0, z = 0; z <= resolution; z++)
        {
            for(int x = 0; x <= resolution; x++)
            {
                vertices[i] = new Vector3(x, 0, z);
                i++;
            }
        }
        for(int z = 0, triangleIndex = 0, vertexIndex = 0; z < resolution; z++, vertexIndex++)
        {
            for(int x = 0; x < resolution; x++, triangleIndex += 6, vertexIndex++)
            {
                triangles[triangleIndex + 0] = vertexIndex;
                triangles[triangleIndex + 1] = vertexIndex + resolution + 1;
                triangles[triangleIndex + 2] = vertexIndex + 1;

                triangles[triangleIndex + 3] = vertexIndex + 1;
                triangles[triangleIndex + 4] = vertexIndex + resolution + 1;
                triangles[triangleIndex + 5] = vertexIndex + resolution + 2;
            }
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}

