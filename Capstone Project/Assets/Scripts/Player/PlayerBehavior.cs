/*************************************************
Author Names : 		    Aidan Ratcliffe, Tyler Hayes, Brad Dixon, Cade Naylor
Date Created : 		    10/1/2025
Date Last Modified : 	2/5/2026 (Brad Dixon)
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
using NaughtyAttributes;

public class PlayerBehavior : MonoBehaviour
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
    [SerializeField] private Vector2Int myPosition;

    [Tooltip("How long in seconds the code should wait before moving in the same direction if a player holds down the direction.")]
    [SerializeField] private float continuousMoveDelay;
    private bool canMove;
    [SerializeField] private List<Vector3> movementPositions = new List<Vector3>();
    [SerializeField] private bool confirmMovement;
    [SerializeField] private List<Vector2Int> previousPositions = new List<Vector2Int>();
    [SerializeField] private float movementSpeed;
    [SerializeField] private int movementRange;
    [SerializeField] private int movementLeft;
    private Vector3 ghostPosition;

    [Button("Move Player")]
    private void MoveDaPlayer()
    {
        StartCoroutine(MovePlayer());
    }

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
        myPosition = GridManager.playerPosition;
        canMove = true;
        confirmMovement = false;
        previousPositions.Add(myPosition);
        ghostPosition = transform.position;
        movementLeft = movementRange;
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
        //PublicEvents.SelectTile += HandleTileClicked;
        TurnPublicEvents.BeginPlayerTurn += StartPlayerTurn;
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
        //PublicEvents.SelectTile -= HandleTileClicked;
        TurnPublicEvents.BeginPlayerTurn -= StartPlayerTurn;
    }

    private void StartPlayerTurn()
    {
        movementLeft = movementRange;
        canMove = true;
    }

    private void FixedUpdate()
    {
        if (confirmMovement)
        {
            StartCoroutine(MovePlayer());
            confirmMovement = false;
        }

        if (canMove)
        {
            if (Input.GetKey(KeyCode.W))
            {
                Vector2Int v = new Vector2Int(myPosition.x, myPosition.y + 1);
                if (GridManager.TileIsInGrid(v) && GridManager.CanMoveToTile(v, previousPositions[previousPositions.Count - 1]) 
                    &&(!previousPositions.Contains(v) || v == previousPositions[previousPositions.Count - 2]))
                {
                    Vector3 newPosition = new Vector3(ghostPosition.x, ghostPosition.y, ghostPosition.z + GridManager.MoveDistances.y);
                    //movementPositions.Add(newPosition);
                    UpdateMovement(v, newPosition);
                }
            }
            else if (Input.GetKey(KeyCode.S))
            {
                Vector2Int v = new Vector2Int(myPosition.x, myPosition.y - 1);
                if (GridManager.TileIsInGrid(v) && GridManager.CanMoveToTile(v, previousPositions[previousPositions.Count - 1])
                    && (!previousPositions.Contains(v) || v == previousPositions[previousPositions.Count - 2]))
                {
                    Vector3 newPosition = new Vector3(ghostPosition.x, ghostPosition.y, ghostPosition.z - GridManager.MoveDistances.y);
                    //movementPositions.Add(newPosition);
                    UpdateMovement(v, newPosition);
                }
            }
            else if (Input.GetKey(KeyCode.D))
            {
                Vector2Int v = new Vector2Int(myPosition.x + 1, myPosition.y);
                if (GridManager.TileIsInGrid(v) && GridManager.CanMoveToTile(v, previousPositions[previousPositions.Count - 1])
                    && (!previousPositions.Contains(v) || v == previousPositions[previousPositions.Count - 2]))
                {
                    Vector3 newPosition = new Vector3(ghostPosition.x + GridManager.MoveDistances.x, ghostPosition.y, ghostPosition.z);
                    //movementPositions.Add(newPosition);
                    UpdateMovement(v, newPosition);
                }
            }
            else if (Input.GetKey(KeyCode.A))
            {
                Vector2Int v = new Vector2Int(myPosition.x - 1, myPosition.y);
                if (GridManager.TileIsInGrid(v) && GridManager.CanMoveToTile(v, previousPositions[previousPositions.Count - 1])
                    && (!previousPositions.Contains(v) || v == previousPositions[previousPositions.Count - 2]))
                {
                    Vector3 newPosition = new Vector3(ghostPosition.x - GridManager.MoveDistances.x, ghostPosition.y, ghostPosition.z);
                    //movementPositions.Add(newPosition);
                    UpdateMovement(v, newPosition);
                }
            }
        }
    }

    private void UpdateMovement(Vector2Int v, Vector3 t)
    {
        canMove = false;
        if (previousPositions.Contains(v))
        {
            GridManager.combatGrid[myPosition.x, myPosition.y].ShowHighlight(false);
            previousPositions.Remove(myPosition);
            movementPositions.RemoveAt(movementPositions.Count - 1);
            ++movementLeft;
        }
        else
        {
            if (movementLeft > 0)
            {
                GridManager.combatGrid[v.x, v.y].SetHighlightColor(Color.black);
                GridManager.combatGrid[v.x, v.y].ShowHighlight(true);
                previousPositions.Add(v);
                movementPositions.Add(t);
                --movementLeft;
                if(movementLeft == 0)
                {
                    ghostPosition = t;
                    GridManager.MoveToTile(myPosition, v, -3);
                    myPosition = v;
                }
            }
        }

        if (movementLeft > 0)
        {
            ghostPosition = t;
            GridManager.MoveToTile(myPosition, v, -3);
            myPosition = v;
        }
        StartCoroutine(MovementDelay());
    }

    IEnumerator MovementDelay()
    {
        yield return new WaitForSeconds(continuousMoveDelay);
        canMove = true;
    }

    IEnumerator MovePlayer()
    {
        canMove = false;
        for(int i = 0; i < movementPositions.Count; ++i)
        {
            Vector2Int nextPosition = previousPositions[i + 1];
            bool isMoving = true;
            while(isMoving)
            {
                transform.position = Vector3.MoveTowards(transform.position, movementPositions[i], .1f);
                if (transform.position == movementPositions[i])
                {
                    isMoving = false;
                    GridManager.MoveToTile(myPosition, nextPosition, -3);
                    myPosition = nextPosition;
                }
                yield return new WaitForSeconds(.1f / movementSpeed);
            }
            GridManager.combatGrid[previousPositions[i].x, previousPositions[i].y].ShowHighlight(false);
        }
        GridManager.combatGrid[myPosition.x, myPosition.y].ShowHighlight(false);
        previousPositions.Clear();
        previousPositions.Add(myPosition);
        movementPositions.Clear();
        ghostPosition = transform.position;
        canMove = true;
    }

    /// <summary>
    /// Gets called whenever the player clicks on the tile
    /// 
    /// moves the player if they can move to the tile clicked on
    /// </summary>
    /// <param name="tBehav"></param>
    //private void HandleTileClicked(TileBehaviour tBehav)
    //{
    //    if(buttonManager == null )
    //    {
    //        buttonManager = FindFirstObjectByType<ButtonManager>();
    //    }

    //    if (PlayerCanMove && tBehav.inPlayerRange)
    //    {
    //        if (GridManager.CanMoveToTile(tBehav.IndexInGrid, myPosition))
    //        {
    //            //turns on the confirmation canvas
    //            targetPosition = tBehav.IndexInGrid;
    //            buttonManager.confirmCanvas.SetActive(true);
    //        }    
    //    }
    //}

    /// <summary>
    /// Finds all the adjacent tiles that are x distance away from the player
    /// and highlights them
    /// </summary>
    //private void EnableMovableTiles()
    //{
    //    if (gm.CurrentActionPoints - gm.MoveActionPoints >= 0)
    //    {
    //        tilesInRange.Clear();
    //        GridManager.combatGrid[MyPosition.x, MyPosition.y].inPlayerRange = true;
    //        tilesInRange.Add(GridManager.combatGrid[MyPosition.x, MyPosition.y]);
    //        List<Vector2Int> tilePositions = GridManager.GetAllValidAdjacentTiles(MyPosition, myPosition);
    //        foreach (Vector2Int v in tilePositions)
    //        {
    //            GridManager.combatGrid[v.x, v.y].inPlayerRange = true;
    //            GridManager.combatGrid[v.x, v.y].entityOnGrid = 5;
    //            tilesInRange.Add(GridManager.combatGrid[v.x, v.y]);
    //        }

    //        for (int i = 1; i < movementRange; ++i)
    //        {
    //            List<Vector2Int> adPositions = new List<Vector2Int>();
    //            foreach (Vector2Int v in tilePositions)
    //            {
    //                adPositions.Add(v);
    //            }

    //            tilePositions.Clear();
    //            foreach (Vector2Int v in adPositions)
    //            {
    //                List<Vector2Int> adAdPositions = GridManager.GetAllValidAdjacentTiles(v, myPosition);
    //                foreach (Vector2Int newPos in adAdPositions)
    //                {
    //                    if (newPos != myPosition)
    //                    {
    //                        tilePositions.Add(newPos);
    //                        GridManager.combatGrid[newPos.x, newPos.y].inPlayerRange = true;
    //                        GridManager.combatGrid[newPos.x, newPos.y].entityOnGrid = 5;
    //                        tilesInRange.Add(GridManager.combatGrid[newPos.x, newPos.y]);
    //                    }
    //                }
    //            }
    //        }

    //        foreach (TileBehaviour t in tilesInRange)
    //        {
    //            t.SetHighlightColor(Color.blue);
    //            t.ShowHighlight(true);
    //        }
    //        GridManager.ClearPathfinding();
    //    }

    //    //Calling here to avoid messing up highlight colors
    //    if(TogglePathVisualizer)
    //    {
    //        VisualizeEnemyPaths();
    //    }
    //}

    /// <summary>
    /// Allows the player to move from tile to tile instead of teleport
    /// </summary>
    //public override void PathfindThroughGrid()
    //{
    //    isEnemy = false;
    //    pathfindingLimit = movementRange;
    //    foreach(TileBehaviour t in tilesInRange)
    //    {
    //        t.ShowHighlight(false);
    //    }
    //    base.PathfindThroughGrid();
    //    anim.SetTrigger("Walk");
    //    StartMoveCoroutine();
    //}

    /// <summary>
    /// Turns the action canvas back on when the player is done moving to their selected tile
    /// </summary>
    /// <returns></returns>
    private void ReEnableActionCanvas()
    {
        gm.UpdateActionPoints(gm.MoveActionPoints);
        buttonManager.ReEnableActionCanvas();
        //EnableMovableTiles();
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
