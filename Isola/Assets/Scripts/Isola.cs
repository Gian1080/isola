﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Isola : MonoBehaviour
{
    [Range(2, 255)]
    public int size;
    [Range(0, 10)]
    public float a;
    [Range(0, 10)]
    public float b;
    [Range(0.1f, 100f)]
    public float meshHeight;
    [Range(0.001f, 33.0f)]
    public float scale;
    public Vector2 noiseStep;
    public bool autoUpdate = false;
    public bool useFallOff = false;
    public bool usePerlin = false;
    public int seed;
    public AnimationCurve curve;



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
        if (isola == null)
        {
            isola = new GameObject("isola");
            myMaterial = new Material(Shader.Find("Standard"));
            isola.AddComponent<MeshRenderer>().sharedMaterial = myMaterial;
            isola.AddComponent<MeshFilter>();
            isola.GetComponent<MeshFilter>().sharedMesh = new Mesh();
            meshMaker = new MeshMaker(isola.GetComponent<MeshFilter>().sharedMesh, size, useFallOff, usePerlin, a, b, scale, noiseStep, seed, meshHeight, curve);
        }
        else
        {
            meshMaker = new MeshMaker(isola.GetComponent<MeshFilter>().sharedMesh, size, useFallOff, usePerlin, a ,b, scale, noiseStep, seed, meshHeight, curve);
        }
    }

    void MakeNewMesh()
    {
        meshMaker.MeshBuilder();
    }
}
