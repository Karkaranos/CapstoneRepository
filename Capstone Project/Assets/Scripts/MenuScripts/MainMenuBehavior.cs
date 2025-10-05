/*************************************************
Author Names : 		Tyler Bouchard 
Date Created : 		9/30/2025
Date Last Modified : 	10/2/2025
Brief Description : 		This class controls the behavior for the Main menu
                            more specificly what it does when the escape button is pressed
External Resources : 
***************************************************/

using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuBehavior : MonoBehaviour
{
    private MenuControls inputActions;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject confirmQuit;

    /// <summary>
    /// Gets a reference to the inputActions and makes sure that the timescale is normal
    /// </summary>
    private void Awake()
    {
        Time.timeScale = 1;
        inputActions = new MenuControls();
    }

    /// <summary>
    /// controls what happend when Escape is pressed
    /// </summary>
    /// <param name="obj"></param>
    private void EscapePressed(InputAction.CallbackContext obj)
    {
        if (mainMenu.activeSelf)
        {
            mainMenu.GetComponent<MenuBehavoir>().ActivateSubMenu(confirmQuit);
            return;
        }
        if (settingsMenu.activeSelf)
        {
            settingsMenu.GetComponent<MenuBehavoir>().Return();
            return;
        }
        if (confirmQuit.activeSelf)
        {
            confirmQuit.GetComponent<MenuBehavoir>().Return();
            return;
        }
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
