using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private static List<Grid> grids = new List<Grid>();
    public static Grid currentGrid;
    /// Creates a new square grid and returns the result
    /// <param name="name"></param>: name of the grid
    /// <param name="dimensions"></param>: the dimensions of the grid
    /// <param name="tileSize"></param>: The dimensions of the tile
    /// <param name="spawnLocation"></param> this is where the center of the grid will be
    /// has tree overloads that are less specific with the parameters
    public static Grid MakeGrid(string name, Vector2Int dimensions, float radius, Vector3 spawnLocation)
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
        Grid grid = new Grid(name, dimensions);

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
                grid.tiles[x, y] = new Tile(tilePos, new Vector2Int(x, y));
                GameObject newTile = Instantiate(Resources.Load<GameObject>("TileMarker"), tilePos, Quaternion.identity);
                newTile.name = "Tile(" + x.ToString() + ", " + y.ToString() + ")";
            }
        }

        // finalize
        grids.Add(grid);

        if (currentGrid == null) {
            currentGrid = grid;
        }
        return grid;
    }

    
}