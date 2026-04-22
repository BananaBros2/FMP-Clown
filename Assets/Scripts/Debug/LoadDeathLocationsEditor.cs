using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
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
#endif