/*************************************************
Author Name(s) : 		Bouchard, Tyler
Date Created : 		    9/25/2025
Date Last Modified : 	9/30/2025
Brief Description :     This class makes the Tile object
External Resources : 	
	***************************************************/
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
public class Tile: MonoBehaviour
{
    public GameObject objectOnTile;
    [HideInInspector] public Vector3 worldPosition;
    [HideInInspector] public Vector2Int coordinate;
    [HideInInspector] public int pathingValue = -1;

    [Button]
    void addObject()
    {
        foreach (Tile tile in GridManager.currentGrid) {
            print(tile.objectOnTile != null);
            if (tile.objectOnTile != null) {
                GridManager.CreateObject(tile.objectOnTile, tile.coordinate);
            }
        }
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
