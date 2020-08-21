using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscTester : MonoBehaviour
{
    [Range(1,50)]
    public float radius = 1;
    public Vector2 regionSize = Vector2.one;
    public int rejectionSampleSize = 30;
    public float displayRadius = 0.8f;
    List<Vector2> points;
    List<Vector3> newPoints;

    void OnValidate()
    {
        points = PoissonDiscSampling.GeneratePoints(radius, regionSize, rejectionSampleSize);
        Converter(points);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(regionSize / 2, regionSize);
        if(points != null)
        {
            foreach(Vector2 point in points)
            {
                Gizmos.DrawSphere(point, displayRadius);
            }
            foreach(Vector3 newPoint in newPoints)
            {
                Gizmos.DrawSphere(newPoint, displayRadius);
            }
        }
    }

    void Converter(List<Vector2> points)
    {
        newPoints = new List<Vector3>();
        foreach(Vector2 point in points)
        {
            newPoints.Add(new Vector3(point.x, 0, point.y));
        }
    }
}
