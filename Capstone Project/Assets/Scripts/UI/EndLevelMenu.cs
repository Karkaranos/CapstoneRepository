/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/30/2025
Date Last Modified : 	12/2/2025
Brief Description : 		Temporary End Level Menu handler for 
                    vertical slice
External Resources : 	
***************************************************/
using TMPro;
using UnityEngine;

using UnityEngine.SceneManagement;

public class EndLevelMenu : MonoBehaviour
{
    #region VARS

    [SerializeField] private CanvasGroup endMenuUi;
    [SerializeField] private TMP_Text text;

    [SerializeField] private FMOD.Studio.Bus MasterBus;

    [SerializeField] private GameObject SkillMenu;
    [SerializeField] private GameObject nextLevelButton;    
    

    #endregion

    #region FUNCTIONS

    /// <summary>
    /// Turn off the endMenuUi on start
    /// </summary>
    private void Start()
    {
        endMenuUi.alpha = 0;
        endMenuUi.interactable = false;
        endMenuUi.blocksRaycasts = false;

        MasterBus = FMODUnity.RuntimeManager.GetBus("Bus:/");
        // Grabs bus manager for audio
    }

    /// <summary>
    /// Toggles ig the EndMenuUi is on or off 
    /// </summary>
    public void EnableEndMenuUi()
    {
        if (endMenuUi.alpha == 0)
        {
            FindFirstObjectByType<TransitionManager>().LevelToEndScreen();
        }

        
    }

    /// <summary>
    /// This function is used for enabling the UI during the transition. 
    /// The place I wanted to intially call the transition from is currently check out.
    /// </summary>
    public void ShowTheEndMenuUI()
    {
        endMenuUi.alpha = 1;
        endMenuUi.interactable = true;
        endMenuUi.blocksRaycasts = true;

        MasterBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        //stops all audio
    }

    /// <summary>
    /// Goes to the main menu
    /// Called when the main menu button is pressed in the end level ui
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Return to Menu");
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Restarts the current scene 
    /// Called when the restart button is pressed
    /// </summary>
    public void RestartLevel()
    {
        Debug.Log("Restart Level");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    /// <summary>
    /// Call the transition to go to the equip menu
    /// </summary>
    public void NextLevel()
    {
        FindFirstObjectByType<TransitionManager>().EndScreenToEquipMenu();
    }

    /// <summary>
    /// Loads the next level
    /// </summary>
    public void LoadNextLevel()
    {
        FindFirstObjectByType<GridTesting>().LoadNextGrid();
        SkillMenu.SetActive(true);
        FindFirstObjectByType<RuneSelectionMenu>(findObjectsInactive: FindObjectsInactive.Include).gameObject.SetActive(true);
        endMenuUi.alpha = 0;
        endMenuUi.interactable = false;
        endMenuUi.blocksRaycasts = false;
    }

    /// <summary>
    /// Sets the text that will appear at the end of the level 
    /// </summary>
    /// <param name="text"></param>
    public void SetText(string text)
    {
        this.text.text = text;
    }

    /// <summary>
    /// toggles the next level button
    /// </summary>
    /// <param name="isActive"></param>
    public void SetNextLevelButton(bool isActive)
    {
        nextLevelButton.SetActive(isActive);
    }
    
    #endregion
}
