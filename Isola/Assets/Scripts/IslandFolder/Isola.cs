using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Xml;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.VFX;
using UnityEngine.XR.WSA.Input;

public class Isola : MonoBehaviour
{
    public bool updateGame = false;
    [Range(122, 255)] public int size;
    [Range(1, 10)] public int screenScale;
    public int seed;
    [Range(2f, 80f)] public float scale;
    [Range(0, 10)] public float a;
    [Range(0, 10)] public float b;
    [Range(1, 6)] public int octaves;
    [Range(0, 1f)] public float persistance;
    public float lacunarity;
    [Range(0.1f, 50f)] public float meshHeight;
    public Vector2 noiseStep;
    public bool useFallOff = false;
    public bool usePerlin = false;
    public bool useColor = false;
    public AnimationCurve curve;

    IslandBuilder meshMaker;
    MeshCollider collider;
    GameObject isola;
    Material islandMaterial;

    WaterMaker waterMaker;
    GameObject water;
    Material waterMaterial;

    [Range(1, 2)] public int islandType;

    public bool grassUpdate = false;
    public GameObject[] grassSkins;
    [Range(8, 30)] public float grassItemRadius;
    [Range(1,30)] public int grassItemSampleAttempts;

    public bool flowerUpdate = false;
    public GameObject[] greenFlowerSkins;
    public GameObject[] pastelFlowerSkins;

    [Range(10, 40)] public float flowerItemRadius;
    [Range(1, 30)] public int flowerItemSampleAttempts;

    public bool bushUpdate = false;
    public GameObject[] normalBushSkins;
    public GameObject[] greenBushSkins;
    public GameObject[] pastelBushSkins;

    [Range(20, 200)] public float bushItemRadius;
    [Range(1, 30)] public int bushItemSampleAttempts;

    public bool rockUpdate = false;
    public GameObject[] rockSkinsBig;
    public GameObject[] rockSkinsMedium;
    public GameObject[] rockSkinsSmall;

    [Range(20, 300)] public float rockItemRadius;
    [Range(1, 30)] public int rockItemSampleAttempts;

    public bool treeUpdate = false;
    public GameObject[] greenTreeSkinsBig;
    public GameObject[] greenTreeSkinsMedium;
    public GameObject[] greenTreeSkinsSmall;

    public GameObject[] pastelTreeSkinsBig;
    public GameObject[] pastelTreeSkinsMedium;
    public GameObject[] pastelTreeSkinsSmall;


