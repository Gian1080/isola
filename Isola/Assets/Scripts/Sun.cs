using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Sun : MonoBehaviour
{
    public VisualEffect sunEffect;
    [Range(1, 2000)] public int sunHeigth;
    [Range(1, 200)] public int sunSpeed;
    [Range(1,100)] public int scaler;
    GameObject dayLight;
    Light mylight;
    private void Awake()
    {
        
    }

    void Start()
    {

        if(GameObject.Find("Directional Light"))
        {
            dayLight = GameObject.Find("Directional Light");
            mylight = GameObject.Find("Directional Light").GetComponent<Light>();
            print("TRUE FIND LIGHT");
        }
        sunEffect.transform.position = new Vector3(0, sunHeigth, 0);
        sunEffect.transform.localScale = new Vector3(scaler, scaler, scaler);
        dayLight.transform.parent = sunEffect.transform;
        dayLight.transform.position = sunEffect.transform.position;
        dayLight.transform.rotation.SetLookRotation(new Vector3(0, -2000, 0));
        

    }

    private void Update()
    {
        sunEffect.transform.RotateAround(new Vector3(0, 0, 0), Vector3.forward, sunSpeed * Time.deltaTime);
        dayLight.transform.position = sunEffect.transform.position;
        if(dayLight.transform.position.y >= -100.0f)
        {
            mylight.intensity = Mathf.Clamp((dayLight.transform.position.y / sunHeigth), 0.5f, 1.1f);
            print(mylight.intensity);
            //print(dayLight.transform.position.y + " Height");
        }
        else
        {
            mylight.intensity = 0.001f;
        }
    }

}
