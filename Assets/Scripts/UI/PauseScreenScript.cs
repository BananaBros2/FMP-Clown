using UnityEngine;

public class PauseScreenScript : MonoBehaviour
{

    [SerializeField]
    UIHandler uiHandler;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


    public void ResumePressed()
    {
        uiHandler.SetPauseState(false);
    }

    public void ExitApplicationPressed()
    {
        Application.Quit();
    }
}
