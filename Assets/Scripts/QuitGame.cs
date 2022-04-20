using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class QuitGame : MonoBehaviour
{
    PlayerInput playerInput;
    public bool IsESCPressed;

    public void ESCPressed(InputAction.CallbackContext ctx)
    {
        IsESCPressed = ctx.ReadValueAsButton();
    }

    public void ReturnToMainMenu()
    {
        if(IsESCPressed)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);
        }
    }

    void Update()
    {
        ReturnToMainMenu();
    }
}
