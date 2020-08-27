using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Moon))]
public class MoonEditor : Editor
{

    Moon moon;
    Editor shapeEditor;
    Editor colourEditor;

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if (check.changed)
            {
                moon.GenerateMoon();
            }
        }

        if (GUILayout.Button("Generate Moon"))
        {
            moon.GenerateMoon();
        }

        DrawSettingsEditor(moon.shapeSettings, moon.OnShapeSettingsUpdated, ref moon.shapeSettingsFoldout, ref shapeEditor);
        DrawSettingsEditor(moon.colourSettings, moon.OnColourSettingsUpdated, ref moon.colourSettingsFoldout, ref colourEditor);
    }

    void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor)
    {
        if (settings != null)
        {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                if (foldout)
                {
                    CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();

                    if (check.changed)
                    {
                        if (onSettingsUpdated != null)
                        {
                            onSettingsUpdated();
                        }
                    }
                }
            }
        }
    }

    private void OnEnable()
    {
        moon = (Moon)target;
    }
}