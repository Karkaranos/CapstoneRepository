using Unity.VisualScripting;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
    public Tile currentTile;


    void Start()
    {
        currentTile = TylersGridManager.GetTileWithObject(gameObject);
        if (currentTile == null) { print("cannot find " + gameObject.name); }
    }
    public void MoveTo(Tile neighbor) {
        if (neighbor != null && neighbor.isEmpty())
        {
            currentTile.objectOnTile = null;
            transform.position = neighbor.worldPosition;
            currentTile = neighbor;
            neighbor.objectOnTile = gameObject;
        }
        else
        {
            print("failed to move up");
        }
    }
   
    public void MoveDownLeft()
    {
        Tile neighbor = TylersGridManager.GetLowerLeftNeighbor(currentTile);
        if (neighbor != null && neighbor.isEmpty())
        {
            transform.position = neighbor.worldPosition;
            currentTile = neighbor;
        }
        else
        {
            print("failed to move down left");
        }
    }
    public void MoveDownRight()
    {
        Tile neighbor = TylersGridManager.GetLowerRightNeighbor(currentTile);
        if (neighbor != null && neighbor.isEmpty())
        {
            transform.position = neighbor.worldPosition;
            currentTile = neighbor;
        }
        else
        {
            print("failed to move down right");
        }
    }
}
