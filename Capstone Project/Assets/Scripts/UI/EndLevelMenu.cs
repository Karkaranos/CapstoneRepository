/*************************************************
Author Names : 		Clare Grady, Tyler Bouchard, Cade Naylor
Date Created : 		10/30/2025
Date Last Modified : 	3/31/2026
Brief Description : 		Temporary End Level Menu handler for 
                    vertical slice
External Resources : 	
***************************************************/
using FMOD;
using FMOD.Studio;
using NUnit.Framework;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevelMenu : MonoBehaviour
{
    #region VARS
    [SerializeField] private GameObject SkillMenu;
    [SerializeField] private GameObject WinMenu;
    [SerializeField] private GameObject LoseMenu;
    [SerializeField] private FMOD.Studio.Bus MasterBus;
    private bool retrying;
    [SerializeField] private bool IsDemo;
    [SerializeField] private GameObject demoMenu;
    [SerializeField] private float popupDelay;
    #endregion

    #region FUNCTIONS

    /// <summary>
    /// Turn off the endMenuUi on start
    /// </summary>
    private void Start()
    {
        WinMenu.SetActive(false);
        LoseMenu.SetActive(false);

        MasterBus = FMODUnity.RuntimeManager.GetBus("Bus:/");
        // Grabs bus manager for audio
    }

    /// <summary>
    /// Toggles ig the EndMenuUi is on or off 
    /// </summary>
    public void EnableEndMenuUi(bool win)
    {
        StartCoroutine(DelayedPopup(win));
    }

    /// <summary>
    /// shows the end level menu after a delay
    /// </summary>
    /// <param name="isWin"></param>
    /// <returns></returns>
    private IEnumerator DelayedPopup(bool isWin)
    {
        yield return new WaitForSeconds(popupDelay);

        if (!IsDemo)
        {
            if (isWin)
            {
                WinMenu.SetActive(true);
            }
            else
            {
                LoseMenu.SetActive(true);
            }
            if (!WinMenu.activeSelf || !LoseMenu.activeSelf)
            {
                FindFirstObjectByType<TransitionManager>().LevelToEndScreen();
            }
            IsDemo = true;
        }
        else
        {
            demoMenu.SetActive(true);
        }
    }

    /// <summary>
    /// This function is used for enabling the UI during the transition. 
    /// The place I wanted to intially call the transition from is currently check out.
    /// </summary>
    public void ShowTheEndMenuUI()
    {
        MasterBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        Pip[] pips = FindObjectsByType<Pip>(FindObjectsSortMode.None);
        foreach (Pip pip in pips)
        {
            pip.RemovePip();
        }
        foreach (TileBehaviour d in PipManager.Instance.hazardTiles)
        {
            d.RemoveHazard();
        }
        //stops all audio
    }

    /// <summary>
    /// Goes to the main menu
    /// Called when the main menu button is pressed in the end level ui
    /// </summary>
    public void QuitGame()
    {
        
        MasterBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Restarts the current scene 
    /// Called when the restart button is pressed
    /// </summary>
    public void RestartLevel()
    {
        retrying = true;
        Pip[] pips = FindObjectsByType<Pip>(FindObjectsSortMode.None);
        foreach (Pip pip in pips)
        {
            pip.RemovePip();
        }
        foreach (TileBehaviour d in PipManager.Instance.hazardTiles)
        {
            d.RemoveHazard();
        }
        FindFirstObjectByType<TransitionManager>().EndScreenToEquipMenu();
    }
    
    /// <summary>
    /// Call the transition to go to the equip menu
    /// </summary>
    public void NextLevel()
    {
        retrying = false;
        Pip[] pips = FindObjectsByType<Pip>(FindObjectsSortMode.None);
        foreach (Pip pip in pips)
        {
            pip.RemovePip();
        }
        foreach (TileBehaviour d in PipManager.Instance.hazardTiles)
        {
            d.RemoveHazard();
        }
        FindFirstObjectByType<TransitionManager>().EndScreenToEquipMenu();
    }

    /// <summary>
    /// Loads the next level
    /// </summary>
    public void LoadNextLevel()
    {
        if (retrying)
        {
            FindFirstObjectByType<GridTesting>().ReloadCurrentGrid();
        }
        else
        {
            FindFirstObjectByType<GridTesting>().LoadNextGrid();
        }
        SkillMenu.SetActive(true);
        FindFirstObjectByType<RuneSelectionMenu>(findObjectsInactive: FindObjectsInactive.Include).gameObject.SetActive(true);
        WinMenu.SetActive(false);
        LoseMenu.SetActive(false);
    }

    /// <summary>
    /// Loads a specific grid
    /// </summary>
    /// <param name="level"></param>
    public void LoadSpecificLevel(int level)
    {
        FindFirstObjectByType<GridTesting>().LoadSpecificGrid(level);

        SkillMenu.SetActive(true);
        FindFirstObjectByType<RuneSelectionMenu>(findObjectsInactive: FindObjectsInactive.Include).gameObject.SetActive(true);
        WinMenu.SetActive(false);
        LoseMenu.SetActive(false);

    }

    /// <summary>
    /// Sets the text that will appear at the end of the level 
    /// </summary>
    /// <param name="text"></param>
    public void SetText(string text)
    {
        //this.text.text = text;
    }

    public void StopAllSound()
    {
        MasterBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    #endregion
}
