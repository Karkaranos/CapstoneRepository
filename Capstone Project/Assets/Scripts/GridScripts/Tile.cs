using System.Collections.Generic;
using UnityEngine;
public class Tile
{
    public Vector3 worldPosition;
    public Vector2Int coordinate;
    public GameObject objectOnTile;

    /// <summary>
    /// Class constructor
    /// </summary>
    /// <param name="_worldPosition"></param>
    /// <param name="_coordinate"></param>
    public Tile(Vector3 _worldPosition, Vector2Int _coordinate) { 
        worldPosition = _worldPosition;
        coordinate = _coordinate;
        objectOnTile = null;
    }

    /// <summary>
    /// returns if the tile is empty or not
    /// </summary>
    /// <returns></returns>
    public bool isEmpty() {
        if (objectOnTile == null) {
            return true;
        }
        return false;
    }
}
