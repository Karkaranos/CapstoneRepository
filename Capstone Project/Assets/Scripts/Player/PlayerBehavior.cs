/*************************************************
Author Names : 		    Aidan Ratcliffe, Tyler Hayes, Brad Dixon, Cade Naylor
Date Created : 		    10/1/2025
Date Last Modified : 	2/19/2026 (Brad Dixon)
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
    [SerializeField, Tooltip("A reference to the object the Animator is on")] private GameObject bookanimObj;
    private Animator bookanim;

    [Tooltip("references the player's game object")]
    public GameObject player;

    //[Tooltip("Player Position and the position it wants to go to")]
    //private Vector2Int playerPosition;
    //public Vector2Int clickedTile;

    [Tooltip("Scripts the playerbehavior is deriving from")]
    private ButtonManager buttonManager;

    [Tooltip("bool to check to see if the mouse input is activated")]
    public bool MouseIsClicked;

    [HideInInspector] public bool CurrentlyTryingToAttack = false;
    #endregion playervariables
    //Outdated but can't be removed because it will cause errors in RuneRangeAndTargeting
    [HideInInspector] public List<TileBehaviour> tilesInRange = new List<TileBehaviour>(); 

    [Tooltip("If true, enemy paths will be shown during the player's turn")]
    public bool TogglePathVisualizer;
    GameManager gm;
    private Vector2Int myPosition;

    [Tooltip("How long in seconds the code should wait before moving in the same direction if a player holds down the direction.")]
    [SerializeField] private float continuousMoveDelay;
    private bool canMove;
    private List<Vector3> movementPositions = new List<Vector3>();
    private List<Vector2Int> previousPositions = new List<Vector2Int>();
    [Tooltip("How fast the player moves from tile to tile.")]
    [SerializeField] private float movementSpeed;
    [Tooltip("Total amount of movement the player has on their turn.")]
    [SerializeField] private int movementRange;
    private int movementLeft;
    [HideInInspector]
    public int MovementLeft
    {
        get { return movementLeft; }
        set { movementLeft = value; }
    }
    private Vector3 ghostPosition;
    [Tooltip("If true, the player will not have to use all of their movement in order to move.")]
    [SerializeField] bool allowLeftoverMovement;
    [Tooltip("If enabled, the player will only be allowed to move once on their turn.")]
    [SerializeField, ShowIf("allowLeftoverMovement")] bool onlyMoveOnce;
    Vector2Int posBeforeMovement;
    private int movementUsed;

    private BoxCollider myCol;
    [SerializeField] Vector3 previousColliderPos;
    private List<Vector2Int> enemyPositions = new List<Vector2Int>();
    [SerializeField] private bool underEffect;


    [Header("Child References")]
    [Tooltip("A reference to the player's Canvas"), SerializeField, Required]
    private Transform pTransform;
    [Tooltip("A reference to the player's Sprite Renderer"), SerializeField, Required]
    private SpriteRenderer pSprite;
    [Tooltip("A reference to the book's Sprite Renderer"), SerializeField, Required]
    private SpriteRenderer bSprite;

    private ButtonManager bm;
    private RuneEvents re;

    /// <summary>
    /// Start is called once before the first execution of Update after the MonoBehaviour is created
    /// Sets player position and target position to reference the grid manager's player position and
    /// target position to the TileBehaviour Index 
    /// </summary>
    void Start()
    {
        gm = FindFirstObjectByType<GameManager>(FindObjectsInactive.Exclude);
        re = FindAnyObjectByType<RuneEvents>(FindObjectsInactive.Exclude);
        anim = animObj.GetComponentInChildren<Animator>();
        bookanim = bookanimObj.GetComponentInChildren<Animator>();
        re.AssignAnim(anim);
        re.AssignAnim(bookanim);
        myPosition = GridManager.playerPosition;
        canMove = true;
        ghostPosition = transform.position;
        movementLeft = movementRange;
        previousPositions.Add(myPosition);
        myCol = GetComponent<BoxCollider>();
        previousColliderPos = myCol.center;
        underEffect = false;
        bm = FindFirstObjectByType<ButtonManager>();

        FindFirstObjectByType<RuneEvents>().Casting = false;
        FindFirstObjectByType<PlayerStats>().FullHeal();
        PublicEvents.NewPlayerCreated?.Invoke(pTransform, pSprite);
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
        PublicEvents.MovementDirection += MoveDirection;
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
        PublicEvents.MovementDirection -= MoveDirection;
    }

    /// <summary>
    /// Updates the variables to where the player is when they begin their turn. Also finds the enemies
    /// </summary>
    private void StartPlayerTurn()
    {
        enemyPositions.Clear();
        movementLeft = movementRange;
        canMove = true;
        posBeforeMovement = myPosition;
        movementUsed = 0;

        foreach (Enemy e in gm.GetComponent<EnemyHandler>().enemies)
        {
            enemyPositions.Add(e.gameObject.GetComponent<GridPathfinding>().MyPosition);
        }

        ShieldBehavior[] allShields = FindObjectsByType<ShieldBehavior>(FindObjectsSortMode.None);
        if(allShields.Length >= 1)
        {

            foreach(ShieldBehavior shield in allShields)
            {

                GridManager.RemoveEntity(shield.GetComponentInParent<TileBehaviour>().IndexInGrid);
                shield.GetDestroyed();

            }

        }

        WindCurrentTracker[] allCurrents = FindObjectsByType<WindCurrentTracker>(FindObjectsSortMode.None);
        if (allCurrents.Length >= 1)
        {

            foreach (WindCurrentTracker current in allCurrents)
            {

                current.DestroyCurrents();

            }

        }

        if (TogglePathVisualizer)
        {
            VisualizeEnemyPaths();
        }

    }

    /// <summary>
    /// Removes the enemy position from the list when an enemy dies. This fixes the 
    /// bug where the player couldn't move the an enemy tile on the same turn it died.
    /// </summary>
    /// <param name="pos"></param>
    public void RemoveEnemyPosition(Vector2Int pos)
    {
        enemyPositions.Remove(pos);
    }

    /// <summary>
    /// Updates the player's reference to the enemies positions when an enemy is moved to a new tile by a spell
    /// </summary>
    public void UpdateEnemyPositions()
    {
        enemyPositions.Clear();
        {
            foreach (Enemy e in gm.GetComponent<EnemyHandler>().enemies)
            {
                enemyPositions.Add(e.gameObject.GetComponent<GridPathfinding>().MyPosition);
            }
        }
    }

    /// <summary>
    /// ensures that the player won't trigger movement while the player is pathing an attack
    /// </summary>
    /// <param name="canThePlayerMove"></param>
    public void SetPlayerMovementStatus(bool canThePlayerMove)
    {

        canMove = canThePlayerMove;

    }

    /// <summary>
    /// Reads in an input to determine which direction the player is moving in
    /// </summary>
    /// <param name="dir"></param> Vector2 input from the player
    private void MoveDirection(Vector2 dir)
    {
        if (canMove)
        {
            if (dir.y >= .5f)
            {
                Vector2Int v = new Vector2Int(myPosition.x, myPosition.y + 1);
                if (GridManager.TileIsInGrid(v) && //Checks to make sure the attempted positsion is withing the index of the grid
                    (GridManager.CanMoveToTile(v, previousPositions[previousPositions.Count - 1], true) || //Checks to make sure the new tile is an open space
                    (GridManager.combatGrid[v.x, v.y].entityOnGrid == -20 && !enemyPositions.Contains(v))) && //If the tile isn't open, this checks to make sure it isn't because of an enemy's projected path
                    (!previousPositions.Contains(v) || v == previousPositions[previousPositions.Count - 2]) && //This check is so you can go back one space in the path but not travel over your path
                    !enemyPositions.Contains(v)) //Checks to make sure you can't move onto where the enemy is due to how visualizing their path works
                {
                    Vector3 newPosition = new Vector3(ghostPosition.x, ghostPosition.y, ghostPosition.z + GridManager.MoveDistances.y);
                    myCol.center = new Vector3(myCol.center.x, myCol.center.y, myCol.center.z + GridManager.MoveDistances.y);
                    //movementPositions.Add(newPosition);
                    UpdateMovement(v, newPosition);
                }
            }
            else if (dir.y <= -.5f)
            {
                Vector2Int v = new Vector2Int(myPosition.x, myPosition.y - 1);
                if (GridManager.TileIsInGrid(v) && (GridManager.CanMoveToTile(v, previousPositions[previousPositions.Count - 1], true) ||
                    (GridManager.combatGrid[v.x, v.y].entityOnGrid == -20 && !enemyPositions.Contains(v)))
                    && (!previousPositions.Contains(v) || v == previousPositions[previousPositions.Count - 2]) && !enemyPositions.Contains(v))
                {
                    Vector3 newPosition = new Vector3(ghostPosition.x, ghostPosition.y, ghostPosition.z - GridManager.MoveDistances.y);
                    myCol.center = new Vector3(myCol.center.x, myCol.center.y, myCol.center.z - GridManager.MoveDistances.y);
                    //movementPositions.Add(newPosition);
                    UpdateMovement(v, newPosition);
                }
            }
            else if (dir.x > .5f)
            {
                Vector2Int v = new Vector2Int(myPosition.x + 1, myPosition.y);
                if (GridManager.TileIsInGrid(v) && (GridManager.CanMoveToTile(v, previousPositions[previousPositions.Count - 1], true) ||
                    (GridManager.combatGrid[v.x, v.y].entityOnGrid == -20 && !enemyPositions.Contains(v)))
                    && (!previousPositions.Contains(v) || v == previousPositions[previousPositions.Count - 2]) && !enemyPositions.Contains(v))
                {
                    Vector3 newPosition = new Vector3(ghostPosition.x + GridManager.MoveDistances.x, ghostPosition.y, ghostPosition.z);
                    myCol.center = new Vector3(myCol.center.x + GridManager.MoveDistances.x, myCol.center.y, myCol.center.z);
                    //movementPositions.Add(newPosition);
                    UpdateMovement(v, newPosition);
                }
            }
            else if (dir.x < -.5f)
            {
                Vector2Int v = new Vector2Int(myPosition.x - 1, myPosition.y);
                if (GridManager.TileIsInGrid(v) && (GridManager.CanMoveToTile(v, previousPositions[previousPositions.Count - 1], true) ||
                    (GridManager.combatGrid[v.x, v.y].entityOnGrid == -20 && !enemyPositions.Contains(v)))
                    && (!previousPositions.Contains(v) || v == previousPositions[previousPositions.Count - 2]) && !enemyPositions.Contains(v))
                {
                    Vector3 newPosition = new Vector3(ghostPosition.x - GridManager.MoveDistances.x, ghostPosition.y, ghostPosition.z);
                    myCol.center = new Vector3(myCol.center.x - GridManager.MoveDistances.x, myCol.center.y, myCol.center.z);
                    //movementPositions.Add(newPosition);
                    UpdateMovement(v, newPosition);
                }
            }
        }
    }

    /// <summary>
    /// Adds or removes the path that a player creates. Also shows that path as a highlight
    /// </summary>
    /// <param name="v"></param>
    /// <param name="t"></param>
    private void UpdateMovement(Vector2Int v, Vector3 t)
    {
        canMove = false;
        //Removes a position
        if (previousPositions.Contains(v))
        {
            GridManager.combatGrid[myPosition.x, myPosition.y].ShowHighlight(false);
            previousPositions.Remove(myPosition);
            movementPositions.RemoveAt(movementPositions.Count - 1);
            ++movementLeft;
            --movementUsed;
        }
        //Adds a position
        else
        {
            if (movementLeft > 0)
            {
                GridManager.combatGrid[v.x, v.y].SetHighlightColor(Color.yellow);
                GridManager.combatGrid[v.x, v.y].ShowHighlight(true);
                previousPositions.Add(v);
                movementPositions.Add(t);
                --movementLeft;
                ++movementUsed;
                //Updates the path on the final movement
                if (movementLeft == 0)
                {
                    ghostPosition = t;
                    GridManager.playerPosition = v;
                    myPosition = v;
                    previousColliderPos = myCol.center;
                }
            }
        }

        if (movementLeft > 0)
        {
            ghostPosition = t;
            GridManager.playerPosition = v;
            myPosition = v;
            previousColliderPos = myCol.center;
        }
        else
        {
            myCol.center = previousColliderPos;
        }
        if (TogglePathVisualizer)
        {
            VisualizeEnemyPaths();
        }
        StartCoroutine(MovementDelay());
    }

    /// <summary>
    /// Creates a delay between player movements to give them more accurate control of their movement
    /// </summary>
    /// <returns></returns>
    IEnumerator MovementDelay()
    {
        yield return new WaitForSeconds(continuousMoveDelay);
        canMove = true;
    }

    /// <summary>
    /// Moves the player along the path that they create for themselves
    /// </summary>
    /// <returns></returns>
    IEnumerator MovePlayer()
    {
        canMove = false;

        if(bm==null)
        {
            bm = FindFirstObjectByType<ButtonManager>();
        }

        

        for(int i = 0; i < movementPositions.Count; ++i)
        {
            Vector2Int nextPosition = previousPositions[i + 1];
            bool isMoving = true;
            bm.HideAllCanvas();
            while (isMoving)
            {
                anim.SetBool("Walk", true);
                transform.position = Vector3.MoveTowards(transform.position, movementPositions[i], .1f);
                if (transform.position == movementPositions[i])
                {
                    isMoving = false;
                    GridManager.MoveToTile(myPosition, nextPosition, -3);
                    myPosition = nextPosition;
                }
                yield return new WaitForSeconds(.1f / movementSpeed);
                bm.HideAllCanvas();
            }
            GridManager.combatGrid[previousPositions[i].x, previousPositions[i].y].ShowHighlight(false);

            TileBehaviour tileOn = GridManager.combatGrid[myPosition.x, myPosition.y];
            if (tileOn.CanApplyTileEffects() && !underEffect)
            {
                tileOn.ApplyTileEffects();
                underEffect = true;
            }
            else if (!tileOn.CanApplyTileEffects() && underEffect)
            {
                underEffect = false;
            }
        }
        GridManager.combatGrid[myPosition.x, myPosition.y].ShowHighlight(false);
        GridManager.MoveToTile(posBeforeMovement, myPosition, -3);
        previousPositions.Clear();
        previousPositions.Add(myPosition);
        movementPositions.Clear();
        ghostPosition = transform.position;
        myCol.center = new Vector3(0, myCol.center.y, 0);
        previousColliderPos = myCol.center;
        canMove = true;
        posBeforeMovement = myPosition;
        movementUsed = 0;
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", true);
        bm.ReEnableActionCanvas();
    }


    /// <summary>
    /// Updates the variables for the new movement system when the player teleports
    /// </summary>
    public void TeleportPlayer()
    {
        previousPositions.Clear();
        myPosition = GridManager.playerPosition;
        posBeforeMovement = myPosition;
        previousPositions.Add(myPosition);
        ghostPosition = transform.position;
        if (TogglePathVisualizer)
        {
            VisualizeEnemyPaths();
        }

        //Damages the player if they teleport onto an electrified tile
        TileBehaviour tileOn = GridManager.combatGrid[myPosition.x, myPosition.y];
        if (tileOn.CanApplyTileEffects() && !underEffect)
        {
            Debug.Log("MY POSITION IS " + myPosition);
            tileOn.ApplyTileEffects();
            underEffect = true;
        }
        else if (!tileOn.CanApplyTileEffects() && underEffect)
        {
            underEffect = false;
        }
    }

    /// <summary>
    /// Public function that gets called when the player presses the confirm button after moving.
    /// Tells the player to move and updates the UI
    /// </summary>
    public void ConfirmMovement()
    {
        buttonManager = FindFirstObjectByType<ButtonManager>();
        if (allowLeftoverMovement)
        {
            if (onlyMoveOnce)
            {
                movementLeft = 0;
            }
            StartCoroutine(MovePlayer());
            gm.GetComponent<PlayerInputHandler>().enableMovement = false;
            buttonManager.ResetCanvas();
        }
        else if(movementLeft == 0)
        {
            StartCoroutine(MovePlayer());
            gm.GetComponent<PlayerInputHandler>().enableMovement = false;
            buttonManager.ResetCanvas();
        }
    }

    /// <summary>
    /// Removes the movement path if the player cancels their movement
    /// </summary>
    public void DeleteMovement()
    {
        gm.GetComponent<PlayerInputHandler>().enableMovement = false;
        myPosition = posBeforeMovement;
        GridManager.playerPosition = posBeforeMovement;
        GridManager.combatGrid[myPosition.x, myPosition.y].entityOnGrid = -3;
        foreach(Vector2Int v in previousPositions)
        {
            GridManager.combatGrid[v.x, v.y].ShowHighlight(false);
        }
        previousPositions.Clear();
        movementPositions.Clear();
        previousPositions.Add(myPosition);
        movementLeft += movementUsed;
        movementUsed = 0;
        ghostPosition = transform.position;
        myCol.center = new Vector3(0, myCol.center.y, 0);
        previousColliderPos = myCol.center;
        if (TogglePathVisualizer)
        {
            VisualizeEnemyPaths();
        }
    }

    /// <summary>
    /// Turns the action canvas back on when the player is done moving to their selected tile
    /// </summary>
    /// <returns></returns>
    private void ReEnableActionCanvas()
    {
        gm.UpdateActionPoints(gm.MoveActionPoints);
        buttonManager.ReEnableActionCanvas();
        //EnableMovableTiles();
        //anim.SetTrigger("Idle");
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
