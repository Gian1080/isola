using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Sun : MonoBehaviour
{
    [Range(100, 2000)] public int sunHeigth;
    [Range(1, 100)] public int sunSpeed;
    [Range(1,400)] public int sunScale;

    public VisualEffect sunEffect;
    GameObject lightLookDirection;
    GameObject sunLight;
    Light light;
    VFXEventAttribute sunRise;


    void Start()
    {
        sunLight = new GameObject("SunLight");
        light = sunLight.AddComponent<Light>();
        sunLight.GetComponent<Light>().type = LightType.Directional;
        sunLight.GetComponent<Light>().intensity = 0.5f;
        sunLight.transform.position = transform.position;

        sunEffect.transform.position = new Vector3(0, sunHeigth * 1.1f, 0);
        sunEffect.transform.localScale = new Vector3(sunScale / 2, sunScale / 2, sunScale / 2);

        Color color = new Color(0.8f, 0.8f, 0.8f, 0.2f);
        light.color = color;
        lightLookDirection = new GameObject("sunLightLookDirection");
        lightLookDirection.transform.position = new Vector3(-7500, -7500, 0);
        
    }

    private void Update()
    {
        transform.RotateAround(new Vector3(0, 0, 0), Vector3.forward, sunSpeed * Time.deltaTime);
        sunLight.transform.position = transform.position;
        sunLight.transform.LookAt(lightLookDirection.transform.position);
        if (sunLight.transform.position.y >= 0.0f)
        {
            light.intensity = Mathf.Clamp((sunLight.transform.position.y / sunHeigth), 0.1f, 0.8f);

        }
        else
        {
            light.intensity = 0.001f;
        }
        if (sunLight.transform.position.y > 0.0f && sunLight.transform.position.x > 0 )
        {
            
            print("DAWN");
            sunEffect.Play();
        }
        else if(sunLight.transform.position.y < sunHeigth / 2 && sunLight.transform.position.x < 100.0f)
        {
            print("SunSet");
            sunEffect.Stop();
        }
    }

}
