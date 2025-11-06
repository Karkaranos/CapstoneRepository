/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/30/2025
Date Last Modified : 	11/2/2025
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

    public void QuitGame()
    {
        Debug.Log("Application.Quit called");
        Application.Quit();
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }
    
    #endregion
}
