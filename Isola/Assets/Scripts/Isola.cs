using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
//using System.Numerics;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Isola : MonoBehaviour
{
    public bool updateGame = false;
    [Range(2, 255)]
    public int size;
    [Range(0, 10)]
    public float a;
    [Range(0, 10)]
    public float b;
    [Range(0.1f, 100f)]
    public float meshHeight;
    [Range(0.001f, 100.0f)]
    public float scale;
    public Vector2 noiseStep;
    public bool autoUpdate = false;
    public bool useFallOff = false;
    public bool usePerlin = false;
    public bool useColor = false;
    public int seed;
    public AnimationCurve curve;
    public Gradient gradient;

    IslandBuilder meshMaker;
    MeshCollider collider;
    GameObject isola;
    Material islandMaterial;

    WaterMaker waterMaker;
    GameObject water;
    Material waterMaterial;

    Vector2 regionSize;
    [Range(5, 300)]
    public float smallItemRadius;
    [Range(1,30)]
    public int smallItemSampleAttempts;
    List<Vector2> rockPoints;
    List<Vector3> rockMap;
    List<Vector3> finalRockPoints;

    [Range(20, 300)]
    public float midItemRadius;
    [Range(1, 30)]
    public int midItemSampleAttempts;
    List<Vector2> bushPoints;
    List<Vector3> bushMap;
    List<Vector3> finalBushPoints;

    [Range(50, 300)]
    public float bigItemRadius;
    [Range(1, 30)]
    public int bigItemSampleAttempts;
    List<Vector2> treePoints;
    List<Vector3> treeMap;
    List<Vector3> finalTreePoints;

    public GameObject[] grassSkins;
    public GameObject[] bushSkins;
    public GameObject[] treeSkins;


    private void OnValidate()
    {
        if (updateGame)
        {
            GenerateIsland();
            GenerateWater();
            MakeNewIsland();
        }
    }

    public void GenerateIsland()
    {
        if (isola == null)
        {
            isola = new GameObject("isola");
            islandMaterial = Resources.Load<Material>("Materials/secondTerrainMaterial");
            isola.AddComponent<MeshRenderer>().sharedMaterial = islandMaterial;
            isola.AddComponent<MeshFilter>();
            collider = isola.AddComponent<MeshCollider>();
            isola.GetComponent<MeshFilter>().sharedMesh = new Mesh();
            meshMaker = new IslandBuilder(isola.GetComponent<MeshFilter>().sharedMesh, size, useFallOff, usePerlin, useColor, a, b, scale, noiseStep, seed, meshHeight, curve, gradient);
            collider.sharedMesh = isola.GetComponent<MeshFilter>().sharedMesh;
            isola.transform.localScale += new Vector3(10, 10, 10);

            
        }
        else
        {
            meshMaker = new IslandBuilder(isola.GetComponent<MeshFilter>().sharedMesh, size, useFallOff, usePerlin, useColor, a ,b, scale, noiseStep, seed, meshHeight, curve, gradient);
        }
    }

    public void GenerateWater()
    {
        if(water == null)
        {
            water = new GameObject("water");
            water.AddComponent<MeshFilter>();
            water.GetComponent<MeshFilter>().sharedMesh = new Mesh();
            waterMaker = new WaterMaker(water.GetComponent<MeshFilter>().sharedMesh, size);
            waterMaterial = Resources.Load<Material>("Materials/TransparentWater");
            water.AddComponent<MeshRenderer>().sharedMaterial = waterMaterial;
            water.transform.localScale += new Vector3(10, 10, 10);
            water.transform.position += new Vector3(0, 15, 0);
        }
        else
        {
            waterMaker = new WaterMaker(water.GetComponent<MeshFilter>().sharedMesh, size);
        }
    }

    void GenerateNatureSpawn()
    {
        regionSize.x = (float)size * 10;
        regionSize.y = (float)size * 10;
        rockPoints = PoissonDiscSampling.GeneratePoints(smallItemRadius, regionSize, smallItemSampleAttempts);
        rockMap = Converter(rockPoints);
        bushPoints = PoissonDiscSampling.GeneratePoints(midItemRadius, regionSize, midItemSampleAttempts);
        bushMap = Converter(bushPoints);
        treePoints = PoissonDiscSampling.GeneratePoints(bigItemRadius, regionSize, bigItemSampleAttempts);
        treeMap = Converter(treePoints);
        finalTreePoints = ObjectHeightAdjuster(treeMap);
        finalBushPoints = ObjectHeightAdjuster(bushMap);
        finalRockPoints = ObjectHeightAdjuster(rockMap);
        TreePlacer(finalTreePoints);
        RockPlacer(finalBushPoints);
        GrassPlacer(finalRockPoints);
    }


    void GrassPlacer(List<Vector3> naturePoints)
    {
        GameObject[] natureObjects = new GameObject[naturePoints.Count];
        GameObject grass = new GameObject("Grass Parent");
        for (int i = 0; i < naturePoints.Count; i++)
        {
            if (naturePoints[i].y >= 20 && naturePoints[i].y < 100)
            {
                Vector3 position = new Vector3(naturePoints[i].x, naturePoints[i].y, naturePoints[i].z);
                Vector3 scale = Vector3.one * size / 22;
                Vector3 rotation = new Vector3(Random.Range(0, 10f), Random.Range(0, 360f), Random.Range(0, 10f));
                GameObject natureThing = Instantiate(grassSkins[Random.Range(0, 10)]);
                natureObjects[i] = natureThing;
                natureObjects[i].transform.parent = grass.transform;
                natureObjects[i].transform.position = position;
                natureObjects[i].transform.localScale = (scale * Random.Range(0.75f, 1.25f));
                natureObjects[i].transform.eulerAngles = rotation;
            }
        }
    }

    void RockPlacer(List<Vector3> naturePoints)
    {
        GameObject[] natureObjects = new GameObject[naturePoints.Count];
        GameObject rock = new GameObject("Rock Parent");
        for (int i = 0; i < naturePoints.Count; i++)
        {
            if (naturePoints[i].y >= 22 && naturePoints[i].y < 100)
            {
                Vector3 position = new Vector3(naturePoints[i].x, naturePoints[i].y, naturePoints[i].z);
                Vector3 scale = Vector3.one * size / 22;
                Vector3 rotation = new Vector3(Random.Range(0, 10f), Random.Range(0, 360f), Random.Range(0, 10f));
                GameObject natureThing = Instantiate(bushSkins[Random.Range(0, 12)]);
                natureObjects[i] = natureThing;
                natureObjects[i].transform.parent = rock.transform;
                natureObjects[i].transform.position = position;
                natureObjects[i].transform.localScale = (scale * Random.Range(0.75f, 1.25f));
                natureObjects[i].transform.eulerAngles = rotation;
            }
        }
    }

    void TreePlacer(List<Vector3> naturePoints)
    {
        GameObject[] natureObjects = new GameObject[naturePoints.Count];
        GameObject tree = new GameObject("Tree Parent");
        for(int i = 0; i < naturePoints.Count; i++)
        {
            if(naturePoints[i].y >= 22 && naturePoints[i].y < 100)
            {
                Vector3 position = new Vector3(naturePoints[i].x, naturePoints[i].y, naturePoints[i].z);
                Vector3 scale = Vector3.one * size / 22;
                Vector3 rotation = new Vector3(Random.Range(0, 10f), Random.Range(0, 360f), Random.Range(0, 10f));
                GameObject natureThing = Instantiate(treeSkins[Random.Range(0,5)]);
                natureObjects[i] = natureThing;
                natureObjects[i].transform.parent = tree.transform;
                natureObjects[i].transform.position = position;
                natureObjects[i].transform.localScale = (scale * Random.Range(0.75f, 1.25f));
                natureObjects[i].transform.eulerAngles = rotation;
            }
        }
    }

    List<Vector3> Converter(List<Vector2> points)
    {
        float halfMap = size * 5;
        List<Vector3> naturePoints = new List<Vector3>();
        foreach (Vector2 point in points)
        {
            float x = point.x;
            float z = point.y;
            x -= halfMap;
            z -= halfMap;
            naturePoints.Add(new Vector3(x, 1000, z));
        }
        return naturePoints;
    }

    List<Vector3> ObjectHeightAdjuster(List<Vector3> objectMap)
    {
        List<Vector3> newMap = new List<Vector3>();
        for(int i = 0; i < objectMap.Count; i++)
        {
            Ray ray = new Ray(new Vector3(objectMap[i].x, objectMap[i].y, objectMap[i].z), Vector3.down);
            RaycastHit hitInfo;
            if(Physics.Raycast(ray, out hitInfo, 1100))
            {
                newMap.Add(new Vector3(objectMap[i].x, hitInfo.point.y, objectMap[i].z));
            }
        }
        return newMap;
    }

    void MakeNewIsland()
    {
        meshMaker.BuildIsland();
        waterMaker.GenerateWater();
        GenerateNatureSpawn();
    }
}
