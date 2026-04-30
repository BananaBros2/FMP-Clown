using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    [Tooltip("Level data for tutorial")]
    public LevelData tutorialData;

    [Tooltip("Level data for level 1")]
    public LevelData level1Data;
    [Tooltip("Level data for level 2")]
    public LevelData level2Data;
    [Tooltip("Level data for level 3")]
    public LevelData level3Data;


    /// <summary>
    /// Default game stat initialisation
    /// </summary>
    public GameData()
    {

        this.tutorialData = new LevelData();
        this.tutorialData.manuscriptsCollected = new List<bool> { false, false, false };

        this.level1Data = new LevelData();
        this.level2Data = new LevelData();
        this.level3Data = new LevelData();

    }
}
