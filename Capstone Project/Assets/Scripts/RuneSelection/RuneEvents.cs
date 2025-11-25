/*************************************************
Author Names : 	Jay Embry
Date Created : 	10/07/2025
Date Last Modified : 11/23/2025
Brief Description : Contains rune types and effects
External Resources : 	
	***************************************************/

using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
using EventReference = FMODUnity.EventReference;

public class RuneEvents : MonoBehaviour
{

    //VARIABLES

    #region SETUP

    public enum Variables
    {

        ComboVariables,
        VisualsAndDebugging,
        Audio

    }

    [SerializeField] private Variables currentInspectorShowing;

    #endregion SETUP


    #region COMBO VARIABLES

    [HorizontalLine(4, EColor.Red)]

    [Header("Lightning/Wind")]

    [Space(10)]

    [ShowIf(nameof(currentInspectorShowing), Variables.ComboVariables), SerializeField]
    int lightningDamageTierOne;

    [ShowIf(nameof(currentInspectorShowing), Variables.ComboVariables), SerializeField]
    int lightningMasteredDamageTierOne;

    [ShowIf(nameof(currentInspectorShowing), Variables.ComboVariables), SerializeField]
    int windPrimaryDamageTierOne;

    [ShowIf(nameof(currentInspectorShowing), Variables.ComboVariables), SerializeField]
    int windSecondaryDamageTierOne;

    [ShowIf(nameof(currentInspectorShowing), Variables.ComboVariables), SerializeField]
    int windMasteredTempHealthTierOne;


    [Space(10)]

    [ShowIf(nameof(currentInspectorShowing), Variables.ComboVariables), SerializeField]
    int lightningDamageTierTwo;

    [ShowIf(nameof(currentInspectorShowing), Variables.ComboVariables), SerializeField]
    int lightningMasteredDamageTierTwo;

    [ShowIf(nameof(currentInspectorShowing), Variables.ComboVariables), SerializeField]
    int windPrimaryDamageTierTwo;

    [ShowIf(nameof(currentInspectorShowing), Variables.ComboVariables), SerializeField]
    int windSecondaryDamageTierTwo;

    [ShowIf(nameof(currentInspectorShowing), Variables.ComboVariables), SerializeField]
    int windMasteredTempHealthTierTwo;


    [Space(10)]

    [ShowIf(nameof(currentInspectorShowing), Variables.ComboVariables), SerializeField]
    int lightningDamageTierThree;

    [ShowIf(nameof(currentInspectorShowing), Variables.ComboVariables), SerializeField]
    int lightningMasteredDamageTierThree;

    [ShowIf(nameof(currentInspectorShowing), Variables.ComboVariables), SerializeField]
    int windPrimaryDamageTierThree;

    [ShowIf(nameof(currentInspectorShowing), Variables.ComboVariables), SerializeField]
    int windSecondaryDamageTierThree;

    [ShowIf(nameof(currentInspectorShowing), Variables.ComboVariables), SerializeField]
    int windMasteredTempHealthTierThree;

    #endregion COMBO VARIABLES


    #region AUDIO

    [HorizontalLine(4, EColor.Orange)]

    //Event reference for sound
    [ShowIf(nameof(currentInspectorShowing), Variables.Audio), SerializeField]
    private EventReference lightningSpellCastedSFX;

    [ShowIf(nameof(currentInspectorShowing), Variables.Audio), SerializeField]
    private GameObject audioListenerObject;

    #endregion AUDIO


    #region VISUALS AND DEBUGGING

    [HorizontalLine(4, EColor.Yellow)]

    //for menu-swapping purposes
    [ShowIf(nameof(currentInspectorShowing), Variables.VisualsAndDebugging), SerializeField]
    GameObject playerMenu;

    [Header("Temporary Textboxes")]

    //early testing stuff
    [ShowIf(nameof(currentInspectorShowing), Variables.VisualsAndDebugging), SerializeField]
    TMP_Text debugText;

    [ShowIf(nameof(currentInspectorShowing), Variables.VisualsAndDebugging), SerializeField]
    TMP_Text debugComboText;

    #endregion VISUALS AND DEBUGGING


    //PREPARING SPELL

    #region INITIALIZATION

    //for waiting on player input
    bool waitingForThePlayer;

    //Stores the currently using rune
    private RuneData storedData;

    //updated everytime the player selects a spell
    public List<TileBehaviour> tilesInRange = new List<TileBehaviour>();

    /// <summary>
    /// Runs whenever this script is loaded into a scene
    /// </summary>
    private void OnEnable()
    {

        PublicEvents.SelectTarget += TargetSelection;
        PublicEvents.RuneSelected += StoreSelectedRuneData;

        PublicEvents.MasteryRunePurchased += MasteryUnlocked;

    }

