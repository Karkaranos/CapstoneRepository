/******************************************************************************
 * Author: Brad Dixon
 * Creation Date: 10/1/2025
 * Last Modified: 10/7/2025
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
    protected Vector2Int myPosition;
    protected Vector2Int targetPosition;
    protected List<string> gridDirections = new List<string>();

    [Tooltip("Caps pathfinding limit so it can't search infinitly if no target is found")]
    [SerializeField] protected int movementRange;

    /// <summary>
    /// Testing function that gets the target location and has the enemy pathfind to it
    /// </summary>
    public void TestPathfinding()
    {
        SetTarget();
        PathfindThroughGrid();
    }

    /// <summary>
    /// Currently sets the player as the target position. Will need to be replaced when the actually targeting is implemented
    /// </summary>
    virtual protected void SetTarget()
    {
        Debug.Log(GridManager.playerPosition);
        targetPosition = GridManager.playerPosition;
    }

    /// <summary>
    /// Takes the current position and pathfinds to a designated location
    /// </summary>
    protected void PathfindThroughGrid()
    {
        Vector2Int originalPosition = myPosition;
        gridDirections.Clear();

        int stepsTaken = 0;
        List<Vector2Int> nextPositions = new List<Vector2Int>();
        List<Vector2Int> currentPositions = new List<Vector2Int>();
        bool reachedTarget = false;
        currentPositions.Add(myPosition);

        while (!reachedTarget && stepsTaken < movementRange)
        {
            foreach (Vector2Int v in nextPositions)
            {
                currentPositions.Add(v);
            }

            //Add the potential tiles to be checked for movement or the target
            foreach (Vector2Int currentTile in currentPositions)
            {
                if (currentTile == targetPosition)
                {
                    reachedTarget = true;
                    currentPositions.Clear();
                    break;
                }
                else
                {
                    GridManager.combatGrid[currentTile.x, currentTile.y] = stepsTaken;
                }
            }

            nextPositions.Clear();

            //Check which directions the enemy can move in
            for (int i = 0; i < currentPositions.Count; ++i)
            {
                myPosition = currentPositions[i];

                if (GridManager.TileIsInGrid(new Vector2Int(myPosition.x + 1, myPosition.y)) && GridManager.CanMoveToTile(new Vector2Int(myPosition.x + 1, myPosition.y)))
                {
                    nextPositions.Add(new Vector2Int(myPosition.x + 1, myPosition.y));
                }
                if (GridManager.TileIsInGrid(new Vector2Int(myPosition.x - 1, myPosition.y)) && GridManager.CanMoveToTile(new Vector2Int(myPosition.x - 1, myPosition.y)))
                {
                    nextPositions.Add(new Vector2Int(myPosition.x - 1, myPosition.y));
                }
                if (myPosition.y % 2 == 0)
                {
                    if (GridManager.TileIsInGrid(new Vector2Int(myPosition.x + 1, myPosition.y + 1)) && GridManager.CanMoveToTile(new Vector2Int(myPosition.x + 1, myPosition.y + 1)))
                    {
                        nextPositions.Add(new Vector2Int(myPosition.x + 1, myPosition.y + 1));
                    }
                    if (GridManager.TileIsInGrid(new Vector2Int(myPosition.x, myPosition.y + 1)) && GridManager.CanMoveToTile(new Vector2Int(myPosition.x, myPosition.y + 1)))
                    {
                        nextPositions.Add(new Vector2Int(myPosition.x, myPosition.y + 1));
                    }
                    if (GridManager.TileIsInGrid(new Vector2Int(myPosition.x + 1, myPosition.y - 1)) && GridManager.CanMoveToTile(new Vector2Int(myPosition.x + 1, myPosition.y - 1)))
                    {
                        nextPositions.Add(new Vector2Int(myPosition.x + 1, myPosition.y - 1));
                    }
                    if (GridManager.TileIsInGrid(new Vector2Int(myPosition.x, myPosition.y - 1)) && GridManager.CanMoveToTile(new Vector2Int(myPosition.x, myPosition.y - 1)))
                    {
                        nextPositions.Add(new Vector2Int(myPosition.x, myPosition.y - 1));
                    }
                }
                else
                {
                    if (GridManager.TileIsInGrid(new Vector2Int(myPosition.x, myPosition.y + 1)) && GridManager.CanMoveToTile(new Vector2Int(myPosition.x, myPosition.y + 1)))
                    {
                        nextPositions.Add(new Vector2Int(myPosition.x, myPosition.y + 1));
                    }
                    if (GridManager.TileIsInGrid(new Vector2Int(myPosition.x - 1, myPosition.y + 1)) && GridManager.CanMoveToTile(new Vector2Int(myPosition.x - 1, myPosition.y + 1)))
                    {
                        nextPositions.Add(new Vector2Int(myPosition.x - 1, myPosition.y + 1));
                    }
                    if (GridManager.TileIsInGrid(new Vector2Int(myPosition.x, myPosition.y - 1)) && GridManager.CanMoveToTile(new Vector2Int(myPosition.x, myPosition.y - 1)))
                    {
                        nextPositions.Add(new Vector2Int(myPosition.x, myPosition.y - 1));
                    }
                    if (GridManager.TileIsInGrid(new Vector2Int(myPosition.x - 1, myPosition.y - 1)) && GridManager.CanMoveToTile(new Vector2Int(myPosition.x - 1, myPosition.y - 1)))
                    {
                        nextPositions.Add(new Vector2Int(myPosition.x - 1, myPosition.y - 1));
                    }
                }
            }
            currentPositions.Clear();
            ++stepsTaken;
        }
        --stepsTaken;

        //Stores the enemy path as a list of directions
        for (int i = stepsTaken - 1; i >= 0; --i)
        {
            if (GridManager.TileIsInGrid(new Vector2Int(targetPosition.x + 1, targetPosition.y)) && GridManager.combatGrid[targetPosition.x + 1, targetPosition.y] == i)
            {
                gridDirections.Add("Left");
                ++targetPosition.x;
            }
            else if (GridManager.TileIsInGrid(new Vector2Int(targetPosition.x - 1, targetPosition.y)) && GridManager.combatGrid[targetPosition.x - 1, targetPosition.y] == i)
            {
                gridDirections.Add("Right");
                --targetPosition.x;
            }
            else if(targetPosition.y % 2 == 1)
            {
                if (GridManager.TileIsInGrid(new Vector2Int(targetPosition.x, targetPosition.y - 1)) && GridManager.combatGrid[targetPosition.x, targetPosition.y - 1] == i)
                {
                    gridDirections.Add("Up Left");
                    --targetPosition.y;
                }
                else if (GridManager.TileIsInGrid(new Vector2Int(targetPosition.x - 1, targetPosition.y - 1)) && GridManager.combatGrid[targetPosition.x - 1, targetPosition.y - 1] == i)
                {
                    gridDirections.Add("Up Right");
                    --targetPosition.x;
                    --targetPosition.y;
                }
                else if (GridManager.TileIsInGrid(new Vector2Int(targetPosition.x, targetPosition.y + 1)) && GridManager.combatGrid[targetPosition.x, targetPosition.y + 1] == i)
                {
                    gridDirections.Add("Down Left");
                    ++targetPosition.y;
                }
                else if (GridManager.TileIsInGrid(new Vector2Int(targetPosition.x - 1, targetPosition.y + 1)) && GridManager.combatGrid[targetPosition.x - 1, targetPosition.y + 1] == i)
                {
                    gridDirections.Add("Down Right");
                    --targetPosition.x;
                    ++targetPosition.y;
                }
            }
            else if(targetPosition.y % 2 == 0)
            {
                if (GridManager.TileIsInGrid(new Vector2Int(targetPosition.x + 1, targetPosition.y - 1)) && GridManager.combatGrid[targetPosition.x + 1, targetPosition.y - 1] == i)
                {
                    gridDirections.Add("Up Left");
                    ++targetPosition.x;
                    --targetPosition.y;
                }
                else if (GridManager.TileIsInGrid(new Vector2Int(targetPosition.x, targetPosition.y - 1)) && GridManager.combatGrid[targetPosition.x, targetPosition.y - 1] == i)
                {
                    gridDirections.Add("Up Right");
                    --targetPosition.y;
                }
                else if (GridManager.TileIsInGrid(new Vector2Int(targetPosition.x + 1, targetPosition.y + 1)) && GridManager.combatGrid[targetPosition.x + 1, targetPosition.y + 1] == i)
                {
                    gridDirections.Add("Down Left");
                    ++targetPosition.x;
                    ++targetPosition.y;
                }
                else if (GridManager.TileIsInGrid(new Vector2Int(targetPosition.x, targetPosition.y + 1)) && GridManager.combatGrid[targetPosition.x, targetPosition.y + 1] == i)
                {
                    gridDirections.Add("Down Right");
                    ++targetPosition.y;
                }
            }
        }

        GridManager.DisplayGridAsText();
        StartCoroutine(MoveEntity());
    }

    /// <summary>
    /// Moves the enemy along the grid until they reach their target
    /// </summary>
    /// <returns></returns>
    protected IEnumerator MoveEntity()
    {
        float tileSizeX = transform.GetComponentInParent<Transform>().localScale.x * 2;
        float tileSizeY = transform.GetComponentInParent<Transform>().localScale.z * 2;

        Vector3 newPosition = GetComponentInParent<Transform>().position;

        int max = movementRange > gridDirections.Count ? gridDirections.Count : movementRange;

        //Uses a list of directions to move an enemy along a path
        for (int i = max - 1; i >= 0; --i)
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

            transform.position = newPosition;
        }

        GridManager.ClearPathfinding();
        GridManager.MoveToTile(myPosition, targetPosition, -2);
    }
}
