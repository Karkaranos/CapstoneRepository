/*************************************************
Author Names : 		    Aidan Ratcliffe
Date Created : 		    10/1/2025
Date Last Modified : 	10/2/2025
Brief Description : 	All Buttons will be managed within this script
External Resources : 	N/A
***************************************************/
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public PlayerBehavior playerBehavior;
    public GameObject playerCanvas;
    public GameObject moveCanvas;
    public GameObject confirmCanvas;
    public Button moveButton;
    public Button moveChoiceButton;
    public Button backButton;
    public Button confirmButton;
    //public Button endButton;
    //public Button MoveChoiceButton;
    public bool playerCanMove;
    public bool playerIsGoingToMove;
    public bool backButtonClicked;
    public bool confirmButtonClicked;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerBehavior = GetComponent<PlayerBehavior>();
        Button mbtn = moveButton.GetComponent<Button>();
        Button mcbtn = moveChoiceButton.GetComponent<Button>();
        Button bbtn = backButton.GetComponent<Button>();
        Button cbtn = confirmButton.GetComponent<Button>();
        //Button mcbtn = MoveChoiceButton.GetComponent<Button>();
        Canvas pCanvas = playerCanvas.GetComponent<Canvas>();
        Canvas mCanvas = moveCanvas.GetComponent<Canvas>();
        Canvas cCanvas = confirmCanvas.GetComponent<Canvas>();
    }

    public void MoveButtonOnClick()
    {
        Debug.Log("The player can move!");
        playerCanMove = true;
        if(playerCanMove)
        {
            moveCanvas.SetActive(true);
            playerCanvas.SetActive(false);
        }
        else
        {
            playerCanMove = false;
        }
    }

    public void BackButtonOnClick()
    {
        Debug.Log("goin back!");
        backButtonClicked = true;
        if (backButtonClicked)
        {
            playerCanvas.SetActive(true);
            moveCanvas.SetActive(false);
        }
        else
        {
            backButtonClicked = false;
        }
    }

    public void MoveChoiceOnClick()
    {
        playerIsGoingToMove = true;
        if (playerIsGoingToMove)
        {
            moveCanvas.SetActive(false);
            playerBehavior.MouseIsClicked = true;
        }
        else
        {
            playerIsGoingToMove = false;
        }
    }

    public void ConfirmOnClick()
    {
        confirmButtonClicked = true;
        if (confirmButtonClicked)
        {
            confirmCanvas.SetActive(false);
        }
        else
        {
            confirmButtonClicked = false;
        }
    }

}
