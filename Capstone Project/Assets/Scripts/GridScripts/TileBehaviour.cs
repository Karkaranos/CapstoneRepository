/******************************************************************************
 * Author: Brad Dixon
 * Creation Date: 10/2/2025
 * Last Modified: 10/2/2025
 * Brief: Stores the tile's index in the grid to help with player movement and
 * stores information about what kind of tile it is
 * External Resources: N/A
 * ***************************************************************************/
using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

public class TileBehaviour : MonoBehaviour
{
    [SerializeField]
    private enum TileSettings
    {
        GridSettings,
        TerrainSettings
    }

    [Tooltip("The index the tile is inside the grid")]
    public Vector2Int IndexInGrid;

    [Tooltip("How far a tile's transform must move in order to be adjacent to another tile")]
    [SerializeField] Vector2 tileDisplacement;

    public List<StatsOnTile> tileStatAffects = new List<StatsOnTile>();

    /// <summary>
    /// Calculates the tile's index based off the transform and the tileDisplacement variable
    /// </summary>
    private void Awake()
    {
        IndexInGrid.x = (int)(transform.position.x / tileDisplacement.x);
        IndexInGrid.y = (int)(transform.position.z / tileDisplacement.y);
    }

    /// <summary>
    /// Sets an entity to be a child of a tile
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider collision)
    {
        collision.transform.SetParent(transform);
    }
}
