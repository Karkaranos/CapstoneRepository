/*************************************************
Author Name(s) : 		Bouchard, Tyler
Date Created : 		    9/25/2025
Date Last Modified : 	9/30/2025
Brief Description :     This class makes the Tile object
External Resources : 	
	***************************************************/
using System.Collections.Generic;
using UnityEngine;
public class Tile
{
    public Vector3 worldPosition;
    public Vector2Int coordinate;
    public GameObject objectOnTile;
    public int pathingValue = -1;

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
