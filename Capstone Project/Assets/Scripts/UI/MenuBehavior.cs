/*************************************************
Author Names : 		Tyler Bouchard 
Date Created : 		9/30/2025
Date Last Modified : 	10/2/2025
Brief Description : 		Every menu gets one, these are the functions that the 
                            buttons call for menu navigation
External Resources : 
***************************************************/
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuBehavior : MonoBehaviour
{
    [HideInInspector] public MenuBehavior previousMenu;
    [SerializeField] private GameObject startingSelectedGO;
    [HideInInspector] public bool isActiveMenu;

    [SerializeField] private FMOD.Studio.Bus MasterBus;


    /// <summary>
    /// Gets a reference to the inputActions and makes sure that the timescale is normal
    /// </summary>
    private void Awake()
    {
        previousMenu = null;

        
    }

    private void OnEnable()
    {
        isActiveMenu = true;
        PublicEvents.ControllerEnabled += ControllerEnabled;

        //if the player is using controller, selects the default obj
        if (FindFirstObjectByType<MainMenuBehavior>().controllerEnabled)
        {
            ControllerEnabled();
        }
    }
    private void OnDisable()
    {
        PublicEvents.ControllerEnabled -= ControllerEnabled;
        MasterBus = FMODUnity.RuntimeManager.GetBus("Bus:/");
        // Grabs bus manager for audio
    }

    /// <summary>
    /// Loads a specific scene by its name
    /// </summary>
    /// <param name="sceneToLoad"></param>
    public void LoadScene(int sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);

        MasterBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        //stops all audio
    }

    /// <summary>
    /// deactivates the current menu and activates a new one. it will set that menus previousMenu to this menu
    /// </summary>
    /// <param name="obj"></param>
    public void ActivateSubMenu(GameObject obj)
    {
        if (FindFirstObjectByType<MainMenuBehavior>().controllerEnabled)
        {
            startingSelectedGO = FindFirstObjectByType<EventSystem>().currentSelectedGameObject;
        }

        obj.SetActive(true);
        obj.GetComponent<MenuBehavior>().previousMenu = GetComponent<MenuBehavior>();
       
        isActiveMenu = false;
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
            previousMenu.isActiveMenu = true;
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// sets the player's selected button
    /// </summary>
    private void ControllerEnabled()
    {
        if (isActiveMenu)
        {
            FindFirstObjectByType<EventSystem>().SetSelectedGameObject(startingSelectedGO);
        }
        //delays so if the menu is opened, it takes priority over the main menu
        //StartCoroutine(DelayedControllerEnabled());
    }

    /// <summary>
    /// sets the player's selected button with a delay
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayedControllerEnabled()
    {
        yield return null;
       
    }
}