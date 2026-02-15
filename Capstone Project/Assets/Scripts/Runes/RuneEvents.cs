/*************************************************
Author Names : 	Jay Embry
Date Created : 	10/07/2025
Date Last Modified : 01/29/2026
Brief Description : Contains rune types and effects
External Resources : 	
	***************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NaughtyAttributes;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.Collections.Unicode;
using static UnityEditor.PlayerSettings;
using EventReference = FMODUnity.EventReference;

public class RuneEvents : MonoBehaviour
{

    //VARIABLES

    #region SETUP

    public enum Variables
    {

        ComboVariables,
        Audio

    }

    [SerializeField] private Variables currentInspectorShowing;

    List<TileBehaviour> targetedTiles;

    RuneData selectedRune;
    Vector2Int selectedTile;
    Vector2Int originalSelectedTile;
    Enemy selectedEnemy;

    Vector3 ghostPos;

    int movementLeft;
    int movementUsed;

    public bool WaitingOnPath = false;

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
    private EventReference lightningSpellSFX_1;

    [ShowIf(nameof(currentInspectorShowing), Variables.Audio), SerializeField]
    private EventReference lightningSpellSFX_2;

    [ShowIf(nameof(currentInspectorShowing), Variables.Audio), SerializeField]
    private EventReference lightningSpellSFX_3;

    [ShowIf(nameof(currentInspectorShowing), Variables.Audio), SerializeField]
    private EventReference lightningSpellSFX_4;

    [ShowIf(nameof(currentInspectorShowing), Variables.Audio), SerializeField]
    private EventReference windSpellSFX_1;

    [ShowIf(nameof(currentInspectorShowing), Variables.Audio), SerializeField]
    private EventReference windSpellSFX_2;

    [ShowIf(nameof(currentInspectorShowing), Variables.Audio), SerializeField]
    private EventReference windSpellSFX_3;

    [ShowIf(nameof(currentInspectorShowing), Variables.Audio), SerializeField]
    private EventReference windSpellSFX_4;

    [ShowIf(nameof(currentInspectorShowing), Variables.Audio), SerializeField]
    private GameObject audioListenerObject;

    #endregion AUDIO


    #region INITIALIZATION

    /// <summary>
    /// Runs whenever this script is loaded into a scene
    /// </summary>
    private void OnEnable()
    {

        PublicEvents.LightningCast += SelectedLightningRuneCast;
        PublicEvents.WindCast += SelectedWindRuneCast;

        PublicEvents.MovementDirection += MoveDirection;

        PublicEvents.MasteryRunePurchased += MasteryUnlocked;

    }

    /// <summary>
    /// Runs whenever this script is destroyed
    /// </summary>
    private void OnDisable()
    {

        PublicEvents.LightningCast -= SelectedLightningRuneCast;
        PublicEvents.WindCast -= SelectedWindRuneCast;

        PublicEvents.MovementDirection -= MoveDirection;

        PublicEvents.MasteryRunePurchased -= MasteryUnlocked;

    }

    #endregion INITIALIZATION


    #region OTHER

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


    }

    public void GetTargets(List<TileBehaviour> tilesInRange)
    {

        targetedTiles = tilesInRange;

    }

    #endregion OTHER


    #region LIGHTNING FUNCTIONS

    /// <summary>
    /// Calls lightning rune effect
    /// </summary>
    /// <param name="tile"> tile that the player has selected </param>
    /// <param name="enemy"> enemy that the player has selected </param>
    /// <param name="player"> when the player has selected themself </param>
    public async void SelectedLightningRuneCast(RuneData rune, TileBehaviour tile, Enemy enemy, PlayerBehavior player)
    {

        float damageDealt = Mathf.CeilToInt(rune.RuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier
        * FindFirstObjectByType<PlayerStats>().BaseAttackMultiplier);

        Enemy[] enemiesOnTheGrid = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        Vector2Int playerOriginalTile = FindFirstObjectByType<PlayerBehavior>().GetComponentInParent<TileBehaviour>().IndexInGrid;

        GameObject VFX;

        switch (rune.NumberOnSkillTree)
        {

            //targets a tile and  electrifies the tiles around it
            case (1):


                if (enemy != null)
                {
                    await Task.Delay(1200);
                    enemy.Damage(damageDealt, Enemy.DamageType.Lightning);

                    CheckRuneCombination(rune, enemy);

                }

                FindAdjacentTiles(tile);

                foreach(TileBehaviour adjacentTile in secondaryTargets)
                {

                    if(adjacentTile.GetComponentInChildren<Enemy>() != null)
                    {

                        adjacentTile.GetComponentInChildren<Enemy>().Damage(Mathf.CeilToInt(rune.SecondaryRuneDamage * FindFirstObjectByType<PlayerStats>()
                        .LightningAttackMultiplier * FindFirstObjectByType<PlayerStats>().BaseAttackMultiplier), Enemy.DamageType.Lightning);

                        CheckRuneCombination(rune, enemy);

                        adjacentTile.ElectrifyTile();

                    }

                }

                tile.ElectrifyTile();

                AudioManager.instance.CreateEventInstance(lightningSpellSFX_1);
                AudioManager.instance.PlayOneShot(lightningSpellSFX_1, audioListenerObject.transform.position);

                VFX = Instantiate(rune.RuneVFX, tile.transform);

                gameObject.GetComponent<RuneRangeAndTargeting>().SetCastStatus(true);

                StartCoroutine(UpdatePlayerStatus());

                break;

            //targets opponents in a cross pattern
            case (2):


                if (enemy == null)
                {

                    return;

                }

                AudioManager.instance.CreateEventInstance(lightningSpellSFX_4);
                AudioManager.instance.PlayOneShot(lightningSpellSFX_4, audioListenerObject.transform.position);


                FindLinesOfTargets(rune, tile);

                foreach (TileBehaviour potentialTarget in secondaryTargets)
                {

                    VFX = Instantiate(rune.RuneVFX, potentialTarget.transform);

                    await Task.Delay(1200);

                    potentialTarget.ElectrifyTile();

                    if (potentialTarget.GetComponentInChildren<Enemy>() != null)
                    {


                        if(potentialTarget != tile)
                        {

                            SubtractFromDamage(rune, tile, potentialTarget);

                            potentialTarget.GetComponentInChildren<Enemy>().Damage(damageDealt - subtraction, Enemy.DamageType.Lightning);

                        }
                        else
                        {

                            potentialTarget.GetComponentInChildren<Enemy>().Damage(damageDealt, Enemy.DamageType.Lightning);

                        }

                        CheckRuneCombination(rune, potentialTarget.GetComponentInChildren<Enemy>());

                    }

                }

                gameObject.GetComponent<RuneRangeAndTargeting>().SetCastStatus(true);

                StartCoroutine(UpdatePlayerStatus());

                break;

            //teleports the player, damages adjacent enemies, and knocks enemies backwards
            case (3):

                if (enemy == null)
                {

                    FindFirstObjectByType<PlayerBehavior>().gameObject.transform.SetParent(tile.transform);
                    FindFirstObjectByType<PlayerBehavior>().gameObject.transform.position = new Vector3(tile.transform.position.x, 0, tile.transform.position.z);
                    GridManager.MoveToTile(playerOriginalTile, tile.IndexInGrid, -3);

                    FindAdjacentTiles(tile);

                    tile.ElectrifyTile();

                    foreach (TileBehaviour adjacentTile in secondaryTargets)
                    {

                        if(adjacentTile.GetComponentInChildren<Enemy>() != null)
                        {

                            await Task.Delay(1200);
                            adjacentTile.GetComponentInChildren<Enemy>().Damage(damageDealt, Enemy.DamageType.Lightning);

                            CheckRuneCombination(rune, adjacentTile.GetComponentInChildren<Enemy>());

                            SendEnemyBackwards(FindFirstObjectByType<PlayerBehavior>().GetComponentInParent<TileBehaviour>(), adjacentTile, adjacentTile.GetComponentInChildren<Enemy>());

                        }

                        tile.ElectrifyTile();

                    }

                    AudioManager.instance.CreateEventInstance(lightningSpellSFX_3);
                    AudioManager.instance.PlayOneShot(lightningSpellSFX_3, audioListenerObject.transform.position);

                    

                    gameObject.GetComponent<RuneRangeAndTargeting>().SetCastStatus(true);

                    StartCoroutine(UpdatePlayerStatus());

                }

                break;

            //targets opponents in a straight line
            case (4):

                TileBehaviour oldPlayerTile = FindFirstObjectByType<PlayerBehavior>().GetComponentInParent<TileBehaviour>();

                if (enemy == null)
                {

                    FindFirstObjectByType<PlayerBehavior>().gameObject.transform.SetParent(tile.transform);
                    FindFirstObjectByType<PlayerBehavior>().gameObject.transform.position = new Vector3(tile.transform.position.x, 0, tile.transform.position.z);
                    GridManager.MoveToTile(playerOriginalTile, tile.IndexInGrid, -3);

                    tile.ElectrifyTile();

                    FindTargetsInPath(oldPlayerTile);

                    AudioManager.instance.CreateEventInstance(lightningSpellSFX_4);
                    AudioManager.instance.PlayOneShot(lightningSpellSFX_4, audioListenerObject.transform.position);

                    foreach (TileBehaviour tileInPath in secondaryTargets)
                    {

                        if (tileInPath.GetComponentInChildren<Enemy>() != null)
                        {

                            await Task.Delay(1200); 
                            tileInPath.GetComponentInChildren<Enemy>().Damage(damageDealt, Enemy.DamageType.Lightning);

                            CheckRuneCombination(rune, tileInPath.GetComponentInChildren<Enemy>());

                        }

                        tile.ElectrifyTile();

                    }

                    

                    gameObject.GetComponent<RuneRangeAndTargeting>().SetCastStatus(true);

                    StartCoroutine(UpdatePlayerStatus());

                }

                break;

            default:

                break;
        }

    }

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

            if (enemy.GetComponentInParent<TileBehaviour>() == target)
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


    //variable that stores targets for an aoe attack
    List<TileBehaviour> secondaryTargets = new List<TileBehaviour>();

    /// <summary>
    /// called when lightning 3 is cast
    /// finds opponents in a straight line relative to the player's position and the initial target
    /// ngl i don't think that i'm seeing the light of heaven for this but it works
    /// </summary>
    /// <param name="initialTarget"> initial target that the player had picked out </param>
    /// <returns> a list of tiles for the spell to target </returns>
    List<TileBehaviour> FindLinesOfTargets(RuneData rune, TileBehaviour initialTarget = null)
    {

        secondaryTargets.Clear();

        foreach (TileBehaviour tile in GridManager.combatGrid)
        {

            if(initialTarget.transform.position.x == tile.transform.position.x && 
            Mathf.Abs(initialTarget.transform.position.x - tile.transform.position.x) <= rune.RuneRange)
            {

                secondaryTargets.Add(tile);

            }

            if(initialTarget.transform.position.z == tile.transform.position.z &&
            Mathf.Abs(initialTarget.transform.position.z - tile.transform.position.z) <= rune.RuneRange)
            {

                secondaryTargets.Add(tile);

            }

        }

        return secondaryTargets;

    }

    /// <summary>
    /// called when lightning 1 is cast
    /// finds opponents adjacent and diagonal to the initial target
    /// </summary>
    /// <param name="target"> initial target </param>
    /// <returns> a list of the tiles adjacent and diagonal to the target </returns>
    List<TileBehaviour> FindAdjacentTiles(TileBehaviour target)
    {

        secondaryTargets.Clear();

        foreach(TileBehaviour tile in GridManager.combatGrid)
        {

            if (Mathf.Abs(tile.transform.position.x - target.transform.position.x) <= 1 &&
                Mathf.Abs(tile.transform.position.z - target.transform.position.z) <= 1)
            {

                secondaryTargets.Add(tile);

            }

        }

        return secondaryTargets;

    }

    /// <summary>
    /// finds tiles between the new and old player positions 
    /// </summary>
    /// <param name="originalTile"> the player's tile prior to casting lightning 3 </param>
    /// <returns> list of tiles </returns>
    List<TileBehaviour> FindTargetsInPath(TileBehaviour originalTile)
    {

        secondaryTargets.Clear();

        foreach(TileBehaviour tile in GridManager.combatGrid)
        {

            if (GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.x != originalTile.transform.position.x &&
            GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.z == originalTile.transform.position.z)
            {

                if (GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.x < originalTile.transform.position.x &&
                tile.transform.position.x > GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.x &&
                tile.transform.position.x < originalTile.transform.position.x &&
                tile.transform.position.z == GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.z)
                {

                    secondaryTargets.Add(tile);

                }

                if (GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.x > originalTile.transform.position.x &&
                tile.transform.position.x < GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.x &&
                tile.transform.position.x > originalTile.transform.position.x &&
                tile.transform.position.z == GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.z)
                {

                    secondaryTargets.Add(tile);

                }

            }

            if (GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.x == originalTile.transform.position.x &&
            GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.z != originalTile.transform.position.z)
            {

                if (GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.z < originalTile.transform.position.z &&
                tile.transform.position.z > GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.z &&
                tile.transform.position.z < originalTile.transform.position.z &&
                tile.transform.position.x == GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.x)
                {

                    secondaryTargets.Add(tile);

                }

                if (GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.z > originalTile.transform.position.z &&
                tile.transform.position.z < GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.z &&
                tile.transform.position.z > originalTile.transform.position.z &&
                tile.transform.position.x == GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.x)
                {

                    secondaryTargets.Add(tile);

                }

            }

            if (GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.x != originalTile.transform.position.x &&
            GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.z != originalTile.transform.position.z)
            {

                if (GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.x < originalTile.transform.position.x &&
                tile.transform.position.x > GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.x &&
                tile.transform.position.x < originalTile.transform.position.x &&
                GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.z < originalTile.transform.position.z &&
                tile.transform.position.z > GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.z &&
                tile.transform.position.z < originalTile.transform.position.z)
                {

                    if (Mathf.Approximately((tile.transform.position.x - GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.x),
                    (tile.transform.position.z - GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.z)))
                    {

                        secondaryTargets.Add(tile);

                    }

                }

                if (GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.x > originalTile.transform.position.x &&
                tile.transform.position.x < GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.x &&
                tile.transform.position.x > originalTile.transform.position.x &&
                GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.z > originalTile.transform.position.z &&
                tile.transform.position.z < GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.y &&
                tile.transform.position.z > originalTile.transform.position.z)
                {

                    if (Mathf.Approximately((tile.transform.position.x - GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.x),
                    (tile.transform.position.z - GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.z)))
                    {

                        secondaryTargets.Add(tile);

                    }

                }

                if (GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.x < originalTile.transform.position.x &&
                tile.transform.position.x > GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.x &&
                tile.transform.position.x < originalTile.transform.position.x &&
                GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.z > originalTile.transform.position.z &&
                tile.transform.position.z < GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.z &&
                tile.transform.position.z > originalTile.transform.position.z)
                {

                    if (Mathf.Approximately((GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.x - tile.transform.position.x),
                    (tile.transform.position.z - GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.z)))
                    {

                        secondaryTargets.Add(tile);

                    }

                }

                if (GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.x > originalTile.transform.position.x &&
                tile.transform.position.x < GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.x &&
                tile.transform.position.x > originalTile.transform.position.x &&
                GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.z < originalTile.transform.position.z &&
                tile.transform.position.z > GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.z &&
                tile.transform.position.z < originalTile.transform.position.z)
                {

                    if (Mathf.Approximately((GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.x - tile.transform.position.x),
                    (GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].transform.position.z - tile.transform.position.z)))
                    {

                        secondaryTargets.Add(tile);

                    }

                }

            }

        }

        return secondaryTargets;

    }


    //for lightning 2a
    float subtraction;

    float SubtractFromDamage(RuneData rune, TileBehaviour originalTarget, TileBehaviour nextTarget)
    {

        if(originalTarget.transform.position.x != nextTarget.transform.position.x)
        {

            subtraction = Mathf.CeilToInt((Mathf.Abs(originalTarget.transform.position.x - nextTarget.transform.position.x) * (rune.RuneDamage * .2f)));

        }
        else if(originalTarget.transform.position.z != nextTarget.transform.position.z)
        {

            subtraction = Mathf.CeilToInt((Mathf.Abs(originalTarget.transform.position.z - nextTarget.transform.position.z) * (rune.RuneDamage * .2f)));

        }
        else
        {

            subtraction = 0;

        }

        return subtraction;

    }

    #endregion LIGHTNING FUNCTIONS


    #region WIND FUNCTIONS

    /// <summary>
    /// Calls wind rune effect
    /// </summary>
    /// <param name="tile"> tile that the player has selected </param>
    /// <param name="enemy"> enemy that the player has selected </param>
    /// <param name="player"> when the player has selected themself </param>
    public async void SelectedWindRuneCast(RuneData rune, TileBehaviour tile, Enemy enemy = null, PlayerBehavior player = null)
    {

        float damageDealt = Mathf.Ceil(rune.RuneDamage * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier
        * FindFirstObjectByType<PlayerStats>().BaseAttackMultiplier);

        GameObject VFX;

        switch (rune.NumberOnSkillTree)
        {

            //knocks adjacent enemies along path and knocks back enemies in path
            case (1):

                if(enemy != null && !WaitingOnPath)
                {

                    GridManager.RemoveHighlight();

                    selectedRune = rune;
                    originalSelectedTile = tile.IndexInGrid;
                    selectedTile = originalSelectedTile;
                    selectedEnemy = enemy;

                    PreviousPos.Add(selectedTile);
                    GridManager.combatGrid[selectedTile.x, selectedTile.y].SetHighlightColor(GetComponent<RuneRangeAndTargeting>().WindSecondaryHighlight);
                    GridManager.combatGrid[selectedTile.x, selectedTile.y].ShowHighlight(true);

                    WaitingOnPath = true;
                    FindFirstObjectByType<PlayerInputHandler>().enableMovement = true;

                    movementLeft = rune.RuneRange;

                    ghostPos = new Vector3(selectedTile.x, 0, selectedTile.y);

                    Debug.Log("START MOVING");

                }
                else if (WaitingOnPath)
                {

                    gameObject.GetComponent<RuneRangeAndTargeting>().SetCastStatus(true);

                    MoveAlongPath(rune);

                    WaitingOnPath = false;
                    FindFirstObjectByType<PlayerInputHandler>().IsPathing = false;
                    FindFirstObjectByType<PlayerInputHandler>().enableMovement = false;

                    await Task.Delay(1200);
                    enemy.Damage(damageDealt, Enemy.DamageType.Wind);

                    CheckRuneCombination(rune, enemy);

                    AudioManager.instance.CreateEventInstance(windSpellSFX_1);
                    AudioManager.instance.PlayOneShot(windSpellSFX_1, audioListenerObject.transform.position);

                }

                    break;

            //targets an opponent for moderate damage
            //has a chance to hit twice
            case (2):

                if(enemy != null)
                {

                    await Task.Delay(1500);
                    enemy.Damage(damageDealt, Enemy.DamageType.Wind);

                    CheckRuneCombination(rune,enemy);

                    if(Random.value <= rune.RuneSecondaryEffectChance)
                    {

                        enemy.Damage(damageDealt, Enemy.DamageType.Wind);

                    }

                    AudioManager.instance.CreateEventInstance(windSpellSFX_2);
                    AudioManager.instance.PlayOneShot(windSpellSFX_2, audioListenerObject.transform.position);

                    gameObject.GetComponent<RuneRangeAndTargeting>().SetCastStatus(true);

                    StartCoroutine(UpdatePlayerStatus());

                }

                break;

            //creates a shield on the player's tile
            case (3):

                ShieldBehavior newShield = tile.gameObject.AddComponent<ShieldBehavior>();

                newShield.OnShieldGenerated(tile.transform, rune.RuneVFX);
                
                AudioManager.instance.CreateEventInstance(windSpellSFX_3);
                AudioManager.instance.PlayOneShot(windSpellSFX_3, audioListenerObject.transform.position);

                gameObject.GetComponent<RuneRangeAndTargeting>().SetCastStatus(true);

                StartCoroutine(UpdatePlayerStatus());

                break;

            //delays target's turn and damages surrounding enemies
            case (4):

                if(player == null)
                {

                    VFX = Instantiate(rune.RuneVFX, tile.transform);
                    await Task.Delay(3200);
                    AudioManager.instance.CreateEventInstance(windSpellSFX_4);
                    AudioManager.instance.PlayOneShot(windSpellSFX_4, audioListenerObject.transform.position);

                    if (enemy != null)
                    {

                        enemy.DelayedTurnStatus(true);

                        enemy.Damage(damageDealt, Enemy.DamageType.Wind);

                        CheckRuneCombination(rune, enemy);

                    }

                    PublicEvents.CheckRange.Invoke(true, 3, tile);

                    //do NOT delete this list again, jay. it's here for a reason
                    List<Enemy> validEnemies = new List<Enemy>();

                    foreach (TileBehaviour tileInRange in targetedTiles)
                    {

                        if (tileInRange == tile)
                        {

                            continue;

                        }

                        if (tileInRange.GetComponentInChildren<Enemy>() != null)
                        {

                            validEnemies.Add(tileInRange.GetComponentInChildren<Enemy>());

                        }

                    }

                    if (validEnemies.Count > 0)
                    {

                        for (int i = 0; i < validEnemies.Count; i++)
                        {

                            validEnemies[i].Damage(Mathf.CeilToInt(damageDealt / validEnemies.Count), Enemy.DamageType.Wind);

                        }

                    }

                    gameObject.GetComponent<RuneRangeAndTargeting>().SetCastStatus(true);

                    StartCoroutine(UpdatePlayerStatus());

                }

                break;

        }

    }

    public static bool CanMoveBackwards(TileBehaviour originTile, TileBehaviour enemyTile)
    {

        Vector2Int newTilePos = enemyTile.IndexInGrid;

        if (originTile.IndexInGrid.x < enemyTile.IndexInGrid.x)
        {

            newTilePos.x += 1;

        }
        else if (originTile.IndexInGrid.x > enemyTile.IndexInGrid.x)
        {

            newTilePos.x -= 1;

        }

        if (originTile.IndexInGrid.y < enemyTile.IndexInGrid.y)
        {

            newTilePos.y += 1;

        }
        else if (originTile.IndexInGrid.y > enemyTile.IndexInGrid.y)
        {

            newTilePos.y -= 1;

        }

        if (GridManager.combatGrid[newTilePos.x, newTilePos.y])
        {

            TileBehaviour newTile = GridManager.combatGrid[newTilePos.x, newTilePos.y];

            return newTile.entityOnGrid == -1;

        }
        else
        {

            return false;

        }

    }

    /// <summary>
    /// shoves the enemy backwards relative from where wind 1 was initially cast
    /// </summary>
    /// <param name="originTile"> tile that the player occupies </param>
    /// <param name="enemyTile"> tile that the enemy occupies </param>
    /// <param name="enemy"> the target </param>
    void SendEnemyBackwards(TileBehaviour originTile, TileBehaviour enemyTile, Enemy enemy)
    {

        Vector2Int newTilePos = enemyTile.IndexInGrid;

        if (originTile.IndexInGrid.x < enemyTile.IndexInGrid.x)
        {

            newTilePos.x += 1;

        }
        else if (originTile.IndexInGrid.x > enemyTile.IndexInGrid.x)
        {

            newTilePos.x -= 1;

        }

        if (originTile.IndexInGrid.y < enemyTile.IndexInGrid.y)
        {

            newTilePos.y += 1;

        }
        else if (originTile.IndexInGrid.y > enemyTile.IndexInGrid.y)
        {

            newTilePos.y -= 1;

        }

        if (GridManager.combatGrid[newTilePos.x, newTilePos.y])
        {

            TileBehaviour newTile = GridManager.combatGrid[newTilePos.x, newTilePos.y];

            if(newTile.entityOnGrid == -1)
            {

                enemy.transform.SetParent(newTile.transform);

                enemy.transform.position = new Vector3(newTile.transform.position.x, 0, newTile.transform.position.z);

                GridManager.MoveToTile(enemyTile.IndexInGrid, newTilePos, -2);

                enemy.GetComponent<GridPathfinding>().SetPosition(newTilePos);

            }

        }

    }

    #endregion WIND FUNCTIONS



    #region PATHING

    public static bool CanMoveThroughTile(Vector2Int tileCoordinates)
    {

        return GridManager.combatGrid[tileCoordinates.x, tileCoordinates.y].entityOnGrid == -1 ||
        GridManager.combatGrid[tileCoordinates.x, tileCoordinates.y].entityOnGrid == -2;

    }

    [HideInInspector] public List<Vector2Int> PreviousPos = new List<Vector2Int>();

    /// <summary>
    /// determines where the player is attempting to path
    /// </summary>
    /// <param name="dir"> the direction that the player moves in </param>
    private void MoveDirection(Vector2 dir)
    {

        if(WaitingOnPath)
        {

            if (dir.y >= .5f)
            {
                Vector2Int v = new Vector2Int(selectedTile.x, selectedTile.y + 1);

                if (GridManager.TileIsInGrid(v) && CanMoveThroughTile(v))
                {
                    Vector3 newPosition = new Vector3(ghostPos.x, ghostPos.y, ghostPos.z + GridManager.MoveDistances.y);
                    UpdateMovement(v, newPosition);
                }
            }
            else if (dir.y <= -.5f)
            {
                Vector2Int v = new Vector2Int(selectedTile.x, selectedTile.y - 1);

                if (GridManager.TileIsInGrid(v) && CanMoveThroughTile(v))
                {
                    Vector3 newPosition = new Vector3(ghostPos.x, ghostPos.y, ghostPos.z - GridManager.MoveDistances.y);
                    UpdateMovement(v, newPosition);
                }
            }
            else if (dir.x > .5f)
            {
                Vector2Int v = new Vector2Int(selectedTile.x + 1, selectedTile.y);

                if (GridManager.TileIsInGrid(v) && CanMoveThroughTile(v))
                {
                    Vector3 newPosition = new Vector3(ghostPos.x + GridManager.MoveDistances.x, ghostPos.y, ghostPos.z);
                    UpdateMovement(v, newPosition);
                }
            }
            else if (dir.x < -.5f)
            {
                Vector2Int v = new Vector2Int(selectedTile.x - 1, selectedTile.y);

                if (GridManager.TileIsInGrid(v) && CanMoveThroughTile(v))
                {
                    Vector3 newPosition = new Vector3(ghostPos.x - GridManager.MoveDistances.x, ghostPos.y, ghostPos.z);
                    UpdateMovement(v, newPosition);
                }
            }

        }

    }

    List<Vector3> movementPos = new List<Vector3>();

    private void UpdateMovement(Vector2Int v, Vector3 t)
    {
        WaitingOnPath = false;
  
        if (PreviousPos.Contains(v))
        {

            ghostPos = new Vector3(PreviousPos[PreviousPos.Count - 2].x, 0, PreviousPos[PreviousPos.Count - 2].y);
            selectedTile = PreviousPos[PreviousPos.Count - 2];

            movementPos.Remove(movementPos[movementPos.Count - 1]);

            GridManager.combatGrid[PreviousPos[PreviousPos.Count - 1].x, PreviousPos[PreviousPos.Count - 1].y].ShowHighlight(false);
            PreviousPos.Remove(PreviousPos[PreviousPos.Count - 1]);

            movementLeft++;
            movementUsed--;

        }
        else
        {
            if (movementLeft > 0)
            {

                switch(selectedRune.TypeOfRune, selectedRune.NumberOnSkillTree)
                {

                    case (RuneType.Wind, 1):

                        if((GridManager.playerPosition.x < originalSelectedTile.x && originalSelectedTile.x < v.x) ||
                        (GridManager.playerPosition.x > selectedTile.x && originalSelectedTile.x > v.x) ||
                        (GridManager.playerPosition.y < selectedTile.y && originalSelectedTile.y < v.y) ||
                        (GridManager.playerPosition.y > selectedTile.y && originalSelectedTile.y > v.y))
                        {

                            if(GridManager.combatGrid[v.x, v.y].GetComponentInChildren<Enemy>())
                            {

                                if (!CanMoveBackwards(GridManager.combatGrid[selectedTile.x, selectedTile.y], GridManager.combatGrid[v.x, v.y]))
                                {

                                    return;

                                }

                            }

                            GridManager.combatGrid[v.x, v.y].SetHighlightColor(GetComponent<RuneRangeAndTargeting>().WindSecondaryHighlight);
                            GridManager.combatGrid[v.x, v.y].ShowHighlight(true);
                            PreviousPos.Add(v);
                            movementPos.Add(t);
                            --movementLeft;
                            ++movementUsed;

                            selectedTile = v;

                            ghostPos = t;

                        }

                        break;

                }

            }
        }

        //if (movementLeft > 0)
        //{

        //    ghostPos = t;

        //}

        StartCoroutine(MovementDelay());
    }

    IEnumerator MovementDelay()
    {
        yield return new WaitForSeconds(.1f);
        WaitingOnPath = true;
    }

    void MoveAlongPath(RuneData rune)
    {

        switch(rune.TypeOfRune, rune.NumberOnSkillTree)
        {

            case (RuneType.Wind, 1):

                for (int i = 0; i < movementPos.Count; ++i)
                {

                    Vector2Int nextPos = PreviousPos[i + 1];

                    if (GridManager.combatGrid[nextPos.x, nextPos.y].GetComponentInChildren<Enemy>())
                    {

                        if (!CanMoveBackwards(GridManager.combatGrid[PreviousPos[i].x, PreviousPos[i].y], GridManager.combatGrid[nextPos.x, nextPos.y]))
                        {

                            selectedEnemy.transform.SetParent(GridManager.combatGrid[PreviousPos[i].x, PreviousPos[i].y].transform);

                            selectedEnemy.transform.position = new Vector3(GridManager.combatGrid[PreviousPos[i].x, PreviousPos[i].y].transform.position.x,
                            0, GridManager.combatGrid[PreviousPos[i].x, PreviousPos[i].y].transform.position.z);

                            GridManager.MoveToTile(originalSelectedTile, PreviousPos[i], -2);

                            selectedEnemy.GetComponent<GridPathfinding>().SetPosition(PreviousPos[i]);

                            PreviousPos.Clear();
                            movementPos.Clear();
                            movementUsed = 0;

                            StartCoroutine(UpdatePlayerStatus());

                            return;

                        }

                        SendEnemyBackwards(GridManager.combatGrid[PreviousPos[i].x, PreviousPos[i].y],
                        GridManager.combatGrid[nextPos.x, nextPos.y],
                        GridManager.combatGrid[nextPos.x, nextPos.y].GetComponentInChildren<Enemy>());

                    }

                    if(i == (movementPos.Count - 1))
                    {

                        selectedEnemy.transform.SetParent(GridManager.combatGrid[nextPos.x, nextPos.y].transform);

                        selectedEnemy.transform.position = new Vector3(GridManager.combatGrid[nextPos.x, nextPos.y].transform.position.x,
                        0, GridManager.combatGrid[nextPos.x, nextPos.y].transform.position.z);

                        GridManager.MoveToTile(PreviousPos[i], nextPos, -2);

                        selectedEnemy.GetComponent<GridPathfinding>().SetPosition(nextPos);

                    }

                }

                PreviousPos.Clear();
                movementPos.Clear();
                movementUsed = 0;

                StartCoroutine(UpdatePlayerStatus());

                break;

        }

    }

    public void CancelPathing()
    {

        WaitingOnPath = false;

        FindFirstObjectByType<PlayerInputHandler>().IsPathing = false;
        FindFirstObjectByType<PlayerInputHandler>().enableMovement = false;

        foreach (Vector2Int v in PreviousPos)
        {
            GridManager.combatGrid[v.x, v.y].ShowHighlight(false);
        }
        PreviousPos.Clear();
        movementPos.Clear();
        movementLeft += movementUsed;
        movementUsed = 0;

    }

    #endregion PATHING


    #region COMBO FUNCTIONS

    /// <summary>
    /// checks if the enemy has been hit by a spell prior
    /// if so, a combo is triggered
    /// </summary>
    /// <param name="enemy"> target </param>
    void CheckRuneCombination(RuneData rune, Enemy enemy)
    {
        if (enemy != null)
        {
            if (!enemy.HasStatusEffect)
            {

                enemy.RuneStatusEffect = rune.TypeOfRune;
                enemy.RuneStatusEffectNumber = rune.NumberOnSkillTree;

                enemy.HasStatusEffect = true;

                Debug.Log("Status effect added!");

            }
            else if (enemy.HasStatusEffect && enemy.RuneStatusEffect != rune.TypeOfRune)
            {

                switch (rune.TypeOfRune, enemy.RuneStatusEffect)
                {

                    case (RuneType.Lightning, RuneType.Wind):

                        LightningAndWindCombo(enemy, rune.NumberOnSkillTree, enemy.RuneStatusEffectNumber);
                        Debug.Log("Combo called!");

                        break;

                    case (RuneType.Wind, RuneType.Lightning):

                        LightningAndWindCombo(enemy, enemy.RuneStatusEffectNumber, rune.NumberOnSkillTree);
                        Debug.Log("Combo called!");

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

        //PART 1: FINDING TARGETS

        PublicEvents.CheckRange.Invoke(true, 2, enemy.GetComponentInParent<TileBehaviour>());

        List<Enemy> validEnemies = new List<Enemy>();

        foreach (TileBehaviour tile in targetedTiles)
        {

            if (tile == enemy.GetComponentInParent<TileBehaviour>())
            {

                continue;

            }

            if(tile.GetComponentInChildren<Enemy>() != null)
            {

                validEnemies.Add(tile.GetComponentInChildren<Enemy>());

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

            validEnemies[i].Damage(lightningDamage, Enemy.DamageType.Lightning);

            validEnemies[i].GetComponentInParent<TileBehaviour>().ElectrifyTile();

        }

        if (lightningMastered)
        {

            if (enemy != null)
            {

                enemy.Damage(lightningMasteredDamage, Enemy.DamageType.Lightning);

            }

            for (int i = 0; i < validEnemies.Count; i++)
            {

                if (validEnemies[i] != null)
                {

                    validEnemies[i].Damage(lightningDamage, Enemy.DamageType.Lightning);

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

            enemy.Damage(windPrimaryDamage, Enemy.DamageType.Wind);

        }

        for (int i = 0; i < validEnemies.Count; i++)
        {


            if (validEnemies[i] != null)
            {

                validEnemies[i].Damage(windSecondaryDamage, Enemy.DamageType.Wind);

            }

        }

        if (windMastered)
        {

            FindFirstObjectByType<PlayerStats>().AddTempHealth(windTempHealth);

        }

    }

    #endregion COMBO FUNCTIONS

    IEnumerator UpdatePlayerStatus()
    {

        int timer = 0;

        while (timer <= 1)
        {

            timer++;

            if (timer == 1)
            {

                PublicEvents.EndCast.Invoke();

            }

            yield return new WaitForSeconds(1);

        }

    }

}
