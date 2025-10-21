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

public class RuneEvents : MonoBehaviour
{

    //for waiting on player input
    bool waitingForThePlayer;

    #region INITIALIZATION

    //temp value for player communication
    [SerializeField] TMP_Text temp;

    //for menu-swapping purposes
    [SerializeField] GameObject playerMenu;

    /// <summary>
    /// Runs whenever this script is loaded into a scene
    /// </summary>
    private void OnEnable()
    {

        PublicEvents.RuneSelected += UseSelectedRune;

    }

    /// <summary>
    /// Runs whenever this script is destroyed
    /// </summary>
    private void OnDisable()
    {

        PublicEvents.RuneSelected += UseSelectedRune;

    }

    #endregion INITIALIZATION


    #region RUNE EVENTS

    private RuneType storedRuneType;
    private int storedRuneNumber;
    private float storedRuneDamage;
    private int storedRuneRange;

    /// <summary>
    /// Stores data for when the player selects an opponent to attack with a rune
    /// </summary>
    /// /// <param name="runeType"> Grabs which rune this is </param>
    /// <param name="runeNumber"> Grabs where this rune is on the skill tree </param>
    public void UseSelectedRune(RuneType runeType, int runeNumber, float runeDamage, int runeRange)
    {

        waitingForThePlayer = true;

        storedRuneType = runeType;
        storedRuneNumber = runeNumber;
        storedRuneDamage = runeDamage;
        storedRuneRange = runeRange;

        temp.text = "Select a target!";

    }

    private void OnMouseDown()
    {

        Vector3 mousePos = Input.mousePosition;

        if (waitingForThePlayer)
        {

            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {

                if (hit.transform.gameObject.GetComponent<Enemy>() != null &&
                    Vector2.Distance(hit.transform.position, GridManager.playerPosition) <= storedRuneRange)
                {

                    switch (storedRuneType)
                    {

                        case (RuneType.Lightning):

                            SelectLightningRune(hit.transform.gameObject.GetComponent<Enemy>());
                            break;

                        case (RuneType.Wind):

                            SelectWindRune(hit.transform.gameObject.GetComponent<Enemy>());
                            break;

                    }

                    waitingForThePlayer = false;

                }
                else
                {

                    temp.text = "Attack cancelled! Was the enemy in range?";

                }

            }

        }

    }


    /// <summary>
    /// Calls lightning rune effect
    /// </summary>
    public void SelectLightningRune(Enemy target)
    {

        switch(storedRuneNumber)
        {

            case (1):

                target.Damage(storedRuneDamage);
                break;

            case (2):

                FindSecondaryTarget(target);

                target.Damage(storedRuneDamage);
                secondaryTarget.Damage(storedRuneDamage);
                break;

            case (3):

                int radius = 3;

                target.Damage(storedRuneDamage);

                Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

                foreach (Enemy enemy in enemies)
                {

                    if(Vector2.Distance(target.transform.position, enemy.transform.position) <= radius)
                    {

                        //hardcoding this feels bad i can change this later
                        enemy.Damage(15);

                    }

                }

                break;

            case (4):

                target.Damage(storedRuneDamage);
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

    Enemy secondaryTarget;

    Enemy FindSecondaryTarget(Enemy target)
    {

        Enemy[] otherEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        //Transform[] enemyPositions = new Transform[otherEnemies.Length];

        //for (int i = 0; i < otherEnemies.Length; i++)
        //{

        //    enemyPositions[i] = otherEnemies[i].transform;

        //}

        float closestDistance = Mathf.Infinity;
        Vector3 primaryTargetPos = target.gameObject.transform.position;

        foreach(Enemy potentialTarget in otherEnemies)
        {

            Vector3 dir = potentialTarget.gameObject.transform.position - primaryTargetPos;
            float distanceFromTarget = dir.sqrMagnitude;

            if(distanceFromTarget < closestDistance)
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
    public void SelectWindRune(Enemy target)
    {

        switch (storedRuneNumber)
        {

            case (1):

                int radius = 3;

                Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

                foreach (Enemy enemy in enemies)
                {

                    if (Vector2.Distance(target.transform.position, enemy.transform.position) <= radius)
                    {

                        enemy.Damage(storedRuneDamage);
                        //PUSH THEM BACK

                    }

                }
                break;

            case (2):

                target.Damage(storedRuneDamage);
                break;

            case (3):

                //gulp
                break;

            case (4):

                int tornadoRadius = 1;

                //MAKE THEM LOSE A TURN
                target.Damage(storedRuneDamage);

                Enemy[] adjacentEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

                foreach (Enemy enemy in adjacentEnemies)
                {

                    if (Vector2.Distance(target.transform.position, enemy.transform.position) <= tornadoRadius)
                    {

                        enemy.Damage(storedRuneDamage);

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
