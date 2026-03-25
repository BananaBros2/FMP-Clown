using UnityEngine;

public class PauseScreenScript : MonoBehaviour
{
    [SerializeField] private UIHandler uiHandler;

    public void ResumePressed()
    {
        // Resume Game
        uiHandler.SetPauseState(false);
    }

    public void ExitApplicationPressed()
    {
        // Quit Game
        Application.Quit();
    }
}
