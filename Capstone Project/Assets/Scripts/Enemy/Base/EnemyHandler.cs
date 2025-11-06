/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/19/2025
Date Last Modified : 	11/2/2025
Brief Description : 		Handler for running the enemy 
                    state machines one after another
External Resources : 	
***************************************************/
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    [HideInInspector]public static EnemyHandler Instance { get; private set; }
    private List<Enemy> enemies = new List<Enemy>();
    private static int index = 0;

    /// <summary>
    /// Make sure that this is a Singleton 
    /// </summary>
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Invoke("SetEnemyList", 2);
    }


    /// <summary>
    /// Set list of all enabled enemies 
    /// public function so it can be called later with the 
    /// multiple grid loading in the same scene
    /// </summary>
    public void SetEnemyList()
    {
        Enemy[] array = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy enemy in array)
        {
            enemies.Add(enemy);
        }
    }

    /// <summary>
    /// Subscribe to begin enemy turn
    /// </summary>
    private void OnEnable()
    {
        TurnPublicEvents.BeginEnemyTurn += RunNextEnemyTurn;
    }

    /// <summary>
    /// Unsubscribe to begin enemy turn
    /// </summary>
    private void OnDisable()
    {
        TurnPublicEvents.BeginEnemyTurn -= RunNextEnemyTurn;
    }

    /// <summary>
    /// Run the next enemy in the lists turn 
    /// If we've gone through the list inform turn manager that EnemyHandler is done
    /// </summary>
    public void RunNextEnemyTurn()
    {
        if (index == enemies.Count)
        {
            index = 0;
            TurnPublicEvents.TurnActionComplete();
            return;
        }
        enemies[index].StartEnemyTurn();
        ++index;
    }

    /// <summary>
    /// Removes the enemy from the list of enemies 
    /// Checks if all enemies are dead if they are end battle
    /// </summary>
    /// <param name="enemy"></param>
    public void RemoveEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);

        if(enemies.Count == 0 )
        {
            //TODO: End Level logic
            EndLevelMenu endLevelMenu = FindFirstObjectByType<EndLevelMenu>();
            endLevelMenu.EnableEndMenuUi();
            Debug.Log("Level Ended");
        }
    }
}
