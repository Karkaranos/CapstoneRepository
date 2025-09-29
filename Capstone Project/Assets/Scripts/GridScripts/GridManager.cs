using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

public class GridManager : MonoBehaviour
{
    private static List<Tile[,]> grids = new List<Tile[,]>();
    public static Tile[,] currentGrid;
   
    
    /// Creates a new square grid and returns the result
    /// <param name="name"></param>: name of the grid
    /// <param name="dimensions"></param>: the dimensions of the grid
    /// <param name="tileSize"></param>: The dimensions of the tile
    /// <param name="spawnLocation"></param> this is where the center of the grid will be
    /// has tree overloads that are less specific with the parameters
    public static void MakeGrid(string name, Vector2Int dimensions, float radius, Vector3 spawnLocation)
    {
        //Hex grid math
        float hexWidth = 2f * radius;
        float hexHeight = Mathf.Sqrt(3f) * radius;
        float horizontalSpacing = 0.75f * hexWidth; // Hex Grids have a 3/4 width overlap

        // start from center offset
        Vector3 startPos = spawnLocation;
        startPos.x -= (dimensions.x - 1) * horizontalSpacing / 2f;
        startPos.z -= (dimensions.y - 1) * hexHeight / 2f;

        // make grid
        Tile[,] grid = new Tile[dimensions.x, dimensions.y];

        for (int x = 0; x < dimensions.x; x++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                // making the collumns offset
                float zOffset = (x % 2 == 0) ? 0f : hexHeight / 2f;

                float worldX = startPos.x + x * horizontalSpacing;
                float worldZ = startPos.z + y * hexHeight + zOffset;

                Vector3 tilePos = new Vector3(worldX, spawnLocation.y, worldZ);

                // creating tile at this position
                grid[x, y] = new Tile(tilePos, new Vector2Int(x, y));
                GameObject newTile = Instantiate(Resources.Load<GameObject>("TileMarker"), tilePos, Quaternion.identity);
                newTile.name = "Tile(" + x.ToString() + ", " + y.ToString() + ")";
            }
        }

        // finalize
        grids.Add(grid);

