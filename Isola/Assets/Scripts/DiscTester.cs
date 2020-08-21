using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscTester : MonoBehaviour
{
    [Range(7,50)]
    public float radius = 1;
    public Vector2 regionSize = Vector2.one;
    public int rejectionSampleSize = 30;
    public float displayRadius = 0.8f;
    
    public float size = 200;
    List<Vector2> points;
    List<Vector3> newPoints;

    void OnValidate()
    {
        points = PoissonDiscSampling.GeneratePoints(radius, regionSize, rejectionSampleSize);
        Converter();
    }

    void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(regionSize / 2, regionSize);
        if(newPoints != null)
        {
            foreach(Vector3 newPoint in newPoints)
            {
                //Gizmos.DrawSphere(newPoint, displayRadius);
            }
        }
    }

    void Converter()
    {
        float halfMap = size * 5;
        //Debug.Log(halfMap);
        newPoints = new List<Vector3>();
        foreach(Vector2 point in points)
        {
            float x = point.x;
            float z = point.y;
            x -= 1000;
            z -= 1000;
            newPoints.Add(new Vector3(x, 2, z));
        }
        

    }
}
