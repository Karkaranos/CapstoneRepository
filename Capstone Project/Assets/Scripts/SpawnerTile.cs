/******************************************************************************
 * Author: Brad Dixon
 * Creation Date: 9/26/2025
 * Last Modified: 9/30/2025
 * Brief: Spawns the appropriate game object on scene start
 * ***************************************************************************/
using UnityEngine;

[SerializeField]
enum Entities
{
    Enemy,
    Player,
    Obstacle
}
public class SpawnerTile : MonoBehaviour
{
    [SerializeField] Vector2Int coordinatesInGrid;
    [SerializeField] GameObject entity;
    [SerializeField] Entities entityType;

    /// <summary>
    /// Spawns the appropriate entity and adds their position to the grid manager
    /// </summary>
    void Start()
    {
        int eType = -1;
        GameObject obj = Instantiate(entity, transform.position, Quaternion.identity);
        obj.transform.SetParent(GetComponentInParent<Transform>().transform);
        switch(entityType)
        {
            case Entities.Enemy:
                eType = -2;
                break;
            case Entities.Player:
                eType = -3;
                break;
            case Entities.Obstacle:
                eType = -4;
                break;
        }
        GridManager.AddEntity(coordinatesInGrid, eType);
    }
}