        if (currentGrid == null) {
            currentGrid = grid;
        }
    }

    /// <summary>
    /// returns true if the current grid has a tile at the specified coords
    /// </summary>
    /// <param name="coords"></param>
    /// <returns></returns>
    public static bool HasTile(Vector2Int coords) {
        if ((coords.x >= 0 && coords.x < currentGrid.GetLength(0)) && (coords.y >= 0 && coords.x < currentGrid.GetLength(1))) {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns the Tile Object at specific Coordinates
    /// </summary>
    /// <param name="coords"></param>
    /// <returns></returns>
    public static Tile GetTile(Vector2Int coords)
    {
        if (HasTile(coords))
        {
            return currentGrid[coords.x, coords.y];
        }
        return null;
    }

    /// <summary>
    /// Returns the Tile that a certian GameObject is on
    /// </summary>
    /// <returns></returns>
    public static Tile GetTileWithObject(GameObject obj) {
        foreach (Tile tile in currentGrid) {
            if (tile.objectOnTile == obj && obj != null) {
                return tile;
            }
        }
        return null;
    }

    /// <summary>
    /// returns a list of tiles that have GameObjects with a specified tag attached to them
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static List<Tile> GetObjectsWithTag(string tag) {
        List<Tile> list = new List<Tile>();
        foreach (Tile tile in currentGrid) {
            if (tile.objectOnTile is GameObject obj) {
                if (obj.tag == tag) {
                    list.Add(tile);
                }
            }
        }
        return list;
    }

    /// <summary>
    /// These 6 functions return the neighboring tile to the parameter's tile that the function name specifies
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public static Tile GetUpperNeighbor(Tile tile)
    {
        Vector2Int neighbor = new Vector2Int(tile.coordinate.x, tile.coordinate.y + 1);
        if (HasTile(neighbor)) {
            return GetTile(neighbor);
        }
        return null;
    }
    public static Tile GetUpperLeftNeighbor(Tile tile)
    {
        Vector2Int neighbor;
        if (tile.coordinate.x % 2 == 0)
        {
            neighbor = new Vector2Int(tile.coordinate.x - 1, tile.coordinate.y);
        }
        else {
            neighbor = new Vector2Int(tile.coordinate.x -1, tile.coordinate.y + 1);
        }
        if (HasTile(neighbor))
        {
            return GetTile(neighbor);
        }
        return null;
    }
    public static Tile GetUpperRightNeighbor(Tile tile)
    {
        Vector2Int neighbor;
        if (tile.coordinate.x % 2 == 0)
        {
            neighbor = new Vector2Int(tile.coordinate.x + 1, tile.coordinate.y);
        }
        else
        {
            neighbor = new Vector2Int(tile.coordinate.x + 1, tile.coordinate.y + 1);
        }
        if (HasTile(neighbor))
        {
            return GetTile(neighbor);
        }
        return null;
    }
    public static Tile GetLowerNeighbor(Tile tile)
    {
        Vector2Int neighbor = new Vector2Int(tile.coordinate.x, tile.coordinate.y - 1);
        if (HasTile(neighbor))
        {
            return GetTile(neighbor);
        }
        return null;
    }
    public static Tile GetLowerLeftNeighbor(Tile tile)
    {
        Vector2Int neighbor;
        if (tile.coordinate.x % 2 == 0)
        {
            neighbor = new Vector2Int(tile.coordinate.x - 1, tile.coordinate.y - 1);
        }
        else
        {
            neighbor = new Vector2Int(tile.coordinate.x - 1, tile.coordinate.y);
        }
        if (HasTile(neighbor))
        {
            return GetTile(neighbor);
        }
        return null;
    }
    public static Tile GetLowerRightNeighbor(Tile tile)
    {
        Vector2Int neighbor;
        if (tile.coordinate.x % 2 == 0)
        {
            neighbor = new Vector2Int(tile.coordinate.x + 1, tile.coordinate.y - 1);
        }
        else
        {
            neighbor = new Vector2Int(tile.coordinate.x + 1, tile.coordinate.y);
        }
        if (HasTile(neighbor))
        {
            return GetTile(neighbor);
        }
        return null;
    }

    /// <summary>
    /// returns a list of all the neighboring tiles
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public static List<Tile> GetAllNeighbors(Tile tile) { 
        List<Tile> list = new List<Tile>();
        if (GetUpperNeighbor(tile) != null) {
            list.Add(GetUpperNeighbor(tile));
        }
        if (GetUpperLeftNeighbor(tile) != null)
        {
            list.Add(GetUpperLeftNeighbor(tile));
        }
        if (GetUpperRightNeighbor(tile) != null)
        {
            list.Add(GetUpperRightNeighbor(tile));
        }
        if (GetLowerNeighbor(tile) != null)
        {
            list.Add(GetLowerNeighbor(tile));
        }
        if (GetLowerLeftNeighbor(tile) != null)
        {
            list.Add(GetLowerLeftNeighbor(tile));
        }
        if (GetLowerRightNeighbor(tile) != null)
        {
            list.Add(GetLowerRightNeighbor(tile));
        }
        return list;
    }

    /// <summary>
    /// returns all empty neighboring tiles
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public static List<Tile> GetAllEmptyNeighbors(Tile tile) {
        List<Tile> list = GetAllNeighbors(tile);
        List<Tile> newList = new List<Tile>();
        foreach (Tile t in list) {
            if (t.isEmpty()) { 
                newList.Add(t);
            }
        }
        return newList;
    }

    /// <summary>
    /// finds all of the tiles that are currently empty on the grid
    /// </summary>
    /// <returns></returns>
    public static List<Tile> FindEmptyTiles() {
        List<Tile> list = new List<Tile>();
        foreach (Tile tile in currentGrid) {
            if (tile.objectOnTile == null) {
                list.Add(tile);
            }
        }
        return list;
    }

    /// <summary>
    /// Spawns a new object onto the grid at specified coordinates
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="tile"></param>
    public static GameObject CreateObject(GameObject obj, Vector2Int coords) {

        if (!HasTile(coords))
        {
            print("Grid doesn't have that position");
            return null;
            
        }

        Tile tile = GetTile(coords);
        if (HasTile(coords) && tile.objectOnTile == null) {
            GameObject newObj = Instantiate(obj, tile.worldPosition, Quaternion.identity);
            tile.objectOnTile = newObj;
            return newObj;
        }
        print("Failed to add" + obj + " at " + coords);
        return null;
    }

    /// <summary>
    /// spawns a new object to the grid on a random tile
    /// </summary>
    /// <param name="obj"></param>
    public static GameObject CreateObject(GameObject obj) {
        List<Tile> emptyTiles = FindEmptyTiles();
        if (emptyTiles.Count <= 0)
        {
            print("Failed to add " + obj + ", Grid was full");
            return null;
        }
        int randomIndex = Random.Range(0, emptyTiles.Count);
        return CreateObject(obj, emptyTiles[randomIndex].coordinate);
    }

    /// <summary>
    /// removes a gameobject from the current grid if its on there.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns> the object that was removed
    public static GameObject RemoveObject(GameObject obj) {
        if (GetTileWithObject(obj) is Tile tile) {
            GameObject objToRemove = tile.objectOnTile;
            GameObject.Destroy(tile.objectOnTile);
            tile.objectOnTile = null;
            return objToRemove;
        }
        print("Failed to remove " + obj);
        return null;
    }

    /// <summary>
    /// removes all gameobjects that are on the grid
    /// </summary>
    public static void ClearGrid() {
        foreach (Tile tile in currentGrid)
        {
            if (tile.objectOnTile != null)
            {
                RemoveObject(tile.objectOnTile);
            }
        }
    }
}