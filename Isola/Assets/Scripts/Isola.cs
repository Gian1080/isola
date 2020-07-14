using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Isola : MonoBehaviour
{
    [Range(2, 255)]
    public int size;
    [Range(0, 7)]
    public float a;
    [Range(0, 7)]
    public float b;
    public bool autoUpdate = false;
    public bool useFallOff = false;
    GameObject isola;
    MeshMaker meshMaker;
    Material myMaterial;
    private void OnValidate()
    {
        BuildUp();
        MakeNewMesh();
    }


    public void BuildUp()
    {
        //  (Material)Resources.Load("red.mat", typeof(Material));
        if (isola == null)
        {
            isola = new GameObject("isola");
            myMaterial = new Material(Shader.Find("Standard"));
            isola.AddComponent<MeshRenderer>().sharedMaterial = myMaterial;
            isola.AddComponent<MeshFilter>();
            isola.GetComponent<MeshFilter>().sharedMesh = new Mesh();
            meshMaker = new MeshMaker(isola.GetComponent<MeshFilter>().sharedMesh, size, useFallOff, a, b);
        }
        else
        {
            meshMaker = new MeshMaker(isola.GetComponent<MeshFilter>().sharedMesh, size, useFallOff, a ,b);
        }
    }

    void MakeNewMesh()
    {
        meshMaker.MeshBuilder();
    }
}
