/******************************************************************************
 * Author: Brad Dixon
 * Creation Date: 9/26/2025
 * Last Modified: 10/7/2025
 * Brief: Stores an instance of the current combat grid. Also stores the positions of
 * the player, enemies, and objects in the grid. 
 * External Resources: N/A
 * ***************************************************************************/
using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    // -1 is an open tile
    // -2 is an enemy occupied tile
    // -3 is a player occupied tile
    // -4 is an obstacle occupied tile
    public static int[,] combatGrid;

    public static Vector2Int playerPosition;

    /// <summary>
    /// Sets the grid instance that everything will reference
    /// </summary>
    /// <param name="gridPrefab"></param> The grid game object
    /// <param name="gridDimensions"></param> How long and wide the grid is
    public static void SetGrid(Vector2Int gridDimensions)
    {
        combatGrid = new int[gridDimensions.x, gridDimensions.y];

        for(int i = 0; i < gridDimensions.y; ++i)
        {
            for(int j = 0; j < gridDimensions.x; ++j)
            {
                combatGrid[j, i] = -1;
            }
        }
    }

    /// <summary>
    /// Adds the location of a spawned entity to the grid
    /// </summary>
    /// <param name="locationInGrid"></param> The index in the grid the entity is being added to
    /// <param name="entityType"></param> The int classification of the entity
    public static void AddEntity(Vector2Int locationInGrid, int entityType)
    {
        combatGrid[locationInGrid.x, locationInGrid.y] = entityType;
        if(entityType == -3)
        {
            playerPosition = locationInGrid;
        }
        Debug.Log("Add Entity");
    }

    /// <summary>
    /// Checks if a tile is empty
    /// </summary>
    /// <param name="locationInGrid"></param> The tile in the grid that is being checked
    /// <returns></returns> Returns true if that tile is empty
    public static bool CanMoveToTile(Vector2Int tileCoordinates)
    {
        return combatGrid[tileCoordinates.x, tileCoordinates.y] == -1 || combatGrid[tileCoordinates.x, tileCoordinates.y] == -3;
    }

    /// <summary>
    /// Checks if the attempted tile is in the bounds of the grid
    /// </summary>
    /// <param name="tileCoordinates"></param> The position being checked
    /// <returns></returns> Returns true if the checked tile is in the grid's bounds
    public static bool TileIsInGrid(Vector2Int tileCoordinates)
    {
        if(tileCoordinates.x < 0 || tileCoordinates.x >= combatGrid.GetLength(0))
        {
            return false;
        }
        if (tileCoordinates.y < 0 || tileCoordinates.y >= combatGrid.GetLength(1))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Updates the grid to show where an entity moved to
    /// </summary>
    /// <param name="originalTile"></param> The tile the entity was on before it moved
    /// <param name="tileMovedTo"></param> The tile  the entity moved to
    /// <param name="entityType"></param> The int classification of the entity
    public static void MoveToTile(Vector2Int originalTile ,Vector2Int tileMovedTo, int entityType)
    {
        combatGrid[originalTile.x, originalTile.y] = -1;
        combatGrid[tileMovedTo.x, tileMovedTo.y] = entityType;
        if(entityType == -3)
        {
            playerPosition = tileMovedTo;
        }
    }

    /// <summary>
    /// Used to clear the grid assignments used when pathfinding
    /// </summary>
    public static void ClearPathfinding()
    {
        for(int i = 0; i < combatGrid.GetLength(0); ++i)
        {
            for(int j = 0; j < combatGrid.GetLength(1); ++j)
            {
                if(combatGrid[i, j] > 0)
                {
                    combatGrid[i, j] = -1;
                }
            }
        }
    }

    /// <summary>
    /// Debugging script to visualize the grid in console
    /// </summary>
    public static void DisplayGridAsText()
    {
        string row = "";
        for (int i = combatGrid.GetLength(1) - 1; i >= 0; --i)
        {
            if(i % 2 == 1)
            {
                row += " ";
            }
            for(int j = 0; j < combatGrid.GetLength(0); ++j)
            {
                row += combatGrid[j, i] + " ";
            }
            row += "\n";
        }
        Debug.Log(row);
    }
}
