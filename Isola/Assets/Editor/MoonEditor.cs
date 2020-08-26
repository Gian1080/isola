using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Moon))]
public class MoonEditor : Editor
{
    Moon moon;
    Editor moonShapeEditor;
    Editor moonColorEditor;

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if(check.changed)
            {
                moon.GenerateMoon();
            }
        }

        if(GUILayout.Button("Generate Moon"))
        {
            moon.GenerateMoon();
        }

        DrawSettingsEditor(moon.moonShapeSettings, moon.OnShapeSettingsUpdated,ref moon.moonShapeSettingsFoldOut, ref moonShapeEditor);
        DrawSettingsEditor(moon.moonColorSettings, moon.OnColorSettingsUpdated,ref moon.moonColorSettingsFoldOut, ref moonColorEditor);

    }

    void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated,ref bool foldOut, ref Editor editor)
    {
        if(settings != null)
        {
            foldOut = EditorGUILayout.InspectorTitlebar(foldOut, settings);
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                if(foldOut)
                {
                    CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();
                }
                if(check.changed)
                {
                    if(onSettingsUpdated != null)
                    {
                        onSettingsUpdated();
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
