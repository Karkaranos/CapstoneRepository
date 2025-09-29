using UnityEngine;

public class GridMovement : MonoBehaviour
{
    private GridManager gridManager;
    public Grid grid;
    public Tile currentTile;
    private void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        //grid = gridManager.currentGrid;
    }
    public void Move(Vector2 direction) {
        gameObject.transform.position = direction;  
    }
}
