using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
public class Tile : MonoBehaviour
{
    public GameObject objectToAdd = null;
    [HideInInspector] public GameObject objectOnTile = null;
    [HideInInspector] public Vector3 worldPosition = new Vector3(0,0,0);
    [HideInInspector] public Vector2Int coordinate = new Vector2Int(0,0);
    [HideInInspector] public int pathingValue = -1;

    [Button]
    void addObject()
    {
        foreach (Tile tile in TylersGridManager.currentGrid)
        {
            if (tile.objectToAdd != null)
            {
                TylersGridManager.CreateObject(tile.objectToAdd, tile.coordinate);
            }
        }
    }

    [Button]
    void RemoveObject()
    {
        TylersGridManager.RemoveObject(objectOnTile);
        objectToAdd = null;
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
