using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Isola : MonoBehaviour
{
    [Range(2, 256)]
    public int resolution;
    public bool autoUpdate = false;
    private bool islandCreated = false;

    GameObject isola;
    MeshMaker meshMaker;

    private void OnValidate()
    {
        BuildUp();
        MakeNew();
    }

    public void BuildUp()
    {
        if (isola == null && islandCreated == false)
        {
            resolution = 10;
            islandCreated = true;
            isola = new GameObject("isola");
            isola.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
            isola.AddComponent<MeshFilter>();
            isola.GetComponent<MeshFilter>().sharedMesh = new Mesh();
            meshMaker = new MeshMaker(isola.GetComponent<MeshFilter>().sharedMesh, resolution);
        }
        else
        {
            meshMaker = new MeshMaker(isola.GetComponent<MeshFilter>().sharedMesh, resolution);
        }
    }

    void MakeNew()
    {
        meshMaker.MeshBuilder();
    }
}
