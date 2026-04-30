using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DataDisplay : MonoBehaviour
{
    [Tooltip("Reference to text object used to display completion percent")]
    [SerializeField] TMP_Text totalCompletionText;

    [Tooltip("Reference to manuscript image renderers\n-Tutorial Group-")]
    [SerializeField] List<Image> tutorialManuscripts = new List<Image>();
    [Tooltip("Reference to manuscript image renderers\n-Level 1 Group-")]
    [SerializeField] List<Image> level1Manuscripts = new List<Image>();
    [Tooltip("Reference to manuscript image renderers\n-Level 2 Group-")]
    [SerializeField] List<Image> level2Manuscripts = new List<Image>();
    [Tooltip("Reference to manuscript image renderers\n-Level 3 Group-")]
    [SerializeField] List<Image> level3Manuscripts = new List<Image>();

    [Tooltip("Sprite used to show collected manuscript")]
    [SerializeField] Sprite manuCollectedSprite;
    [Tooltip("Sprite used to show uncollected manuscript")]
    [SerializeField] Sprite manuMissingSprite;

    [Tooltip("Total manuscripts across all level groups")]
    int totalManuscripts = 0;
    [Tooltip("Total manuscripts collected in save slot")]
    int manuscriptsCollected = 0;


    /// <summary>
    /// Load data values on save slot
    /// </summary>
    /// <param name="gameData">Game data to show on save slot</param>
    public void ProjectData(GameData gameData)
    {
        // Reset manuscript collection values
        totalManuscripts = 0;
        manuscriptsCollected = 0;

        // Set manuscript collection images
        SetManuscriptCollection(tutorialManuscripts, gameData.tutorialData.manuscriptsCollected);
        SetManuscriptCollection(level1Manuscripts, gameData.level1Data.manuscriptsCollected);
        SetManuscriptCollection(level2Manuscripts, gameData.level2Data.manuscriptsCollected);
        SetManuscriptCollection(level3Manuscripts, gameData.level3Data.manuscriptsCollected);

        // Calculate save slot completion percentage
        float completionValue = (float)manuscriptsCollected * 100 / (float)totalManuscripts;
        completionValue = Mathf.Floor(completionValue);
        totalCompletionText.text = (completionValue).ToString() + "%";

    }

    /// <summary>
    /// Change images to display manuscript collection in a level group
    /// </summary>
    /// <param name="manuscriptGroup">Level group of collection</param>
    /// <param name="manuscriptGroupStatus">Manuscript collection save data</param>
    public void SetManuscriptCollection(List<Image> manuscriptGroup, List<bool> manuscriptGroupStatus)
    {
        int manuscriptIndex = 0;
        foreach (Image manuscript in manuscriptGroup) // Check per each manuscript collection slot
        {
            if (manuscriptGroupStatus[manuscriptIndex])
            {
                manuscript.sprite = manuCollectedSprite; // Manuscript has been collected
                manuscriptsCollected++; // Add to total collected
            }
            else
            {
                manuscript.sprite = manuMissingSprite; // Manuscript has not been collected
            }

            totalManuscripts++; // Add to total manuscript number
            manuscriptIndex++;
        }
    }

}
