using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{

    public bool EscapeKey;
    public static bool GameIsPaused;

    public GameObject PauseMenuUI;

    public void getESC(InputAction.CallbackContext ctx)
    {
        EscapeKey = ctx.ReadValueAsButton();
    }
    // Update is called once per frame
    void Update()
    {
        if(EscapeKey)
        {
            if(GameIsPaused == true)
            {
                Resume();
            }
            else{
                Pause();
            }
        }
    }

    public void Resume()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {   
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void QuitGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);
    }
}
