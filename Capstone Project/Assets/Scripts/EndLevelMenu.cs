/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/30/2025
Date Last Modified : 	11/2/2025
Brief Description : 		Temporary End Level Menu handler for 
                    vertical slice
External Resources : 	
***************************************************/
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevelMenu : MonoBehaviour
{
    #region VARS

    [SerializeField] private CanvasGroup endMenuUi;

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
    /// Enables the EndMenuUi is on or off 
    /// </summary>
    public void EnableEndMenuUi()
    {
        endMenuUi.alpha = 1;
        endMenuUi.interactable = true;
        endMenuUi.blocksRaycasts = true;
    }

    /// <summary>
    /// Logic for when the restart button is clicked
    /// </summary>
    public void RestartLevelClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    /// <summary>
    /// Logic for when the main menu button is pressed
    /// </summary>
    public void MainMenuButtonClicked()
    {
        SceneManager.LoadScene(0);
    }
    #endregion
}
