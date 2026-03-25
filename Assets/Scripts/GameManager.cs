using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] GameObject playerPrefab;

    private GameObject currentPlayerObject;

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

        SpawnPlayerCharacter(spawnPosition);
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

        StartCoroutine(LoadScene());

    }



    IEnumerator LoadScene()
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


        SpawnPlayerCharacter(spawnPosition);
    }


    private void SpawnPlayerCharacter(Vector3 startPosition)
    {
        currentPlayerObject = Instantiate(playerPrefab, startPosition, Quaternion.identity);
        SetupCameras();
    }

    private void SetupCameras()
    {
        SetTrackingTarget[] trackingTargetScripts = FindObjectsByType<SetTrackingTarget>(FindObjectsSortMode.None);
        foreach (SetTrackingTarget trackingTargetScript in trackingTargetScripts)
        {
            trackingTargetScript.SetupTarget();
        }
    }

    public GameObject GetPlayerObject()
    {
        return currentPlayerObject;
    }

    public void ChangeRoomNameDisplay(string roomName)
    {

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



}
