using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] GameObject playerPrefab;

    
    private GameObject currentPlayerObject;
    public GameObject cineCam;
    public UIHandler uiHandler;
    [SerializeField] private GameObject blackSquareOfDoom;

    private bool environmentPaused;

    private string currentRoom = "Unknown";
    private int currentCheckpoint = 0;

    private bool sceneLoading = false;


    private int deathCount = 0;
    public List<Vector2> deathLocations;

    public string currentPlaytime = "Unknown";



    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        currentPlaytime = System.DateTime.Now.Second.ToString() + "s" 
            + System.DateTime.Now.Minute.ToString() + "m"
            + System.DateTime.Now.Hour.ToString() + "h"
            + System.DateTime.Now.Day.ToString() + "d.log";
        print(currentPlaytime);
        DontDestroyOnLoad(gameObject);
        StartLevel();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

    public void IHateScreenTransitioning()
    {
        ControlDoomSquare(false);
    }


    public void UpdateCheckpoint(int newCheckpoint)
    {
        currentCheckpoint = newCheckpoint;
    }

    public void RespawnAtCheckpoint()
    {

        deathCount++;
        print(deathCount);

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

    IEnumerator TEMPTOLETPLAYERMOVE()
    {
        yield return new WaitForSeconds(0.5f);
        currentPlayerObject.transform.GetComponent<MovementController>().DisablePlayerControls(false);
        cineCam.GetComponent<CinemachineBrain>().DefaultBlend.Time = 0.45f;
    }

    IEnumerator RoomTransition()
    {
        yield return new WaitForSeconds(0.5f);

        currentPlayerObject.GetComponent<MovementController>().ResumeMomentum();

    }


    private void SetupCameras()
    {
        SetTrackingTarget[] trackingTargetScripts = FindObjectsByType<SetTrackingTarget>(FindObjectsSortMode.None);
        foreach (SetTrackingTarget trackingTargetScript in trackingTargetScripts)
        {
            trackingTargetScript.SetupTarget();
        }
    }

    public void HandleRoomTransition(string sourceRoom)
    {
        if (currentRoom == sourceRoom) { return; }
        currentRoom = sourceRoom;
        currentPlayerObject.GetComponent<MovementController>().FreezeMomentum();
        StartCoroutine(RoomTransition());
    }




    public GameObject GetPlayerObject()
    {
        return currentPlayerObject;
    }

    public void ChangeRoomNameDisplay(string roomName)
    {

    }

    public void SwitchLevel(int levelID = 0)
    {
        string levelName = "Level 1";

        switch (levelID)
        {
            case 0:
                levelName = "Tutorial";
                break;
            case 1:
                levelName = "Level 1";
                break;
            case 2:
                levelName = "Level 2";
                break;
            case 3:
                levelName = "Level 3";
                break;
            default:
                Debug.LogError("You and I both know this level doesn't exist");
                break;
        }

        StartCoroutine(LoadScene(levelName));

    }


    public void LoadData(GameData data)
    {
        this.deathCount = data.deathCount;
        this.deathLocations = data.deathLocations;
    }

    public void SaveData(ref GameData data) 
    {
        data.deathCount = this.deathCount;
        data.deathLocations = this.deathLocations;
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
    public bool GetEnvironmentPausedStatus()
    {
        return environmentPaused;
    }
}
