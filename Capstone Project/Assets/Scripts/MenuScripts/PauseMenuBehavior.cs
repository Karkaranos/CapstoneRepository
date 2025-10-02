using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuBehavoir : MonoBehaviour
{
    private MenuControls inputActions;
    private MenuBehavoir mb;
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject settingsMenu;
    private bool gamePaused;
    private void Awake()
    {
        inputActions = new MenuControls();
    }
    private void EscapePressed(InputAction.CallbackContext obj)
    {
        

        if (PauseMenu.activeSelf)
        {
            PauseMenu.SetActive(false);
            UnpauseGame();
        }
        else if (!PauseMenu.activeSelf && !gamePaused)
        {
            PauseMenu.SetActive(true);
            PauseGame();
        }
        else if (settingsMenu.activeSelf)
        {
            settingsMenu.GetComponent<MenuBehavoir>().Return();
        }
    }

    private void PauseGame()
    {
        gamePaused=true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
    }
    public void UnpauseGame()
    {
        gamePaused = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        PauseMenu.SetActive(false);
        Time.timeScale = 1;
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
