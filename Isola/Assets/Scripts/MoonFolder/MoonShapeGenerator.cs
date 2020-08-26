using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonShapeGenerator
{
    MoonShapeSettings moonShapeSettings;

    public MoonShapeGenerator(MoonShapeSettings moonShapeSettings)
    {
        this.moonShapeSettings = moonShapeSettings;
    }

    public Vector3 CalculatePointOnMoon(Vector3 pointOnUnitSphere)
    {
        return pointOnUnitSphere * moonShapeSettings.radius;
    }
}
