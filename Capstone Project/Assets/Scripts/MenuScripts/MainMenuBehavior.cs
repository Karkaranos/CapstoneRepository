using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuBehavior : MonoBehaviour
{
    private MenuControls inputActions;
    private MenuBehavoir mb;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject confirmQuit;
    private void Awake()
    {
        Time.timeScale = 1;
        inputActions = new MenuControls();
    }
    private void EscapePressed(InputAction.CallbackContext obj)
    {
        if (mainMenu.activeSelf)
        {
            mainMenu.GetComponent<MenuBehavoir>().ActivateSubMenu(confirmQuit);
        }
        else if (settingsMenu.activeSelf)
        {
            settingsMenu.GetComponent<MenuBehavoir>().Return();
        }
        else if (confirmQuit.activeSelf)
        {
            confirmQuit.GetComponent<MenuBehavoir>().Return();
        }
    }
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
