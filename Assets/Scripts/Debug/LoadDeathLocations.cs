using System;
using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class LoadDeathLocations : MonoBehaviour
{
    [SerializeField] private GameObject deathMarkerObject; // Object used to mark death location

    public void GenerateMarkers()
    {
        string path = Path.Combine(Application.persistentDataPath, "Analysis");


        if (!Directory.Exists(path))
        {
            Debug.LogWarning("Persistent data path does not exist: " + path);
            return;
        }

        print(path);

        string[] files = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);

        foreach (string file in files)
        {
            Debug.Log(file);

            string fullPath = Path.Combine(path, file);

            string dataToLoad = "";
            using (FileStream stream = new FileStream(fullPath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    dataToLoad = reader.ReadToEnd();
                }
            }

            GameData loadedData = JsonUtility.FromJson<GameData>(dataToLoad);

            foreach (Vector2 deathLocation in loadedData.deathLocations)
            {
                Instantiate(deathMarkerObject, deathLocation, Quaternion.identity, transform);
            }
            
        }

    }


    /// <summary>
    /// Remove all children objects
    /// </summary>
    public void RemoveMarkers()
    {
        while (transform.childCount > 0) // Keep removing children until none are left
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }


}
#endif