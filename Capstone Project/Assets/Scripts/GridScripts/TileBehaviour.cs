/******************************************************************************
 * Author: Brad Dixon, Tyler Bouchard
 * Creation Date: 10/2/2025
 * Last Modified: 10/21/2025 (Tyler Bouchard)
 * Brief: Stores the tile's index in the grid to help with player movement and
 * stores information about what kind of tile it is
 * External Resources: N/A
 * ***************************************************************************/
using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using static UnityEngine.EventSystems.EventTrigger;

public class TileBehaviour : MonoBehaviour
{
    [Header("Tile Info")]
    [Tooltip("The index the tile is inside the grid")]
    public Vector2Int IndexInGrid;

    [SerializeField, Tooltip("How far a tile's transform must move in order to be adjacent to another tile")]
    Vector2 tileDisplacement;
    private enum EntityType
    {
        Enemy,
        Player,
        Obstacle
    }
    private enum HazardType
    {
        block,
        damage,
        slow 
    }

    [Header("Objects On This Tile")]
    [SerializeField] private bool TileHasEntities = false;
    [SerializeField, ShowIf(nameof(TileHasEntities)), Foldout("Entities")] private EntityType entityType;
    [SerializeField, ShowIf(nameof(TileHasEntities)), Foldout("Entities")] private GameObject entityObject;

    [SerializeField] private bool TileHasHazards = false;
    [SerializeField, ShowIf(nameof(TileHasHazards)), Foldout("Hazards")] private HazardType hazardType;
    [SerializeField, ShowIf(nameof(TileHasHazards)), Foldout("Hazards")] private GameObject hazardObject;
    [SerializeField, ShowIf(nameof(ShowDamageVars)), Foldout("Hazards")] private float damageAmount;
    [SerializeField, ShowIf(nameof(ShowSlowVars)), Foldout("Hazards")] private int movesLost;

    private bool ShowDamageVars() {
        return hazardType == HazardType.damage && TileHasHazards;
    }
    private bool ShowSlowVars()
    {
        return hazardType == HazardType.slow && TileHasHazards;
    }

    [HideInInspector] public List<StatsOnTile> tileStatAffects = new List<StatsOnTile>();

    /// <summary>
    /// Calculates the tile's index based off the transform and the tileDisplacement variable
    /// </summary>
    private void Awake()
    {
        IndexInGrid.x = (int)(transform.position.x / tileDisplacement.x);
        IndexInGrid.y = (int)(transform.position.z / tileDisplacement.y);
    }

    private void Start()
    {
        AddObjectsToTile();
    }

    /// <summary>
    /// adds the entities and hazards to the tile, updates the grid manager with positions
    /// </summary>
    private void AddObjectsToTile() {
        int eType = -1;

        //spawns an Entity if theres one to spawn
        if (TileHasEntities && entityObject != null) {
            print("Spawning");
            GameObject obj = Instantiate(entityObject, transform.position, Quaternion.identity);

            // if the entity has a gridpathfinding componet
            if (obj.GetComponent<GridPathfinding>() != null)
            {
                obj.GetComponent<GridPathfinding>().MyPosition = IndexInGrid;
            }

            switch (entityType)
            {
                case EntityType.Enemy:
                    eType = -2;
                    break;
                case EntityType.Player:
                    eType = -3;
                    break;
                case EntityType.Obstacle:
                    eType = -4;
                    break;
            }
            GridManager.AddEntity(IndexInGrid, eType);
        }

        //spawns a hazard if theres one to spawn
        if (TileHasHazards && hazardObject != null)
        {
            GameObject obj = Instantiate(hazardObject, transform.position, Quaternion.identity);
        }
    }

    /// <summary>
    /// Sets an entity to be a child of a tile
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider collision)
    {
        print("added " + collision.name + " to " + gameObject.name);
        collision.transform.SetParent(transform);
    }
}