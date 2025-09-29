using System.Collections.Generic;
using UnityEngine;
public class Tile
{
    public Vector3 worldPosition;
    public Vector2Int coordinate;
    public GameObject objectOnTile;

    //class constructor
    public Tile(Vector3 _worldPosition, Vector2Int _coordinate) { 
        worldPosition = _worldPosition;
        coordinate = _coordinate;
        objectOnTile = null;
    }

    //returns if the tile has an object on it or not
    public bool isEmpty() {
        if (objectOnTile == null) {
            return true;
        }
        return false;
    }
}
