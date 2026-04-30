using System.IO;
using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;




#if UNITY_EDITOR
using UnityEditor;
public class LoadDeathLocations : MonoBehaviour
{
    [Tooltip("Reference to prefab used to mark death locations")]
    [SerializeField] private GameObject deathMarkerObject;

    [Tooltip("ID for the level\n0: Tutorial\n1: Level 1\n1: Level 2\n1: Level 3")]
    [SerializeField, UnityEngine.Range(0,3)] private int currentLevelID;


    /// <summary>
    /// Read death location data from save files and instantiate a marker at each position
    /// </summary>
    public void GenerateMarkers()
    {
        string path = Path.Combine(Application.persistentDataPath, "Save Data"); // Create directory filepath

        if (!Directory.Exists(path)) // Ensure location exists
        {
            Debug.LogWarning("Save data path does not exist: " + path);
            return;
        }

        string[] files = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly); // Get all files in the directory

        // Check if any files were even in directory, if not; return
        if (files.Length == 0) { Debug.Log("No Savefiles under \"Analysis\" file path "); return;  }


        int totalDeaths = 0;

        foreach (string file in files) // Scan each file
        {
            string dataToLoad = "";
            using (FileStream stream = new FileStream(Path.Combine(path, file), FileMode.Open)) // Open file 
            {
                using (StreamReader reader = new StreamReader(stream)) // Read file
                {
                    dataToLoad = reader.ReadToEnd();
                }
            }


            GameData loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            List<Vector2> deathLocations = new List<Vector2>();

            switch (currentLevelID) // Retrieve correct level death list
            {
                case 0:
                    deathLocations = loadedData.tutorialData.deathLocations;
                    break;
                case 1:
                    deathLocations = loadedData.level1Data.deathLocations;
                    break;
                case 2:
                    deathLocations = loadedData.level2Data.deathLocations;
                    break;
                case 3:
                    deathLocations = loadedData.level3Data.deathLocations;
                    break;
                default:
                    Debug.LogError("Invalid Level ID");
                    break;
            }


            foreach (Vector2 deathLocation in deathLocations) // Read death location data
            {
                Instantiate(deathMarkerObject, deathLocation, Quaternion.identity, transform); // Place locator at position read
                totalDeaths++;
            }

        }

        // Confirmation Text
        Debug.Log(files.Length + " files scanned for a total of " + totalDeaths + " death locators placed");

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
        Debug.Log("Death locators removed");
    }


}
#endif