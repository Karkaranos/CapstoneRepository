/*************************************************
Author Names : 		    Aidan Ratcliffe
Date Created : 		    10/1/2025
Date Last Modified : 	10/2/2025
Brief Description : 	All Buttons will be managed within this script
External Resources : 	N/A
***************************************************/
using NaughtyAttributes;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    #region variables

    private enum Buttons
    {
        buttonsettings,
        Refs
    }

    [SerializeField] private Buttons showingButtons;

    [SerializeField, ShowIf(nameof(showingButtons), Buttons.Refs)] private PlayerBehavior playerBehavior;
    [SerializeField, ShowIf(nameof(showingButtons), Buttons.Refs)] public GameObject playerCanvas;
    [SerializeField, ShowIf(nameof(showingButtons), Buttons.Refs)] public GameObject moveCanvas;
    [SerializeField, ShowIf(nameof(showingButtons), Buttons.Refs)] public GameObject confirmCanvas;
    [SerializeField, ShowIf(nameof(showingButtons), Buttons.Refs)] public GameObject runeCanvas;
    [SerializeField, ShowIf(nameof(showingButtons), Buttons.Refs)] public Button moveButton;
    [SerializeField, ShowIf(nameof(showingButtons), Buttons.Refs)] public Button attackButton;
    [SerializeField, ShowIf(nameof(showingButtons), Buttons.Refs)] public Button backButton;
    [SerializeField, ShowIf(nameof(showingButtons), Buttons.Refs)] public Button confirmButton;
    [SerializeField, ShowIf(nameof(showingButtons), Buttons.Refs)] public Button endButton;
    public bool playerCanMove;
    public bool playerIsGoingToMove;
    public bool backButtonClicked;
    public bool confirmButtonClicked;
    public bool endButtonClicked;
    #endregion

    /// <summary>
    /// Grabs the player's script and button objects within the scene
    /// </summary>
    void Start()
    {
        playerBehavior = FindFirstObjectByType<PlayerBehavior>();
    }

    #region functions
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
    #endregion
}

