/******************************************************************************
 * Author: Brad Dixon, Tyler Bouchard
 * Creation Date: 10/2/2025
 * Last Modified: 2/9/2026 (Brad Dixon)
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
        Obstacle,
        Pip
    }
    private enum HazardType
    {
        block,
        damage,
        slow 
    }

    [Header("Tile Info")] 
    public Vector2Int IndexInGrid;
    [HideInInspector] public bool inPlayerRange;
    [HideInInspector] public int entityOnGrid;
    [HideInInspector] private GameObject ObjectOnTile;
    [HideInInspector] private GameObject tileHighlight;
    [SerializeField] private TileType tileType;
    
    [Header("Water Tile Vars")]
    [SerializeField, ShowIf(nameof(tileType), TileType.Water)] private bool isElectrified;
    [HideInInspector, ShowIf(nameof(tileType), TileType.Water)] private int turnsSinceElectrification;
    [SerializeField, ShowIf(nameof(tileType), TileType.Water)] private int damageWhenElectrified;
    [SerializeField, ShowIf(nameof(tileType), TileType.Water)] private int electrificationDuration;
    

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
        //Transform parentTransform = transform.parent.gameObject.transform;
        //IndexInGrid.x = (int)(transform.position.x - parentTransform.position.x / transform.localScale.x);
        //IndexInGrid.y = (int)(transform.position.z - parentTransform.position.z / transform.localScale.z);

        gameObject.name = "[" + IndexInGrid.x + ", " + IndexInGrid.y + "]";
        tileHighlight = transform.GetChild(0).gameObject;

        inPlayerRange = false;
    }

    /// <summary>
    /// Spawns entities on start
    /// </summary>
    private void Start()
    {
        //AddObjectsToTile();
        tileHighlight.SetActive(false);
        //Invoke("AddObjectsToTile", 1.5f);
    }

    /// <summary>
    /// adds the entities and hazards to the tile, updates the grid manager with positions
    /// </summary>
    public void AddObjectsToTile() {
        int eType = -1;

        //spawns an Entity if theres one to spawn
        if (TileHasEntities && entityObject != null) {
            GameObject obj = Instantiate(entityObject, transform.position, Quaternion.identity, transform);

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
            GameObject obj = Instantiate(hazardObject, transform);
        }
    }

    /// <summary>
    /// This is what should be called whenever the tile is struck with electricity
    /// </summary>
    public void ElectrifyTile()
    {
        if (tileType == TileType.Water) 
        {
            isElectrified = true;
            turnsSinceElectrification = 0;
        }
    }

    /// <summary>
    /// Finds all the connected water tiles so they all get electrified at the same time
    /// </summary>
    public void ElectrifyAdTiles()
    {
        List<TileBehaviour> adWaterTiles = new List<TileBehaviour>();
        List<Vector2Int> alreadyChecked = new List<Vector2Int>();
        adWaterTiles.Add(GridManager.combatGrid[IndexInGrid.x, IndexInGrid.y]);
        alreadyChecked.Add(IndexInGrid);

        List<Vector2Int> adTiles = GridManager.GetAllAdjacentTiles(IndexInGrid);
        //Gets the adjacent tiles of the tile that was hit
        foreach (Vector2Int v in adTiles)
        {
            if (GridManager.combatGrid[v.x, v.y].CanBeElectrified())
            {
                adWaterTiles.Add(GridManager.combatGrid[v.x, v.y]);
            }
            alreadyChecked.Add(v);
        }

        bool foundAll = false;
        //Loops until a new tile doesn't get added
        while(!foundAll)
        {
            List<Vector2Int> temp = new List<Vector2Int>();
            List<Vector2Int> temp2 = new List<Vector2Int>();
            foundAll = true;
            List<Vector2Int> adAdTiles = new List<Vector2Int>();

            //Gets the adjacent tiles of already electrified ones
            foreach(Vector2Int a1 in adTiles)
            {
                adAdTiles = GridManager.GetAllAdjacentTiles(a1);

                foreach (Vector2Int v in adAdTiles)
                {
                    if (!alreadyChecked.Contains(v))
                    {
                        if(foundAll)
                        {
                            foundAll = false;
                        }
                        if (GridManager.combatGrid[v.x, v.y].CanBeElectrified())
                        {
                            adWaterTiles.Add(GridManager.combatGrid[v.x, v.y]);

                            temp2 = GridManager.GetAllAdjacentTiles(v);
                            foreach (Vector2Int t in temp2)
                            {
                                temp.Add(t);
                            }
                        }
                        alreadyChecked.Add(v);
                    }
                }
            }
            adTiles.Clear();
            foreach(Vector2Int v in temp)
            {
                adTiles.Add(v);
            }
        }

        //Once all connected tiles are found, electrify them all
        foreach(TileBehaviour t in adWaterTiles)
        {
            t.ElectrifyTile();
        }
    }

    /// <summary>
    /// Public check to see if a tile is able to be electrified
    /// </summary>
    /// <returns></returns>
    public bool CanBeElectrified()
    {
        return tileType == TileType.Water;
    }

    /// <summary>
    /// Public call so tile effects can be applied before a turn ends
    /// </summary>
    public void ApplyTileEffects() {
        if (hazardType == HazardType.damage && TileHasHazards)
        {
            DamageEntity(damageAmount);
        }

        if (tileType == TileType.Water && isElectrified) 
        {
            DamageEntity(damageWhenElectrified);
        }
    }

    /// <summary>
    /// Has tiles do specific things when a turn is ended
    /// </summary>
    private void EndTurnTileEffects()
    {
        if (hazardType == HazardType.damage && TileHasHazards)
        {
            DamageEntity(damageAmount);
        }

        if (tileType == TileType.Water && isElectrified)
        {
            DamageEntity(damageWhenElectrified);
            turnsSinceElectrification++;
            if (turnsSinceElectrification >= electrificationDuration)
            {
                isElectrified = false;
                turnsSinceElectrification = 0;
            }
        }
        TurnPublicEvents.TurnActionComplete();
    }

    /// <summary>
    /// Checks if a tile can apply its effects
    /// </summary>
    /// <returns></returns>
    public bool CanApplyTileEffects()
    {
        if (hazardType == HazardType.damage && TileHasHazards)
        {
            return true;
        }
        if (tileType == TileType.Water && isElectrified)
        {
            return true;
        }
        return false;
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
        color.a = .5f;
        tileHighlight.GetComponent<SpriteRenderer>().color = color;
    }

    /// <summary>
    /// applys the damage to the entities
    /// </summary>
    /// <param name="amount"></param>
    private void DamageEntity(int amount) {
        //calls the player damage
        if (ObjectOnTile != null) {
            if (ObjectOnTile.GetComponent<PlayerBehavior>() != null)
            {
                FindFirstObjectByType<PlayerStats>().TakeDamage(amount);
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
        TurnPublicEvents.BeginEndTurn += EndTurnTileEffects;
    }

    /// <summary>
    /// Used so it doesn't listen to a null reference of unity events
    /// </summary>
    private void OnDisable()
    {
        TurnPublicEvents.BeginEndTurn -= EndTurnTileEffects;
    }

    public void AddPip(GameObject pip)
    {
        Vector3 pos = transform.position;
        pos.y = pos.y + 1;
        Instantiate(pip, pos, Quaternion.identity, transform);
        GridManager.AddEntity(IndexInGrid, -5);
    }
}