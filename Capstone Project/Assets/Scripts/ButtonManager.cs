/*************************************************
Author Names : 		    Aidan Ratcliffe, Cade Naylor, Tyler Hayes, Brad Dixon
Date Created : 		    10/1/2025
Date Last Modified : 	2/12/2026 (Brad Dixon)
Brief Description : 	All Buttons will be managed within this script
External Resources : 	N/A
***************************************************/
using NaughtyAttributes;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;
using System.Threading.Tasks;

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
    [SerializeField, ShowIf(nameof(showingButtons), Buttons.Refs)] private CameraManager cameraManager;
    [SerializeField, ShowIf(nameof(showingButtons), Buttons.Refs)] private GameObject playerCanvas;
    //[SerializeField, ShowIf(nameof(showingButtons), Buttons.Refs)] private GameObject moveCanvas;
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
    public bool playerIsGoingToMove;
    public bool backButtonClicked;
    public bool confirmButtonClicked;
    public bool endButtonClicked;

    private GameManager gm; // temp variable
    private TransitionManager tm;

    private bool isPlayersTurn;
    private bool castingSpell;

    [HideInInspector] public bool Moving = false;

    private GameObject wasdObject;
    #endregion

    /// <summary>
    /// Grabs the player's script and button objects within the scene
    /// </summary>
    void Start()
    {
        cameraManager = FindFirstObjectByType<CameraManager>();
        gm = FindFirstObjectByType<GameManager>();
        tm = FindFirstObjectByType<TransitionManager>();
        playerBehavior = FindFirstObjectByType<PlayerBehavior>();        
    }

    #region functions

    /// <summary>
    /// Subscribes to all public events
    /// </summary>
    private void OnEnable()
    {
        TurnPublicEvents.BeginPlayerTurn += PlayerStartTurn;
        TurnPublicEvents.BeginEnemyTurn += EnemyTurnStarted;
        PublicEvents.NewLevel += SetPlayerReference;
    }

    /// <summary>
    /// Unsubscribes to all public events
    /// </summary>
    private void OnDisable()
    {
        TurnPublicEvents.BeginPlayerTurn -= PlayerStartTurn;
        TurnPublicEvents.BeginEnemyTurn -= EnemyTurnStarted;
        PublicEvents.NewLevel -= SetPlayerReference;
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
        moveButton.interactable = true;
        runeCanvas.SetActive(true);
        castingSpell = false;
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

        TurnPublicEvents.TurnActionComplete();
    }

    /// <summary>
    /// Controls the player movement, sets confirm canvas to true
    /// and playerCanvas to false
    /// </summary>
    public void MoveButtonOnClick()
    {

        Moving = true;

        if(playerBehavior == null)
        {
            playerBehavior = FindFirstObjectByType<PlayerBehavior>();
        }
        if(playerBehavior.MovementLeft > 0)
        {

            playerBehavior.SetPlayerMovementStatus(true);

            gm.GetComponent<PlayerInputHandler>().enableMovement = true;
            playerBehavior.UpdateEnemyPositions();
            confirmCanvas.SetActive(true);
            playerCanvas.SetActive(false);
            GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].entityOnGrid = -1;
            PublicEvents.MoveButton();
        }   
    }

    /// <summary>
    /// Sets the runeCanvas to true and playerCanvas to false
    /// </summary>
    public void AttackOnClick()
    {

        runeCanvas.SetActive(true);
        playerCanvas.SetActive(false);
        castingSpell = true;

    }

    /// <summary>
    /// Sets backButtonClicked to true
    /// bool sets the PlayerCanvas to true and sets the moveCanvas and runeCanvas to false
    /// </summary>
    public void BackButtonOnClick()
    {
        PublicEvents.HideDamagePreview.Invoke();
        if(Moving)
        {
            Moving = false;
        }

        confirmCanvas.SetActive(false);
        playerCanvas.SetActive(true);

        if (FindFirstObjectByType<RuneRangeAndTargeting>().WaitingForThePlayer)
        {

            if (confirmButton.interactable == false)
            {
                confirmButton.interactable = true;
            }
            PublicEvents.EndCast.Invoke();
            return;

        }

        playerBehavior = FindFirstObjectByType<PlayerBehavior>();
        playerBehavior.DeleteMovement();

        PublicEvents.EndCast?.Invoke();
        PublicEvents.MoveButton();
    }

    /// <summary>
    /// Skips the cutscene on button click
    /// Sets VideoCanvas to false
    /// </summary>
    public void SkipCutscene()
    {
        if(tm == null)
        {
            tm = FindFirstObjectByType<TransitionManager>();
        }
        tm.SkipButtonTransition();
    }

    /// <summary>
    /// Disables the video canvas at a specific spot in the transition
    /// </summary>
    public void DisableVideoCanvas()
    {
        videoCanvas.SetActive(false);
    }

    /// <summary>
    /// Public call for when the cutscene ends naturally
    /// </summary>
    public void CutsceneEnd()
    {
        if (tm == null)
        {
            tm = FindFirstObjectByType<TransitionManager>();
        }
        //videoCanvas.SetActive(false);
        tm.CutsceneTransition();
    }


    /// <summary>
    /// Sets PlayerCanMove bool from playerBehavior to false
    /// Sets confirmCanvas to false
    /// Sets playerCanvas to true
    /// </summary>
    public void ConfirmOnClick()
    {
        PublicEvents.HideDamagePreview();

        if(!FindFirstObjectByType<RuneEvents>().WaitingOnPath && FindFirstObjectByType<RuneEvents>().Pathing)
        {
            return;
        }

        if(FindFirstObjectByType<RuneRangeAndTargeting>().WaitingForThePlayer)
        {

            if(FindFirstObjectByType<RuneEvents>().WaitingOnPath)
            {

                FindFirstObjectByType<RuneRangeAndTargeting>().selectedTile =
                GridManager.combatGrid[FindFirstObjectByType<RuneEvents>().selectedTile.x, 
                FindFirstObjectByType<RuneEvents>().selectedTile.y];

            }

            playerCanvas.SetActive(false);
            confirmCanvas.SetActive(false);
            PublicEvents.SpellConfirmed.Invoke();

        }
        else
        {

            playerBehavior.ConfirmMovement();
            moveButton.interactable = false;
            PublicEvents.MoveButton();

        }
      
    }

    /// <summary>
    /// Used to reenable the player canvas when canceling the movement
    /// </summary>
    public void ResetCanvas()
    {
        confirmCanvas.SetActive(false);
        playerCanvas.SetActive(true);
    }

    /// <summary>
    /// Turns the action canvas back on and disables the move button if needed
    /// </summary>
    public void ReEnableActionCanvas()
    {

        Moving = false;

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
    /// Hides the player and confirm canvas
    /// </summary>
    public void HideAllCanvas()
    {
        playerCanvas.SetActive(false);
        confirmCanvas.SetActive(false);
    }

    /// <summary>
    /// Enables the endButtonClicked bool to true
    /// Ends Player Turn once clicked
    /// </summary>
    public void EndTurnClick()
    {
        endButtonClicked = true;

        playerCanvas.SetActive(false);
        
        if(playerBehavior == null) { playerBehavior = FindFirstObjectByType<PlayerBehavior>(); }


        TurnPublicEvents.ForceEndCurrentPhase();
     }

    private async void SetPlayerReference()
    {
        //await Task.Delay(500);
        playerBehavior = FindFirstObjectByType<PlayerBehavior>();
    }

    #endregion
}

