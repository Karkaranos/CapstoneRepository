using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

public class GridBuilder : MonoBehaviour
{
    [SerializeField] GridData grid;
    [SerializeField] private Vector2Int dimensions;
    [SerializeField] private float hexRadius;
    [SerializeField] private Vector3 spawnLocation;
    [SerializeField] private List<ObjectOnGrid> objectsOnGrid;

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
        grid. dimensions = dimensions;
        grid.hexRadius = hexRadius;
        grid.spawnLocation = spawnLocation;
        grid.objectsOnGrid = SaveGridObjects();
        GridManager.DestroyGrid();
        GridManager.MakeGrid(grid);
    }
    private List<ObjectOnGrid> SaveGridObjects() {
        List<ObjectOnGrid> gridObjects = new List<ObjectOnGrid>();
        foreach (Tile tile in GridManager.currentGrid) {
            if (!tile.isEmpty()) {
                gridObjects.Add(new ObjectOnGrid(tile.coordinate, tile.objectOnTile));
            }
        }
        return gridObjects;
    }
}
