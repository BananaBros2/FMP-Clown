using UnityEngine;
using UnityEngine.InputSystem;

public class UIHandler : MonoBehaviour
{
    bool gamePaused = false;

    [SerializeField] private GameObject pauseMenu;

    void Start()
    {
        pauseMenu.SetActive(gamePaused); // Set initial state (Good for if I forget to turn the UI off)
    }


    /// <summary>
    /// Handle pause input
    /// </summary>
    /// <param name="pause"></param>
    public void OnPause(InputAction.CallbackContext pause)
    {
        if (pause.started)
        {
            gamePaused = !gamePaused;

            pauseMenu.SetActive(gamePaused);
        }
    }


    public void SetPauseState(bool state)
    {
        gamePaused = state;
        pauseMenu.SetActive(gamePaused);
    }

}
