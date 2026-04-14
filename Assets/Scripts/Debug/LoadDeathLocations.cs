using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LoadDeathLocations : MonoBehaviour
{
    public GameObject deathMarkerObject;

    public void MyButtonAction()
    {
        Debug.Log("Button clicked!");
        GenerateMarkers();
    }

    private void GenerateMarkers()
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





        //        public void LoadData(GameData data)
        //{
        //    this.deathCount = data.deathCount;
        //    this.deathLocations = data.deathLocations;
        //}

        //public void SaveData(ref GameData data)
        //{
        //    data.deathCount = this.deathCount;
        //    data.deathLocations = this.deathLocations;
        //}

    }

}
