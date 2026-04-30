using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenScript : MonoBehaviour
{
    [SerializeField] private GameObject saveSlots;
    [SerializeField] private GameObject saveDataDisplay;
    [SerializeField] private GameObject noSaveDataDisplay;

    private void Start()
    {
        DisplaySaveSlotData(1);
        DisplaySaveSlotData(2);
        DisplaySaveSlotData(3);
    }

    private void DisplaySaveSlotData(int saveSlotID)
    {
        GameData slotData = DataPersistenceManager.instance.GetSaveData(saveSlotID);
        Transform currentSlot = saveSlots.transform.GetChild(saveSlotID - 1);

        GameObject dataDisplay;
        if (slotData == null)
        {
            Instantiate(noSaveDataDisplay, currentSlot);
            return;
        }
        else 
        {
            dataDisplay = Instantiate(saveDataDisplay, currentSlot);
        }

        dataDisplay.GetComponent<DataDisplay>().ProjectData(slotData);

        //dataDisplay.GetComponent


        //slotData
    }

    /// <summary>
    /// Open save select screen
    /// </summary>
    public void StartGamePressed()
    {
        SceneManager.LoadScene("Tutorial");
    }

    /// <summary>
    /// Open options menu
    /// </summary>
    public void OptionsPressed()
    {

    }

    /// <summary>
    /// Exit Application
    /// </summary>
    public void QuitApplicationPressed()
    {
        // Quit Game
        Application.Quit();
    }



    public void SaveFilePressed(int saveSlotID)
    {
        DataPersistenceManager.instance.HandleSaveFilePressed(saveSlotID);
        SceneManager.LoadScene("Tutorial");
    }

}
