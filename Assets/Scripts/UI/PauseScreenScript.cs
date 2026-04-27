using UnityEngine;

public class PauseScreenScript : MonoBehaviour
{
    [Tooltip("UIHandler script reference")]
    [SerializeField] private UIHandler uiHandler;

    /// <summary>
    /// Resume game by turning off UI
    /// </summary>
    public void ResumePressed()
    {
        // Resume Game
        uiHandler.SetPauseState(false);
    }

    /// <summary>
    /// Exit Application
    /// </summary>
    public void ExitApplicationPressed()
    {
        // Quit Game
        Application.Quit();
    }
}