    /// <summary>
    /// Runs whenever this script is destroyed
    /// </summary>
    private void OnDisable()
    {

        PublicEvents.SelectTarget -= TargetSelection;
        PublicEvents.RuneSelected -= StoreSelectedRuneData;

        PublicEvents.MasteryRunePurchased -= MasteryUnlocked;

    }

    private bool lightningMastered;
    private bool windMastered;

    /// <summary>
    /// indicates when the third tier of a spell has been unlocked
    /// </summary>
    /// <param name="runeType"> type of rune unlocked </param>
    void MasteryUnlocked(RuneType runeType)
    {

        switch (runeType)
        {

            case (RuneType.Lightning):

                lightningMastered = true;

                break;

            case (RuneType.Wind):

                windMastered = true;

                break;

            default:

                break;

        }

        Debug.Log(runeType + " mastery unlocked!");

    }

    #endregion INITIALIZATION


    #region TARGETING FUNCTIONS

    /// <summary>
    /// Prepares the rune that the player chooses to attack with
    /// </summary>
    /// <param name="rd"> Rune Data </param>
    public void StoreSelectedRuneData(RuneData rd)
    {

        waitingForThePlayer = true;

        storedData = rd;

        if (debugText != null)
        {

            debugText.text = "Waiting on a target...";

        }

        RangeCheck(false);

    }

    /// <summary>
    /// exits attack menu if waiting on a target
    /// </summary>
    public void CancelCasting()
    {

        if (waitingForThePlayer)
        {

            waitingForThePlayer = false;

            if (debugText != null)
            {

                debugText.text = "";

            }

        }



    }

    /// <summary>
    /// checks to see if a tile has literally anything other than an obstacle
    /// thanks brad
    /// </summary>
    /// <param name="tileCoordinates"> the tile that the player has selected </param>
    /// <returns> whether or not the tile can be targeted </returns>
    public static bool CanAttackTile(Vector2Int tileCoordinates)
    {

        if (tileCoordinates == GridManager.playerPosition)
        {
            return true;
        }

        return GridManager.combatGrid[tileCoordinates.x, tileCoordinates.y].entityOnGrid == -1 ||
            GridManager.combatGrid[tileCoordinates.x, tileCoordinates.y].entityOnGrid == -2 ||
            GridManager.combatGrid[tileCoordinates.x, tileCoordinates.y].entityOnGrid == -3;

    }

    /// <summary>
    /// creates a list of all targetable tiles upon selecting a spell
    /// </summary>
    /// <param name="currentTile"> the tile that the player has selected </param>
    /// <returns> list of targetable tiles </returns>
    public static List<Vector2Int> GetAllValidTargetableTiles(Vector2Int currentTile)
    {
        List<Vector2Int> validTiles = new List<Vector2Int>();

        if (GridManager.TileIsInGrid(new Vector2Int(currentTile.x + 1, currentTile.y)) && CanAttackTile(new Vector2Int(currentTile.x + 1, currentTile.y)))
        {
            validTiles.Add(new Vector2Int(currentTile.x + 1, currentTile.y));
        }
        if (GridManager.TileIsInGrid(new Vector2Int(currentTile.x - 1, currentTile.y)) && CanAttackTile(new Vector2Int(currentTile.x - 1, currentTile.y)))
        {
            validTiles.Add(new Vector2Int(currentTile.x - 1, currentTile.y));
        }
        if (GridManager.TileIsInGrid(new Vector2Int(currentTile.x, currentTile.y + 1)) && CanAttackTile(new Vector2Int(currentTile.x, currentTile.y + 1)))
        {
            validTiles.Add(new Vector2Int(currentTile.x, currentTile.y + 1));
        }
        if (GridManager.TileIsInGrid(new Vector2Int(currentTile.x, currentTile.y - 1)) && CanAttackTile(new Vector2Int(currentTile.x, currentTile.y - 1)))
        {
            validTiles.Add(new Vector2Int(currentTile.x, currentTile.y - 1));
        }

        return validTiles;
    }

