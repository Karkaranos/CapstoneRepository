using UnityEngine;

public class GridTesting : MonoBehaviour
{
    public Vector2Int gridSize;
    public Vector2 tileSize;
    public Vector3 location;
    public GridManager gm;
    private void Start()
    {
        GenerateGrid();
    }
    public void GenerateGrid() {
        gm.MakeGrid(gridSize, tileSize, location);
    }
}
