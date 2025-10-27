/*************************************************
Author Names : 		    Aidan Ratcliffe, Tyler Hayes
Date Created : 		    10/1/2025
Date Last Modified : 	10/27/2025
Brief Description : 	This how the player will detect where the grid is
External Resources : 	N/A
***************************************************/
using NUnit.Framework;
using PlayerInputActions;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerBehavior : MonoBehaviour
{
    #region player variables
    [Tooltip("reference to the player movement and its actions")]
    public Input playerInput;
    [SerializeField] private InputAction playerClick;
    [SerializeField] private InputAction playermoveClick;

    [Tooltip("references the player's game object")]
    public GameObject player;

    [Tooltip("Player Position and the position it wants to go to")]
    private Vector2Int playerPosition;
    public Vector2Int targetPosition;

    [Tooltip("Scripts the playerbehavior is deriving from")]
    private TileBehavior tileBehavior;
    private GridManager gridManager;
    private ButtonManager buttonManager;

    [Tooltip("bool to check to see if the mouse input is activated")]
    public bool MouseIsClicked;

    [HideInInspector] public bool PlayerCanMove = false;
    [HideInInspector] public bool CurrentlyTryingToAttack = false;
    #endregion playervariables

    /// <summary>
    /// Start is called once before the first execution of Update after the MonoBehaviour is created
    /// Sets player position and target position to reference the grid manager's player position and
    /// target position to the TileBehaviour Index 
    /// </summary>
    void Start()
    {
        buttonManager = FindFirstObjectByType<ButtonManager>();
        playerPosition = new Vector2Int(GridManager.playerPosition.x, GridManager.playerPosition.y);
        targetPosition = GridManager.playerPosition;
    }

    #region player input
    
    /// <summary>
    /// Enables PlayerClick Input Action
    /// </summary>
    public void OnEnable()
    {
        playerClick.Enable();
        playermoveClick.Enable();
        playermoveClick.started += playermoveClickPerformed;
        PublicEvents.SelectTile += HandleTileClicked;
    }

    //Sets the boolean to true when left mouse button is clicked
    private void playermoveClickPerformed(InputAction.CallbackContext context)
    {
        MouseIsClicked = true;
    }

    /// <summary>
    /// Disables PlayerClick Input Action
    /// </summary>
    void OnDisable()
    {
        playerClick.Disable();
        playermoveClick.Disable();
        playermoveClick.started -= playermoveClickPerformed;
        PublicEvents.SelectTile -= HandleTileClicked;
    }

    /// <summary>
    /// Gets called whenever the player clicks on the tile
    /// 
    /// moves the player if they can move to the tile clicked on
    /// </summary>
    /// <param name="tBehav"></param>
    private void HandleTileClicked(TileBehaviour tBehav)
    {
        if (PlayerCanMove)
        {
            //moves the player to the selected tile
            gameObject.transform.position = tBehav.gameObject.transform.position;
            GridManager.MoveToTile(playerPosition, tBehav.IndexInGrid, -3);
            playerPosition = tBehav.IndexInGrid;

            //turns on the confirmation canvas
            buttonManager.confirmCanvas.SetActive(true);
        }
    }


    
    #endregion
}
