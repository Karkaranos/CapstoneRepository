using System.Collections.Generic;
using UnityEngine;
public class Tile
{
    public Grid parentGrid;
    public Vector3 worldPosition;
    public Vector2Int coordinate;
    public GameObject objectOnTile;

    //class constructor
    public Tile(Vector3 _worldPosition, Vector2Int _coordinate) { 
        worldPosition = _worldPosition;
        coordinate = _coordinate;
        objectOnTile = null;
    }

    //these four fnctionsget neighboring tiles
    public Tile GetTopNeighbor() {
        return parentGrid.GetTile(new Vector2Int(coordinate.x, coordinate.y + 1));
    }
    public Tile GetBottomNeighbor()
    {
        return parentGrid.GetTile(new Vector2Int(coordinate.x, coordinate.y - 1));
    }
    public Tile GetLeftNeighbor()
    {
        return parentGrid.GetTile(new Vector2Int(coordinate.x - 1, coordinate.y));
    }
    public Tile GetRightNeighbor()
    {
        return parentGrid.GetTile(new Vector2Int(coordinate.x + 1, coordinate.y));
    }


    //returns a list of the neighboring tiles that exist
    public List<Tile> GetAllNeighbors() {
        List<Tile> neighbors = new List<Tile>();

        if (GetTopNeighbor() != null) { neighbors.Add(GetTopNeighbor()); }
        if (GetBottomNeighbor() != null) { neighbors.Add(GetBottomNeighbor()); }
        if (GetLeftNeighbor() != null) { neighbors.Add(GetLeftNeighbor()); }
        if (GetRightNeighbor() != null) { neighbors.Add(GetRightNeighbor()); }

        return neighbors;
    }

    //returns a list of the existing and neighboring tiles that dont have an object on them
    public List<Tile> GetAllEmptyNeighbors()
    {
        List<Tile> neighbors = GetAllNeighbors();

        foreach (Tile tile in neighbors) {
            if (tile.isEmpty()) { 
                neighbors.Add(tile);
            }
        }

        return neighbors;
    }

    //returns if the tile has an object on it or not
    public bool isEmpty() {
        if (objectOnTile == null) {
            return true;
        }
        return false;
    }
}
