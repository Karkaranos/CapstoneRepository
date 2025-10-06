using UnityEngine;

[System.Serializable]
public class ObjectOnGrid
{
    public TileType tileType = TileType.Default;
    public Vector2Int coords;
    public GameObject obj;

    public ObjectOnGrid(Vector2Int coords, GameObject obj, TileType tileType)
    {
        this.coords = coords;
        this.obj = obj;
        this.tileType = tileType;
    }
}
