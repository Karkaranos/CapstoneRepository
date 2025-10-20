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
    [SerializeField] GameObject runeCanvas;
    public Button moveButton;
    [SerializeField] Button attackButton;
    public Button moveChoiceButton;
    public Button backButton;
    public Button confirmButton;
    public Button endButton;
    public bool playerCanMove;
    public bool playerIsGoingToMove;
    public bool backButtonClicked;
    public bool confirmButtonClicked;
    public bool endButtonClicked;

    /// <summary>
    /// Grabs the player's script and button objects within the scene
    /// </summary>
    void Start()
    {
        playerBehavior = FindFirstObjectByType<PlayerBehavior>();
        Button mbtn = moveButton.GetComponent<Button>();
        Button mcbtn = moveChoiceButton.GetComponent<Button>();
        Button bbtn = backButton.GetComponent<Button>();
        Button cbtn = confirmButton.GetComponent<Button>();
        Button ebtn = endButton.GetComponent<Button>();
        Canvas pCanvas = playerCanvas.GetComponent<Canvas>();
        Canvas mCanvas = moveCanvas.GetComponent<Canvas>();
        Canvas cCanvas = confirmCanvas.GetComponent<Canvas>();
    }

    /// <summary>
    /// Controls the player movement, sets confirm canvas to true
    /// and playerCanvas to false
    /// </summary>
    public void MoveButtonOnClick()
    {
        Debug.Log("The player can move!");
        if(playerBehavior == null)
        {
            playerBehavior = FindFirstObjectByType<PlayerBehavior>();
        }
        playerBehavior.PlayerCanMove = true;
        confirmCanvas.SetActive(true);
        playerCanvas.SetActive(false);
        //playerCanMove = true;
        //if(playerCanMove)
        //{
        //    moveCanvas.SetActive(true);
        //    playerCanvas.SetActive(false);
        //}
        //else
        //{
        //    playerCanMove = false;
        //}
    }

    /// <summary>
    /// Sets the runeCanvas to true and playerCanvas to false
    /// </summary>
    public void AttackOnClick()
    {

        runeCanvas.SetActive(true);
        playerCanvas.SetActive(false);

    }

    /// <summary>
    /// Sets backButtonClicked to true
    /// bool sets the PlayerCanvas to true and sets the moveCanvas and runeCanvas to false
    /// </summary>
    public void BackButtonOnClick()
    {
        Debug.Log("goin back!");
        backButtonClicked = true;
        if (backButtonClicked)
        {
            playerCanvas.SetActive(true);
            moveCanvas.SetActive(false);
            runeCanvas.SetActive(false);
        }
        else
        {
            backButtonClicked = false;
        }
    }

    /// <summary>
    /// Sets the MoveCanvas to false and playerIsGoingToMove bool to true
    /// </summary>
    public void MoveChoiceOnClick()
    {
        playerIsGoingToMove = true;
        if (playerIsGoingToMove)
        {
            moveCanvas.SetActive(false);
            //GetComponent<PlayerBehavior>().enabled = true;
        }
        else
        {
            playerIsGoingToMove = false;
        }
    }

    /// <summary>
    /// Sets PlayerCanMove bool from playerBehavior to false
    /// Sets confirmCanvas to false
    /// Sets playerCanvas to true
    /// </summary>
    public void ConfirmOnClick()
    {
        playerBehavior.PlayerCanMove = false;
        confirmCanvas.SetActive(false);
        playerCanvas.SetActive(true);
        //confirmButtonClicked = true;
        //if (confirmButtonClicked)
        //{
        //    confirmCanvas.SetActive(false);
        //    playerCanvas.SetActive(true);
        //}
        //else
        //{
        //    confirmButtonClicked = false;
        //}
    }

    /// <summary>
    /// Enables the endButtonClicked bool to true
    /// Ends Player Turn once clicked
    /// </summary>
    public void EndTurnClick()
    {
        Debug.Log("button clicked");
        endButtonClicked = true;


        TurnPublicEvents.TurnActionComplete();
        /*        if (endButtonClicked)
                {
                    if (EnemyTurn())
                    {
                        //playerCanvas.SetActive(false);
                    }
                    else
                    {*/
        //FindFirstObjectByType<TurnBasedBattleSystem>().PlayerTurnTime();
         //   }
        }
    }

