/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/19/2025
Date Last Modified : 	11/25/2025
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
    public List<Enemy> enemies = new List<Enemy>();
    private static int index = 0;
    private bool isPlayerDead;

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
        PublicEvents.NewLevel += GetNewEnemies;
        
        PublicEvents.HideDamagePreview += HideDamagePreview;
    }

    /// <summary>
    /// Unsubscribe to begin enemy turn
    /// </summary>
    private void OnDisable()
    {
        TurnPublicEvents.BeginEnemyTurn -= RunNextEnemyTurn;
        PublicEvents.NewLevel -= GetNewEnemies;
        PublicEvents.HideDamagePreview -= HideDamagePreview;
    }

    /// <summary>
    /// Run the next enemy in the lists turn 
    /// If we've gone through the list inform turn manager that EnemyHandler is done
    /// Set the player turn indicator to false
    /// Set previous enemy indicator to false set current to true
    /// </summary>
    public void RunNextEnemyTurn()
    {
        bool nullcheck = false;
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] == null)
            {
                nullcheck = true;
            }
        }
        if (nullcheck)
        {
            NullCheckWholeEnemyList(0);
        }

        if (enemies.Count > 0 && !isPlayerDead)
        {
            if (index == 0)
            {
                GridManager.ClearGhostEntities();
                //GridManager.RemoveHighlight();
            }

            if (index <= enemies.Count - 1)
            {
                if (enemies[index] == null)
                {
                    EnemyNullCheck(index);
                }
            }
            

            if (enemies[0].playerStats.turnIndicator.activeSelf)
            {
                enemies[0].playerStats.turnIndicator.SetActive(false);
            }

            if (index == enemies.Count)
            {
                try
                {
                    enemies[index - 1].turnIndicator.SetActive(false);
                    index = 0;
                    TurnPublicEvents.TurnActionComplete();
                    enemies[0].playerStats.turnIndicator.SetActive(true);
                }
                catch { }
                
                return;
            }

            if (index != 0)
            {
                enemies[index - 1].turnIndicator.SetActive(false);
            }

            
            enemies[index].turnIndicator.SetActive(true);
            enemies[index].StartEnemyTurn();
            ++index;
        }
        
    }

    /// <summary>
    /// If enemy is null remove from enemies list 
    /// Check if next enemy is null
    /// repeat
    /// </summary>
    /// <param name="index"></param>
    private void EnemyNullCheck(int index)
    {
        if (enemies.Count > 0)
        {
            if (enemies[index] == null)
            {
                enemies.Remove(enemies[index]);
                EnemyNullCheck(index);
            }
        }
        
    }

    /// <summary>
    /// clears out all the nulls from the enemy list
    /// </summary>
    /// <param name="index"></param>
    private void NullCheckWholeEnemyList(int index)
    {
        if (enemies.Count > 0 && index < enemies.Count)
        {
            if (enemies[index] == null)
            {
                enemies.Remove(enemies[index]);
                NullCheckWholeEnemyList(index);
            }
            else
            {
                NullCheckWholeEnemyList(index + 1);
            }
        }
    }

    /// <summary>
    /// Removes the enemy from the list of enemies 
    /// Checks if all enemies are dead if they are end battle
    /// </summary>
    /// <param name="enemy"></param>
    public void RemoveEnemy(Enemy enemy, Enemy.DamageType dType)
    {
        bool nullcheck = false;
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] == null)
            {
                nullcheck = true;
            }
        }
        if (nullcheck)
        {
            NullCheckWholeEnemyList(0);
        }

        enemies.Remove(enemy);
        if (index > 0)
        {
            --index;
        }

        if(enemies.Count == 0 )
        {
            //TODO: End Level logic
            EndLevelMenu endLevelMenu = FindFirstObjectByType<EndLevelMenu>();
            //endLevelMenu.SetText("You Beat the Level!");
            //endLevelMenu.SetNextLevelButton(true);
            bool callInstant = (dType == Enemy.DamageType.Lightning || dType == Enemy.DamageType.Wind);
            endLevelMenu.EnableEndMenuUi(true, callInstant);
        }
    }

    private void GetNewEnemies()
    {
        isPlayerDead = false;
        index = 0;
        enemies.Clear();
        SetEnemyList();
    }

    /// <summary>
    /// Hide's the damage preview of enemies
    /// </summary>
    private void HideDamagePreview()
    {
        foreach(Enemy enemy in enemies)
        {
            if(enemy.isShowingPreview)
            {
                enemy.HideDamagePreivew();
            }
        }
    }

    /// <summary>
    /// called when the player dies
    /// </summary>
    public void PlayerDied()
    {
        isPlayerDead = true;
    }
}
