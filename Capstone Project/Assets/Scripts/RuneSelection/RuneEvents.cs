/*************************************************
Author Names : 	Jay Embry
Date Created : 	10/07/2025
Date Last Modified : 10/28/2025
Brief Description : Contains rune types and effects
External Resources : 	
	***************************************************/

using Mono.Cecil;
using NaughtyAttributes;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RuneEvents : MonoBehaviour
{

    #region INITIALIZATION

    public enum Prep
    {

        Visuals,
        Testing

    }

    [SerializeField] private Prep currentInspectorShowing;

    //for waiting on player input
    bool waitingForThePlayer;

    //Stores the currently using rune
    private RuneData storedData;

    /// <summary>
    /// Runs whenever this script is loaded into a scene
    /// </summary>
    private void OnEnable()
    {

        PublicEvents.SelectTile += TargetSelectedTile;
        PublicEvents.RuneSelected += StoreSelectedRuneData;

    }

    /// <summary>
    /// Runs whenever this script is destroyed
    /// </summary>
    private void OnDisable()
    {

        PublicEvents.SelectTile -= TargetSelectedTile;
        PublicEvents.RuneSelected -= StoreSelectedRuneData;

    }

    #endregion INITIALIZATION


    #region VISUALS

    [HorizontalLine(4, EColor.Red)]

    [ShowIf(nameof(currentInspectorShowing), Prep.Visuals), SerializeField]
    //for menu-swapping purposes
    GameObject playerMenu;

    #endregion VISUALS


    #region TESTING

    [HorizontalLine(4, EColor.Yellow)]

    [ShowIf(nameof(currentInspectorShowing), Prep.Testing), SerializeField]
    //temp value for player communication
    TMP_Text temp;

    [ShowIf(nameof(currentInspectorShowing), Prep.Testing), SerializeField]
    TileBehaviour tileBehaviour;


    //stores the rune that the player has most recently selected
    //they can be shown for now for testing purposes
    //ideally, these shouldn't be messed with in the future
    [Header("Test Variables")]

    [ShowIf(nameof(currentInspectorShowing), Prep.Testing), SerializeField]
    RuneType storedRuneType;

    [ShowIf(nameof(currentInspectorShowing), Prep.Testing), SerializeField]
    int storedRuneNumber;

    [ShowIf(nameof(currentInspectorShowing), Prep.Testing), SerializeField]
    float storedRuneDamage;

    [ShowIf(nameof(currentInspectorShowing), Prep.Testing), SerializeField]
    int storedRuneRange;

    [ShowIf(nameof(currentInspectorShowing), Prep.Testing), SerializeField]
    GameObject storedRuneVFX;

    [ShowIf(nameof(currentInspectorShowing), Prep.Testing), SerializeField]
    int storedRuneCost;


    //test
    [Button("Attack Tile Test")]
    public void AttackTileTest()
    {

        waitingForThePlayer = true;
        TargetSelectedTile(tileBehaviour);
        storedData = new RuneData(storedRuneType, storedRuneNumber, "Test", "Test Description", storedRuneDamage, storedRuneRange, storedRuneVFX);

    }

    #endregion TESTING


    #region RUNE EVENTS

    /// <summary>
    /// Prepares the rune that the player chooses to attack with
    /// </summary>
    /// <param name="rd"> Rune Data </param>
    public void StoreSelectedRuneData(RuneData rd)
    {

        waitingForThePlayer = true;

        storedRuneType = rd.TypeOfRune;
        storedRuneNumber = rd.NumberOnSkillTree;
        storedRuneDamage = rd.RuneDamage;
        storedRuneRange = rd.RuneRange;
        storedRuneVFX = rd.RuneVFX;
        storedRuneCost = rd.RuneActionPoints;

        storedData = rd;

        //to prevent softlocking FOR NOW
        //playerMenu.SetActive(true);
        //this.gameObject.SetActive(false);

    }


    /// <summary>
    /// Checks if the selected tile has an enemy in it
    /// If it does, the player's selected rune will target the enemy on the selected tile
    /// </summary>
    /// <param name="tile"> tile that the player has selected </param>
    public void TargetSelectedTile(TileBehaviour tile)
    {

        if (waitingForThePlayer &&
            FindFirstObjectByType<GameManager>().CurrentActionPoints >= storedRuneCost)
        {

             switch (storedRuneType)
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
    /// Calls lightning rune effect
    /// </summary>
    /// <param name="target"> tile that the player has selected </param>
    public void SelectLightningRune(TileBehaviour target)
    {

        int distance = Mathf.RoundToInt(Vector2.Distance(target.transform.position, GridManager.playerPosition));
        GameObject vfx;

        switch (storedRuneNumber)
        {

            //targets one opponent for moderate damage
            case (1):

                if(target.gameObject.GetComponentInChildren<Enemy>() != null &&
                    (distance / 2) <= storedRuneRange)
                {

                    target.GetComponentInChildren<Enemy>().Damage
                        (storedRuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier);
                    CheckRuneCombination(target.GetComponentInChildren<Enemy>());

                    vfx = Instantiate(storedRuneVFX, target.transform);
                    vfx.GetComponentInChildren<TextMeshPro>().text =
                        (storedRuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier).ToString();

                    EndPlayerAttackPhase();

                }

                break;

            //targets two opponents
            //one is directly targeted, and the other is the closest to the original target
            case (2):

                if (target.gameObject.GetComponentInChildren<Enemy>() != null &&
                    (distance / 2) <= storedRuneRange)
                {

                    FindSecondaryTarget(target);

                    target.GetComponentInChildren<Enemy>().Damage
                        (storedRuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier);
                    CheckRuneCombination(target.GetComponentInChildren<Enemy>());

                    vfx = Instantiate(storedRuneVFX, target.transform);
                    vfx.GetComponentInChildren<TextMeshPro>().text =
                        (storedRuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier).ToString();


                    secondaryTarget.GetComponentInChildren<Enemy>().Damage
                        (storedRuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier);
                    CheckRuneCombination(secondaryTarget.GetComponentInChildren<Enemy>());

                    vfx = Instantiate(storedRuneVFX, secondaryTarget.transform);
                    vfx.GetComponentInChildren<TextMeshPro>().text =
                        (storedRuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier).ToString();

                    EndPlayerAttackPhase();

                }

                break;

            //targets one opponent and all other opponents in range for less damage
            case (3):


                if (target.gameObject.GetComponentInChildren<Enemy>() != null &&
                    (distance / 2) <= storedRuneRange)
                {

                    int radius = 3;

                    target.GetComponentInChildren<Enemy>().Damage
                        (storedRuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier);
                    CheckRuneCombination(target.GetComponentInChildren<Enemy>());

                    vfx = Instantiate(storedRuneVFX, target.transform);
                    vfx.GetComponentInChildren<TextMeshPro>().text =
                        (storedRuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier).ToString();

                    TileBehaviour[] enemies = FindObjectsByType<TileBehaviour>(FindObjectsSortMode.None);

                    foreach (TileBehaviour enemy in enemies)
                    {

                        if (enemy == target)
                        {

                            continue;

                        }

                        if ((Vector2.Distance(target.transform.position, enemy.transform.position) / 2) <= radius &&
                            enemy.GetComponentInChildren<Enemy>() != null)
                        {

                            //hardcoding this feels bad i can change this later
                            enemy.GetComponentInChildren<Enemy>().Damage
                                (15 * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier);
                            CheckRuneCombination(enemy.GetComponentInChildren<Enemy>());

                            vfx = Instantiate(storedRuneVFX, enemy.transform);
                            vfx.GetComponentInChildren<TextMeshPro>().text =
                                (15 * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier).ToString();

                        }

                    }

                    EndPlayerAttackPhase();

                }

                break;

            //targets one opponent for a large amount of damage
            case (4):

                if(target.gameObject.GetComponentInChildren<Enemy>() != null &&
                    (distance / 2) <= storedRuneRange)
                {

                    target.GetComponentInChildren<Enemy>().Damage
                        (storedRuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier);
                    CheckRuneCombination(target.GetComponentInChildren<Enemy>());

                    vfx = Instantiate(storedRuneVFX, target.transform);
                    vfx.GetComponentInChildren<TextMeshPro>().text =
                        (storedRuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier).ToString();

                    EndPlayerAttackPhase();

                }

                break;

            default:

                break;
        }

        PublicEvents.RuneCast(storedRuneCost);

    }

    //variable that stores the enemy that's closest to the target
    //for lightning 2
    TileBehaviour secondaryTarget;

    /// <summary>
    /// Calls lightning rune effect
    /// </summary>
    /// <param name="target"> tile that the player has selected </param>
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


    /// <summary>
    /// Calls wind rune effect
    /// </summary>
    /// <param name="target"> tile that the player has selected </param>
    public void SelectWindRune(TileBehaviour target)
    {

        int distance = Mathf.RoundToInt(Vector2.Distance(target.transform.position, GridManager.playerPosition));
        GameObject vfx;
        int radius;

        switch (storedRuneNumber)
        {

            //knocks adjacent enemies backwards and damages them
            case (1):

                if ((distance / 2) <= storedRuneRange)
                {

                    radius = 2;

                    TileBehaviour[] tiles = FindObjectsByType<TileBehaviour>(FindObjectsSortMode.None);

                    List<TileBehaviour> enemies = tiles.ToList();

                    foreach (TileBehaviour enemy in tiles)
                    {

                        if ((Vector2.Distance(target.transform.position, enemy.transform.position) / 2) <= radius &&
                            enemy.GetComponentInChildren<Enemy>() != null)
                        {

                            enemy.GetComponentInChildren<Enemy>().Damage
                                (storedRuneDamage * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier);
                            CheckRuneCombination(enemy.GetComponentInChildren<Enemy>());

                            vfx = Instantiate(storedRuneVFX, enemy.transform);
                            vfx.GetComponentInChildren<TextMeshPro>().text =
                                (storedRuneDamage * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier).ToString();

                            //moves enemy backwards
                            if(enemy != target)
                            {

                                SendEnemyBackwards(target, enemy, enemies);

                            }

                        }

                    }

                    EndPlayerAttackPhase();

                }

                break;

            //targets an opponent for moderate damage
            //MAYBE it will target another opponent
            //will need more concrete information
            case (2):

                if(target.gameObject.GetComponentInChildren<Enemy>() != null &&
                    (distance / 2) <= storedRuneRange)
                {

                    target.GetComponentInChildren<Enemy>().Damage
                        (storedRuneDamage * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier);
                    CheckRuneCombination(target.GetComponentInChildren<Enemy>());

                    vfx = Instantiate(storedRuneVFX, target.transform);
                    vfx.GetComponentInChildren<TextMeshPro>().text =
                        (storedRuneDamage * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier).ToString();

                    EndPlayerAttackPhase();

                }

                break;

            //creates a shield on the player's tile
            case (3):

                if(target.GetComponentInChildren<PlayerBehavior>() != null)
                {

                    ShieldBehavior newShield = target.gameObject.AddComponent<ShieldBehavior>();

                    newShield.OnShieldGenerated(target.transform, storedRuneVFX);

                    EndPlayerAttackPhase();

                }

                break;

            //delays target's turn and damages surrounding enemies
            case (4):

                if((distance / 2) <= storedRuneRange)
                {

                    List<TileBehaviour> validEnemies = new List<TileBehaviour>();

                    radius = 3;

                    vfx = Instantiate(storedRuneVFX, target.transform);

                    if (target.GetComponentInChildren<Enemy>() != null)
                    {

                        target.GetComponentInChildren<Enemy>().DelayedTurnStatus(true);

                        target.GetComponentInChildren<Enemy>().Damage
                            (storedRuneDamage * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier);
                        CheckRuneCombination(target.GetComponentInChildren<Enemy>());

                        vfx.GetComponentInChildren<TextMeshPro>().text =
                            (storedRuneDamage * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier).ToString();

                    }

                    TileBehaviour[] enemies = FindObjectsByType<TileBehaviour>(FindObjectsSortMode.None);

                    foreach (TileBehaviour enemy in enemies)
                    {

                        if(enemy == target)
                        {

                            continue;

                        }

                        if ((Vector2.Distance(target.transform.position, enemy.transform.position) / 2) <= radius &&
                            enemy.GetComponentInChildren<Enemy>() != null)
                        {

                            validEnemies.Add(enemy);

                        }

                    }

                    if(validEnemies.Count > 0)
                    {

                        for (int i = 0; i < validEnemies.Count; i++)
                        {

                            validEnemies[i].GetComponentInChildren<Enemy>().Damage
                                    (Mathf.RoundToInt((storedRuneDamage * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier) / validEnemies.Count));
                            CheckRuneCombination(target.GetComponentInChildren<Enemy>());

                            vfx = Instantiate(storedRuneVFX, validEnemies[i].transform);
                            vfx.GetComponentInChildren<TextMeshPro>().text =
                                (Mathf.RoundToInt(storedRuneDamage * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier) / validEnemies.Count).ToString();

                        }

                    }

                    EndPlayerAttackPhase();

                }

                break;

        }

    }

    /// <summary>
    /// finds the tile in the opposite direction from the player adjacent to an enemy and moves them there
    /// sorry if this is a fucked way of going about this. i'm just glad this works
    /// </summary>
    /// <param name="originalTarget"> original tile that the player had targeted </param>
    /// <param name="enemy"> the enemy getting blown back </param>
    /// <param name="enemies"> the rest of the tiles with enemies on them </param>
    void SendEnemyBackwards(TileBehaviour originalTarget, TileBehaviour enemy, List<TileBehaviour> enemies)
    {

        Vector3 newTilePos;
        if(originalTarget.transform.position.z != enemy.transform.position.z)
        {

            newTilePos.x = (Mathf.Sign(enemy.transform.position.x - originalTarget.transform.position.x) + enemy.transform.position.x);
            newTilePos.z = ((Mathf.Sign(enemy.transform.position.z - originalTarget.transform.position.z) * 1.5f) + enemy.transform.position.z);

        }
        else
        {

            newTilePos.x = ((Mathf.Sign(enemy.transform.position.x - originalTarget.transform.position.x) * 2) + enemy.transform.position.x);
            newTilePos.z = enemy.transform.position.z;

        }

        newTilePos.y = 0f;

        TileBehaviour newTile = enemies.Find(x => x.transform.position == newTilePos);

        if(newTile == null)
        {

            return;

        }

        if (newTile.GetComponentInChildren<SpriteRenderer>() != null && newTile.GetComponentInChildren<Enemy>() != null)
        {

            if(newTile.GetComponentInChildren<Enemy>() != null)
            {

                SendEnemyBackwards(originalTarget, newTile, enemies);

            }
            else { return; }

        }

        Enemy movedEnemy = enemy.GetComponentInChildren<Enemy>();
        movedEnemy.gameObject.transform.parent = newTile.transform;
        movedEnemy.transform.localPosition  = new Vector3 (0, 0, 0);

    }

    /// <summary>
    /// checks if the enemy has been hit by a spell prior
    /// if so, a combo is triggered
    /// </summary>
    /// <param name="enemy"> target </param>
    void CheckRuneCombination(Enemy enemy)
    {



    }


    /// <summary>
    /// runs whenever an enemy is successfully targeted
    /// made into a function to prevent SOME clutter
    /// </summary>
    void EndPlayerAttackPhase()
    {

        waitingForThePlayer = false;

        if (PublicEvents.EnemyTurnStarted != null)
        {

            PublicEvents.EnemyTurnStarted();

        }

        playerMenu.SetActive(true);
        this.gameObject.SetActive(false);

    }

    #endregion RUNE EVENTS

}
