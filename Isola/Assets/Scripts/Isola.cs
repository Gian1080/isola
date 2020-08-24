using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Isola : MonoBehaviour
{
    public bool updateGame = false;
    [Range(122, 255)] public int size;
    [Range(1, 10)] public int screenScale;
    [Range(0, 10)] public float a;
    [Range(0, 10)] public float b;
    [Range(0.1f, 100f)] public float meshHeight;
    [Range(0.001f, 100.0f)] public float scale;
    public Vector2 noiseStep;
    //public bool autoUpdate = false;
    public bool useFallOff = false;
    public bool usePerlin = false;
    public bool useColor = false;
    public int seed;
    public AnimationCurve curve;

    IslandBuilder meshMaker;
    MeshCollider collider;
    GameObject isola;
    Material islandMaterial;

    WaterMaker waterMaker;
    GameObject water;
    Material waterMaterial;

    public bool grassUpdate = false;
    public GameObject[] grassSkins;
    [Range(5, 300)] public float grassItemRadius;
    [Range(1,30)] public int grassItemSampleAttempts;

    public bool flowerUpdate = false;
    public GameObject[] flowerSkins;
    [Range(5, 300)] public float flowerItemRadius;
    [Range(1, 30)] public int flowerItemSampleAttempts;

    public bool rockUpdate = false;
    public GameObject[] rockSkins;
    [Range(20, 300)] public float rockItemRadius;
    [Range(1, 30)] public int rockItemSampleAttempts;

    public bool treeUpdate = false;
    public GameObject[] treeSkins;
    [Range(50, 300)] public float treeItemRadius;
    [Range(1, 30)] public int treeItemSampleAttempts;


    private void OnValidate()
    {
        if (updateGame)
        {
            GenerateIsland();
            GenerateWater();
            MakeNewIsland();
        }
    }

    private void Update()
    {
    }

    private void Start()
    {
        foreach (GameObject o in Object.FindObjectsOfType<GameObject>())
        {
            if(o != GameObject.Find("Main Camera") && o != GameObject.Find("Directional Light")  && o != GameObject.Find("IslandMaker") && o != GameObject.Find("FPSPlayer") && o != GameObject.Find("Main Camera") && o != GameObject.Find("Capsule"))
            {
                print(o.ToString());
                Destroy(o);
            }
        }
    }

    public void GenerateIsland()
    {
        if(isola == null)
        {
            isola = new GameObject("isola");
            islandMaterial = Resources.Load<Material>("Materials/secondTerrainMaterial");
            isola.AddComponent<MeshRenderer>().sharedMaterial = islandMaterial;
            isola.AddComponent<MeshFilter>();
            collider = isola.AddComponent<MeshCollider>();
            isola.GetComponent<MeshFilter>().sharedMesh = new Mesh();
            meshMaker = new IslandBuilder(isola.GetComponent<MeshFilter>().sharedMesh, size, useFallOff, usePerlin, useColor, a, b, scale, noiseStep, seed, meshHeight, curve);
            collider.sharedMesh = isola.GetComponent<MeshFilter>().sharedMesh;
            isola.transform.localScale = new Vector3(screenScale, screenScale, screenScale);
        }
        else if(isola != null)
        {
            meshMaker = new IslandBuilder(isola.GetComponent<MeshFilter>().sharedMesh, size, useFallOff, usePerlin, useColor, a ,b, scale, noiseStep, seed, meshHeight, curve);
            isola.transform.localScale = new Vector3(screenScale, screenScale, screenScale);
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
            water.transform.localScale = new Vector3(screenScale, screenScale, screenScale);
            water.transform.position = new Vector3(0, screenScale * 1.75f, 0);
        }
        else
        {
            waterMaker = new WaterMaker(water.GetComponent<MeshFilter>().sharedMesh, size);
            water.transform.localScale = new Vector3(screenScale, screenScale, screenScale);


        }
    }

    void GenerateNatureSpawn()
    {
        Vector2 regionSize = new Vector2((float) size * screenScale, (float) size * screenScale);

        if (grassUpdate)
        {
            List<Vector2> grassPoints = PoissonDiscSampling.GeneratePoints(grassItemRadius, regionSize, grassItemSampleAttempts);
            List<Vector3> grassMap = Converter(grassPoints);
            List<Vector3> finalGrassPoints = ObjectHeightAdjuster(grassMap);
            GrassPlacer(finalGrassPoints);
        }

        if (flowerUpdate)
        {
            List<Vector2> flowerPoints = PoissonDiscSampling.GeneratePoints(flowerItemRadius, regionSize, flowerItemSampleAttempts);
            List<Vector3> flowerMap = Converter(flowerPoints);
            List<Vector3> finalFlowerPoints = ObjectHeightAdjuster(flowerMap);
            FlowerPlacer(finalFlowerPoints);
        }

        if (rockUpdate)
        {
            List<Vector2> rockPoints = PoissonDiscSampling.GeneratePoints(rockItemRadius, regionSize, rockItemSampleAttempts);
            List<Vector3> rockMap = Converter(rockPoints);
            List<Vector3> finalRockPoints = ObjectHeightAdjuster(rockMap);
            RockPlacer(finalRockPoints);
        }

        if (treeUpdate)
        {
            List<Vector2> treePoints = PoissonDiscSampling.GeneratePoints(treeItemRadius, regionSize, treeItemSampleAttempts);
            print(treePoints.Count + " treepoints");
            List<Vector3> treeMap = Converter(treePoints);
            print(treeMap.Count + " treeMap");
            List<Vector3> finalTreePoints = ObjectHeightAdjuster(treeMap);
            print(finalTreePoints.Count + " FinalTree");
            TreePlacer(finalTreePoints);
        }
    }


    void GrassPlacer(List<Vector3> naturePoints)
    {
        GameObject[] natureObjects = new GameObject[naturePoints.Count];
        if (GameObject.Find("Grass Parent"))
        {
            Destroy(GameObject.Find("Grass Parent"));
        }
        GameObject grass = new GameObject("Grass Parent");
        for (int i = 0; i < naturePoints.Count; i++)
        {
            if (naturePoints[i].y >= 25 && naturePoints[i].y < 80)
            {
                Vector3 position = new Vector3(naturePoints[i].x, naturePoints[i].y, naturePoints[i].z);
                Vector3 scale = Vector3.one * size / 22;
                Vector3 rotation = new Vector3(Random.Range(0, 10f), Random.Range(0, 360f), Random.Range(0, 10f));
                GameObject natureThing = Instantiate(grassSkins[Random.Range(0, grassSkins.Length)]);
                natureObjects[i] = natureThing;
                natureObjects[i].transform.parent = grass.transform;
                natureObjects[i].transform.position = position;
                natureObjects[i].transform.localScale = (scale * Random.Range(0.75f, 1.25f));
                natureObjects[i].transform.eulerAngles = rotation;
            }
        }
    }

    void FlowerPlacer(List<Vector3> naturePoints)
    {
        GameObject[] natureObjects = new GameObject[naturePoints.Count];
        if (GameObject.Find("Flower Parent"))
        {
            Destroy(GameObject.Find("Flower Parent"));
        }
        GameObject flower = new GameObject("Flower Parent");
        if(flower.gameObject.transform.childCount > 0)
        {
            Destroy(flower);
            flower = new GameObject("Flower Parent");
        }
        for (int i = 0; i < naturePoints.Count; i++)
        {
            if (naturePoints[i].y >= 25 && naturePoints[i].y < 80)
            {
                Debug.Log(flower.gameObject.transform.childCount);
                Vector3 position = new Vector3(naturePoints[i].x, naturePoints[i].y, naturePoints[i].z);
                Vector3 scale = Vector3.one * size / 22;
                Vector3 rotation = new Vector3(Random.Range(0, 10f), Random.Range(0, 360f), Random.Range(0, 10f));
                GameObject natureThing = Instantiate(flowerSkins[Random.Range(0, flowerSkins.Length)]);
                natureObjects[i] = natureThing;
                natureObjects[i].transform.parent = flower.transform;
                natureObjects[i].transform.position = position;
                natureObjects[i].transform.localScale = (scale * Random.Range(0.75f, 1.25f));
                natureObjects[i].transform.eulerAngles = rotation;
            }
        }
    }

    void RockPlacer(List<Vector3> naturePoints)
    {
        GameObject[] natureObjects = new GameObject[naturePoints.Count];
        if (GameObject.Find("Rock Parent"))
        {
            Destroy(GameObject.Find("Rock Parent"));
        }
        GameObject rock = new GameObject("Rock Parent");
        for (int i = 0; i < naturePoints.Count; i++)
        {
            if (naturePoints[i].y >= 40 && naturePoints[i].y < 100)
            {
                Vector3 position = new Vector3(naturePoints[i].x, naturePoints[i].y, naturePoints[i].z);
                Vector3 scale = Vector3.one * size / 22;
                Vector3 rotation = new Vector3(Random.Range(0, 10f), Random.Range(0, 360f), Random.Range(0, 10f));
                GameObject natureThing = Instantiate(rockSkins[Random.Range(0, rockSkins.Length)]);
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
        if(GameObject.Find("Tree Parent"))
        {
            Destroy(GameObject.Find("Tree Parent"));
        }
        GameObject tree = new GameObject("Tree Parent");
        for(int i = 0; i < naturePoints.Count; i++)
        {
            if(naturePoints[i].y >= 45 && naturePoints[i].y < 90)
            {
                Vector3 position = new Vector3(naturePoints[i].x, naturePoints[i].y, naturePoints[i].z);
                Vector3 scale = Vector3.one * size / 22;
                Vector3 rotation = new Vector3(Random.Range(0, 10f), Random.Range(0, 360f), Random.Range(0, 10f));
                GameObject natureThing = Instantiate(treeSkins[Random.Range(0,treeSkins.Length)]);
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

    public float turnSpeed = 4.0f;
    public float moveSpeed = 2.0f;

    public float minTurnAngle = -90.0f;
    public float maxTurnAngle = 90.0f;
    private float rotX;

}
