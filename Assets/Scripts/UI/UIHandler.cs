using UnityEngine;
using UnityEngine.InputSystem;

public class UIHandler : MonoBehaviour
{
    bool gamePaused = false;

    [SerializeField]
    GameObject pauseMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseMenu.SetActive(gamePaused);
    }

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
