﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonColorGenerator
{
    MoonColorSettings settings;
    Texture2D texture;
    const int textureResolution = 50;
    public void UpdateSettings(MoonColorSettings settings)
    {
        this.settings = settings;
        if (texture == null)
        {
            this.texture = new Texture2D(textureResolution, 1);
        }
    }

    public void UpdateElevation(MoonMinMax elevationMinMax)
    {
        settings.moonMaterial.SetVector("_elevationMinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max));
    }

    public void UpdateColors()
    {
        Color[] colors = new Color[textureResolution];
        for (int i = 0; i < textureResolution; i++)
        {
            colors[i] = settings.gradient.Evaluate(i / (textureResolution - 1f));
        }
        texture.SetPixels(colors);
        texture.Apply();
        settings.moonMaterial.SetTexture("_moonTexture", texture);
    }
}
