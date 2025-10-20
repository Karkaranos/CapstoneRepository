/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/19/2025
Date Last Modified : 	10/19/2025
Brief Description : 		Handler for running the enemy 
                    state machines one after another
External Resources : 	
***************************************************/
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    public static EnemyHandler Instance { get; private set; }

    private Enemy[] enemies;

    /// <summary>
    /// Make sure that this is a Singleton 
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Invoke("SetEnemyList", 2);
    }

    private void SetEnemyList()
    {
        enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
    }
}
