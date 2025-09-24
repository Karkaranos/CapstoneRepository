using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public List<Tile[,]> grids;
    public Tile[,] currentGrid;

    /// Creates a new square grid and returns the result
    /// <param name="dimensions"></param>: the dimensions of the grid
    /// <param name="tileSize"></param>: The dimensions of the tile
    /// <param name="spawnLocation"></param> this is where the center of the grid will be
    /// has two overloads that are less specific with the parameters
    public Tile[,] MakeGrid(Vector2 dimensions, Vector2 tileSize, Vector3 spawnLocation)
    {

        //finding the starting tile spawn point based off spawnLocation
        Vector3 spawnPosition = new Vector3();
        spawnPosition.x = spawnLocation.x - (dimensions.x / 0.5f - 1f);
        spawnPosition.y = spawnLocation.y - (dimensions.y / 0.5f - 1f);
        spawnPosition.z = spawnLocation.z;

        //making grid
        Tile[,] grid = new Tile[(int)dimensions.x, (int)dimensions.y];
        for (int i = 0; i < dimensions.x; i++)
        {
            for (int j = 0; j < dimensions.y; j++)
            {
                grid[i, j] = new Tile(spawnPosition, new Vector2(i, j));
                spawnPosition.y += tileSize.y;
            }
            spawnPosition.y -= tileSize.y * dimensions.y;
            spawnPosition.x += tileSize.x;
        }

        //finalizing
        if (currentGrid == null) {
            currentGrid = grid;
        }
        grids.Add(grid);
        return grid;
    }
    public Tile[,] MakeGrid(Vector2 dimensions, Vector2 tileSize)
    {
        return MakeGrid(dimensions, tileSize, Vector3.zero);
    }
    public Tile[,] MakeGrid(Vector2 dimensions) {
        return MakeGrid(dimensions, Vector2.one, Vector3.zero);
    }

    public Tile[,] useGrid(int index) {
        if () { 
            currentGrid = grids[index];
        }
        return currentGrid;
    }



}
