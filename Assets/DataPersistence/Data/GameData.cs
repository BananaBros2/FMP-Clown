using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int deathCount;
    public List<Vector2> deathLocations;

    public GameData()
    {
        this.deathCount = 0;
        this.deathLocations = new List<Vector2>();
    }
}
