using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

public class GridBuilder : MonoBehaviour
{
    [SerializeField] GridData grid;
    [SerializeField] private Vector2Int dimensions;
    [SerializeField] private float hexRadius;
    [SerializeField] private Vector3 spawnLocation;

    private void Start()
    {
        dimensions = grid.dimensions;
        hexRadius = grid.hexRadius;
        spawnLocation = grid.spawnLocation;
        GridManager.MakeGrid(grid);
    }
    [Button]
    void UpdateGrid()
    {
        grid.dimensions = dimensions;
        grid.hexRadius = hexRadius;
        grid.spawnLocation = spawnLocation;
        grid.objectsOnGrid = SaveGridObjects();
        if (GridManager.currentGrid != null) {
            GridManager.DestroyGrid();
        }
        GridManager.MakeGrid(grid);
    }
    private List<ObjectOnGrid> SaveGridObjects() {
        List<ObjectOnGrid> gridObjects = new List<ObjectOnGrid>();
        foreach (Tile tile in GridManager.currentGrid) {
            if (!tile.isEmpty() && TileExists(tile.coordinate)) {
                gridObjects.Add(new ObjectOnGrid(tile.coordinate, tile.objectToAdd));
            }
        }
        return gridObjects;
    }

    private bool TileExists(Vector2Int coords) {
        if ((coords.x >= 0 && coords.x < dimensions.x) && (coords.y >= 0 && coords.y < dimensions.y))
        {
            return true;
        }
        return false;
    }
}
