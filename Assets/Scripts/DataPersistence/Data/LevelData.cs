using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    [Tooltip("Manuscripts collected in level")]
    public List<bool> manuscriptsCollected;

    [Tooltip("Number of deaths")]
    public int deathCount;
     
    [Tooltip("Location of every death location in level")]
    public List<Vector2> deathLocations;

    /// <summary>
    /// Default level stat initialisation
    /// </summary>
    public LevelData()
    {
        this.deathCount = 0;
        this.deathLocations = new List<Vector2>();
        this.manuscriptsCollected = new List<bool> { false, false, false, false, false };

    }

}
