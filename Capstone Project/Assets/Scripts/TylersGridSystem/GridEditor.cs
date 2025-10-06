using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class GridEditor : MonoBehaviour
{
    [SerializeField] private GridData grid;
    [SerializeField] private Vector2Int girdDimensions;
    [SerializeField] private float HexRadius;
    [SerializeField] private Vector3 gridLocation;
    [SerializeField] private List<ObjectOnGrid> gridObjects;


    private void Start()
    {
        girdDimensions = grid.dimensions;
        HexRadius = grid.hexRadius;
        gridLocation = grid.spawnLocation;
        gridObjects = grid.objectsOnGrid;
        TylersGridManager.MakeGrid(grid);
    }
    [Button]
    void UpdateAndSaveGrid() {
        grid.dimensions = girdDimensions;
        grid.hexRadius = HexRadius;
        grid.spawnLocation = gridLocation;
        grid.objectsOnGrid = SaveGridObjects();
        TylersGridManager.DestroyGrid();
        TylersGridManager.MakeGrid(grid);
    }

    private List<ObjectOnGrid> SaveGridObjects() {
        List<ObjectOnGrid> list = new List<ObjectOnGrid>();
        foreach (Tile tile in TylersGridManager.grid) {
            if ((tile.objectToAdd != null || tile.tileType != TileType.Default) && GridHasTile(tile.coordinate)) {
                list.Add(new ObjectOnGrid(tile.coordinate, tile.objectToAdd,tile.tileType));
            }
        }
        return list;
    }

    private bool GridHasTile(Vector2Int coords) {
        if (coords.x >= 0 && coords.x < girdDimensions.x && coords.y >= 0 && coords.y < girdDimensions.y)
        {
            return true;
        }
        return false;
    }

}
