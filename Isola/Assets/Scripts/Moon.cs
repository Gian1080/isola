using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Moon : MonoBehaviour
{
    
    [Range(2, 255)] public int resolution;

    [SerializeField, HideInInspector] MeshFilter[] meshFilters;
    MoonFace[] moonFaces;
    Material moonMaterial;


    private void OnValidate()
    {
        //Initialize();
        //GenerateMesh();
    }

    private void Start()
    {
        Initialize();
        GenerateMesh();

    }

    private void Update()
    {
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
                print("Face made " + i.ToString());
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
