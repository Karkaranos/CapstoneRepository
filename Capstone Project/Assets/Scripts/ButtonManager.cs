/*************************************************
Author Names : 		    Aidan Ratcliffe
Date Created : 		    10/1/2025
Date Last Modified : 	10/1/2025
Brief Description : 	All Buttons will be managed within this script
External Resources : 	N/A
***************************************************/
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public GameObject PlayerCanvas;
    public GameObject MoveCanvas;
    public Button MoveButton;
    public Button BackButton;
    //public Button ConfirmButton;
    //public Button MoveChoiceButton;
    public bool PlayerCanMove;
    public bool PlayerIsGoingToMove;
    public bool BackButtonClicked;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Button mbtn = MoveButton.GetComponent<Button>();
        Button bbtn = BackButton.GetComponent<Button>();
        //Button cbtn = ConfirmButton.GetComponent<Button>();
        //Button mcbtn = MoveChoiceButton.GetComponent<Button>();
        Canvas playerCanvas = PlayerCanvas.GetComponent<Canvas>();
        Canvas moveCanvas = MoveCanvas.GetComponent<Canvas>();
    }

    public void MoveButtonOnClick()
    {
        Debug.Log("The player can move!");
        PlayerCanMove = true;
        if(PlayerCanMove)
        {
            MoveCanvas.SetActive(true);
            PlayerCanvas.SetActive(false);
        }
        else
        {
            PlayerCanMove = false;
        }
    }

    public void BackButtonOnClick()
    {
        Debug.Log("goin back!");
        BackButtonClicked = true;
        if (BackButtonClicked)
        {
            PlayerCanvas.SetActive(true);
            MoveCanvas.SetActive(false);
        }
        else
        {
            BackButtonClicked = false;
        }
    }

    public void ConfirmOnClick()
    {

    }

}
