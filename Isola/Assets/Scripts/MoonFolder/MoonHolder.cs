using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


public class MoonHolder : MonoBehaviour
{
    public VisualEffect moonEffect;
    GameObject nightLight;
    Light moonLight;
    [Range(1, 2000)] public int moonHeight;
    [Range(1, 100)] public int moonSpeed;
    [Range(1, 100)] public int moonScale;
    void Start()
    {
        if (GameObject.Find("Moon Light"))
        {
            nightLight = GameObject.Find("Moon Light");
            moonLight = GameObject.Find("Moon Light").GetComponent<Light>();
        }
        moonEffect.transform.localScale = new Vector3(moonScale, moonScale, moonScale);
        moonEffect.transform.position = new Vector3(0, -moonHeight, 0);
        moonEffect.transform.parent = moonEffect.transform;
        nightLight.transform.parent = moonEffect.transform;
        nightLight.transform.position = transform.position;
        
    }

    void Update()
    {
        moonEffect.transform.rotation.SetLookRotation(new Vector3(-100, 90, -270));
        moonEffect.transform.RotateAround(new Vector3(0, 0, 0), Vector3.forward, moonSpeed * Time.deltaTime);
        nightLight.transform.position = moonEffect.transform.position;
        if (nightLight.transform.position.y >= -50.0f)
        {
            moonLight.intensity = Mathf.Clamp((nightLight.transform.position.y / moonHeight), 0.5f, 1.1f) / 2;
        }
        else
        {
            moonLight.intensity = 0;
        }
    }
}
