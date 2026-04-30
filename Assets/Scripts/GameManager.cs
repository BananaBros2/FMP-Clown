using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IDataPersistence
{
    /// <summary>
    /// Make GameManager an instance
    /// </summary>
    public static GameManager Instance { get; private set; }

    [Tooltip("Player Prefab to spawn")]
    [SerializeField] GameObject playerPrefab;

    
    private GameObject currentPlayerObject;
    public GameObject GetPlayerObject() { return currentPlayerObject; }

    public GameObject cineCam;
    public UIHandler uiHandler;
    [SerializeField] private GameObject blackSquareOfDoom;

    private bool environmentPaused;
    public bool GetEnvironmentPausedStatus() { return environmentPaused; }

    private string currentRoom = "Unknown";
    private int currentCheckpoint = 0;

    private bool sceneLoading = false;


    private int currentLevelID = 0;




    GameData unsavedGameData;

    bool canAlterSaveData = true;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);

        try
        {
            LoadData(DataPersistenceManager.instance.currentLoadedData);
        }
        catch
        {
            Debug.LogWarning("No DataPersistenceManager, saving/loading is disabled");
            canAlterSaveData = false;
        }

        StartLevel();
    }


    #region LEVEL HANDLING

    private void StartLevel()
    {
        Vector3 spawnPosition = Vector3.zero;
        CheckPoint[] checkPoints = FindObjectsByType<CheckPoint>(FindObjectsSortMode.None);
        foreach (CheckPoint checkPoint in checkPoints)
        {
            if (checkPoint.GetID() == 0)
            {
                spawnPosition = checkPoint.GetSpawnPosition();
                break;
            }
        }

        cineCam = GameObject.FindGameObjectWithTag("MainCamera");
        cineCam.GetComponent<CinemachineBrain>().DefaultBlend.Time = 0f;

        SpawnPlayerCharacter(spawnPosition);
    }

    public void ControlDoomSquare(bool activity)
    {
        blackSquareOfDoom.SetActive(activity);
    }

    /// <summary>
    /// Method for specifically turning off the black box blocking the screen 
    /// </summary>
    public void IHateScreenTransitioning() // Name is staying
    {
        ControlDoomSquare(false);
    }

    public void UpdateCheckpoint(int newCheckpoint)
    {
        currentCheckpoint = newCheckpoint;
    }

    public void RespawnAtCheckpoint()
    {
        if (sceneLoading) { return; }

        StartCoroutine(ReloadScene());

    }


    IEnumerator ReloadScene()
    {
        yield return null;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);

        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            //print("Loading: " + (asyncOperation.progress * 100) + "%");

            if (asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;

            }

            yield return null;
        }

        sceneLoading = false;

        bool foundCheckpoint = false;
        Vector3 spawnPosition = Vector3.zero;
        Vector3 startPosition = Vector3.zero;

        CheckPoint[] checkPoints = FindObjectsByType<CheckPoint>(FindObjectsSortMode.None);
        foreach (CheckPoint checkPoint in checkPoints)
        {
            if (checkPoint.GetID() == currentCheckpoint)
            {
                spawnPosition = checkPoint.GetSpawnPosition();
                foundCheckpoint = true;
                break;
            }
            else if (checkPoint.GetID() == 0)
            {
                startPosition = checkPoint.GetSpawnPosition();
            }
        }

        if (!foundCheckpoint)
        {
            spawnPosition = startPosition;
        }


        cineCam = GameObject.FindGameObjectWithTag("MainCamera");
        cineCam.GetComponent<CinemachineBrain>().DefaultBlend.Time = 0f;

        SpawnPlayerCharacter(spawnPosition);
    }

    public void SwitchLevel(int levelID = 0)
    {
        string levelName = "Level 1";

        switch (levelID)
        {
            case 0:
                currentLevelID = 0;
                levelName = "Tutorial";
                break;
            case 1:
                currentLevelID = 1;
                levelName = "Level 1";
                break;
            case 2:
                currentLevelID = 2;
                levelName = "Level 2";
                break;
            case 3:
                currentLevelID = 3;
                levelName = "Level 3";
                break;
            default:
                Debug.LogError("You and I both know this level doesn't exist");
                break;
        }

        StartCoroutine(LoadScene(levelName));

    }

    IEnumerator LoadScene(string levelName)
    {
        yield return null;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(levelName);

        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            //print("Loading: " + (asyncOperation.progress * 100) + "%");

            if (asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;

            }

            yield return null;
        }

        sceneLoading = false;

        currentCheckpoint = 0;
        Vector3 startPosition = Vector3.zero;

        CheckPoint[] checkPoints = FindObjectsByType<CheckPoint>(FindObjectsSortMode.None);
        foreach (CheckPoint checkPoint in checkPoints)
        {
            if (checkPoint.GetID() == 0)
            {
                startPosition = checkPoint.GetSpawnPosition();
                break;
            }
        }

        cineCam = GameObject.FindGameObjectWithTag("MainCamera");
        cineCam.GetComponent<CinemachineBrain>().DefaultBlend.Time = 0f;
        SpawnPlayerCharacter(startPosition);
    }

    private void SpawnPlayerCharacter(Vector3 startPosition)
    {
        currentPlayerObject = Instantiate(playerPrefab, startPosition, Quaternion.identity);
        SetupCameras();
        StartCoroutine(TEMPTOLETPLAYERMOVE());

    }

    private void SetupCameras()
    {
        SetTrackingTarget[] trackingTargetScripts = FindObjectsByType<SetTrackingTarget>(FindObjectsSortMode.None);
        foreach (SetTrackingTarget trackingTargetScript in trackingTargetScripts)
        {
            trackingTargetScript.SetupTarget();
        }
    }


    #endregion LEVEL HANDLING


    #region GAME FUNCTIONALITY


    public void HandleRoomTransition(string sourceRoom)
    {
        if (currentRoom == sourceRoom) { return; }
        currentRoom = sourceRoom;
        currentPlayerObject.GetComponent<MovementController>().FreezeMomentum();
        StartCoroutine(RoomTransition());
    }

    IEnumerator RoomTransition()
    {
        yield return new WaitForSeconds(0.5f);

        currentPlayerObject.GetComponent<MovementController>().ResumeMomentum();

    }

    public void DoHitFreeze(float duration = 0.125f)
    {
        StartCoroutine(HitFreeze(duration));
    }
    IEnumerator HitFreeze(float realTime)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(realTime);
        Time.timeScale = 1;

    }

    public void PauseEnvironment(bool state)
    {
        environmentPaused = state;
    }


    #endregion GAME FUNCTIONALITY



    #region DATA


    public void LoadData(GameData data)
    {
        if (!canAlterSaveData) { return; }

        this.unsavedGameData = data;
    }

    public void SaveData(ref GameData data)
    {
        if (!canAlterSaveData) { return; }

        DataPersistenceManager.instance.currentLoadedData = this.unsavedGameData;
    }

    #region MANUSCRIPT DATA

    /// <summary>
    /// Get collection status of manuscript in level
    /// </summary>
    /// <param name="ManuID">ID of manuscript in level</param>
    public bool GetManuscriptStatus(int ManuID)
    {
        if (!canAlterSaveData) { return false; }

        return DataPersistenceManager.instance.GetManuscriptData(currentLevelID, ManuID);
    }

    /// <summary>
    /// Save collected manuscript 
    /// </summary>
    /// <param name="ManuID">ID of the manuscript in the current level</param>
    public void CollectManuscript(int ManuID)
    {
        if (!canAlterSaveData) { return; }

        switch (currentLevelID)
        {
            case 0: // Tutorial
                currentLevelID = 0;
                unsavedGameData.tutorialData.manuscriptsCollected[ManuID] = true;
                break;
            case 1: // Level 1
                currentLevelID = 1;
                unsavedGameData.level1Data.manuscriptsCollected[ManuID] = true;
                break;
            case 2: // Level 2
                currentLevelID = 2;
                unsavedGameData.level2Data.manuscriptsCollected[ManuID] = true;
                break;
            case 3: // Level 3
                currentLevelID = 3;
                unsavedGameData.level3Data.manuscriptsCollected[ManuID] = true;
                break;
        }

        SaveData(ref unsavedGameData);
    }

    #endregion MANUSCRIPT DATA


    #endregion DATA




    IEnumerator TEMPTOLETPLAYERMOVE()
    {
        yield return new WaitForSeconds(0.5f);
        currentPlayerObject.transform.GetComponent<MovementController>().DisablePlayerControls(false);
        cineCam.GetComponent<CinemachineBrain>().DefaultBlend.Time = 0.45f;
    }



    public void AddDeathLocation(Vector2 location)
    {
        if (!canAlterSaveData) { return; }

        switch (currentLevelID)
        {
            case 0: // Tutorial
                currentLevelID = 0;
                unsavedGameData.tutorialData.deathLocations.Add(location);
                unsavedGameData.tutorialData.deathCount = unsavedGameData.tutorialData.deathLocations.Count;
                break;
            case 1: // Level 1
                currentLevelID = 1;
                unsavedGameData.level1Data.deathLocations.Add(location);
                unsavedGameData.level1Data.deathCount = unsavedGameData.level1Data.deathLocations.Count;
                break;
            case 2: // Level 2
                currentLevelID = 2;
                unsavedGameData.level2Data.deathLocations.Add(location);
                unsavedGameData.level2Data.deathCount = unsavedGameData.level2Data.deathLocations.Count;
                break;
            case 3: // Level 3
                currentLevelID = 3;
                unsavedGameData.level3Data.deathLocations.Add(location);
                unsavedGameData.level3Data.deathCount = unsavedGameData.level3Data.deathLocations.Count;
                break;
        }

        SaveData(ref unsavedGameData);
    }





}
