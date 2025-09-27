/******************************************************************************
 * Author: Brad Dixon
 * Creation Date: 9/26/2025
 * Last Modified: 9/26/2025
 * Brief: Spawns the appropriate game object on scene start
 * ***************************************************************************/
using UnityEngine;

public class SpawnerTile : MonoBehaviour
{
    [SerializeField] Vector2Int coordinatesInGrid;
    [SerializeField] int entityType;
    [SerializeField] GameObject entity;

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        Instantiate(entity, transform.position, Quaternion.identity);
        GridManager.AddEntity(coordinatesInGrid, entityType);
    }
}
