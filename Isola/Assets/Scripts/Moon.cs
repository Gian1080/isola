using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon : MonoBehaviour
{
    [Range(2, 255)] public int resolution;
    [Range(1, 100)] public int moonScale;
    [Range(1, 2000)] public int moonHeight;
    [SerializeField, HideInInspector] MeshFilter[] meshFilters;
    MoonFace[] moonFaces;
    Material moonMaterial;
    GameObject nightLight;
    Light moonLight;

    private void OnValidate()
    {
        //Initialize();
        //GenerateMesh();
    }

    private void Start()
    {
        Initialize();
        GenerateMesh();
        if (GameObject.Find("Moon Light"))
        {
            nightLight = GameObject.Find("Moon Light");
            moonLight = GameObject.Find("Moon Light").GetComponent<Light>();
        }
       // sunEffect.transform.position = new Vector3(0, moonHeight, 0);
       // sunEffect.transform.localScale = new Vector3(moonScale, moonScale, moonScale);
        //nightLight.transform.parent = sunEffect.transform;
       // nightLight.transform.position = sunEffect.transform.position;
        nightLight.transform.rotation.SetLookRotation(new Vector3(0, -2000, 0));




        transform.localScale = new Vector3(moonScale, moonScale, moonScale);
        transform.position = new Vector3(0, -1500, 0);
    }

    void Initialize()
    {
        if(meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        moonFaces = new MoonFace[6];
        Vector3[] direction = {Vector3.up, Vector3.down, Vector3.left,
                                Vector3.right, Vector3.forward, Vector3.back } ;

        for (int i = 0; i < 6; i++)
        {
            if(meshFilters[i] == null)
            {
                GameObject meshObject = new GameObject("MoonMesh");
                meshObject.transform.parent = transform;
                moonMaterial = Resources.Load<Material>("Materials/CartoonWater");
                meshObject.AddComponent<MeshRenderer>().sharedMaterial = moonMaterial;
                meshFilters[i] = meshObject.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }
            moonFaces[i] = new MoonFace(meshFilters[i].sharedMesh, resolution, direction[i]);
        }
    }

    void GenerateMesh()
    {
        foreach(MoonFace face in moonFaces)
        {
            face.ConstructMesh();
        }
    }
}
