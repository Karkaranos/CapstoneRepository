/*************************************************
Author Names : 		Tyler Bouchard 
Date Created : 		9/30/2025
Date Last Modified : 	10/2/2025
Brief Description : 		This class controls the behavior for the Main menu
                            more specificly what it does when the escape button is pressed
External Resources : 
***************************************************/

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MainMenuBehavior : MonoBehaviour
{
    private PlayerInput input;
    private InputAction pause;
    private EventSystem eSystem;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject creditsScreen;
    [SerializeField] private GameObject confirmQuit;
    [SerializeField] private GameObject defaultSelectedGO;

    [HideInInspector] public bool controllerEnabled;

    /// <summary>
    /// Gets a reference to the inputActions and makes sure that the timescale is normal
    /// </summary>
    private void Awake()
    {
        Time.timeScale = 1;
        input = GetComponent<PlayerInput>();
        eSystem = FindFirstObjectByType<EventSystem>();
        

        pause = input.currentActionMap.FindAction("Pause");
    }

    /// <summary>
    /// controls what happend when Escape is pressed
    /// </summary>
    /// <param name="obj"></param>
    private void EscapePressed(InputAction.CallbackContext obj)
    {
        if(TransitionManager.instance.TransitioningBetweenLevels)
        {
            return;
        }
        if (mainMenu.activeSelf)
        {
            mainMenu.GetComponent<MenuBehavior>().ActivateSubMenu(confirmQuit);
            return;
        }
        if (settingsMenu.activeSelf)
        {
            settingsMenu.GetComponent<MenuBehavior>().Return();
            return;
        }
        if (creditsScreen.activeSelf)
        {
            creditsScreen.GetComponent<MenuBehavior>().Return();
            return;
        }
        if (confirmQuit.activeSelf)
        {
            confirmQuit.GetComponent<MenuBehavior>().Return();
            return;
        }
    }

    /// <summary>
    /// these make sure that the input is created and destroyed properly
    /// </summary>
    private void OnEnable()
    {
        input.currentActionMap.Enable();
        pause.started += EscapePressed;
        input.onControlsChanged += Input_onControlsChanged;
    }

    private void Input_onControlsChanged(PlayerInput obj)
    {
        if (obj.currentControlScheme == "KeyboardAndMouse")
        {
            PublicEvents.ControllerDisabled?.Invoke();
            eSystem.SetSelectedGameObject(null);
            controllerEnabled = false;
        }
        else
        {
            if (obj.currentControlScheme == "Controller")
            {
                PublicEvents.ControllerEnabled?.Invoke();
                //eSystem.SetSelectedGameObject(defaultSelectedGO);
                controllerEnabled = true;
            }
        }
    }

    private void OnDisable()
    {
        input.currentActionMap.Disable();
        pause.started -= EscapePressed;
        input.onControlsChanged -= Input_onControlsChanged;
    }

    
    
}
