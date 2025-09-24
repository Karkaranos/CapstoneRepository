using UnityEngine;
public class Tile
{
    public Vector3 worldPosition;
    public Vector2 coordinate;
    public GameObject objectOnTile;
    public int pathingValue;
    public Tile(Vector3 _worldPosition, Vector2 _coordinate) { 
        worldPosition = _worldPosition;
        coordinate = _coordinate;
        objectOnTile = null;
        pathingValue = 0;
    }
    public bool isEmpty() {
        if (objectOnTile == null) {
            return true;
        }
        return false;
    }
}
