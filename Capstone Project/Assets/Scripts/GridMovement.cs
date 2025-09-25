using UnityEngine;

public class GridMovement : MonoBehaviour
{
    private GridManager gridManager;
    public Grid currentGird;
    public Tile currentTile;
    void Awake()
    {
        gridManager = FindFirstObjectByType<GridManager>();
    }

}
