/*************************************************
Author Names : 		    Aidan Ratcliffe
Date Created : 		    10/6/2025
Date Last Modified : 	10/6/2025
Brief Description : 	A script to hold the BattleSystem
External Resources : 	https://youtu.be/_1pz_ohupPs
***************************************************/
using System;
using System.Collections;
using UnityEngine;

public enum battleStates { Start, PlayerTurn, EnemyTurn, Won, Loss }

public class TurnBasedBattleSystem : MonoBehaviour
{
    public ButtonManager buttonManager;
    public GameObject player;
    public GameObject enemy;
    public battleStates State;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonManager = FindFirstObjectByType<ButtonManager>();
        State = battleStates.Start;
        SetUpBattle();
    }


    IEnumerator SetUpBattle()
    {

        //PublicEvents.StartBattle();

        GameObject playerStart = Instantiate(player, playerBattleStation);
        playerStart.GetComponent<PlayerBehavior>();

        GameObject enemyStart = Instantiate(enemy, enemyBattleStation);
        enemyStart.GetComponent<Enemy>();

        Instantiate(enemy, enemyBattleStation);

        State = battleStates.PlayerTurn;
        PlayerTurn();

        yield return new WaitForSeconds(5f);

        State = battleStates.EnemyTurn;
    }

    IEnumerator PlayerChoice()
    {
        //Checks to see if player's turn has ended
        if (buttonManager.endButtonClicked)
        {
            Debug.Log("PlayerTurnDone!");

            yield return new WaitForSeconds(5f);
            EnemyTurn();
        }
    }


    void PlayerTurn()
    {
        if(State != battleStates.PlayerTurn)
        {
            StartCoroutine(PlayerChoice());
        }
    }

    public bool EnemyTurn()
    {
       // buttonManager.playerCanvas.gameObject.SetActive(false);
       
        PublicEvents.EnemyTurnStarted.Invoke();
        Debug.Log("enemy turn has begun");
        return true;
    }

}
