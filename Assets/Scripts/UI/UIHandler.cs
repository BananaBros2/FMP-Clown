using UnityEngine;
using UnityEngine.InputSystem;

public class UIHandler : MonoBehaviour
{
    [Tooltip("Current paused status")]
    private bool gamePaused = false;

    [Tooltip("Pause Menu object reference")]
    [SerializeField] private GameObject pauseMenu;

    void Start()
    {
        // Set initial state (Good for if I leave the UI in the wrong activity state)
        pauseMenu.SetActive(gamePaused); 
    }

    /// <summary>
    /// Handle pause input
    /// </summary>
    /// <param name="pause">Pause Input</param>
    public void OnPause(InputAction.CallbackContext pause)
    {
        if (pause.started)
        {
            // Invert pause status
            gamePaused = !gamePaused;
            pauseMenu.SetActive(gamePaused);
        }
    }

    /// <summary>
    /// Directly set pause state of UIHandler
    /// </summary>
    /// <param name="state">New pause status</param>
    public void SetPauseState(bool state)
    {
        // Set and apply new paused status
        gamePaused = state;
        pauseMenu.SetActive(gamePaused);
    }

}
