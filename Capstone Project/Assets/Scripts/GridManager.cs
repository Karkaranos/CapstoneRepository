using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public List<Grid> grids = new List<Grid>();
    public Grid currentGrid;
    public GameObject tileMarker;

    /// Creates a new square grid and returns the result
    /// <param name="dimensions"></param>: the dimensions of the grid
    /// <param name="tileSize"></param>: The dimensions of the tile
    /// <param name="spawnLocation"></param> this is where the center of the grid will be
    /// has two overloads that are less specific with the parameters
    public Grid MakeGrid(Vector2Int dimensions, Vector2 tileSize, Vector3 spawnLocation)
    {

        //finding the starting tile spawn point based off the grids spawnLocation
        Vector3 spawnPosition = new Vector3();
        spawnPosition.x = spawnLocation.x - (dimensions.x / 2f - 0.5f);
        spawnPosition.y = spawnLocation.y;
        spawnPosition.z = spawnLocation.z - (dimensions.y / 2f - 0.5f);

        //making the grid
        Tile[,] grid = new Tile[dimensions.x, dimensions.y];
        for (int i = 0; i < dimensions.x; i++)
        {
            for (int j = 0; j < dimensions.y; j++)
            {
                Instantiate(tileMarker, spawnPosition, Quaternion.identity);
                grid[i, j] = new Tile(spawnPosition, new Vector2Int(i, j));
                spawnPosition.z += tileSize.y;
            }
            spawnPosition.z -= tileSize.y * dimensions.y;
            spawnPosition.x += tileSize.x;
        }

        //finalizing
        Grid newGrid = (new Grid(grid));
        foreach (Tile tile in grid) { 
            tile.parentGrid = newGrid;
        }
        grids.Add(newGrid);
        if (currentGrid == null) {
            currentGrid = newGrid;
        }
        return newGrid;
    }
    public Grid MakeGrid(Vector2Int dimensions, Vector2 tileSize)
    {
        return MakeGrid(dimensions, tileSize, Vector3.zero);
    }
    public Grid MakeGrid(Vector2Int dimensions) {
        return MakeGrid(dimensions, Vector2.one, Vector3.zero);
    }

    /// <summary>
    /// changes the grid that is currently being used returns the new current grid
    /// </summary>
    /// <param name="index"></param>: index of the grid that should be used
    /// <returns></returns> : returns the current grid
    public Grid useGrid(int index) {
        if (index >= 0 && index < grids.Count) { 
            currentGrid = grids[index];
        }
        return currentGrid;
    }    
}