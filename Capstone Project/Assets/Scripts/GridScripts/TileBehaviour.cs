/******************************************************************************
 * Author: Brad Dixon, Tyler Bouchard
 * Creation Date: 10/2/2025
 * Last Modified: 10/22/2025 (Tyler Bouchard)
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
    private enum TileType
    {
        Default,
        Water,
    }
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

    [Header("Tile Info")]
    public GameObject objectOnTile = null;
    public Vector2Int IndexInGrid;

    [SerializeField, Tooltip("How far a tile's transform must move in order to be adjacent to another tile")]
    private Vector2 tileDisplacement;

    [SerializeField] TileType tileType;
    
    //Water Tile Variables
    [SerializeField, ShowIf("tileType", TileType.Water)] private int damageWhenElectric;
    [ShowIf("tileType", TileType.Water)] public bool isElectrified;
    [ShowIf("tileType", TileType.Water)] public int electrificationEffectDuration;
    [ShowIf("tileType", TileType.Water)] public int turnsSinceElectrification;


    [Header("Objects On This Tile")]
    [SerializeField] private bool TileHasEntities = false;
    [SerializeField, ShowIf(nameof(TileHasEntities)), Foldout("Entities")] private EntityType entityType;
    [SerializeField, ShowIf(nameof(TileHasEntities)), Foldout("Entities")] private GameObject entityObject;

    [SerializeField] private bool TileHasHazards = false;
    [SerializeField, ShowIf(nameof(TileHasHazards)), Foldout("Hazards")] private HazardType hazardType;
    [SerializeField, ShowIf(nameof(TileHasHazards)), Foldout("Hazards")] private GameObject hazardObject;
    [SerializeField, ShowIf(nameof(ShowDamageVars)), Foldout("Hazards")] private int damageAmount;
    [SerializeField, ShowIf(nameof(ShowSlowVars)), Foldout("Hazards")] private int movesLost;

    /// <summary>
    /// checker for the showif function
    /// </summary>
    /// <returns></returns>
    private bool ShowDamageVars() {
        return hazardType == HazardType.damage && TileHasHazards;
    }
    /// <summary>
    /// checker for the showif function
    /// </summary>
    /// <returns></returns>
    private bool ShowSlowVars()
    {
        return hazardType == HazardType.slow && TileHasHazards;
    }

    //IDK what this is, it isnt used but causes errors in another script when removed so it gets to stay -Tyler B
    [HideInInspector] public List<StatsOnTile> tileStatAffects = new List<StatsOnTile>();

    /// <summary>
    /// Calculates the tile's index based off the transform and the tileDisplacement variable
    /// </summary>
    private void Awake()
    {
        IndexInGrid.x = (int)(transform.position.x / tileDisplacement.x);
        IndexInGrid.y = (int)(transform.position.z / tileDisplacement.y);
    }

    /// <summary>
    /// all this does right now is call the add objects to tile function
    /// </summary>
    private void Start()
    {
        AddObjectsToTile();
        //TurnPublicEvents.BeginStartTurn.AddListener(ApplyTileEffects);
    }

    /// <summary>
    /// adds the entities and hazards to the tile, updates the grid manager with positions
    /// </summary>
    private void AddObjectsToTile() {
        int eType = -1;

        //spawns an Entity if theres one to spawn
        if (TileHasEntities && entityObject != null) {
            GameObject obj = Instantiate(entityObject, transform.position, Quaternion.identity);

            // if the entity has a gridpathfinding componet
            if (obj.GetComponent<GridPathfinding>() != null)
            {
                obj.GetComponent<GridPathfinding>().MyPosition = IndexInGrid;
            }
            
            
            if (entityType == EntityType.Enemy) 
            {
                eType = -2;
            } 
            else if (entityType == EntityType.Player) 
            {
                eType = -3;
            }
            else if(entityType == EntityType.Obstacle)
            {
                eType = -4;
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
    /// makes the tile electrifyed
    /// </summary>
    public void ElectrifyTile() {
        if (tileType == TileType.Water) {
            isElectrified = true;
            turnsSinceElectrification = 0;
        } 
    }

    /// <summary>
    /// applys all of the effects that the tile is suposed to dish out when called
    /// </summary>
    public void ApplyTileEffects() {
        //damage hazard effect
        if (hazardType == HazardType.damage)
        {
            DealDamageToEntity(damageAmount);
        }

        //electrifyed water effect
        if (tileType == TileType.Water && isElectrified)
        {
            DealDamageToEntity(damageWhenElectric);
            turnsSinceElectrification++;
            if (turnsSinceElectrification >= electrificationEffectDuration) { 
                isElectrified = false; 
            }
        }
    }
    /// <summary>
    /// Deals the damage to the object on the tile
    /// </summary>
    /// <param name="amount"></param>
    private void DealDamageToEntity(int amount) {
        if (objectOnTile != null) {
            if (objectOnTile.GetComponent<PlayerStats>() != null)
            {
                objectOnTile.GetComponent<PlayerStats>().TakeDamage(amount);
            }
            if (objectOnTile.GetComponent<MeleeEnemy>() != null)
            {
                objectOnTile.GetComponent<MeleeEnemy>().Damage(amount);
            }
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
        objectOnTile = collision.gameObject;
    }
}