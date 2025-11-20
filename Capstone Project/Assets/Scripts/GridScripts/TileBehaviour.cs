/******************************************************************************
 * Author: Brad Dixon, Tyler Bouchard
 * Creation Date: 10/2/2025
 * Last Modified: 11/2/2025 (Tyler Bouchard)
 * Brief: Stores the tile's index in the grid to help with player movement and
 * stores information about what kind of tile it is
 * External Resources: N/A
 * ***************************************************************************/
using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.Animations;

public class TileBehaviour : MonoBehaviour
{
    private enum TileType
    {
        Default,
        Water
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
    [HideInInspector] public Vector2Int IndexInGrid;
    [HideInInspector] public bool inPlayerRange;
    [HideInInspector] public int entityOnGrid;
    [HideInInspector] private GameObject ObjectOnTile;
    [SerializeField] private TileType tileType;
    [SerializeField] private GameObject tileHighlight;
    
    [Header("Water Tile Vars")]
    [SerializeField, ShowIf(nameof(tileType), TileType.Water)] private GameObject WaterTileVisualizer;
    [SerializeField, ShowIf(nameof(tileType), TileType.Water)] private bool isElectrified;
    [SerializeField, ShowIf(nameof(tileType), TileType.Water)] private int damageWhenElectrified;
    [SerializeField, ShowIf(nameof(tileType), TileType.Water)] private int electrificationDuration;
    [SerializeField, ShowIf(nameof(tileType), TileType.Water)] private int turnsSinceElectrification;

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
        //finding the tile coords
        Transform parentTransform = transform.parent.gameObject.transform;
        IndexInGrid.x = (int)(transform.position.x - parentTransform.position.x / transform.localScale.x);
        IndexInGrid.y = (int)(transform.position.z - parentTransform.position.z / transform.localScale.z);

        //renaming the tile for easier debuging and such
        gameObject.name = "[" + IndexInGrid.x + ", " + IndexInGrid.y + "]";

        inPlayerRange = false;
    }

    /// <summary>
    /// Spawns entities on start
    /// </summary>
    private void Start()
    {
        //AddObjectsToTile();
        tileHighlight.SetActive(false);
        Invoke("AddObjectsToTile", 1.5f);
    }

    /// <summary>
    /// adds the entities and hazards to the tile, updates the grid manager with positions
    /// </summary>
    public void AddObjectsToTile() {
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

        if (tileType == TileType.Water) {
            GameObject obj = Instantiate(WaterTileVisualizer, transform.position, Quaternion.identity);
        }
    }

    /// <summary>
    /// This is what should be called whenever the tile is struck with electricity
    /// </summary>
    public void ElectrifyTile()
    {
        if (tileType == TileType.Water) {
            isElectrified = true;
            turnsSinceElectrification = 0;
        }
    }

    /// <summary>
    /// applys all the effects that the tile should deal out to whatever is on it durring the tiles turn
    /// </summary>
    public void ApplyTileEffects() {
        if (hazardType == HazardType.damage)
        {
            DamageEntity(damageAmount);
        }

        if (tileType == TileType.Water && isElectrified) {
            DamageEntity(damageWhenElectrified);
            turnsSinceElectrification++;
            if (turnsSinceElectrification >= electrificationDuration) { 
                isElectrified = false;
            }
        }
        TurnPublicEvents.TurnActionComplete();
    }

    /// <summary>
    /// turns the highlight of the tile on or off
    /// </summary>
    public void ShowHighlight(bool active) {
        tileHighlight.SetActive(active);
    }

    /// <summary>
    /// changes the color of the highlight
    /// </summary>
    /// <param name="color"></param>
    public void SetHighlightColor(Color color)
    {
        tileHighlight.GetComponent<SpriteRenderer>().color = color;
    }

    /// <summary>
    /// applys the damage to the entities
    /// </summary>
    /// <param name="amount"></param>
    private void DamageEntity(int amount) {
        //calls the player damage
        if (ObjectOnTile != null) {
            if (ObjectOnTile.GetComponent<PlayerStats>() != null)
            {
                ObjectOnTile.GetComponent<PlayerStats>().TakeDamage(amount);
            }

            //calls the enemy damage
            if (ObjectOnTile.GetComponent<MeleeEnemy>() != null)
            {
                ObjectOnTile.GetComponent<MeleeEnemy>().Damage(amount);
            }
        }
    }

    /// <summary>
    /// Sets an entity to be a child of a tile
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider collision)
    {
        collision.transform.SetParent(transform);
        ObjectOnTile = collision.gameObject;
    }

    /// <summary>
    /// Lets the tile listen for unity events
    /// </summary>
    private void OnEnable()
    {
        TurnPublicEvents.BeginEndTurn += ApplyTileEffects;
    }

    ///// <summary>
    ///// Used so it doesn't listen to a null reference of unity events
    ///// </summary>
    private void OnDisable()
    {
        TurnPublicEvents.BeginEndTurn -= ApplyTileEffects;
    }
}