/******************************************************************************
 * Author: Brad Dixon
 * Creation Date: 9/26/2025
 * Last Modified: 10/2/2025
 * Brief: Spawns the appropriate game object on scene start
 * External Resources: N/A
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
    [Tooltip("The x and y index inside the grid")]
    [SerializeField] Vector2Int coordinatesInGrid;
    [Tooltip("The game object that is being spawned")]
    [SerializeField] GameObject entity;
    [Tooltip("The type of thing being spawned")]
    [SerializeField] Entities entityType;

    /// <summary>
    /// Spawns the appropriate entity and adds their position to the grid manager
    /// </summary>
    void Start()
    {
        coordinatesInGrid = GetComponent<TileBehaviour>().IndexInGrid;
        int eType = -1;
        GameObject obj = Instantiate(entity, transform.position, Quaternion.identity);
        if(obj.GetComponent<GridPathfinding>() != null)
        {
            obj.GetComponent<GridPathfinding>().MyPosition = coordinatesInGrid;
        }
        //obj.transform.SetParent(GetComponentInParent<Transform>().transform);
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
