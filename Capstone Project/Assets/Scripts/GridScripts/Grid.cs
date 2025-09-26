using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    public Tile[,] grid;
    public string name;

    //class constructor
    public Grid(string _name, Tile[,] _grid) {
        name = _name;
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
    // adds an object to the grid if theres room for it returns true if succesful
    public bool AddObjectToGrid(GameObject obj)
    {
        List<Tile> emptyTiles = FindEmptyTiles();
        if (emptyTiles.Count <= 0)
        {
            return false;
        }
        int randomIndex = Random.Range(0, emptyTiles.Count);
        AddObjectToTile(obj, emptyTiles[randomIndex].coordinate);
        return true;
    }

    //removes the object on a specific tile if it has one and then returns what the object on the tile was 
    public GameObject RemoveObjectFromTile(Vector2Int coords)
    {
        if (HasTile(coords))
        {
            Tile tile = grid[(int)coords.x, (int)coords.y];
            GameObject objectRemoved = tile.objectOnTile;

            tile.objectOnTile = null;
            return objectRemoved;
        }
        return null;
    }

    //attempts to remove an object from the grid and then returns if it was succesful or not
    public bool RemoveObjectFromGrid(GameObject obj)
    {
        foreach (Tile tile in grid) {
            if (tile.objectOnTile == obj) {
                RemoveObjectFromTile(tile.coordinate);
                return true;
            }
        }
        return false;
    }

    //returns the tile that a specific game object is on
    public Tile FindObjectOnGrid(GameObject obj) {
        foreach (Tile tile in grid) {
            if (tile.objectOnTile == obj) {
                return tile;
            }
        }
        return null;
    }
    
    //returns a list of all the empty tiles on the grid
    private List<Tile> FindEmptyTiles() {
        List<Tile> emptyTiles = new List<Tile>();
        foreach (Tile tile in grid) {
            if (tile.isEmpty()) { 
                emptyTiles.Add(tile);
            }
        }
        return emptyTiles;
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
