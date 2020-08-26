using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Moon : MonoBehaviour
{
    [Range(1, 100)] public int moonScale;
    [Range(2, 255)] public int resolution;
    public bool autoUpdate = true;

    [HideInInspector] public bool moonShapeSettingsFoldOut;
    [HideInInspector] public bool moonColorSettingsFoldOut;


    public MoonShapeSettings moonShapeSettings;
    public MoonColorSettings moonColorSettings;
    MoonShapeGenerator moonShapeGenerator;

    [SerializeField, HideInInspector] MeshFilter[] meshFilters;
    MoonFace[] moonFaces;
    Material moonMaterial;


    private void OnValidate()
    {
        GenerateMoon();
    }

    private void Start()
    {
        GenerateMoon();
        transform.localScale = new Vector3(moonScale, moonScale, moonScale);
        transform.position = new Vector3(0, -1250, 0);
    }

    private void Update()
    {
    }


    void Initialize()
    {
        moonShapeGenerator = new MoonShapeGenerator(moonShapeSettings);
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
                moonMaterial = Resources.Load<Material>("Materials/someShit");
                meshObject.AddComponent<MeshRenderer>().sharedMaterial = moonMaterial;
                meshFilters[i] = meshObject.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
                print("Face made " + i.ToString());
            }
            moonFaces[i] = new MoonFace(moonShapeGenerator, meshFilters[i].sharedMesh, resolution, direction[i]);
        }
    }

    public void GenerateMoon()
    {
        Initialize();
        GenerateMesh();
        GenerateColors();
    }

    public void OnShapeSettingsUpdated()
    {
        if(autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }

    public void OnColorSettingsUpdated()
    {
        if(autoUpdate)
        {
            Initialize();
            GenerateColors();
        }
    }

    void GenerateMesh()
    {
        foreach(MoonFace face in moonFaces)
        {
            face.ConstructMesh();
        }
    }

    void GenerateColors()
    {
        foreach(MeshFilter filter in meshFilters)
        {
            filter.GetComponent<MeshRenderer>().sharedMaterial.color = moonColorSettings.moonColor;
        }
    }
}
