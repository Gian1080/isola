using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon : MonoBehaviour
{

    [Range(2, 256)]
    public int resolution = 10;
    public bool autoUpdate = true;

    public MoonShapeSettings shapeSettings;
    public MoonColorSettings colourSettings;

    [HideInInspector] public bool shapeSettingsFoldout;
    [HideInInspector] public bool colourSettingsFoldout;

    MoonShapeGenerator moonShapeGenerator = new MoonShapeGenerator();
    MoonColorGenerator moonColorGenerator = new MoonColorGenerator();

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    MoonTerrainFace[] moonFaces;

    private void Start()
    {
        GenerateMoon();
        transform.localScale = new Vector3(1, 1, 1);
        transform.position = GameObject.Find("Sun").transform.position;
    }

    private void Update()
    {
        transform.Rotate(0, -Time.deltaTime * 3, 0, Space.World);
    }

    void Initialize()
    {
        moonShapeGenerator.UpdateSettings(shapeSettings);
        moonColorGenerator.UpdateSettings(colourSettings);
        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        moonFaces = new MoonTerrainFace[6];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("MoonMesh");
                meshObj.transform.parent = transform;
                meshObj.transform.localScale = new Vector3(10, 10, 10);
                meshObj.AddComponent<MeshRenderer>();
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }
            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colourSettings.moonMaterial;
            moonFaces[i] = new MoonTerrainFace(moonShapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i]);

        }
    }

    public void GenerateMoon()
    {
        Initialize();
        GenerateMesh();
        GenerateColours();
    }

    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }

    public void OnColourSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateColours();
        }
    }

    void GenerateMesh()
    {
        foreach (MoonTerrainFace face in moonFaces)
        {
            face.ConstructMesh();
        }
        moonColorGenerator.UpdateElevation(moonShapeGenerator.elevationMinMax);
    }

    void GenerateColours()
    {
        moonColorGenerator.UpdateColors();
    }
}