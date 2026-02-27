/******************************************************************************
 * Author: Brad Dixon
 * Creation Date: 10/1/2025
 * Last Modified: 2/3/2026
 * Brief: Allows anything that moves to pathfind through the grid while 
 * avoiding occupied tiles
 * External Resources: N/A
 * ***************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GridPathfinding : MonoBehaviour
{
    public Vector2Int MyPosition
    {
        get { return myPosition; }
        set { myPosition = value; }
    }
    [SerializeField] protected Vector2Int myPosition;
    [SerializeField] protected Vector2Int targetPosition;
    [SerializeField] List<Vector2Int> nextPos = new List<Vector2Int>();
    [SerializeField] protected List<string> gridDirections = new List<string>();
    List<Vector3> newPositions = new List<Vector3>();
    Vector2Int nextPosition = Vector2Int.zero;
    [SerializeField] float movementSpeed;
    protected bool isEnemy = true;

    [Tooltip("Caps pathfinding limit so it can't search infinitly if no target is found. Also serves at the player's movement limit.")]
    [SerializeField] protected int movementRange;
    protected int pathfindingLimit;
    bool isMoving = false;
    private Vector2Int ghostPos;

    [SerializeField] bool underEffect;

    /// <summary>
    /// Testing function that gets the target location and has the enemy pathfind to it
    /// </summary>
    public void TestPathfinding()
    {
        GetComponent<TargetingBehaviour>().behaviours = TargetingBehaviour.TargetingBehaviours.ranged;
        GetComponent<TargetingBehaviour>().FindTarget();
        PathfindThroughGrid();
    }

    /// <summary>
    /// Lets the pathfinding know if its the player or enemy trying to pathfind
    /// </summary>
    private void Start()
    {
        isEnemy = true;
        underEffect = false;
    }

    /// <summary>
    /// Checks to see if the enemy has a target. Pathfinds if yes
    /// </summary>
    public void HasATarget()
    {
        GridManager.combatGrid[myPosition.x, myPosition.y].entityOnGrid = -2;
        if (isEnemy && GetComponent<TargetingBehaviour>().targetLocations.Count > 0)
        {
            PathfindThroughGrid();
        }
    }

    /// <summary>
    /// Takes the current position and pathfinds to a designated location
    /// </summary>
    virtual public void PathfindThroughGrid()
    {
        if (ghostPos != myPosition)
        {
            GridManager.combatGrid[ghostPos.x, ghostPos.y].entityOnGrid = -1;
        }
        nextPos.Clear();
        if (!isEnemy)
        {
            nextPos.Add(targetPosition);
        }
        Vector2Int originalPosition = myPosition;
        gridDirections.Clear();

        int stepsTaken = 0;
        Debug.Log("Movement Range = " + movementRange);
        List<Vector2Int> nextPositions = new List<Vector2Int>();
        List<Vector2Int> currentPositions = new List<Vector2Int>();
        bool reachedTarget = false;
        currentPositions.Add(myPosition);

        while (!reachedTarget && stepsTaken < pathfindingLimit)
        {
            foreach (Vector2Int v in nextPositions)
            {
                currentPositions.Add(v);
            }

            //Add the potential tiles to be checked for movement or the target
            foreach (Vector2Int currentTile in currentPositions)
            {
                if (isEnemy)
                {
                    if (GetComponent<TargetingBehaviour>().targetLocations.Contains(currentTile))
                    {
                        targetPosition = currentTile;
                        nextPos.Add(targetPosition);
                        reachedTarget = true;
                        currentPositions.Clear();
                        break;
                    }
                    else
                    {
                        GridManager.combatGrid[currentTile.x, currentTile.y].entityOnGrid = stepsTaken;
                    }
                }
                else
                {
                    if (targetPosition == currentTile)
                    {
                        nextPos.Add(targetPosition);
                        reachedTarget = true;
                        currentPositions.Clear();
                        break;
                    }
                    else
                    {
                        GridManager.combatGrid[currentTile.x, currentTile.y].entityOnGrid = stepsTaken;
                    }
                }
            }

            nextPositions.Clear();

            //Check which directions the enemy can move in
            for (int i = 0; i < currentPositions.Count; ++i)
            {
                myPosition = currentPositions[i];
                List<Vector2Int> temp = GridManager.GetAllValidAdjacentTiles(myPosition, myPosition, !isEnemy);

                foreach(Vector2Int v in temp)
                {
                    nextPositions.Add(v);
                }
            }
            currentPositions.Clear();
            ++stepsTaken;
        }
        if (stepsTaken != pathfindingLimit)
        {
            --stepsTaken;
        }

        myPosition = originalPosition;
        Vector2Int originalTarget = targetPosition;
        //Stores the enemy path as a list of directions
        for (int i = stepsTaken - 1; i >= 0; --i)
        {
            if (GridManager.TileIsInGrid(new Vector2Int(targetPosition.x + 1, targetPosition.y)) && GridManager.combatGrid[targetPosition.x + 1, targetPosition.y].entityOnGrid == i)
            {
                gridDirections.Add("Left");
                ++targetPosition.x;
            }
            else if (GridManager.TileIsInGrid(new Vector2Int(targetPosition.x - 1, targetPosition.y)) && GridManager.combatGrid[targetPosition.x - 1, targetPosition.y].entityOnGrid == i)
            {
                gridDirections.Add("Right");
                --targetPosition.x;
            }
            else if (GridManager.TileIsInGrid(new Vector2Int(targetPosition.x, targetPosition.y + 1)) && GridManager.combatGrid[targetPosition.x, targetPosition.y + 1].entityOnGrid == i)
            {
                gridDirections.Add("Down");
                ++targetPosition.y;
            }
            else if (GridManager.TileIsInGrid(new Vector2Int(targetPosition.x, targetPosition.y - 1)) && GridManager.combatGrid[targetPosition.x, targetPosition.y - 1].entityOnGrid == i)
            {
                gridDirections.Add("Up");
                --targetPosition.y;
            }
            nextPos.Add(targetPosition);
        }

        targetPosition = originalTarget;
        GridManager.ClearPathfinding();
        //GridManager.DisplayGridAsText();
    }

    public void StartMoveCoroutine()
    {
        if (!isEnemy)
        {
            StartCoroutine(MoveEntity());
        }
        else if (GetComponent<TargetingBehaviour>().targetLocations.Count > 0)
        {
            StartCoroutine(MoveEntity());
        }
    }

    /// <summary>
    /// Moves the enemy along the grid until they reach their target
    /// </summary>
    /// <returns></returns>
    protected IEnumerator MoveEntity()
    {
        HidePath();
        newPositions.Clear();

        Vector3 newPosition = GetComponentInParent<Transform>().position;

        int max = gridDirections.Count - 1;
        int min = movementRange > gridDirections.Count ? 0 : gridDirections.Count - movementRange;

        //Uses a list of directions to move an enemy along a path
        for (int i = max; i >= min; --i)
        {
            yield return new WaitForSeconds(.5f);
            switch (gridDirections[i])
            {
                case "Right":
                    newPosition.x += GridManager.MoveDistances.x;
                    break;
                case "Left":
                    newPosition.x -= GridManager.MoveDistances.x;
                    break;
                case "Up":
                    newPosition.z += GridManager.MoveDistances.y;
                    break;
                case "Down":
                    newPosition.z -= GridManager.MoveDistances.y;
                    break;
                default:
                    Debug.Log("Error!!!");
                    break;
            }

            newPositions.Add(newPosition);
        }
        StartCoroutine(MoveToTile());
    }

    /// <summary>
    /// Causes the enemy to move from one tile to the next over time
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveToTile()
    {

        WindCurrentTracker[] trackers = FindObjectsByType<WindCurrentTracker>(FindObjectsSortMode.None);

        int eType = isEnemy ? -2 : -3;
        //How many tiles the enemy has to move to
        for (int i = 0; i < newPositions.Count; ++i)
        {
            nextPosition = nextPos[gridDirections.Count - 1 - i];
            isMoving = true;
            //Loops until they finish moving to the adjacent tile
            while (isMoving)
            {
                transform.position = Vector3.MoveTowards(transform.position, newPositions[i], .1f);
                if (transform.position == newPositions[i])
                {
                    isMoving = false;
                    GridManager.MoveToTile(myPosition, nextPosition, eType);
                    myPosition = nextPosition;

                    foreach(WindCurrentTracker tracker in trackers)
                    {

                        if (tracker.WindCurrentTiles.Contains(GridManager.combatGrid[myPosition.x, myPosition.y]))
                        {

                            this.GetComponent<Enemy>().Damage(tracker.CurrentDamage, Enemy.DamageType.Wind);

                            tracker.SendThroughWindCurrent
                            (tracker.WindCurrentTiles.IndexOf(GridManager.combatGrid[myPosition.x, myPosition.y]), this.GetComponent<Enemy>());

                            yield break;

                        }

                    }

                }

                yield return new WaitForSeconds(.1f / movementSpeed);
            }

            TileBehaviour tileOn = GridManager.combatGrid[nextPosition.x, nextPosition.y];
            if(tileOn.CanApplyTileEffects() && !underEffect)
            {
                tileOn.ApplyTileEffects();
                underEffect = true;
            }
            else if(!tileOn.CanApplyTileEffects() && underEffect)
            {
                underEffect = false;
            }
        }

        //if(!isEnemy)
        //{
        //    ReEnableActionCanvas();
        //}
    }

    /// <summary>
    /// Does nothing in the base script, because trying to overwrite coroutines causes problems
    /// </summary>
    //virtual protected void ReEnableActionCanvas()
    //{ }

    /// <summary>
    /// Public call to tell an enemy to show their path
    /// </summary>
    public void ShowPath()
    {
        gameObject.GetComponent<TargetingBehaviour>().FindTarget();
        if (GetComponent<TargetingBehaviour>().targetLocations.Count > 0)
        {
            if (ghostPos != myPosition)
            {
                GridManager.combatGrid[ghostPos.x, ghostPos.y].entityOnGrid = -1;
            }
            HidePath();
            PathfindThroughGrid();
            DisplayPath();
        }
    }

    /// <summary>
    /// Highlights the enemy's path
    /// </summary>
    private void DisplayPath()
    {
        int max = nextPos.Count > movementRange ? movementRange + 1: 0;
        Vector2Int v = new Vector2Int();
        for (int i = 1; i <= max; ++i)
        {
            v = nextPos[nextPos.Count - i];
            GridManager.combatGrid[v.x, v.y].SetHighlightColor(Color.red);
            GridManager.combatGrid[v.x, v.y].ShowHighlight(true);
        }

        if (myPosition != targetPosition)
        {
            GridManager.combatGrid[myPosition.x, myPosition.y].entityOnGrid = -1;
        }
        GridManager.combatGrid[v.x, v.y].entityOnGrid = -20;
        GridManager.AddGhostEntity(v);
        ghostPos = v;
    }

    /// <summary>
    /// Removes the highlight for the enemie's path. Also used to reset it after the enemy moves
    /// </summary>
    private void HidePath()
    {
        int max = nextPos.Count > movementRange ? movementRange + 1 : 0;

        //Resets the highlight
        if (nextPos.Count > 0)
        {
            for (int i = 1; i <= max; ++i)
            {
                Vector2Int v = nextPos[nextPos.Count - i];
                GridManager.combatGrid[v.x, v.y].ShowHighlight(false);
            }
        }
    }

    #region GETTERS AND SETTERS

    /// <summary>
    /// Function that enemies call to set their movement range
    /// </summary>
    /// <param name="movementRange"></param>
    public void SetMovementRange(int movementRange)
    {
        this.movementRange = movementRange;
    }

    /// <summary>
    /// sets the enemy's position when the enemy is knocked back (for now)
    /// </summary>
    /// <param name="newPos"> the enemy's new position </param>
    public void SetPosition(Vector2Int newPos)
    {

        myPosition = newPos;

    }

    /// <summary>
    /// Function that enemies call to set their aggro range
    /// </summary>
    /// <param name="aggroRange"></param>
    public void SetAggroRange(int aggroRange)
    {
        pathfindingLimit = aggroRange;
    }

    /// <summary>
    /// Function that enemies call to set their movementSpeed
    /// </summary>
    /// <param name="movementSpeed"></param>
    public void SetMovementSpeed(float movementSpeed)
    {
        this.movementSpeed = movementSpeed;
    }

    /// <summary>
    /// Returns a reference to target movement position 
    /// </summary>
    /// <returns></returns>
    public Vector2Int GetTargetPosition()
    {
        return targetPosition;
    }

    #endregion
}
