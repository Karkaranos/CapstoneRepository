/******************************************************************************
 * Author: Brad Dixon
 * Creation Date: 10/1/2025
 * Last Modified: 11/2/2025
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

    [Tooltip("Caps pathfinding limit so it can't search infinitly if no target is found")]
    [SerializeField] protected int movementRange;
    protected int pathfindingLimit;
    bool isMoving = false;

    /// <summary>
    /// Testing function that gets the target location and has the enemy pathfind to it
    /// </summary>
    public void TestPathfinding()
    {
        GetComponent<TargetingBehaviour>().behaviours = TargetingBehaviour.TargetingBehaviours.ranged;
        GetComponent<TargetingBehaviour>().FindTarget();
        PathfindThroughGrid();
    }

    private void Start()
    {
        isEnemy = true;
    }

    /// <summary>
    /// Takes the current position and pathfinds to a designated location
    /// </summary>
    virtual public void PathfindThroughGrid()
    {
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
                List<Vector2Int> temp = GridManager.GetAllValidAdjacentTiles(myPosition, myPosition);

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
            else if(targetPosition.y % 2 == 1)
            {
                if (GridManager.TileIsInGrid(new Vector2Int(targetPosition.x + 1, targetPosition.y - 1)) && GridManager.combatGrid[targetPosition.x + 1, targetPosition.y - 1].entityOnGrid == i)
                {
                    gridDirections.Add("Up Left");
                    ++targetPosition.x;
                    --targetPosition.y;
                }
                else if (GridManager.TileIsInGrid(new Vector2Int(targetPosition.x, targetPosition.y - 1)) && GridManager.combatGrid[targetPosition.x, targetPosition.y - 1].entityOnGrid == i)
                {
                    gridDirections.Add("Up Right");
                    --targetPosition.y;
                }
                else if (GridManager.TileIsInGrid(new Vector2Int(targetPosition.x + 1, targetPosition.y + 1)) && GridManager.combatGrid[targetPosition.x + 1, targetPosition.y + 1].entityOnGrid == i)
                {
                    gridDirections.Add("Down Left");
                    ++targetPosition.x;
                    ++targetPosition.y;
                }
                else if (GridManager.TileIsInGrid(new Vector2Int(targetPosition.x, targetPosition.y + 1)) && GridManager.combatGrid[targetPosition.x, targetPosition.y + 1].entityOnGrid == i)
                {
                    gridDirections.Add("Down Right");
                    ++targetPosition.y;
                }
            }
            else if(targetPosition.y % 2 == 0)
            {
                if (GridManager.TileIsInGrid(new Vector2Int(targetPosition.x, targetPosition.y - 1)) && GridManager.combatGrid[targetPosition.x, targetPosition.y - 1].entityOnGrid == i)
                {
                    gridDirections.Add("Up Left");
                    --targetPosition.y;
                }
                else if (GridManager.TileIsInGrid(new Vector2Int(targetPosition.x - 1, targetPosition.y - 1)) && GridManager.combatGrid[targetPosition.x - 1, targetPosition.y - 1].entityOnGrid == i)
                {
                    gridDirections.Add("Up Right");
                    --targetPosition.x;
                    --targetPosition.y;
                }
                else if (GridManager.TileIsInGrid(new Vector2Int(targetPosition.x, targetPosition.y + 1)) && GridManager.combatGrid[targetPosition.x, targetPosition.y + 1].entityOnGrid == i)
                {
                    gridDirections.Add("Down Left");
                    ++targetPosition.y;
                }
                else if (GridManager.TileIsInGrid(new Vector2Int(targetPosition.x - 1, targetPosition.y + 1)) && GridManager.combatGrid[targetPosition.x - 1, targetPosition.y + 1].entityOnGrid == i)
                {
                    gridDirections.Add("Down Right");
                    --targetPosition.x;
                    ++targetPosition.y;
                }
            }
            nextPos.Add(targetPosition);
        }

        targetPosition = originalTarget;
        GridManager.DisplayGridAsText();
        StartCoroutine(MoveEntity());
    }

    /// <summary>
    /// Moves the enemy along the grid until they reach their target
    /// </summary>
    /// <returns></returns>
    protected IEnumerator MoveEntity()
    {
        newPositions.Clear();
        float tileSizeX = transform.GetComponentInParent<Transform>().localScale.x * 2;
        float tileSizeY = transform.GetComponentInParent<Transform>().localScale.z * 2;

        Vector3 newPosition = GetComponentInParent<Transform>().position;

        int max = gridDirections.Count - 1;
        int min = movementRange > gridDirections.Count ? 0 : gridDirections.Count - movementRange;

        //Uses a list of directions to move an enemy along a path
        for (int i = max; i >= min; --i)
        {
            yield return new WaitForSeconds(.5f);
            switch (gridDirections[i])
            {
                case "Up Left":
                    newPosition.x -= (tileSizeX / 2);
                    newPosition.z += (tileSizeY * .75f);
                    break;
                case "Up Right":
                    newPosition.x += (tileSizeX / 2);
                    newPosition.z += (tileSizeY * .75f);
                    break;
                case "Down Left":
                    newPosition.x -= (tileSizeX / 2);
                    newPosition.z -= (tileSizeY * .75f);
                    break;
                case "Down Right":
                    newPosition.x += (tileSizeX / 2);
                    newPosition.z -= (tileSizeY * .75f);
                    break;
                case "Right":
                    newPosition.x += tileSizeX;
                    break;
                case "Left":
                    newPosition.x -= tileSizeX;
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
    IEnumerator MoveToTile()
    {
        int eType = isEnemy ? -2 : -3;
        GridManager.ClearPathfinding();
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
                }
                yield return new WaitForSeconds(.1f / movementSpeed);
            }
        }

        //if(!isEnemy)
        //{
        //    PublicEvents.PlayerMove();
        //}
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
