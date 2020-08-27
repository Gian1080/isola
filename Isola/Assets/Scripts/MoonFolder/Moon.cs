using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Moon : MonoBehaviour
{
    [Range(100, 2000)] public int moonHeight;
    [Range(1, 100)] public int moonSpeed;
    [Range(2, 256)] public int resolution = 10;
    public bool autoUpdate = true;
    public VisualEffect moonEffect;
    public MoonShapeSettings shapeSettings;
    public MoonColorSettings colourSettings;
    [HideInInspector] public bool shapeSettingsFoldout;
    [HideInInspector] public bool colourSettingsFoldout;

    MoonShapeGenerator moonShapeGenerator = new MoonShapeGenerator();
    MoonColorGenerator moonColorGenerator = new MoonColorGenerator();
    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    MoonTerrainFace[] moonFaces;

    GameObject moonLooker;
    GameObject moonLight;
    Light light;
    GameObject lightLookDirection;

    private void Start()
    {
        moonLight = new GameObject("MoonLight");
        light = moonLight.AddComponent<Light>();
        moonLight.GetComponent<Light>().type = LightType.Directional;
        moonLight.GetComponent<Light>().intensity = 0.5f;
        moonLight.transform.position = transform.position;

        moonLooker = new GameObject("MoonLooker");
        moonLooker.transform.position = new Vector3(16000, 100, 0);
        GenerateMoon();
        moonEffect.transform.position = transform.position;
        moonEffect.transform.parent = transform;
        transform.localScale = new Vector3(1, 1, 1);
        transform.position = new Vector3(0, -moonHeight * 2.2f, 0);
        Color color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        light.color = color;
        lightLookDirection = new GameObject("MoonLightLookDirection");
        lightLookDirection.transform.position = new Vector3(-7500, -7500, 0);
    }

    private void Update()
    {

        transform.RotateAround(new Vector3(0, 0, 0), Vector3.forward, moonSpeed * Time.deltaTime);
        transform.LookAt(moonLooker.transform.position);
        moonLight.transform.position = transform.position;
        moonLight.transform.LookAt(lightLookDirection.transform.position);
        if (moonLight.transform.position.y >= 0.0f)
        {
            light.intensity = Mathf.Clamp((moonLight.transform.position.y / moonHeight), 0.1f, 0.3f);
        }
        else
        {
            light.intensity = 0.001f;
        }
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