    [Range(20, 200)] public float treeItemRadius;
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
/*        foreach (GameObject o in Object.FindObjectsOfType<GameObject>())
        {
            if(o != GameObject.Find("Main Camera") && o != GameObject.Find("Sun Light")  && o != GameObject.Find("IslandMaker") &&
                o != GameObject.Find("FPSPlayer") && o != GameObject.Find("Main Camera") &&
                o != GameObject.Find("Capsule") && o != GameObject.Find("Sun") &&
                o != GameObject.Find("Moon") && o != GameObject.Find("MoonFace") &&
                o != GameObject.Find("Moon Light") && o != GameObject.Find("MoonMesh")
                && o != GameObject.Find("MoonEffect") && o != GameObject.Find("NightLight")
                && o != GameObject.Find("MoonHolder"))
            {
               // Destroy(o);

            }
        }*/
        GenerateIsland();
        GenerateWater();
        MakeNewIsland();


    }

    public void GenerateIsland()
    {
        if (GameObject.Find("isola"))
        {
            Destroy(GameObject.Find("isola"));
        }
        isola = new GameObject("isola");
        
        islandMaterial = Resources.Load<Material>("Materials/Isola Materials/IsolaTerrain");
        isola.AddComponent<MeshRenderer>().sharedMaterial = islandMaterial;
        isola.AddComponent<MeshFilter>();
        collider = isola.AddComponent<MeshCollider>();
        isola.GetComponent<MeshFilter>().sharedMesh = new Mesh();
        meshMaker = new IslandBuilder(isola.GetComponent<MeshFilter>().sharedMesh, size, useFallOff, usePerlin, useColor, a, b, scale, noiseStep, seed, meshHeight, curve, octaves, persistance, lacunarity);
        collider.sharedMesh = isola.GetComponent<MeshFilter>().sharedMesh;
        isola.transform.localScale = new Vector3(screenScale, screenScale, screenScale);
    }

    public void GenerateWater()
    {
        if (GameObject.Find("water"))
        {
            Destroy(GameObject.Find("water"));
        }
        if (islandType == 1)
        {
            water = new GameObject("water");
            water.AddComponent<MeshFilter>();
            water.GetComponent<MeshFilter>().sharedMesh = new Mesh();
            waterMaker = new WaterMaker(water.GetComponent<MeshFilter>().sharedMesh, size);
            waterMaterial = Resources.Load<Material>("Materials/Isola Materials/TransparentWater");
            water.AddComponent<MeshRenderer>().sharedMaterial = waterMaterial;
            water.transform.localScale = new Vector3(screenScale, screenScale, screenScale);
            water.transform.position = new Vector3(0, screenScale * 1.725f, 0);
        }
        else if (islandType == 2)
        {
            water = new GameObject("water");
            water.AddComponent<MeshFilter>();
            water.GetComponent<MeshFilter>().sharedMesh = new Mesh();
            waterMaker = new WaterMaker(water.GetComponent<MeshFilter>().sharedMesh, size);
            waterMaterial = Resources.Load<Material>("Materials/Isola Materials/CartoonWater");
            water.AddComponent<MeshRenderer>().sharedMaterial = waterMaterial;
            water.transform.localScale = new Vector3(screenScale, screenScale, screenScale);
            water.transform.position = new Vector3(0, screenScale * 1.65f, 0);
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

        if (bushUpdate)
        {
            List<Vector2> bushPoints = PoissonDiscSampling.GeneratePoints(bushItemRadius, regionSize, bushItemSampleAttempts);
            List<Vector3> bushMap = Converter(bushPoints);
            List<Vector3> finalBushPoints = ObjectHeightAdjuster(bushMap);
            BushPlacer(finalBushPoints);
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
            List<Vector3> treeMap = Converter(treePoints);
            List<Vector3> finalTreePoints = ObjectHeightAdjuster(treeMap);
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
            if (naturePoints[i].y >= 33 && naturePoints[i].y < 60)
            {
                Vector3 position = new Vector3(naturePoints[i].x, naturePoints[i].y, naturePoints[i].z);
                Vector3 scale = Vector3.one * (size / meshHeight);
                Vector3 rotation = new Vector3(Random.Range(0, 10f), Random.Range(0, 360f), Random.Range(0, 10f));
                GameObject natureThing = Instantiate(grassSkins[Random.Range(0, grassSkins.Length)]);
                natureObjects[i] = natureThing;
                natureObjects[i].transform.parent = grass.transform;
                natureObjects[i].transform.position = position;
                natureObjects[i].transform.localScale = (scale * Random.Range(0.8f, 1.2f));
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
        for (int i = 0; i < naturePoints.Count; i++)
        {
            //int randomFlower = Random.Range(1, 100);
            if (naturePoints[i].y >= 25 && naturePoints[i].y < 80)
            {
                Debug.Log(flower.gameObject.transform.childCount);
                Vector3 position = new Vector3(naturePoints[i].x, naturePoints[i].y, naturePoints[i].z);
                Vector3 scale = Vector3.one * size / (screenScale + meshHeight);
                Vector3 rotation = new Vector3(Random.Range(0, 10f), Random.Range(0, 360f), Random.Range(0, 10f));

                if (islandType == 1)
                {
                    GameObject natureThing = Instantiate(greenFlowerSkins[Random.Range(0, greenFlowerSkins.Length)]);
                    natureObjects[i] = natureThing;
                    natureObjects[i].transform.parent = flower.transform;
                    natureObjects[i].transform.position = position;
                    natureObjects[i].transform.localScale = (scale * Random.Range(0.9f, 1.25f));
                    natureObjects[i].transform.eulerAngles = rotation;
                }
                else if(islandType == 2)
                {
                    GameObject natureThing = Instantiate(pastelFlowerSkins[Random.Range(0, pastelFlowerSkins.Length)]);
                    natureObjects[i] = natureThing;
                    natureObjects[i].transform.parent = flower.transform;
                    natureObjects[i].transform.position = position;
                    natureObjects[i].transform.localScale = (scale * Random.Range(0.9f, 1.25f));
                    natureObjects[i].transform.eulerAngles = rotation;
                }
            }
        }
    }

    void BushPlacer(List<Vector3> naturePoints)
    {
        GameObject[] natureObjects = new GameObject[naturePoints.Count];
        if (GameObject.Find("Bush Parent"))
        {
            Destroy(GameObject.Find("Bush Parent"));
        }
        GameObject Bush = new GameObject("Bush Parent");
        for (int i = 0; i < naturePoints.Count; i++)
        {
            int bushRandom = Random.Range(1, 100);
            if (naturePoints[i].y >= 25 && naturePoints[i].y < 80)
            {
                Debug.Log(Bush.gameObject.transform.childCount);
                Vector3 position = new Vector3(naturePoints[i].x, naturePoints[i].y, naturePoints[i].z);
                Vector3 scale = Vector3.one * size / (screenScale + meshHeight);
                Vector3 rotation = new Vector3(Random.Range(0, 10f), Random.Range(0, 360f), Random.Range(0, 10f));
                if (islandType == 1)
                {
                    if(bushRandom <= 75)
                    {
                        GameObject natureThing = Instantiate(normalBushSkins[Random.Range(0, normalBushSkins.Length)]);
                        natureObjects[i] = natureThing;
                        natureObjects[i].transform.parent = Bush.transform;
                        natureObjects[i].transform.position = position;
                        natureObjects[i].transform.localScale = (scale * Random.Range(0.9f, 1.3f));
                        natureObjects[i].transform.eulerAngles = rotation;
                    }
                    else
                    {
                        GameObject natureThing = Instantiate(greenBushSkins[Random.Range(0, greenBushSkins.Length)]);
                        natureObjects[i] = natureThing;
                        natureObjects[i].transform.parent = Bush.transform;
                        natureObjects[i].transform.position = position;
                        natureObjects[i].transform.localScale = (scale * Random.Range(0.9f, 1.3f));
                        natureObjects[i].transform.eulerAngles = rotation;
                    }

                }
                else if (islandType == 2)
                {
                    if (bushRandom >= 75)
                    {
                        GameObject natureThing = Instantiate(normalBushSkins[Random.Range(0, normalBushSkins.Length)]);
                        natureObjects[i] = natureThing;
                        natureObjects[i].transform.parent = Bush.transform;
                        natureObjects[i].transform.position = position;
                        natureObjects[i].transform.localScale = (scale * Random.Range(0.9f, 1.3f));
                        natureObjects[i].transform.eulerAngles = rotation;
                    }
                    else
                    {
                        GameObject natureThing = Instantiate(pastelBushSkins[Random.Range(0, pastelBushSkins.Length)]);
                        natureObjects[i] = natureThing;
                        natureObjects[i].transform.parent = Bush.transform;
                        natureObjects[i].transform.position = position;
                        natureObjects[i].transform.localScale = (scale * Random.Range(0.9f, 1.3f));
                        natureObjects[i].transform.eulerAngles = rotation;
                    }
                }
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
            int rockRandom = Random.Range(1, 100);
            if (naturePoints[i].y >= 50 && naturePoints[i].y < 100)
            {
                Vector3 position = new Vector3(naturePoints[i].x, naturePoints[i].y, naturePoints[i].z);
                Vector3 scale = Vector3.one * size / meshHeight;
                Vector3 rotation = new Vector3(Random.Range(0, 10f), Random.Range(0, 360f), Random.Range(0, 10f));
                if (rockRandom >= 92)
                {
                    GameObject natureThing = Instantiate(rockSkinsBig[Random.Range(0, rockSkinsBig.Length)]);
                    natureObjects[i] = natureThing;
                    natureObjects[i].transform.parent = rock.transform;
                    natureObjects[i].transform.position = position;
                    natureObjects[i].transform.localScale = (scale * Random.Range(0.8f, 1.3f));
                    natureObjects[i].transform.eulerAngles = rotation;
                }
                else if (rockRandom >= 75)
                {
                    GameObject natureThing = Instantiate(rockSkinsMedium[Random.Range(0, rockSkinsMedium.Length)]);
                    natureObjects[i] = natureThing;
                    natureObjects[i].transform.parent = rock.transform;
                    natureObjects[i].transform.position = position;
                    natureObjects[i].transform.localScale = (scale * Random.Range(0.75f, 1.25f));
                    natureObjects[i].transform.eulerAngles = rotation;
                }
                else if (rockRandom < 75)
                {
                    GameObject natureThing = Instantiate(rockSkinsSmall[Random.Range(0, rockSkinsSmall.Length)]);
                    natureObjects[i] = natureThing;
                    natureObjects[i].transform.parent = rock.transform;
                    natureObjects[i].transform.position = position;
                    natureObjects[i].transform.localScale = (scale * Random.Range(0.75f, 1.25f));
                    natureObjects[i].transform.eulerAngles = rotation;
                }
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
            int treeRandom = Random.Range(1, 100);
            if(naturePoints[i].y >= 45 && naturePoints[i].y < 90)
            {
                Vector3 position = new Vector3(naturePoints[i].x, naturePoints[i].y, naturePoints[i].z);
                Vector3 scale = Vector3.one * size / 22;
                Vector3 rotation = new Vector3(Random.Range(0, 10f), Random.Range(0, 360f), Random.Range(0, 10f));
                if(islandType == 1)
                {
                    if(treeRandom >= 95)
                    {
                        GameObject natureThing = Instantiate(greenTreeSkinsBig[Random.Range(0, greenTreeSkinsBig.Length)]);
                        natureObjects[i] = natureThing;
                        natureObjects[i].transform.parent = tree.transform;
                        natureObjects[i].transform.position = position;
                        natureObjects[i].transform.localScale = (scale * Random.Range(0.75f, 1.25f));
                        natureObjects[i].transform.eulerAngles = rotation;
                    }
                    else if(treeRandom >= 80)
                    {
                        GameObject natureThing = Instantiate(greenTreeSkinsMedium[Random.Range(0, greenTreeSkinsMedium.Length)]);
                        natureObjects[i] = natureThing;
                        natureObjects[i].transform.parent = tree.transform;
                        natureObjects[i].transform.position = position;
                        natureObjects[i].transform.localScale = (scale * Random.Range(0.75f, 1.25f));
                        natureObjects[i].transform.eulerAngles = rotation;
                    }
                    else if (treeRandom < 80)
                    {
                        GameObject natureThing = Instantiate(greenTreeSkinsSmall[Random.Range(0, greenTreeSkinsSmall.Length)]);
                        natureObjects[i] = natureThing;
                        natureObjects[i].transform.parent = tree.transform;
                        natureObjects[i].transform.position = position;
                        natureObjects[i].transform.localScale = (scale * Random.Range(0.75f, 1.25f));
                        natureObjects[i].transform.eulerAngles = rotation;
                    }
                }
                if (islandType == 2)
                {
                    if (treeRandom >= 95)
                    {
                        GameObject natureThing = Instantiate(pastelTreeSkinsBig[Random.Range(0, pastelTreeSkinsBig.Length)]);
                        natureObjects[i] = natureThing;
                        natureObjects[i].transform.parent = tree.transform;
                        natureObjects[i].transform.position = position;
                        natureObjects[i].transform.localScale = (scale * Random.Range(0.75f, 1.25f));
                        natureObjects[i].transform.eulerAngles = rotation;
                    }
                    else if (treeRandom >= 80)
                    {
                        GameObject natureThing = Instantiate(pastelTreeSkinsMedium[Random.Range(0, pastelTreeSkinsMedium.Length)]);
                        natureObjects[i] = natureThing;
                        natureObjects[i].transform.parent = tree.transform;
                        natureObjects[i].transform.position = position;
                        natureObjects[i].transform.localScale = (scale * Random.Range(0.75f, 1.25f));
                        natureObjects[i].transform.eulerAngles = rotation;
                    }
                    else if (treeRandom < 80)
                    {
                        GameObject natureThing = Instantiate(pastelTreeSkinsSmall[Random.Range(0, pastelTreeSkinsSmall.Length)]);
                        natureObjects[i] = natureThing;
                        natureObjects[i].transform.parent = tree.transform;
                        natureObjects[i].transform.position = position;
                        natureObjects[i].transform.localScale = (scale * Random.Range(0.75f, 1.25f));
                        natureObjects[i].transform.eulerAngles = rotation;
                    }
                }
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
            
            if(Physics.Raycast(ray, out hitInfo, 1100) && hitInfo.collider.name == "isola")
            {
                newMap.Add(new Vector3(objectMap[i].x, hitInfo.point.y - (size * 0.0025f), objectMap[i].z));
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

/*    [HideInInspector] public float turnSpeed = 1.0f;
    [HideInInspector] public float moveSpeed = 20.0f;
    [HideInInspector] public float minTurnAngle = -120.0f;
    [HideInInspector] public float maxTurnAngle = 120.0f;
    //[HideInInspector] private float rotX;*/

}
