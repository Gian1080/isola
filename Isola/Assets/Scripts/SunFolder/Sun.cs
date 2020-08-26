using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Sun : MonoBehaviour
{
    public VisualEffect sunEffect;
    [Range(1, 2000)] public int sunHeigth;
    [Range(1, 200)] public int sunSpeed;
    [Range(1,100)] public int sunScale;
    GameObject dayLight;
    Light sunLight;
    private void Awake()
    {
        
    }

    void Start()
    {

        if(GameObject.Find("Sun Light"))
        {
            dayLight = GameObject.Find("Sun Light");
            sunLight = GameObject.Find("Sun Light").GetComponent<Light>();
        }
        sunEffect.transform.position = new Vector3(0, sunHeigth, 0);
        sunEffect.transform.localScale = new Vector3(sunScale, sunScale, sunScale);
        dayLight.transform.parent = sunEffect.transform;
        dayLight.transform.position = sunEffect.transform.position;
        dayLight.transform.rotation.SetLookRotation(new Vector3(0, -2000, 0));
        

    }

    private void Update()
    {
        sunEffect.transform.RotateAround(new Vector3(0, 0, 0), Vector3.forward, sunSpeed * Time.deltaTime);
        dayLight.transform.position = sunEffect.transform.position;
        if(dayLight.transform.position.y >= -50.0f)
        {
            sunLight.intensity = Mathf.Clamp((dayLight.transform.position.y / sunHeigth), 0.5f, 1.1f);
        }
        else
        {
            sunLight.intensity = 0.001f;
        }
    }

}
