/*************************************************
Author Names : 	Jay Embry
Date Created : 	10/07/2025
Date Last Modified : 10/09/2025
Brief Description : Contains rune types and effects
External Resources : 	
	***************************************************/

using System.Collections;
using Mono.Cecil;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using PlayerInputActions;
using UnityEngine.Rendering;
using System.Collections.Generic;
using NaughtyAttributes;

public class RuneEvents : MonoBehaviour
{

    #region INITIALIZATION

    [SerializeField] TileBehavior tileBehavior;

    [Button("Attack Tile Test")]
    public void AttackTileTest()
    {

        storedRuneType = RuneType.Lightning;
        storedRuneNumber = 1;
        storedRuneDamage = 40;
        storedRuneRange = 10;
        TargetSelectedTile(tileBehavior);

    }

    //for waiting on player input
    bool waitingForThePlayer;

    //temp value for player communication
    [SerializeField] TMP_Text temp;

    public List<GameObject> runeVisuals = new List<GameObject>();

    //for menu-swapping purposes
    [SerializeField] GameObject playerMenu;

    //[SerializeField] InputAction playerClick;
    //[SerializeField] InputAction playerClickPerformed;

    /// <summary>
    /// Runs whenever this script is loaded into a scene
    /// </summary>
    private void OnEnable()
    {

        //playerClick.Enable();
        //playerClickPerformed.started += playerClickedConfirmed;
        PublicEvents.SelectTile += TargetSelectedTile;
        PublicEvents.RuneSelected += StoreSelectedRuneData;

    }

    /// <summary>
    /// Runs whenever this script is destroyed
    /// </summary>
    private void OnDisable()
    {

        //playerClick.Disable();
        //playerClickPerformed.started -= playerClickedConfirmed;
        PublicEvents.SelectTile -= TargetSelectedTile;
        PublicEvents.RuneSelected -= StoreSelectedRuneData;

    }

    #endregion INITIALIZATION


    #region RUNE EVENTS

    private RuneType storedRuneType;

    private int storedRuneNumber;

    private float storedRuneDamage;

    private int storedRuneRange;

    /// <summary>
    /// Prepares the rune that the player chooses to attack with
    /// </summary>
    /// /// <param name="runeType"> Rune element </param>
    /// <param name="runeNumber"> Rune's number on the skill tree </param>
    /// <param name="runeDamage"> How much damage the rune is supposed to do </param>
    /// <param name="runeRange"> How close the player has to be to their target </param>
    public void StoreSelectedRuneData(RuneType runeType, int runeNumber, float runeDamage, int runeRange)
    {

        waitingForThePlayer = true;

        storedRuneType = runeType;
        storedRuneNumber = runeNumber;
        storedRuneDamage = runeDamage;
        storedRuneRange = runeRange;

        temp.text = "Select a target!";

    }

    //for later
    GameObject visual;

    public void TargetSelectedTile(TileBehavior tile)
    {

        Debug.Log("You made it!");

        if(waitingForThePlayer && 
            Vector2.Distance(tile.transform.position, GridManager.playerPosition) <= storedRuneRange &&
            tile.gameObject.GetComponentInChildren<Enemy>() != null)
        {

             switch (storedRuneType)
                    {

                        case (RuneType.Lightning):

                            //visual = Instantiate(runeVisuals[0]);
                            //visual.transform.position = FindFirstObjectByType<PlayerBehavior>().transform.position;

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

    //work on this later

    IEnumerator LightningRuneAnimation(TileBehavior target)
    {
        int timer = 0;
        Vector3 startingPos = visual.transform.position;

        while (timer < 1)
        {

            visual.transform.position = Vector3.Lerp
                (startingPos, target.GetComponentInChildren<Enemy>().transform.position, timer);
            timer++;

            yield return new WaitForSeconds(1);

        }

        visual.transform.position = target.GetComponentInChildren<Enemy>().transform.position;
        SelectLightningRune(target);

    }



    /// <summary>
    /// Calls lightning rune effect
    /// </summary>
    public void SelectLightningRune(TileBehavior target)
    {

        switch (storedRuneNumber)
        {

            case (1):

                target.GetComponentInChildren<Enemy>().Damage
                    (storedRuneDamage * FindFirstObjectByType<PlayerStats>().lightningAttackMultiplier);
                break;

            case (2):

                FindSecondaryTarget(target);

                target.GetComponentInChildren<Enemy>().Damage
                    (storedRuneDamage * FindFirstObjectByType<PlayerStats>().lightningAttackMultiplier);
                secondaryTarget.GetComponentInChildren<Enemy>().Damage
                    (storedRuneDamage * FindFirstObjectByType<PlayerStats>().lightningAttackMultiplier);
                break;

            case (3):

                int radius = 3;

                target.GetComponentInChildren<Enemy>().Damage
                    (storedRuneDamage * FindFirstObjectByType<PlayerStats>().lightningAttackMultiplier);

                TileBehavior[] enemies = FindObjectsByType<TileBehavior>(FindObjectsSortMode.None);

                foreach (TileBehavior enemy in enemies)
                {

                    if (Vector2.Distance(target.transform.position, enemy.transform.position) <= radius &&
                        enemy.GetComponentInChildren<Enemy>() != null)
                    {

                        //hardcoding this feels bad i can change this later
                        enemy.GetComponentInChildren<Enemy>().Damage
                            (15 * FindFirstObjectByType<PlayerStats>().lightningAttackMultiplier);

                    }

                }

                break;

            case (4):

                target.GetComponentInChildren<Enemy>().Damage
                    (storedRuneDamage * FindFirstObjectByType<PlayerStats>().lightningAttackMultiplier);
                break;

            default:
                break;
        }

        //delete later
        Logger.Log("You used Lightning " + storedRuneNumber + "!", false);
        temp.text = "You used Lightning " + storedRuneNumber + "!";

        if(PublicEvents.EnemyTurnStarted != null)
        {

            PublicEvents.EnemyTurnStarted();

        }

        playerMenu.SetActive(true);
        this.gameObject.SetActive(false);

    }

    TileBehavior secondaryTarget;
    TileBehavior FindSecondaryTarget(TileBehavior target)
    {

        TileBehavior[] otherEnemies = FindObjectsByType<TileBehavior>(FindObjectsSortMode.None);

        float closestDistance = Mathf.Infinity;
        Vector3 primaryTargetPos = target.gameObject.transform.position;

        foreach(TileBehavior potentialTarget in otherEnemies)
        {

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
    public void SelectWindRune(TileBehavior target)
    {

        switch (storedRuneNumber)
        {

            case (1):

                int radius = 3;

                TileBehavior[] enemies = FindObjectsByType<TileBehavior>(FindObjectsSortMode.None);

                foreach (TileBehavior enemy in enemies)
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

                TileBehavior[] adjacentEnemies = FindObjectsByType<TileBehavior>(FindObjectsSortMode.None);

                foreach (TileBehavior enemy in adjacentEnemies)
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
