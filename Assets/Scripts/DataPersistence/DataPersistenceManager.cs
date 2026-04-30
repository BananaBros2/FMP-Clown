using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    /// <summary>
    /// Make DataPersistenceManager an instance
    /// </summary>
    public static DataPersistenceManager instance { get; private set; }

    [Header("File Storage Config")]

    [SerializeField] private string curFileName;


    public GameData currentLoadedData;

    private List<IDataPersistence> dataPersistenceObjects;

    private FileDataHandler dataHandler;

    public string currentPlaytime = "Unknown";


    string savePath;



    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects =
            FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }






    private void Awake()
    {
        if (instance != null && instance != this) // Check if DPO already exists
        {
            Destroy(gameObject);
            Debug.LogWarning("Already existing data manager");
            return;
        }

        instance = this; // Set as permanant instance
        DontDestroyOnLoad(gameObject);

        this.dataHandler = new FileDataHandler();
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();

        savePath = Path.Combine(Application.persistentDataPath, "Save Data"); // Make save directory path 
        CheckSaveLocation();

    }

    /// <summary>
    /// Check save directory, create if not found
    /// </summary>
    private void CheckSaveLocation()
    {
        string savePath = Path.Combine(Application.persistentDataPath, "Save Data"); // Make save directory 

        if (!Directory.Exists(savePath)) // Ensure location exists, create if not
        {
            Directory.CreateDirectory(savePath);
        }
    }




    public GameData GetSaveData(int saveNumber)
    {

        // Attempt to load save data
        string fileName = "Save" + saveNumber + ".sav";
        print(dataHandler);
        dataHandler.SetDataHandlerLocation(savePath, fileName);
        GameData checkedGameData = dataHandler.Load();

        // Check if save data exists
        if (checkedGameData == null) 
        {
            Debug.LogWarning("No save data found for slot " + saveNumber);
        }

        return checkedGameData;
    }


    public void HandleSaveFilePressed(int saveNumber)
    {
        // Attempt to load save data
        string fileName = "Save" + saveNumber + ".sav";

        dataHandler.SetDataHandlerLocation(savePath, fileName);
        GameData checkedGameData = dataHandler.Load();

        // Check if save data exists to determine action
        if (checkedGameData == null)
        {
            Debug.Log("Creating new save data for slot " + saveNumber);
            CreateNewSave(fileName);
        }
        else
        {
            Debug.Log("Loading data in slot " + saveNumber);
            LoadSaveFile(fileName);
        }



    }



    public void CreateNewSave(string fileName)
    {
        this.currentLoadedData = new GameData();
        SaveGame();

    }

    public void LoadSaveFile(string fileName)
    {
        // Load the data
        dataHandler.SetDataHandlerLocation(savePath, fileName);
        this.currentLoadedData = dataHandler.Load();

        if (this.currentLoadedData == null) // Check that save data is valid
        {
            Debug.LogError("No save data found?");
            return;
        }

        //foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        //{
        //    dataPersistenceObj.LoadData(gameData);
        //}

        //print(gameData.deathCount);
        //gameData.deathCount++;
        SaveGame();
        //print(gameData.deathCount);
    }




    public void SaveGame()
    {
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref currentLoadedData);
        }

        dataHandler.Save(currentLoadedData);

    }


    /// <summary>
    /// Find and return data of requested manuscript collection status
    /// </summary>
    /// <param name="level">ID of level</param>
    /// <param name="manuscriptID">ID of the manuscript in the level</param>
    /// <returns></returns>
    public bool GetManuscriptData(int level, int manuscriptID)
    {
        switch (level) // Locate specified manuscript status
        {
            case 0:
                return currentLoadedData.tutorialData.manuscriptsCollected[manuscriptID];
            case 1:
                return currentLoadedData.level1Data.manuscriptsCollected[manuscriptID];
            case 2:
                return currentLoadedData.level2Data.manuscriptsCollected[manuscriptID];
            case 3:
                return currentLoadedData.level3Data.manuscriptsCollected[manuscriptID];
            default:
                return false;
        }

    }









    //public void StartSaveFile(int saveNumber)
    //{


    //    //this.dataHandler = new FileDataHandler(savePath, "Save" + saveNumber + ".sav");
    //    //this.gameData = new GameData();
    //    //SaveGame();

    //}



    //public void LoadGame()
    //{
    //    this.gameData = dataHandler.Load();

    //    if (this.gameData == null)
    //    {
    //        Debug.LogWarning("No save data found");
    //        CreateSave();
    //    }

    //    foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
    //    {
    //        dataPersistenceObj.LoadData(gameData);
    //    }

    //}












    //public void DeleteSaveFile()
    //{

    //}















    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
