/*************************************************
Author Names : 	Jay Embry
Date Created : 	10/07/2025
Date Last Modified : 11/08/2025
Brief Description : Contains rune types and effects
External Resources : 	
	***************************************************/

using NaughtyAttributes;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using EventReference = FMODUnity.EventReference;

public class RuneEvents : MonoBehaviour
{
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

        PublicEvents.SelectTile += TargetSelectedTile;
        PublicEvents.RuneSelected += StoreSelectedRuneData;

        PublicEvents.MasteryRunePurchased += MasteryUnlocked;

    }

    /// <summary>
    /// Runs whenever this script is destroyed
    /// </summary>
    private void OnDisable()
    {

        PublicEvents.SelectTile -= TargetSelectedTile;
        PublicEvents.RuneSelected -= StoreSelectedRuneData;

        PublicEvents.MasteryRunePurchased -= MasteryUnlocked;

    }

    private bool lightningMastered;
    private bool windMastered;

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


    #region RUNE EVENTS

    /// <summary>
    /// Prepares the rune that the player chooses to attack with
    /// </summary>
    /// <param name="rd"> Rune Data </param>
    public void StoreSelectedRuneData(RuneData rd)
    {

        waitingForThePlayer = true;

        storedData = rd;

        if(debugText != null)
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

        if(waitingForThePlayer)
        {

            waitingForThePlayer = false;

            if (debugText != null)
            {

                debugText.text = "";

            }

        }

    }

    /// <summary>
    /// Checks if the selected tile has an enemy in it
    /// If it does, the player's selected rune will target the enemy on the selected tile
    /// </summary>
    /// <param name="tile"> tile that the player has selected </param>
    public void TargetSelectedTile(TileBehaviour tile)
    {

        if (waitingForThePlayer &&
            FindFirstObjectByType<GameManager>().CurrentActionPoints >= storedData.RuneActionPoints &&
            tilesInRange.Contains(tile))
        {

             switch (storedData.TypeOfRune)
                    {

                        case (RuneType.Lightning):

                            SelectLightningRune(tile);
                            break;

                        case (RuneType.Wind):

                            SelectWindRune(tile);
                            break;

                        default:

                            break;

                    }

        }

    }

    /// <summary>
    /// checks to see if a tile or enemy is in range before executing a spell
    /// </summary>
    /// <param name="isRadiusCheck"> determines where the function is checking adjacent tiles from </param>
    /// <param name="radius"> a spell's potential radius/aoe </param>
    /// <param name="target"> the initial tile hit if isRadiusCheck </param>
    public void RangeCheck(bool isRadiusCheck, int radius = 0, TileBehaviour target = null)
    {

        tilesInRange.Clear();

        List<Vector2Int> validTiles = new List<Vector2Int>();
        List<Vector2Int> searchedTiles = new List<Vector2Int>();

        if (!isRadiusCheck)
        {

            tilesInRange.Add(GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y]);
            searchedTiles.Add(GridManager.playerPosition);

            for (int i = 0; i < storedData.RuneRange +1; i++)
            {

                if (i == 1)
                {

                    List<Vector2Int> initialAdjacentTiles = GridManager.GetAllValidAdjacentTiles(GridManager.playerPosition, GridManager.playerPosition);

                    foreach (Vector2Int tile in initialAdjacentTiles.ToList())
                    {

                        if(tile != GridManager.playerPosition)
                        {

                            validTiles.Add(tile);

                        }

                    }

                    continue;

                }

                foreach (Vector2Int validTile in validTiles.ToList())
                {

                    if (searchedTiles.Contains(validTile))
                    {

                        continue;

                    }

                    List<Vector2Int> adjacentTiles = new List<Vector2Int>();

                    adjacentTiles = GridManager.GetAllValidAdjacentTiles(validTile, GridManager.playerPosition);

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

                    searchedTiles.Add(validTile);

                }

            }

            foreach (Vector2Int tile in validTiles.ToList())
            {

                tilesInRange.Add(GridManager.combatGrid[tile.x, tile.y]);

            }

            validTiles.Clear();
            searchedTiles.Clear();

        }
        else
        {

            Vector2Int targetPos = target.IndexInGrid;

            for (int i = 0; i < radius +1; i++)
            {

                if (i == 1)
                {

                    List<Vector2Int> initialAdjacentTiles = GridManager.GetAllValidAdjacentTiles(targetPos, targetPos);

                    foreach (Vector2Int tile in initialAdjacentTiles.ToList())
                    {

                        validTiles.Add(tile);

                    }

                    continue;

                }

                foreach (Vector2Int validTile in validTiles.ToList())
                {

                    if (searchedTiles.Contains(validTile))
                    {

                        continue;

                    }

                    List<Vector2Int> adjacentTiles = new List<Vector2Int>();

                    adjacentTiles = GridManager.GetAllValidAdjacentTiles(validTile, targetPos);

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

                    searchedTiles.Add(validTile);

                }

            }

            foreach (Vector2Int tile in validTiles.ToList())
            {

                tilesInRange.Add(GridManager.combatGrid[tile.x, tile.y]);

            }

            validTiles.Clear();
            searchedTiles.Clear();

        }
        
    }


    /// <summary>
    /// Calls lightning rune effect
    /// </summary>
    /// <param name="target"> tile that the player has selected </param>
    public void SelectLightningRune(TileBehaviour target)
    {

        float damageDealt = Mathf.CeilToInt(storedData.RuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier 
            * FindFirstObjectByType<PlayerStats>().BaseAttackMultiplier);

        GameObject VFX;

        switch (storedData.NumberOnSkillTree)
        {

            //targets one opponent for moderate damage
            case (1):

                if(target.GetComponentInChildren<Enemy>() != null)
                {

                    target.GetComponentInChildren<Enemy>().Damage(damageDealt);

                    CheckRuneCombination(target.GetComponentInChildren<Enemy>());

                    //AudioManager.instance.CreateEventInstance(lightningSpellCastedSFX);
                    //AudioManager.instance.PlayOneShot(lightningSpellCastedSFX, audioListenerObject.transform.position);

                    VFX = Instantiate(storedData.RuneVFX, target.transform);
                  
                    EndPlayerAttackPhase();

                }

                break;

            //targets two opponents
            //one is directly targeted, and the other is the closest to the original target
            case (2):

                if (target.GetComponentInChildren<Enemy>() != null)
                {

                    FindSecondaryTarget(target);

                    target.GetComponentInChildren<Enemy>().Damage(damageDealt);

                    CheckRuneCombination(target.GetComponentInChildren<Enemy>());

                    //AudioManager.instance.PlayOneShot(lightningSpellCastedSFX, audioListenerObject.transform.position);

                    VFX = Instantiate(storedData.RuneVFX, target.transform);
                   
                    if(secondaryTarget != null)
                    {

                        secondaryTarget.GetComponentInChildren<Enemy>().Damage(damageDealt);

                        //AudioManager.instance.CreateEventInstance(lightningSpellCastedSFX);
                        //AudioManager.instance.PlayOneShot(lightningSpellCastedSFX, audioListenerObject.transform.position);

                        if(storedData.SecondaryRuneVFX != null)
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


                if (target.GetComponentInChildren<Enemy>() != null)
                {

                    target.GetComponentInChildren<Enemy>().Damage(damageDealt);

                    CheckRuneCombination(target.GetComponentInChildren<Enemy>());

                    //AudioManager.instance.CreateEventInstance(lightningSpellCastedSFX);
                    //AudioManager.instance.PlayOneShot(lightningSpellCastedSFX, audioListenerObject.transform.position);

                    VFX = Instantiate(storedData.RuneVFX, target.transform);

                    RangeCheck(true, 3, target);

                    foreach(TileBehaviour tile in tilesInRange)
                    {

                        if(tile != target && tile.GetComponentInChildren<Enemy>() != null)
                        {

                            tile.GetComponentInChildren<Enemy>().Damage(Mathf.CeilToInt(0.40f * damageDealt));

                            if (storedData.SecondaryRuneVFX != null)
                            {

                                VFX = Instantiate(storedData.SecondaryRuneVFX, tile.transform);

                            }
                            else
                            {

                                VFX = Instantiate(storedData.RuneVFX, tile.transform);

                            }

                        }

                    }

                    EndPlayerAttackPhase();

                }

                break;

            //targets opponents in a straight line
            case (4):

                FindLineOfTargets(target);

                foreach(TileBehaviour potentialTarget in potentialTargetsForLightningStrikes)
                {

                    VFX = Instantiate(storedData.RuneVFX, potentialTarget.transform);

                    if(potentialTarget.GetComponentInChildren<Enemy>() != null)
                    {

                        potentialTarget.GetComponentInChildren<Enemy>().Damage(damageDealt);

                        CheckRuneCombination(potentialTarget.GetComponentInChildren<Enemy>());

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
    TileBehaviour secondaryTarget;

    /// <summary>
    /// Calls lightning rune effect
    /// </summary>
    /// <param name="target"> tile that the player has selected </param>
    /// <returns> the second target that lightning 2a will hit </returns>
    TileBehaviour FindSecondaryTarget(TileBehaviour target)
    {

        TileBehaviour[] otherEnemies = FindObjectsByType<TileBehaviour>(FindObjectsSortMode.None);

        float closestDistance = Mathf.Infinity;
        Vector3 primaryTargetPos = target.gameObject.transform.position;

        foreach(TileBehaviour potentialTarget in otherEnemies)
        {

            if(potentialTarget == target)
            {

                continue;

            }

            Vector3 dir = potentialTarget.gameObject.transform.position - primaryTargetPos;
            float distanceFromTarget = dir.sqrMagnitude;

            if(distanceFromTarget < closestDistance && potentialTarget.GetComponentInChildren<Enemy>() != null)
            {

                closestDistance = distanceFromTarget;
                secondaryTarget = potentialTarget;

            }

        }

        return secondaryTarget;

    }


    //variable that stores the straight line necessary for lightning 3
    List<TileBehaviour> potentialTargetsForLightningStrikes = new List<TileBehaviour>();

    /// <summary>
    /// called when lightning 3 is cast
    /// finds opponents in a straight line relative to the player's position and the initial target
    /// </summary>
    /// <param name="initialTarget"> initial target that the player had picked out </param>
    /// <returns> a list of tiles for the spell to target </returns>
    List<TileBehaviour> FindLineOfTargets(TileBehaviour initialTarget)
    {

        if (tilesInRange[0].transform.position.x == initialTarget.transform.position.x)
        {

            if (tilesInRange[0].transform.position.z < initialTarget.transform.position.z)
            {

                foreach (TileBehaviour tile in tilesInRange)
                {

                    if (tilesInRange[0].transform.position.x == tile.transform.position.x &&
                        tilesInRange[0].transform.position.z < tile.transform.position.z)
                    {

                        potentialTargetsForLightningStrikes.Add(tile);

                    }

                }

            }
            else if (tilesInRange[0].transform.position.z > initialTarget.transform.position.z)
            {

                foreach (TileBehaviour tile in tilesInRange)
                {

                    if (tilesInRange[0].transform.position.x == tile.transform.position.x &&
                        tilesInRange[0].transform.position.z > tile.transform.position.z)
                    {

                        potentialTargetsForLightningStrikes.Add(tile);

                    }

                }

            }

        }
        else if (tilesInRange[0].transform.position.x < initialTarget.transform.position.x)
        {

            if (tilesInRange[0].transform.position.z == initialTarget.transform.position.z)
            {

                foreach (TileBehaviour tile in tilesInRange)
                {

                    if (tilesInRange[0].transform.position.x < tile.transform.position.x &&
                        tilesInRange[0].transform.position.z == tile.transform.position.z)
                    {

                        potentialTargetsForLightningStrikes.Add(tile);

                    }

                }

            }
            else if (tilesInRange[0].transform.position.z < initialTarget.transform.position.z)
            {

                foreach (TileBehaviour tile in tilesInRange)
                {

                    if (tilesInRange[0].transform.position.x < tile.transform.position.x &&
                        tilesInRange[0].transform.position.z < tile.transform.position.z)
                    {

                        potentialTargetsForLightningStrikes.Add(tile);

                    }

                }

            }
            else if (tilesInRange[0].transform.position.z > initialTarget.transform.position.z)
            {

                foreach (TileBehaviour tile in tilesInRange)
                {

                    if (tilesInRange[0].transform.position.x < tile.transform.position.x &&
                        tilesInRange[0].transform.position.z > tile.transform.position.z)
                    {

                        potentialTargetsForLightningStrikes.Add(tile);

                    }

                }

            }

        }
        else if (tilesInRange[0].transform.position.x > initialTarget.transform.position.x)
        {

            if (tilesInRange[0].transform.position.z == initialTarget.transform.position.z)
            {

                foreach (TileBehaviour tile in tilesInRange)
                {

                    if (tilesInRange[0].transform.position.x > tile.transform.position.x &&
                        tilesInRange[0].transform.position.z == tile.transform.position.z)
                    {

                        potentialTargetsForLightningStrikes.Add(tile);

                    }

                }

            }
            else if (tilesInRange[0].transform.position.z < initialTarget.transform.position.z)
            {

                foreach (TileBehaviour tile in tilesInRange)
                {

                    if (tilesInRange[0].transform.position.x > tile.transform.position.x &&
                        tilesInRange[0].transform.position.z < tile.transform.position.z)
                    {

                        potentialTargetsForLightningStrikes.Add(tile);

                    }

                }

            }
            else if (tilesInRange[0].transform.position.z > initialTarget.transform.position.z)
            {

                foreach (TileBehaviour tile in tilesInRange)
                {

                    if (tilesInRange[0].transform.position.x > tile.transform.position.x &&
                        tilesInRange[0].transform.position.z > tile.transform.position.z)
                    {

                        potentialTargetsForLightningStrikes.Add(tile);

                    }

                }

            }

        }

        Debug.Log(potentialTargetsForLightningStrikes);

        return potentialTargetsForLightningStrikes;

    }


    /// <summary>
    /// Calls wind rune effect
    /// </summary>
    /// <param name="target"> tile that the player has selected </param>
    public void SelectWindRune(TileBehaviour target)
    {

        float damageDealt = Mathf.Ceil(storedData.RuneDamage * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier
            * FindFirstObjectByType<PlayerStats>().BaseAttackMultiplier);

        GameObject VFX;

        switch (storedData.NumberOnSkillTree)
        {

            //knocks adjacent enemies backwards and damages them
            case (1):

                VFX = Instantiate(storedData.RuneVFX, target.transform);

                RangeCheck(true, 2, target);

                TileBehaviour[] tiles = FindObjectsByType<TileBehaviour>(FindObjectsSortMode.None);
                List<TileBehaviour> enemies = tiles.ToList();

                foreach (TileBehaviour tile in tilesInRange)
                {

                    if (tile.GetComponentInChildren<Enemy>() != null)
                    {

                        tile.GetComponentInChildren<Enemy>().Damage(damageDealt);

                        if (tile != target)
                        {

                            SendEnemyBackwards(target, tile);

                        }

                        else
                        {

                            CheckRuneCombination(tile.GetComponentInChildren<Enemy>());

                        }

                    }

                }

                EndPlayerAttackPhase();

                break;

            //targets an opponent for moderate damage
            //has a chance to hit twice
            case (2):

                if(target.gameObject.GetComponentInChildren<Enemy>() != null)
                {

                    target.GetComponentInChildren<Enemy>().Damage(damageDealt);

                    CheckRuneCombination(target.GetComponentInChildren<Enemy>());

                    VFX = Instantiate(storedData.RuneVFX, target.transform);


                    //TODO: redo math based on design input/potential luck stat???
                    if(Random.value <= storedData.RuneSecondaryEffectChance)
                    {

                        target.GetComponentInChildren<Enemy>().Damage(damageDealt);

                    }

                    EndPlayerAttackPhase();

                }

                break;

            //creates a shield on the player's tile
            case (3):

                if(target.GetComponentInChildren<PlayerBehavior>() != null)
                {

                    ShieldBehavior newShield = target.gameObject.AddComponent<ShieldBehavior>();

                    if (debugText != null)
                    {

                        debugText.text = "Shield added!";

                    }

                    newShield.OnShieldGenerated(target.transform, storedData.RuneVFX);

                    EndPlayerAttackPhase();

                }

                break;

            //delays target's turn and damages surrounding enemies
            //ASK DESIGN IF THE PLAYER MUST CHOOSE
            case (4):

                VFX = Instantiate(storedData.RuneVFX, target.transform);

                if (target.GetComponentInChildren<Enemy>() != null)
                {

                    target.GetComponentInChildren<Enemy>().DelayedTurnStatus(true);

                    target.GetComponentInChildren<Enemy>().Damage(damageDealt);

                    CheckRuneCombination(target.GetComponentInChildren<Enemy>());

                }

                RangeCheck(true, 3, target);

                //do NOT delete this list again, jay. it's here for a reason
                List<TileBehaviour> validEnemies = new List<TileBehaviour>();

                foreach(TileBehaviour tile in tilesInRange)
                {

                    if(tile == target)
                    {

                        continue;

                    }

                    if(tile.GetComponentInChildren<Enemy>() != null)
                    {

                        validEnemies.Add(tile);

                    }

                }

                if (validEnemies.Count > 0)
                {

                    for (int i = 0; i < validEnemies.Count; i++)
                    {

                        validEnemies[i].GetComponentInChildren<Enemy>().Damage
                            (Mathf.CeilToInt(damageDealt / validEnemies.Count));

                    }

                }

                EndPlayerAttackPhase();

                break;

        }

    }



    /// <summary>
    /// finds the tile in the opposite direction from the player adjacent to an enemy and moves them there
    /// sorry if this is a fucked way of going about this. i'm just glad this works
    /// THIS FUNCTION SHOULD BE MOVED. RETURN TO LATER.
    /// </summary>
    /// <param name="originalTarget"> original tile that the player had targeted </param>
    /// <param name="newTarget"> the enemy getting blown back </param>
    void SendEnemyBackwards(TileBehaviour originalTarget, TileBehaviour newTarget)
    {

        List<Vector2Int> adjacentTiles = GridManager.GetAllValidAdjacentTiles(newTarget.IndexInGrid, newTarget.IndexInGrid);

        List<TileBehaviour> adjacentTileBehaviours = new List<TileBehaviour>();

        foreach (Vector2Int tile in adjacentTiles.ToList())
        {

            adjacentTileBehaviours.Add(GridManager.combatGrid[tile.x, tile.y]);

        }

        TileBehaviour currentTile = newTarget;

        for(int i = 0; i < adjacentTileBehaviours.Count; i++)
        {

            if (originalTarget.transform.position.x > currentTile.transform.position.x &&
                adjacentTileBehaviours[i].transform.position.x < currentTile.transform.position.x)
            {

                currentTile = adjacentTileBehaviours[i];

            }
            else if(originalTarget.transform.position.x < currentTile.transform.position.x &&
                adjacentTileBehaviours[i].transform.position.x > currentTile.transform.position.x)
            {

                currentTile = adjacentTileBehaviours[i];

            }

            if(originalTarget.transform.position.z > currentTile.transform.position.z &&
                adjacentTileBehaviours[i].transform.position.z < currentTile.transform.position.z)
            {

                currentTile = adjacentTileBehaviours[i];

            }
            else if(originalTarget.transform.position.z < currentTile.transform.position.z &&
                adjacentTileBehaviours[i].transform.position.z > currentTile.transform.position.z)
            {

                currentTile = adjacentTileBehaviours[i];

            }

        }

        //this part is temporary?? i'm assuming that obstacles will be handled differently later
 
        if(currentTile.GetComponentInChildren<Rigidbody>() != null)
        {

            GameObject tileOccupant = currentTile.GetComponentInChildren<Rigidbody>().gameObject;

            if(tileOccupant.tag == "Obstacle")
            {

                return;

            }
            else if (tileOccupant.GetComponent<Enemy>() != null &&
                !tilesInRange.Contains(tileOccupant.GetComponentInParent<TileBehaviour>()))
            {

                SendEnemyBackwards(originalTarget, tileOccupant.GetComponentInParent<TileBehaviour>());

            }

        }

        Enemy relocatedEnemy = newTarget.GetComponentInChildren<Enemy>();

        relocatedEnemy.gameObject.transform.parent = currentTile.transform;

        relocatedEnemy.transform.localPosition = new Vector3(0, 0, 0);

    }

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

        //PART 1: FINDING TARGETS

        //until we get actual vfx for this i'm leaving it blank because it will be sooooo cluttered
        int radius = 2;

        TileBehaviour[] tiles = FindObjectsByType<TileBehaviour>(FindObjectsSortMode.None);
        List<TileBehaviour> validEnemies = new List<TileBehaviour>();

        foreach (TileBehaviour tile in tiles)
        {

            if (tile == enemy.GetComponentInParent<TileBehaviour>())
            {

                continue;

            }

            if (Mathf.RoundToInt(Vector2.Distance(enemy.transform.position, tile.transform.position) / 2) <= radius &&
               tile.GetComponentInChildren<Enemy>() != null)
            {

                validEnemies.Add(tile);

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

            validEnemies[i].GetComponentInChildren<Enemy>().Damage(lightningDamage);

        }

        if(lightningMastered)
        {

            if(enemy != null)
            {

                enemy.Damage(lightningMasteredDamage);

            }

            for (int i = 0; i < validEnemies.Count; i++)
            {

                if (validEnemies[i] != null)
                {

                    validEnemies[i].GetComponentInChildren<Enemy>().Damage(lightningDamage);

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

        if(enemy != null)
        {

            enemy.Damage(windPrimaryDamage);

        }

        for (int i = 0; i < validEnemies.Count; i++)
        {


            if (validEnemies[i] != null)
            {

                validEnemies[i].GetComponentInChildren<Enemy>().Damage(windSecondaryDamage);

            }

        }

        if(windMastered)
        {

            FindFirstObjectByType<PlayerStats>().AddTempHealth(windTempHealth);

        }

    }


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

        if(debugText != null)
        {

            debugText.text = "";

        }

        if(debugComboText != null)
        {

            debugComboText.text = "";

        }

    }

    #endregion RUNE EVENTS

}
