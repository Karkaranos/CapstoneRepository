/*************************************************
Author Names : 	Jay Embry
Date Created : 	10/07/2025
Date Last Modified : 10/22/2025
Brief Description : Contains rune types and effects
External Resources : 	
	***************************************************/

using TMPro;
using UnityEngine;
using NaughtyAttributes;

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


    //test
    [Button("Attack Tile Test")]
    public void AttackTileTest()
    {

        waitingForThePlayer = true;
        TargetSelectedTile(tileBehaviour);

    }

    #endregion TESTING


    #region RUNE EVENTS

    /// <summary>
    /// Prepares the rune that the player chooses to attack with
    /// </summary>
    /// <param name="runeType"> Rune element </param>
    /// <param name="runeNumber"> Rune's number on the skill tree </param>
    /// <param name="runeDamage"> How much damage the rune is supposed to do </param>
    /// <param name="runeRange"> How close the player has to be to their target </param>
    /// <param name="runeVFX"> Rune's visual effect </param>
    public void StoreSelectedRuneData(RuneType runeType, int runeNumber, float runeDamage, int runeRange, GameObject runeVFX)
    {

        waitingForThePlayer = true;

        storedRuneType = runeType;
        storedRuneNumber = runeNumber;
        storedRuneDamage = runeDamage;
        storedRuneRange = runeRange;
        storedRuneVFX = runeVFX;

        temp.text = "Select a target!";

    }


    /// <summary>
    /// Checks if the selected tile has an enemy in it
    /// If it does, the player's selected rune will target the enemy on the selected tile
    /// </summary>
    /// <param name="tile"> tile that the player has selected </param>
    public void TargetSelectedTile(TileBehaviour tile)
    {

        int distance = Mathf.RoundToInt(Vector2.Distance(tile.transform.position, GridManager.playerPosition));

        if (waitingForThePlayer && 
            (distance/2) <= storedRuneRange &&
            tile.gameObject.GetComponentInChildren<Enemy>() != null)
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

                    waitingForThePlayer = false;

        }

    }

    /// <summary>
    /// Calls lightning rune effect
    /// </summary>
    /// <param name="target"> tile that the player has selected </param>
    public void SelectLightningRune(TileBehaviour target)
    {

        switch (storedRuneNumber)
        {

            case (1):

                target.GetComponentInChildren<Enemy>().Damage
                    (storedRuneDamage * FindFirstObjectByType<PlayerStats>().lightningAttackMultiplier);

                GameObject vfx = Instantiate(storedRuneVFX, target.transform);
                vfx.GetComponentInChildren<TextMeshPro>().text = 
                    (storedRuneDamage * FindFirstObjectByType<PlayerStats>().lightningAttackMultiplier).ToString();
                break;

            case (2):

                FindSecondaryTarget(target);

                target.GetComponentInChildren<Enemy>().Damage
                    (storedRuneDamage * FindFirstObjectByType<PlayerStats>().lightningAttackMultiplier);

                vfx = Instantiate(storedRuneVFX, target.transform);
                vfx.GetComponentInChildren<TextMeshPro>().text =
                    (storedRuneDamage * FindFirstObjectByType<PlayerStats>().lightningAttackMultiplier).ToString();


                secondaryTarget.GetComponentInChildren<Enemy>().Damage
                    (storedRuneDamage * FindFirstObjectByType<PlayerStats>().lightningAttackMultiplier);

                vfx = Instantiate(storedRuneVFX, secondaryTarget.transform);
                vfx.GetComponentInChildren<TextMeshPro>().text =
                    (storedRuneDamage * FindFirstObjectByType<PlayerStats>().lightningAttackMultiplier).ToString();

                Debug.Log(secondaryTarget);

                break;

            case (3):

                int radius = 3;

                target.GetComponentInChildren<Enemy>().Damage
                    (storedRuneDamage * FindFirstObjectByType<PlayerStats>().lightningAttackMultiplier);
                vfx = Instantiate(storedRuneVFX, target.transform);
                vfx.GetComponentInChildren<TextMeshPro>().text =
                    (storedRuneDamage * FindFirstObjectByType<PlayerStats>().lightningAttackMultiplier).ToString();

                TileBehaviour[] enemies = FindObjectsByType<TileBehaviour>(FindObjectsSortMode.None);

                foreach (TileBehaviour enemy in enemies)
                {

                    if(enemy == target)
                    {

                        continue;

                    }

                    if ((Vector2.Distance(target.transform.position, enemy.transform.position)/2) <= radius &&
                        enemy.GetComponentInChildren<Enemy>() != null)
                    {

                        //hardcoding this feels bad i can change this later
                        enemy.GetComponentInChildren<Enemy>().Damage
                            (15 * FindFirstObjectByType<PlayerStats>().lightningAttackMultiplier);

                        vfx = Instantiate(storedRuneVFX, enemy.transform);
                        vfx.GetComponentInChildren<TextMeshPro>().text =
                            (15 * FindFirstObjectByType<PlayerStats>().lightningAttackMultiplier).ToString();

                        Debug.Log(enemy);

                    }

                }

                break;

            case (4):

                target.GetComponentInChildren<Enemy>().Damage
                    (storedRuneDamage * FindFirstObjectByType<PlayerStats>().lightningAttackMultiplier);

                vfx = Instantiate(storedRuneVFX, target.transform);
                vfx.GetComponentInChildren<TextMeshPro>().text =
                    (storedRuneDamage * FindFirstObjectByType<PlayerStats>().lightningAttackMultiplier).ToString();
                break;

            default:
                break;
        }

        //delete later
        temp.text = "You used Lightning " + storedRuneNumber + " for " + storedRuneDamage + " damage!";

        if (PublicEvents.EnemyTurnStarted != null)
        {

            PublicEvents.EnemyTurnStarted();

        }

        playerMenu.SetActive(true);
        this.gameObject.SetActive(false);

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

        switch (storedRuneNumber)
        {

            case (1):

                int radius = 3;

                TileBehaviour[] enemies = FindObjectsByType<TileBehaviour>(FindObjectsSortMode.None);

                foreach (TileBehaviour enemy in enemies)
                {

                    if (Vector2.Distance(target.transform.position, enemy.transform.position) <= radius &&
                        target.GetComponentInChildren<Enemy>() != null)
                    {

                        target.GetComponentInChildren<Enemy>().Damage
                            (storedRuneDamage * FindFirstObjectByType<PlayerStats>().windAttackMultiplier);
                        //PUSH THEM BACK

                    }

                }
                break;

            case (2):

                target.GetComponentInChildren<Enemy>().Damage
                    (storedRuneDamage * FindFirstObjectByType<PlayerStats>().windAttackMultiplier);
                break;

            case (3):

                //gulp
                break;

            case (4):

                int tornadoRadius = 1;

                //MAKE THEM LOSE A TURN
                target.GetComponentInChildren<Enemy>().Damage
                    (storedRuneDamage * FindFirstObjectByType<PlayerStats>().windAttackMultiplier);

                TileBehaviour[] adjacentEnemies = FindObjectsByType<TileBehaviour>(FindObjectsSortMode.None);

                foreach (TileBehaviour enemy in adjacentEnemies)
                {

                    if (Vector2.Distance(target.transform.position, enemy.transform.position) <= tornadoRadius &&
                        target.GetComponentInChildren<Enemy>() != null)
                    {

                        enemy.GetComponentInChildren<Enemy>().Damage
                            (storedRuneDamage * FindFirstObjectByType<PlayerStats>().windAttackMultiplier);

                    }

                }
                break;

        }

        //delete later
        Logger.Log("You used Wind " + storedRuneNumber + "!", false);
        temp.text = "You used Wind " + storedRuneNumber + "!";

        if (PublicEvents.EnemyTurnStarted != null)
        {

            PublicEvents.EnemyTurnStarted();

        }

        playerMenu.SetActive(true);
        this.gameObject.SetActive(false);

    }

    #endregion RUNE EVENTS

}
