using JetBrains.Annotations;
using NUnit.Framework;
using UnityEngine;

public class Grid
{
    public Tile[,] grid;
    public Grid(Tile[,] _grid) { 
        grid = _grid;
    }
    //returns a Tile at specific coordinates
    public Tile GetTile(Vector2Int coordinates) {
        if (HasTile(coordinates)) {
            return grid[coordinates.x, coordinates.y];
        }
        return null;
    }

    //adds an object to the grid at a specific grid coordinate returns true if succesful
    public bool AddObjectToTile(GameObject obj, Vector2Int coords)
    {
        if (HasTile(coords))
        {
            Tile tile = grid[(int)coords.x, (int)coords.y];
            if (tile.isEmpty())
            {
                tile.objectOnTile = obj;
                return true;
            }
        }
        return false;
    }

    //figures out if a given coordinate is in the current grid
    private bool HasTile(Vector2Int coords)
    {
        bool hasTile = true;
        if (coords.x < 0 || coords.x >= grid.GetLength(0))
        {
            hasTile = false;
        }
        if (coords.y < 0 || coords.y >= grid.GetLength(1))
        {
            hasTile = false;
        }
        return hasTile;
    }
}
