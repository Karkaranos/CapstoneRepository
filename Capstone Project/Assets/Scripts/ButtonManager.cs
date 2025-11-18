/*************************************************
Author Names : 		    Aidan Ratcliffe, Cade Naylor, Tyler Hayes
Date Created : 		    10/1/2025
Date Last Modified : 	11/7/2025 Clare Grady
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

    [HorizontalLine(4, EColor.Red)]

    [SerializeField, ShowIf(nameof(showingButtons), Buttons.Refs)] private PlayerBehavior playerBehavior;
    [SerializeField, ShowIf(nameof(showingButtons), Buttons.Refs)] private GameObject playerCanvas;
    [SerializeField, ShowIf(nameof(showingButtons), Buttons.Refs)] private GameObject moveCanvas;
    [SerializeField, ShowIf(nameof(showingButtons), Buttons.Refs)] public GameObject confirmCanvas;
    [SerializeField, ShowIf(nameof(showingButtons), Buttons.Refs)] public GameObject videoCanvas;
    [SerializeField, ShowIf(nameof(showingButtons), Buttons.Refs)] private GameObject runeCanvas;
    [SerializeField, ShowIf(nameof(showingButtons), Buttons.Refs)] private Button moveButton;
    [SerializeField, ShowIf(nameof(showingButtons), Buttons.Refs)] private Button attackButton;
    [SerializeField, ShowIf(nameof(showingButtons), Buttons.Refs)] private Button backButton;
    [SerializeField, ShowIf(nameof(showingButtons), Buttons.Refs)] private Button confirmButton;
    [SerializeField, ShowIf(nameof(showingButtons), Buttons.Refs)] private Button endButton;
    [SerializeField, ShowIf(nameof(showingButtons), Buttons.Refs)] private Button skipcutButton;
    public bool playerCanMove;
    public bool cutsceneSkipped;
    public bool playerIsGoingToMove;
    public bool backButtonClicked;
    public bool confirmButtonClicked;
    public bool endButtonClicked;

    private GameManager gm; // temp variable

    private bool isPlayersTurn;
    #endregion

    /// <summary>
    /// Grabs the player's script and button objects within the scene
    /// </summary>
    void Start()
    {
        playerBehavior = FindFirstObjectByType<PlayerBehavior>();
        gm = FindFirstObjectByType<GameManager>();
    }

    #region functions

    /// <summary>
    /// Subscribes to all public events
    /// </summary>
    private void OnEnable()
    {
        TurnPublicEvents.BeginPlayerTurn += PlayerStartTurn;
        TurnPublicEvents.BeginEnemyTurn += EnemyTurnStarted;
    }

    /// <summary>
    /// Unsubscribes to all public events
    /// </summary>
    private void OnDisable()
    {
        TurnPublicEvents.BeginPlayerTurn -= PlayerStartTurn;
        TurnPublicEvents.BeginEnemyTurn -= EnemyTurnStarted;
    }

    /// <summary>
    /// Turns on the canvas when the player's turn starts
    /// 
    /// Does not call turn action complete cus the player's turn ends when they press the button or
    /// when they run out of AP
    /// </summary>
    private void PlayerStartTurn()
    {
        isPlayersTurn = true;
        playerCanvas.SetActive(true);
    }

    /// <summary>
    /// Closes the player's canvas when the enemies turn starts, turns off the canvas
    /// </summary>
    private void EnemyTurnStarted()
    {
        isPlayersTurn = false;
        playerCanvas.SetActive(false);
        runeCanvas.SetActive(false);
        moveButton.interactable = true;

        if (playerBehavior.tilesInRange.Count > 0)
        {
            foreach (TileBehaviour t in playerBehavior.tilesInRange)
            {
                t.ShowHighlight(true);
            }
        }

        TurnPublicEvents.TurnActionComplete();
    }

    /// <summary>
    /// Controls the player movement, sets confirm canvas to true
    /// and playerCanvas to false
    /// </summary>
    public void MoveButtonOnClick()
    {
        if(gm.CurrentActionPoints >= gm.MoveActionPoints)
        {
            Debug.Log("The player can move!");
            if (playerBehavior == null)
            {
                playerBehavior = FindFirstObjectByType<PlayerBehavior>();
            }
            playerBehavior.PlayerCanMove = true;
            //confirmCanvas.SetActive(true);
            playerCanvas.SetActive(false);
        }
        else
        {
            Logger.Warning("Not enough Action Points!");
        }    
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
        playerCanvas.SetActive(true);
        moveCanvas.SetActive(false);
        runeCanvas.GetComponent<RuneEvents>().CancelCasting();
        runeCanvas.SetActive(false);
        confirmCanvas.SetActive(false);

        if (playerBehavior != null)
        {
            playerBehavior.PlayerCanMove = false;
        }

    }

    public void SkipCutscene()
    {
        cutsceneSkipped = true;
        if (cutsceneSkipped)
        {
            videoCanvas.SetActive(false);
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
        playerBehavior.PathfindThroughGrid();
    }

    /// <summary>
    /// Turns the action canvas back on and disables the move button if needed
    /// </summary>
    public void ReEnableActionCanvas()
    {
        if (isPlayersTurn)
        {
            if (gm.CurrentActionPoints - gm.MoveActionPoints < 0)
            {
                moveButton.interactable = false;
            }
            playerCanvas.SetActive(true);
        }
    }

    /// <summary>
    /// Enables the endButtonClicked bool to true
    /// Ends Player Turn once clicked
    /// </summary>
    public void EndTurnClick()
    {
        Debug.Log("button clicked");
        endButtonClicked = true;

        playerCanvas.SetActive(false);
        
        if(playerBehavior == null) { playerBehavior = FindFirstObjectByType<PlayerBehavior>(); }
       

        TurnPublicEvents.ForceEndCurrentPhase();
     }

    #endregion
}

