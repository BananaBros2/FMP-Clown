using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(LoadDeathLocations))]
public class LoadDeathLocationsEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LoadDeathLocations deathLocator = (LoadDeathLocations)target;
        if (GUILayout.Button("Generate Markers")) // Generate markers when button is clicked
        {
            deathLocator.GenerateMarkers();
        }
        if (GUILayout.Button("Remove Markers")) // Destroy all markers (children) when button is clicked
        {
            deathLocator.RemoveMarkers();
        }
    }

}
#endif