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
    }

    /// <summary>
    /// Toggles ig the EndMenuUi is on or off 
    /// </summary>
    public void EnableEndMenuUi()
    {
        endMenuUi.alpha = 1;
        endMenuUi.interactable = true;
        endMenuUi.blocksRaycasts = true;
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
    /// Sets the text that will appear at the end of the level 
    /// </summary>
    /// <param name="text"></param>
    public void SetText(string text)
    {
        this.text.text = text;
    }
    
    #endregion
}
