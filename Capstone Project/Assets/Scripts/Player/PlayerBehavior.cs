/*************************************************
Author Names : 		    Aidan Ratcliffe, Tyler Hayes, Brad Dixon, Cade Naylor
Date Created : 		    10/1/2025
Date Last Modified : 	1/28/2026 (Brad Dixon)
Brief Description : 	This how the player will detect where the grid is
External Resources : 	N/A
***************************************************/
using NUnit.Framework;
using PlayerInputActions;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Unity.Cinemachine;

public class PlayerBehavior : GridPathfinding
{
    #region player variables
    [Tooltip("reference to the player movement and its actions")]
    public Input playerInput;
    [SerializeField] private InputAction playerClick;
    [SerializeField] private InputAction playermoveClick;
    [SerializeField, Tooltip("A reference to the object the Animator is on")] private GameObject animObj;
    private Animator anim;

    [Tooltip("references the player's game object")]
    public GameObject player;

    //[Tooltip("Player Position and the position it wants to go to")]
    //private Vector2Int playerPosition;
    //public Vector2Int clickedTile;

    [Tooltip("Scripts the playerbehavior is deriving from")]
    private ButtonManager buttonManager;

    [Tooltip("bool to check to see if the mouse input is activated")]
    public bool MouseIsClicked;

    [HideInInspector] public bool PlayerCanMove = false;
    [HideInInspector] public bool CurrentlyTryingToAttack = false;
    #endregion playervariables
    public List<TileBehaviour> tilesInRange = new List<TileBehaviour>();

    [Tooltip("If true, enemy paths will be shown during the player's turn")]
    public bool TogglePathVisualizer;
    GameManager gm;
    /// <summary>
    /// Start is called once before the first execution of Update after the MonoBehaviour is created
    /// Sets player position and target position to reference the grid manager's player position and
    /// target position to the TileBehaviour Index 
    /// </summary>
    void Start()
    {
        buttonManager = FindFirstObjectByType<ButtonManager>();
        gm = FindFirstObjectByType<GameManager>(FindObjectsInactive.Exclude);
        anim = animObj.GetComponent<Animator>();
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
        TurnPublicEvents.BeginPlayerTurn += EnableMovableTiles;
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
        TurnPublicEvents.BeginPlayerTurn -= EnableMovableTiles;
    }

    /// <summary>
    /// Gets called whenever the player clicks on the tile
    /// 
    /// moves the player if they can move to the tile clicked on
    /// </summary>
    /// <param name="tBehav"></param>
    private void HandleTileClicked(TileBehaviour tBehav)
    {
        if(buttonManager == null )
        {
            buttonManager = FindFirstObjectByType<ButtonManager>();
        }

        if (PlayerCanMove && tBehav.inPlayerRange)
        {
            if (GridManager.CanMoveToTile(tBehav.IndexInGrid, myPosition))
            {
                //turns on the confirmation canvas
                targetPosition = tBehav.IndexInGrid;
                buttonManager.confirmCanvas.SetActive(true);
            }    
        }
    }

    /// <summary>
    /// Finds all the adjacent tiles that are x distance away from the player
    /// and highlights them
    /// </summary>
    private void EnableMovableTiles()
    {
        if (gm.CurrentActionPoints - gm.MoveActionPoints >= 0)
        {
            tilesInRange.Clear();
            GridManager.combatGrid[MyPosition.x, MyPosition.y].inPlayerRange = true;
            tilesInRange.Add(GridManager.combatGrid[MyPosition.x, MyPosition.y]);
            List<Vector2Int> tilePositions = GridManager.GetAllValidAdjacentTiles(MyPosition, myPosition);
            foreach (Vector2Int v in tilePositions)
            {
                GridManager.combatGrid[v.x, v.y].inPlayerRange = true;
                GridManager.combatGrid[v.x, v.y].entityOnGrid = 5;
                tilesInRange.Add(GridManager.combatGrid[v.x, v.y]);
            }

            for (int i = 1; i < movementRange; ++i)
            {
                List<Vector2Int> adPositions = new List<Vector2Int>();
                foreach (Vector2Int v in tilePositions)
                {
                    adPositions.Add(v);
                }

                tilePositions.Clear();
                foreach (Vector2Int v in adPositions)
                {
                    List<Vector2Int> adAdPositions = GridManager.GetAllValidAdjacentTiles(v, myPosition);
                    foreach (Vector2Int newPos in adAdPositions)
                    {
                        if (newPos != myPosition)
                        {
                            tilePositions.Add(newPos);
                            GridManager.combatGrid[newPos.x, newPos.y].inPlayerRange = true;
                            GridManager.combatGrid[newPos.x, newPos.y].entityOnGrid = 5;
                            tilesInRange.Add(GridManager.combatGrid[newPos.x, newPos.y]);
                        }
                    }
                }
            }

            foreach (TileBehaviour t in tilesInRange)
            {
                t.SetHighlightColor(Color.blue);
                t.ShowHighlight(true);
            }
            GridManager.ClearPathfinding();
        }

        //Calling here to avoid messing up highlight colors
        if(TogglePathVisualizer)
        {
            VisualizeEnemyPaths();
        }
    }

    /// <summary>
    /// Allows the player to move from tile to tile instead of teleport
    /// </summary>
    public override void PathfindThroughGrid()
    {
        isEnemy = false;
        pathfindingLimit = movementRange;
        foreach(TileBehaviour t in tilesInRange)
        {
            t.ShowHighlight(false);
        }
        base.PathfindThroughGrid();
        anim.SetTrigger("Walk");
        StartMoveCoroutine();
    }

    /// <summary>
    /// Turns the action canvas back on when the player is done moving to their selected tile
    /// </summary>
    /// <returns></returns>
    protected override void ReEnableActionCanvas()
    {
        gm.UpdateActionPoints(gm.MoveActionPoints);
        buttonManager.ReEnableActionCanvas();
        EnableMovableTiles();
        anim.SetTrigger("Idle");
    }

    /// <summary>
    /// When called, displays the projected path that enemies will take
    /// </summary>
    public void VisualizeEnemyPaths()
    {
        //GridManager.ClearGhostEntities();

        foreach (Enemy e in gm.GetComponent<EnemyHandler>().enemies)
        {
            e.gameObject.GetComponent<GridPathfinding>().ShowPath();
        }
    }
    #endregion
}
