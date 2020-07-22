using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallShaper
{
    Mesh mesh;
    SimplexNoise noise;
    int size;
    int wallWidth;
    int wallHeight;
    float meshHeight;

    public WallShaper(Mesh mesh, int size, float meshHeight)
    {
        this.mesh = mesh;
        this.size = size;
        wallWidth = size + (size / 5);
        wallHeight = size / 2; 
        this.meshHeight = meshHeight;
        noise = new SimplexNoise();
    }

    public void BuildThatWall()
    {
        Vector3[] vertices = new Vector3[(wallHeight + 1) * (wallWidth + 1)];
        int[] triangles = new int[wallHeight * wallWidth * 6];
        for(int z = 0, i = 0; z <= wallHeight ; z++)
        {
            for(int x = 0; x <= wallWidth; x++, i++)
            {
                vertices[i] = new Vector3(x, 0, z);
                vertices[i].y = (noise.Evaluate(vertices[i]) + 1) * 0.5f;
                //vertices[i].y *= meshHeight;
            }
        }
        for(int z = 0, vertIndex = 0, trianglesIndex = 0; z < wallHeight * 0.2f ; z++, vertIndex++)
        {
            for(int x = 0; x < wallWidth; x++, trianglesIndex += 6, vertIndex++)
            {
                triangles[trianglesIndex + 0] = vertIndex;
                triangles[trianglesIndex + 1] = vertIndex + wallWidth + 1;
                triangles[trianglesIndex + 2] = vertIndex + 1;

                triangles[trianglesIndex + 3] = vertIndex + 1;
                triangles[trianglesIndex + 4] = vertIndex + wallWidth + 1;
                triangles[trianglesIndex + 5] = vertIndex + wallWidth + 2;
            }
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
