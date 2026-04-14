using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LoadDeathLocations))]
public class LoadDeathLocationsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LoadDeathLocations deathLocator = (LoadDeathLocations)target;
        if (GUILayout.Button("Click Me"))
        {
            deathLocator.MyButtonAction();
        }
    }
}