    /// <summary>
    /// checks to see if a tile or enemy is in range before executing a spell
    /// this could be cleaner but i can worry about that later
    /// </summary>
    /// <param name="isRadiusCheck"> determines where the function is checking adjacent tiles from </param>
    /// <param name="radius"> a spell's potential radius/aoe </param>
    /// <param name="target"> the initial tile hit if isRadiusCheck </param>
    public void RangeCheck(bool isRadiusCheck, int radius = 0, TileBehaviour target = null)
    {

        tilesInRange.Clear();

        List<Vector2Int> validTiles = new List<Vector2Int>();

        if (!isRadiusCheck)
        {

            tilesInRange.Add(GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y]);

            for (int i = 0; i < storedData.RuneRange + 1; i++)
            {

                if (i == 1)
                {

                    List<Vector2Int> initialAdjacentTiles = GetAllValidTargetableTiles(GridManager.playerPosition);

                    foreach (Vector2Int tile in initialAdjacentTiles.ToList())
                    {

                        if (tile != GridManager.playerPosition)
                        {

                            validTiles.Add(tile);

                        }

                    }

                    continue;

                }

                foreach (Vector2Int validTile in validTiles.ToList())
                {

                    List<Vector2Int> adjacentTiles = new List<Vector2Int>();

                    adjacentTiles = GetAllValidTargetableTiles(validTile);

                    foreach (Vector2Int tile in adjacentTiles.ToList())
                    {

                        if (validTiles.Contains(tile))
                        {

                            continue;

                        }
                        else if (tile != GridManager.playerPosition)
                        {

                            validTiles.Add(tile);

                        }

                    }

                }

            }

            foreach (Vector2Int tile in validTiles.ToList())
            {

                tilesInRange.Add(GridManager.combatGrid[tile.x, tile.y]);

            }

            validTiles.Clear();

        }
        else
        {

            Vector2Int targetPos = target.IndexInGrid;

            for (int i = 0; i < radius + 1; i++)
            {

                if (i == 1)
                {

                    List<Vector2Int> initialAdjacentTiles = GetAllValidTargetableTiles(targetPos);

                    foreach (Vector2Int tile in initialAdjacentTiles.ToList())
                    {

                        validTiles.Add(tile);

                    }

                    continue;

                }

                foreach (Vector2Int validTile in validTiles.ToList())
                {

                    List<Vector2Int> adjacentTiles = new List<Vector2Int>();

                    adjacentTiles = GetAllValidTargetableTiles(validTile);

                    foreach (Vector2Int tile in adjacentTiles.ToList())
                    {

                        if (validTiles.Contains(tile))
                        {

                            continue;

                        }
                        else
                        {

                            validTiles.Add(tile);

                        }

                    }

                }

            }

            foreach (Vector2Int tile in validTiles.ToList())
            {

                tilesInRange.Add(GridManager.combatGrid[tile.x, tile.y]);

            }

            validTiles.Clear();

        }

    }

    /// <summary>
    /// triggers spells based on the tile or enemy that the player has selected
    /// </summary>
    /// <param name="tile"> the tile that the player has selected </param>
    /// <param name="enemy"> the enemy that the player has selected </param>
    /// <param name="player"> for when the player is targeting themself, for whatever reason </param>
    public void TargetSelection(TileBehaviour tile, Enemy enemy, PlayerBehavior player)
    {

        if(waitingForThePlayer &&
            FindFirstObjectByType<GameManager>().CurrentActionPoints >= storedData.RuneActionPoints &&
            tilesInRange.Contains(tile))
        {

            switch (storedData.TypeOfRune)
            {

                case (RuneType.Lightning):

                    SelectLightningRune(tile, enemy, player);
                    break;

                case (RuneType.Wind):

                    SelectWindRune(tile, enemy, player);
                    break;

                default:

                    break;

            }

        }

    }

    #endregion TARGETING FUNCTIONS


    //SPELL FUNCTIONS

    #region LIGHTNING FUNCTIONS

    /// <summary>
    /// Calls lightning rune effect
    /// </summary>
    /// <param name="tile"> tile that the player has selected </param>
    /// <param name="enemy"> enemy that the player has selected </param>
    /// <param name="player"> when the player has selected themself </param>
    public void SelectLightningRune(TileBehaviour tile, Enemy enemy, PlayerBehavior player)
    {

        float damageDealt = Mathf.CeilToInt(storedData.RuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier
            * FindFirstObjectByType<PlayerStats>().BaseAttackMultiplier);

        Enemy[] enemiesOnTheGrid = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        GameObject VFX;

        switch (storedData.NumberOnSkillTree)
        {

            //targets one opponent for moderate damage
            case (1):

                if (enemy != null)
                {

                    enemy.Damage(damageDealt);

                    CheckRuneCombination(enemy);

                    tile.ElectrifyTile();

                    //AudioManager.instance.CreateEventInstance(lightningSpellCastedSFX);
                    //AudioManager.instance.PlayOneShot(lightningSpellCastedSFX, audioListenerObject.transform.position);

                    VFX = Instantiate(storedData.RuneVFX, tile.transform);

                    EndPlayerAttackPhase();

                }

                break;

            //targets two opponents
            //one is directly targeted, and the other is the closest to the original target
            case (2):

                if (enemy != null)
                {

                    FindSecondaryTarget(tile);

                    enemy.Damage(damageDealt);

                    CheckRuneCombination(enemy);

                    tile.ElectrifyTile();

                    //AudioManager.instance.PlayOneShot(lightningSpellCastedSFX, audioListenerObject.transform.position);

                    VFX = Instantiate(storedData.RuneVFX, tile.transform);

                    if (secondaryTarget != null)
                    {

                        secondaryTarget.Damage(damageDealt);

                        GridManager.combatGrid[(int)secondaryTarget.transform.position.x, (int)secondaryTarget.transform.position.z].ElectrifyTile();

                        //AudioManager.instance.CreateEventInstance(lightningSpellCastedSFX);
                        //AudioManager.instance.PlayOneShot(lightningSpellCastedSFX, audioListenerObject.transform.position);

                        if (storedData.SecondaryRuneVFX != null)
                        {

                            VFX = Instantiate(storedData.SecondaryRuneVFX, secondaryTarget.transform);

                        }
                        else
                        {

                            VFX = Instantiate(storedData.RuneVFX, secondaryTarget.transform);

                        }

                        secondaryTarget = null;

                    }

                    EndPlayerAttackPhase();

                }

                break;

            //targets one opponent and all other opponents in range for less damage
            case (3):


                if (enemy != null)
                {

                    enemy.Damage(damageDealt);

                    CheckRuneCombination(enemy);

                    tile.ElectrifyTile();

                    //AudioManager.instance.CreateEventInstance(lightningSpellCastedSFX);
                    //AudioManager.instance.PlayOneShot(lightningSpellCastedSFX, audioListenerObject.transform.position);

                    VFX = Instantiate(storedData.RuneVFX, tile.transform);

                    RangeCheck(true, 3, tile);

                    foreach (TileBehaviour tileInRange in tilesInRange)
                    {

                        foreach (Enemy newEnemy in enemiesOnTheGrid)
                        {

                            if ((int)newEnemy.transform.position.x == tileInRange.IndexInGrid.x && (int)newEnemy.transform.position.z == tileInRange.IndexInGrid.y)
                            {

                                newEnemy.Damage(Mathf.CeilToInt(0.40f * damageDealt));

                                tileInRange.ElectrifyTile();

                                if (storedData.SecondaryRuneVFX != null)
                                {

                                    VFX = Instantiate(storedData.SecondaryRuneVFX, tileInRange.transform);

                                }
                                else
                                {

                                    VFX = Instantiate(storedData.RuneVFX, tileInRange.transform);

                                }

                                break;

                            }

                        }

                    }

                    EndPlayerAttackPhase();

                }

                break;

            //targets opponents in a straight line
            case (4):

                if (player != null || Mathf.Abs(GridManager.playerPosition.x - tile.transform.position.x) > 1 ||
                    Mathf.Abs(GridManager.playerPosition.y - tile.transform.position.z) > 1)
                {

                    return;

                }

                FindLineOfTargets(tile);

                foreach (TileBehaviour potentialTarget in potentialTargetsForLightningStrikes)
                {

                    VFX = Instantiate(storedData.RuneVFX, potentialTarget.transform);

                    Debug.Log(potentialTarget + "HAS BEEN HIT");

                    potentialTarget.ElectrifyTile();

                    foreach (Enemy newEnemy in enemiesOnTheGrid)
                    {

                        if ((int)newEnemy.transform.position.x == potentialTarget.IndexInGrid.x &&
                            (int)newEnemy.transform.position.z == potentialTarget.IndexInGrid.y)
                        {

                            newEnemy.Damage(damageDealt);

                            CheckRuneCombination(newEnemy);

                            break;

                        }

                    }

                }

                EndPlayerAttackPhase();

                break;

            default:

                break;
        }

    }

    //variable that stores the enemy that's closest to the target
    //for lightning 2a
    Enemy secondaryTarget;

    /// <summary>
    /// Calls lightning rune effect
    /// </summary>
    /// <param name="target"> tile that the player has selected </param>
    /// <returns> the second target that lightning 2a will hit </returns>
    Enemy FindSecondaryTarget(TileBehaviour target)
    {

        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        float closestDistance = Mathf.Infinity;
        Vector3 primaryTargetPos = target.gameObject.transform.position;

        foreach (Enemy enemy in enemies)
        {

            if ((int)enemy.transform.position.x == target.IndexInGrid.x && (int)enemy.transform.position.z == target.IndexInGrid.y)
            {

                continue;

            }

            Vector3 dir = enemy.gameObject.transform.position - primaryTargetPos;
            float distanceFromTarget = dir.sqrMagnitude;

            if (distanceFromTarget < closestDistance)
            {

                closestDistance = distanceFromTarget;
                secondaryTarget = enemy;

            }

        }

        return secondaryTarget;

    }


    //variable that stores the straight line necessary for lightning 3
    List<TileBehaviour> potentialTargetsForLightningStrikes = new List<TileBehaviour>();

    /// <summary>
    /// called when lightning 3 is cast
    /// finds opponents in a straight line relative to the player's position and the initial target
    /// ngl i don't think that i'm seeing the light of heaven for this but it works
    /// </summary>
    /// <param name="initialTarget"> initial target that the player had picked out </param>
    /// <returns> a list of tiles for the spell to target </returns>
    List<TileBehaviour> FindLineOfTargets(TileBehaviour initialTarget)
    {

        potentialTargetsForLightningStrikes.Clear();

        foreach (TileBehaviour tile in GridManager.combatGrid)
        {

            if (GridManager.playerPosition.x != initialTarget.transform.position.x && GridManager.playerPosition.y == initialTarget.transform.position.z)
            {

                if (GridManager.playerPosition.x < initialTarget.transform.position.x &&
                    tile.transform.position.x > GridManager.playerPosition.x &&
                    tile.transform.position.z == GridManager.playerPosition.y &&
                    Mathf.Abs(tile.transform.position.x - GridManager.playerPosition.x) <= storedData.RuneRange)
                {

                    potentialTargetsForLightningStrikes.Add(tile);

                }

                if (GridManager.playerPosition.x > initialTarget.transform.position.x &&
                    tile.transform.position.x < GridManager.playerPosition.x &&
                    tile.transform.position.z == GridManager.playerPosition.y &&
                    Mathf.Abs(tile.transform.position.x - GridManager.playerPosition.x) <= storedData.RuneRange)
                {

                    potentialTargetsForLightningStrikes.Add(tile);

                }

            }

            if (GridManager.playerPosition.x == initialTarget.transform.position.x && GridManager.playerPosition.y != initialTarget.transform.position.z)
            {

                if (GridManager.playerPosition.y < initialTarget.transform.position.z &&
                    tile.transform.position.z > GridManager.playerPosition.y &&
                    tile.transform.position.x == GridManager.playerPosition.x &&
                    Mathf.Abs(tile.transform.position.z - GridManager.playerPosition.y) <= storedData.RuneRange)
                {

                    potentialTargetsForLightningStrikes.Add(tile);

                }

                if (GridManager.playerPosition.y > initialTarget.transform.position.z &&
                    tile.transform.position.z < GridManager.playerPosition.y &&
                    tile.transform.position.x == GridManager.playerPosition.x &&
                    Mathf.Abs(tile.transform.position.z - GridManager.playerPosition.y) <= storedData.RuneRange)
                {

                    potentialTargetsForLightningStrikes.Add(tile);

                }

            }

            if (GridManager.playerPosition.x != initialTarget.transform.position.x && GridManager.playerPosition.y != initialTarget.transform.position.z)
            {

                if (GridManager.playerPosition.x < initialTarget.transform.position.x &&
                    tile.transform.position.x > GridManager.playerPosition.x &&
                    GridManager.playerPosition.y < initialTarget.transform.position.z &&
                    tile.transform.position.z > GridManager.playerPosition.y &&
                    Mathf.Abs(tile.transform.position.x - GridManager.playerPosition.x) <= storedData.RuneRange &&
                    Mathf.Abs(tile.transform.position.z - GridManager.playerPosition.y) <= storedData.RuneRange)
                {

                    if ((tile.transform.position.x - GridManager.playerPosition.x) == (tile.transform.position.z - GridManager.playerPosition.y))
                    {

                        potentialTargetsForLightningStrikes.Add(tile);

                    }

                }

                if (GridManager.playerPosition.x > initialTarget.transform.position.x &&
                    tile.transform.position.x < GridManager.playerPosition.x &&
                    GridManager.playerPosition.y > initialTarget.transform.position.z &&
                    tile.transform.position.z < GridManager.playerPosition.y &&
                    Mathf.Abs(tile.transform.position.x - GridManager.playerPosition.x) <= storedData.RuneRange &&
                    Mathf.Abs(tile.transform.position.z - GridManager.playerPosition.y) <= storedData.RuneRange)
                {

                    if ((tile.transform.position.x - GridManager.playerPosition.x) == (tile.transform.position.z - GridManager.playerPosition.y))
                    {

                        potentialTargetsForLightningStrikes.Add(tile);

                    }

                }

                if (GridManager.playerPosition.x < initialTarget.transform.position.x &&
                    tile.transform.position.x > GridManager.playerPosition.x &&
                    GridManager.playerPosition.y > initialTarget.transform.position.z &&
                    tile.transform.position.z < GridManager.playerPosition.y &&
                    Mathf.Abs(tile.transform.position.x - GridManager.playerPosition.x) <= storedData.RuneRange &&
                    Mathf.Abs(tile.transform.position.z - GridManager.playerPosition.y) <= storedData.RuneRange)
                {

                    if ((GridManager.playerPosition.x - tile.transform.position.x) == (tile.transform.position.z - GridManager.playerPosition.y))
                    {

                        potentialTargetsForLightningStrikes.Add(tile);

                    }

                }

                if (GridManager.playerPosition.x > initialTarget.transform.position.x &&
                   tile.transform.position.x < GridManager.playerPosition.x &&
                   GridManager.playerPosition.y < initialTarget.transform.position.z &&
                   tile.transform.position.z > GridManager.playerPosition.y &&
                   Mathf.Abs(tile.transform.position.x - GridManager.playerPosition.x) <= storedData.RuneRange &&
                   Mathf.Abs(tile.transform.position.z - GridManager.playerPosition.y) <= storedData.RuneRange)
                {

                    if ((GridManager.playerPosition.x - tile.transform.position.x) == (GridManager.playerPosition.y - tile.transform.position.z))
                    {

                        potentialTargetsForLightningStrikes.Add(tile);

                    }

                }

            }

        }

        return potentialTargetsForLightningStrikes;

    }

    #endregion LIGHTNING FUNCTIONS


    #region WIND FUNCTIONS

    /// <summary>
    /// Calls wind rune effect
    /// </summary>
    /// <param name="tile"> tile that the player has selected </param>
    /// <param name="enemy"> enemy that the player has selected </param>
    /// <param name="player"> when the player has selected themself </param>
    public void SelectWindRune(TileBehaviour tile, Enemy enemy = null, PlayerBehavior player = null)
    {

        float damageDealt = Mathf.Ceil(storedData.RuneDamage * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier
            * FindFirstObjectByType<PlayerStats>().BaseAttackMultiplier);

        Enemy[] enemiesOnTheGrid = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        GameObject VFX;

        switch (storedData.NumberOnSkillTree)
        {

            //knocks adjacent enemies backwards and damages them
            //ASK DESIGN ABOUT THE RADIUS
            case (1):

                if(player != null)
                {

                    return;

                }

                List<Enemy> enemiesAlreadyPushedBack = new List<Enemy>();

                VFX = Instantiate(storedData.RuneVFX, tile.transform);

                RangeCheck(true, 2, tile);

                foreach (TileBehaviour tileInRange in tilesInRange)
                {

                    foreach (Enemy newEnemy in enemiesOnTheGrid)
                    {

                        if((int)newEnemy.transform.position.x == tileInRange.IndexInGrid.x && (int)newEnemy.transform.position.z == tileInRange.IndexInGrid.y)
                        {

                            newEnemy.Damage(damageDealt);

                            if (tileInRange == tile && !enemiesAlreadyPushedBack.Contains(newEnemy))
                            {

                                newEnemy.SendEnemyBackwards
                                    (GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y], tileInRange);

                                enemiesAlreadyPushedBack.Add(newEnemy);

                                CheckRuneCombination(newEnemy);

                            }

                            break;

                        }

                    }

                }

                EndPlayerAttackPhase();

                break;

            //targets an opponent for moderate damage
            //has a chance to hit twice
            case (2):

                if(enemy != null)
                {

                    enemy.Damage(damageDealt);

                    CheckRuneCombination(enemy);

                    VFX = Instantiate(storedData.RuneVFX, tile.transform);


                    //TODO: redo math based on design input/potential luck stat???
                    if(Random.value <= storedData.RuneSecondaryEffectChance)
                    {

                        enemy.Damage(damageDealt);

                    }

                    EndPlayerAttackPhase();

                }

                break;

            //creates a shield on the player's tile
            case (3):

                ShieldBehavior newShield = tile.gameObject.AddComponent<ShieldBehavior>();

                if (debugText != null)
                {

                    debugText.text = "Shield added!";

                }

                newShield.OnShieldGenerated(tile.transform, storedData.RuneVFX);

                EndPlayerAttackPhase();

                break;

            //delays target's turn and damages surrounding enemies
            case (4):

                if(player != null)
                {

                    return;

                }

                VFX = Instantiate(storedData.RuneVFX, tile.transform);

                if (enemy != null)
                {

                    enemy.DelayedTurnStatus(true);

                    enemy.Damage(damageDealt);

                    CheckRuneCombination(enemy);

                }

                RangeCheck(true, 3, tile);

                //do NOT delete this list again, jay. it's here for a reason
                List<Enemy> validEnemies = new List<Enemy>();

                foreach(TileBehaviour tileInRange in tilesInRange)
                {

                    if (tileInRange == tile)
                    {

                        continue;

                    }

                    foreach (Enemy newEnemy in enemiesOnTheGrid)
                    {

                        if ((int)newEnemy.transform.position.x == tileInRange.IndexInGrid.x && (int)newEnemy.transform.position.z == tileInRange.IndexInGrid.y)
                        {

                            validEnemies.Add(newEnemy);

                            break;

                        }

                    }

                }

                if (validEnemies.Count > 0)
                {

                    for (int i = 0; i < validEnemies.Count; i++)
                    {

                        validEnemies[i].Damage(Mathf.CeilToInt(damageDealt / validEnemies.Count));

                    }

                }

                EndPlayerAttackPhase();

                break;

        }

    } 

    #endregion WIND FUNCTIONS


    #region COMBO FUNCTIONS

    /// <summary>
    /// checks if the enemy has been hit by a spell prior
    /// if so, a combo is triggered
    /// </summary>
    /// <param name="enemy"> target </param>
    void CheckRuneCombination(Enemy enemy)
    {
        if (enemy != null)
        {
            if (!enemy.HasStatusEffect)
            {

                enemy.RuneStatusEffect = storedData.TypeOfRune;
                enemy.RuneStatusEffectNumber = storedData.NumberOnSkillTree;

                enemy.HasStatusEffect = true;

                Debug.Log("Status effect added!");

            }
            else if (enemy.HasStatusEffect && enemy.RuneStatusEffect != storedData.TypeOfRune)
            {

                switch (storedData.TypeOfRune, enemy.RuneStatusEffect)
                {

                    case (RuneType.Lightning, RuneType.Wind):

                        LightningAndWindCombo(enemy, storedData.NumberOnSkillTree, enemy.RuneStatusEffectNumber);
                        Debug.Log("Combo called!");

                        if (debugComboText != null)
                        {

                            debugComboText.text = "Lighting/Wind Combo!";

                        }

                        break;

                    case (RuneType.Wind, RuneType.Lightning):

                        LightningAndWindCombo(enemy, enemy.RuneStatusEffectNumber, storedData.NumberOnSkillTree);
                        Debug.Log("Combo called!");

                        if (debugComboText != null)
                        {

                            debugComboText.text = "Wind/Lightning Combo!";

                        }

                        break;

                    default:

                        break;
                }

                enemy.HasStatusEffect = false;

            }
        }
    }

    /// <summary>
    /// calls lightning and wind combo effect
    /// </summary>
    /// <param name="enemy"> initial target </param>
    /// <param name="lightningTier"> which lightning rune was last used on this enemy </param>
    /// <param name="windTier"> which wind rune was last used on this enemy </param>
    void LightningAndWindCombo(Enemy enemy, int lightningTier, int windTier)
    {

        Enemy[] enemiesOnTheGrid = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        //PART 1: FINDING TARGETS

        RangeCheck(true, 2, enemy.GetComponentInParent<TileBehaviour>());

        List<Enemy> validEnemies = new List<Enemy>();

        foreach (TileBehaviour tile in tilesInRange)
        {

            if (tile == enemy.GetComponentInParent<TileBehaviour>())
            {

                continue;

            }

            foreach (Enemy newEnemy in enemiesOnTheGrid)
            {

                if ((int)newEnemy.transform.position.x == tile.IndexInGrid.x && (int)newEnemy.transform.position.z == tile.IndexInGrid.y)
                {

                    validEnemies.Add(newEnemy);

                    break;

                }


            }

        }


        //PART 2: LIGHTNING DAMAGE

        int lightningDamage;
        int lightningMasteredDamage;

        switch (lightningTier)
        {

            case (1):

                lightningDamage = Mathf.CeilToInt(lightningDamageTierOne * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier
                    * FindFirstObjectByType<PlayerStats>().BaseAttackMultiplier);

                lightningMasteredDamage = Mathf.CeilToInt(lightningMasteredDamageTierOne * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier
                    * FindFirstObjectByType<PlayerStats>().BaseAttackMultiplier);

                break;

            case (2):

                lightningDamage = Mathf.CeilToInt(lightningDamageTierTwo * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier
                    * FindFirstObjectByType<PlayerStats>().BaseAttackMultiplier);

                lightningMasteredDamage = Mathf.CeilToInt(lightningMasteredDamageTierTwo * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier
                    * FindFirstObjectByType<PlayerStats>().BaseAttackMultiplier);

                break;

            case (3):

                lightningDamage = Mathf.CeilToInt(lightningDamageTierTwo * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier
                    * FindFirstObjectByType<PlayerStats>().BaseAttackMultiplier);

                lightningMasteredDamage = Mathf.CeilToInt(lightningMasteredDamageTierTwo * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier
                    * FindFirstObjectByType<PlayerStats>().BaseAttackMultiplier);

                break;

            case (4):

                lightningDamage = Mathf.CeilToInt(lightningDamageTierThree * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier
                    * FindFirstObjectByType<PlayerStats>().BaseAttackMultiplier);

                lightningMasteredDamage = Mathf.CeilToInt(lightningMasteredDamageTierThree * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier
                    * FindFirstObjectByType<PlayerStats>().BaseAttackMultiplier);

                break;

            default:

                lightningDamage = 0;

                lightningMasteredDamage = 0;

                break;
        }

        for (int i = 0; i < validEnemies.Count; i++)
        {

            validEnemies[i].Damage(lightningDamage);

            GridManager.combatGrid[(int)validEnemies[i].transform.position.x, (int)validEnemies[i].transform.position.z].ElectrifyTile();

        }

        if (lightningMastered)
        {

            if (enemy != null)
            {

                enemy.Damage(lightningMasteredDamage);

            }

            for (int i = 0; i < validEnemies.Count; i++)
            {

                if (validEnemies[i] != null)
                {

                    validEnemies[i].Damage(lightningDamage);

                }

            }



        }


        //PART 3: WIND DAMAGE

        int windPrimaryDamage;

        int windSecondaryDamage;

        int windTempHealth;

        switch (windTier)
        {

            case (1):

                windPrimaryDamage = Mathf.CeilToInt(windPrimaryDamageTierOne * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier
                    * FindFirstObjectByType<PlayerStats>().BaseAttackMultiplier);

                windSecondaryDamage = Mathf.CeilToInt(windSecondaryDamageTierOne * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier
                    * FindFirstObjectByType<PlayerStats>().BaseAttackMultiplier);

                windTempHealth = windMasteredTempHealthTierOne;

                break;

            case (2):

                windPrimaryDamage = Mathf.CeilToInt(windPrimaryDamageTierTwo * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier
                    * FindFirstObjectByType<PlayerStats>().BaseAttackMultiplier);

                windSecondaryDamage = Mathf.CeilToInt(windSecondaryDamageTierTwo * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier
                    * FindFirstObjectByType<PlayerStats>().BaseAttackMultiplier);

                windTempHealth = windMasteredTempHealthTierTwo;

                break;

            case (4):

                windPrimaryDamage = Mathf.CeilToInt(windPrimaryDamageTierThree * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier
                    * FindFirstObjectByType<PlayerStats>().BaseAttackMultiplier);

                windSecondaryDamage = Mathf.CeilToInt(windSecondaryDamageTierThree * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier
                    * FindFirstObjectByType<PlayerStats>().BaseAttackMultiplier);

                windTempHealth = windMasteredTempHealthTierThree;

                break;

            default:

                windPrimaryDamage = 0;

                windSecondaryDamage = 0;

                windTempHealth = 0;

                break;
        }

        if (enemy != null)
        {

            enemy.Damage(windPrimaryDamage);

        }

        for (int i = 0; i < validEnemies.Count; i++)
        {


            if (validEnemies[i] != null)
            {

                validEnemies[i].Damage(windSecondaryDamage);

            }

        }

        if (windMastered)
        {

            FindFirstObjectByType<PlayerStats>().AddTempHealth(windTempHealth);

        }

    }

    #endregion COMBO FUNCTIONS


    //AFTER CASTING

    #region END OF TURN

    /// <summary>
    /// runs whenever an enemy is successfully targeted
    /// made into a function to prevent SOME clutter
    /// </summary>
    void EndPlayerAttackPhase()
    {

        waitingForThePlayer = false;

        PublicEvents.RuneCast(storedData.RuneActionPoints);

        if (TurnManager.currentStatus == TurnStates.PlayerTurn)
        {
            playerMenu.SetActive(true);
        }

        this.gameObject.SetActive(false);

        Invoke("ClearText", 1);

    }

    /// <summary>
    /// evil temporary code for evil temporary text
    /// clears debug text
    /// </summary>
    void ClearText()
    {

        if (debugText != null)
        {

            debugText.text = "";

        }

        if (debugComboText != null)
        {

            debugComboText.text = "";

        }

    }

    #endregion END OF TURN

}
