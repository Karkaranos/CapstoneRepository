/*************************************************
Author Names : 		Tyler Bouchard 
Date Created : 		9/30/2025
Date Last Modified : 	10/2/2025
Brief Description : 		Every menu gets one, these are the functions that the 
                            buttons call for menu navigation
External Resources : 
***************************************************/
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBehavior : MonoBehaviour
{
    [HideInInspector] public MenuBehavior previousMenu;

    /// <summary>
    /// Gets a reference to the inputActions and makes sure that the timescale is normal
    /// </summary>
    private void Awake()
    {
        previousMenu = null;
    }

    /// <summary>
    /// Loads a specific scene by its name
    /// </summary>
    /// <param name="sceneToLoad"></param>
    public void LoadScene(int sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    /// <summary>
    /// deactivates the current menu and activates a new one. it will set that menus previousMenu to this menu
    /// </summary>
    /// <param name="obj"></param>
    public void ActivateSubMenu(GameObject obj)
    {
        obj.SetActive(true);
        obj.GetComponent<MenuBehavior>().previousMenu = GetComponent<MenuBehavior>();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// colses the game
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }

    /// <summary>
    /// goes to the previous menu and deacitvates itself
    /// </summary>
    public void Return()
    {
        if (previousMenu != null)
        {
            previousMenu.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}