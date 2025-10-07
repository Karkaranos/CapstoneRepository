using NaughtyAttributes;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// tile type enum for all to use
/// </summary>
public enum TileType
{
    Default,
    Damage,
    Slow
}


public class Tile : MonoBehaviour
{
    public TileType tileType = TileType.Default;
    public GameObject objectToAdd = null;
    [HideInInspector] public GameObject objectOnTile = null;
    [HideInInspector] public Vector3 worldPosition = new Vector3(0,0,0);
    [HideInInspector] public Vector2Int coordinate = new Vector2Int(0,0);
    [HideInInspector] public int pathingValue = -1;

    [Button]
    void UpdateTile()
    {
        foreach (Tile tile in TylersGridManager.grid)
        {
            UpdateTileType(tile);
            if (tile.objectOnTile == null && tile.objectToAdd != null)
            {
                TylersGridManager.CreateObject(tile.objectToAdd, tile.coordinate);
            }
        }
    }

    [Button]
    void ResetTile()
    {
        if (objectOnTile != null) {
            TylersGridManager.RemoveObject(objectOnTile);
            objectToAdd = null;
        }
        tileType = TileType.Default;
        UpdateTileType(this);
    }

    public void UpdateTileType(Tile tile) {
        if (tile.tileType == TileType.Default) 
        {
            tile.gameObject.GetComponent<Renderer>().material.color = Color.white;
        }
        if (tile.tileType == TileType.Damage)
        {
            tile.gameObject.GetComponent<Renderer>().material.color = Color.red;
        }
        if (tile.tileType == TileType.Slow)
        {
            tile.gameObject.GetComponent<Renderer>().material.color = Color.gray;
        }
    }
    

    /// <summary>
    /// returns if the tile is empty or not
    /// </summary>
    /// <returns></returns>
    public bool isEmpty()
    {
        if (objectOnTile == null)
        {
            return true;
        }
        return false;
    }
}
