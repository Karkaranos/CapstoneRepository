using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GridData", menuName = "Scriptable Objects/GridData")]
public class GridData : ScriptableObject
{
    public Vector2Int dimensions = new Vector2Int(5, 5);
    public float hexRadius = 1;
    public Vector3 spawnLocation = Vector3.zero;
    public List<ObjectOnGrid> objectsOnGrid;
}
