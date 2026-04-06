/******************************************************************************
 * Author: Brad Dixon, Tyler Bouchard, Clare Grady
 * Creation Date: 9/26/2025
 * Last Modified: 2/10/2026 (Clare Grady)
 * Brief: Stores an instance of the current combat grid. Also stores the positions of
 * the player, enemies, and objects in the grid. 
 * External Resources: N/A
 * ***************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class GridManager : MonoBehaviour
{
    // -1 is an open tile
    // -2 is an enemy occupied tile
    // -3 is a player occupied tile
    // -4 is an obstacle occupied tile
    // -5 is a pip occupied tile
    // -6 is a hazard occupied tile
    //-7 is a shield occupied tile
    public static TileBehaviour[,] combatGrid;

    public static Vector2Int playerPosition;

    public static Vector2 MoveDistances = new Vector2();

    public static List<Vector2Int> GhostEntities = new List<Vector2Int>();

    /// <summary>
    /// Sets the grid instance that everything will reference
    /// </summary>
    /// <param name="gridPrefab"></param> The grid game object
    /// <param name="gridDimensions"></param> How long and wide the grid is
    public static void SetGrid(Vector2Int gridDimensions, GameObject gridPrefab)
    {
        combatGrid = new TileBehaviour[gridDimensions.x, gridDimensions.y];
        TileBehaviour[] tiles = gridPrefab.GetComponentsInChildren<TileBehaviour>();
        MoveDistances = new Vector2(tiles[0].gameObject.transform.localScale.x, tiles[0].gameObject.transform.localScale.z);

        foreach (TileBehaviour tb in tiles) {
            //this line causes a bug where the tiles go to the wrong position when the scene is loaded so I removed it
            //tile.transform.position = new Vector3(MoveDistances.x * x, tile.transform.position.y, MoveDistances.y * y);
            combatGrid[tb.IndexInGrid.x, tb.IndexInGrid.y] = tb;
            combatGrid[tb.IndexInGrid.x, tb.IndexInGrid.y].entityOnGrid = -1;
            combatGrid[tb.IndexInGrid.x, tb.IndexInGrid.y].AddObjectsToTile();
            //combatGrid[tb.IndexInGrid.x, tb.IndexInGrid.y].gameObject.name = "[" + tb.IndexInGrid.x + ", " + tb.IndexInGrid.y + "]";
        }
    }

    /// <summary>
    /// Adds the location of a spawned entity to the grid
    /// </summary>
    /// <param name="locationInGrid"></param> The index in the grid the entity is being added to
    /// <param name="entityType"></param> The int classification of the entity
    public static void AddEntity(Vector2Int locationInGrid, int entityType)
    {
        combatGrid[locationInGrid.x, locationInGrid.y].entityOnGrid = entityType;
        if(entityType == -3)
        {
            playerPosition = locationInGrid;
        }
    }

    /// <summary>
    /// Removes the location of a destory entity from the grid
    /// </summary>
    /// <param name="locationInGrid"></param>
    public static void RemoveEntity(Vector2Int locationInGrid)
    {
        combatGrid[locationInGrid.x, locationInGrid.y].entityOnGrid = -1;
    }

    /// <summary>
    /// Checks if a tile is empty
    /// </summary>
    /// <param name="locationInGrid"></param> The tile in the grid that is being checked
    /// <returns></returns> Returns true if that tile is empty
    public static bool CanMoveToTile(Vector2Int tileCoordinates, Vector2Int myPosition, bool isPlayer)
    {
        if(tileCoordinates == myPosition)
        {
            return true;
        }
        if(isPlayer)
        {
            return combatGrid[tileCoordinates.x, tileCoordinates.y].entityOnGrid == -1 || 
                combatGrid[tileCoordinates.x, tileCoordinates.y].entityOnGrid == -5 ||
                combatGrid[tileCoordinates.x, tileCoordinates.y].entityOnGrid == -6;
        }
        return combatGrid[tileCoordinates.x, tileCoordinates.y].entityOnGrid == -1 ||
            combatGrid[tileCoordinates.x, tileCoordinates.y].entityOnGrid == -6;
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
        if (combatGrid[tileCoordinates.x, tileCoordinates.y] == null)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Returns a list of all the adjacent tiles, of a provided tile, that can be moved to
    /// </summary>
    /// <param name="currentTile"></param> The provided tile
    /// <returns></returns> The list of available adjacent tiles
    public static List<Vector2Int> GetAllValidAdjacentTiles(Vector2Int currentTile, Vector2Int myPos, bool isPlayer)
    {
        List<Vector2Int> validTiles = new List<Vector2Int>();

        if (TileIsInGrid(new Vector2Int(currentTile.x + 1, currentTile.y)) && CanMoveToTile(new Vector2Int(currentTile.x + 1, currentTile.y), myPos, isPlayer))
        {
            validTiles.Add(new Vector2Int(currentTile.x + 1, currentTile.y));
        }
        if (TileIsInGrid(new Vector2Int(currentTile.x - 1, currentTile.y)) && CanMoveToTile(new Vector2Int(currentTile.x - 1, currentTile.y), myPos, isPlayer))
        {
            validTiles.Add(new Vector2Int(currentTile.x - 1, currentTile.y));
        }
        if (TileIsInGrid(new Vector2Int(currentTile.x, currentTile.y + 1)) && CanMoveToTile(new Vector2Int(currentTile.x, currentTile.y + 1), myPos, isPlayer))
        {
            validTiles.Add(new Vector2Int(currentTile.x, currentTile.y + 1));
        }
        if (TileIsInGrid(new Vector2Int(currentTile.x, currentTile.y - 1)) && CanMoveToTile(new Vector2Int(currentTile.x, currentTile.y - 1), myPos, isPlayer))
        {
            validTiles.Add(new Vector2Int(currentTile.x, currentTile.y - 1));
        }
        
        return validTiles;
    }

    /// <summary>
    /// Returns a list of all the adjacent tiles
    /// </summary>
    /// <param name="currentTile"></param> The provided tile
    /// <returns></returns> The list of available adjacent tiles
    public static List<Vector2Int> GetAllAdjacentTiles(Vector2Int currentTile)
    {
        List<Vector2Int> validTiles = new List<Vector2Int>();

        if (TileIsInGrid(new Vector2Int(currentTile.x + 1, currentTile.y)))
        {
            validTiles.Add(new Vector2Int(currentTile.x + 1, currentTile.y));
        }
        if (TileIsInGrid(new Vector2Int(currentTile.x - 1, currentTile.y)))
        {
            validTiles.Add(new Vector2Int(currentTile.x - 1, currentTile.y));
        }
        if (TileIsInGrid(new Vector2Int(currentTile.x, currentTile.y + 1)))
        {
            validTiles.Add(new Vector2Int(currentTile.x, currentTile.y + 1));
        }
        if (TileIsInGrid(new Vector2Int(currentTile.x, currentTile.y - 1)))
        {
            validTiles.Add(new Vector2Int(currentTile.x, currentTile.y - 1));
        }

        return validTiles;
    }

    /// <summary>
    /// Updates the grid to show where an entity moved to
    /// </summary>
    /// <param name="originalTile"></param> The tile the entity was on before it moved
    /// <param name="tileMovedTo"></param> The tile  the entity moved to
    /// <param name="entityType"></param> The int classification of the entity
    public static void MoveToTile(Vector2Int originalTile ,Vector2Int tileMovedTo, int entityType)
    {
        if (originalTile != tileMovedTo)
        {
            combatGrid[originalTile.x, originalTile.y].entityOnGrid = -1;
            combatGrid[tileMovedTo.x, tileMovedTo.y].entityOnGrid = entityType;
            if (entityType == -3)
            {
                playerPosition = tileMovedTo;
            }
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
                if(combatGrid[i, j].entityOnGrid >= 0)
                {
                    combatGrid[i, j].entityOnGrid = -1;
                }
            }
        }
    }

    /// <summary>
    /// Used to clear the ghost positions of enemies when they were planning on moving
    /// </summary>
    public static void ClearGhostEntities()
    {
        foreach(Vector2Int v in GhostEntities)
        {
            combatGrid[v.x, v.y].entityOnGrid = -1;
        }

        GhostEntities.Clear();
    }

    /// <summary>
    /// Adds the index of where an enemy is trying to move. Used to help visualize what paths all enemies will take
    /// </summary>
    /// <param name="v"></param>
    public static void AddGhostEntity(Vector2Int v)
    {
        GhostEntities.Add(v);
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
                row += combatGrid[j, i].entityOnGrid + " ";
            }
            row += "\n";
        }
    }

    /// <summary>
    /// returns the how many tiles away a certain tile is from another tile
    /// </summary>
    /// <returns></returns>
    public static int DistanceToTile(TileBehaviour startTile, TileBehaviour targetTile) { 
        int distanceX = Mathf.Abs(startTile.IndexInGrid.x - targetTile.IndexInGrid.x);
        int distanceY = Mathf.Abs(startTile.IndexInGrid.y - targetTile.IndexInGrid.y);
        return distanceX + distanceY;
    }

    /// <summary>
    /// finds all of the itlebehaviours in a given range around a certain orgin tile
    /// </summary>
    /// <returns></returns>
    public static List<TileBehaviour> FindTilesInRange(TileBehaviour orginTile, int range) {
        List<TileBehaviour> tilesInRange = new List<TileBehaviour>();
        foreach (TileBehaviour tile in combatGrid)
        {
            if (DistanceToTile(tile, orginTile) <= range) { 
                tilesInRange.Add(tile);
            }
        }
        return tilesInRange;
    }

    /// <summary>
    /// highlights the tiles in a certain range
    /// </summary>
    public static void HiglightTilesInRange(TileBehaviour orginTile, int range, Color highlightColor) {
        List<TileBehaviour> tilesInRange = FindTilesInRange(orginTile, range);
        foreach (TileBehaviour tile in tilesInRange)
        {
            tile.SetHighlightColor(highlightColor);
            tile.ShowHighlight(true);
        }
    }

    /// <summary>
    /// turns off the highlight on all tiles
    /// </summary>
    public static void RemoveHighlight() {
        foreach (TileBehaviour tile in combatGrid) {
            tile.ShowHighlight(false);
        }
    }
    
    /// <summary>
    /// Checks if passed tile location is empty (not containing player, enemy, etc...)
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public static bool TileIsEmpty(Vector2Int tile)
    {
        if(combatGrid==null)
        {
            return true ;
        }
        if(tile == null)
        {
            return true;
        }
        return combatGrid[tile.x, tile.y].entityOnGrid == -1;
    }
}
