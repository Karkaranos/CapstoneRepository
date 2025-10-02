using UnityEngine;

[System.Serializable]
public class ObjectOnGrid
{
    public Vector2Int coords;
    public GameObject obj;

    public ObjectOnGrid(Vector2Int coords, GameObject obj)
    {
        this.coords = coords;
        this.obj = obj;
    }
}