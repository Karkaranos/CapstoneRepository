/*************************************************
Author Names : 		Tyler Bouchard 
Date Created : 		9/30/2025
Date Last Modified : 	10/2/2025
Brief Description : 		This class controls the behavior for the pause menu
                            more specificly what it does when the escape button is pressed
External Resources : 
***************************************************/

using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuBehavoir : MonoBehaviour
{
    private MenuControls inputActions;

    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject settingsMenu;

    private bool gamePaused;

    /// <summary>
    /// Gets a reference to the inputActions and makes sure that the timescale is normal
    /// </summary>
    private void Awake()
    {
        inputActions = new MenuControls();
    }

    /// <summary>
    /// controls what happens when escape is pressed
    /// </summary>
    /// <param name="obj"></param>
    private void EscapePressed(InputAction.CallbackContext obj)
    {
        

        if (PauseMenu.activeSelf)
        {
            PauseMenu.SetActive(false);
            UnpauseGame();
            return;
        }
        if (!PauseMenu.activeSelf && !gamePaused)
        {
            PauseMenu.SetActive(true);
            PauseGame();
            return;
        }
        if (settingsMenu.activeSelf)
        {
            settingsMenu.GetComponent<MenuBehavior>().Return();
            PauseGame();
            return;
        }
    }

    /// <summary>
    /// pauses the game (duh)
    /// </summary>
    private void PauseGame()
    {
        gamePaused=true;
        Time.timeScale = 0;
    }

    /// <summary>
    /// unpauses the game (also duh)
    /// </summary>
    public void UnpauseGame()
    {
        gamePaused = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        PauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    /// <summary>
    /// these make sure that the input is created and destroyed properly
    /// </summary>
    private void OnEnable()
    {
        inputActions.MenuActions.Enable();
        inputActions.MenuActions.Escape.performed += EscapePressed;
    }
    private void OnDisable()
    {
        inputActions.MenuActions.Disable();
        inputActions.MenuActions.Escape.performed -= EscapePressed;
    }
}