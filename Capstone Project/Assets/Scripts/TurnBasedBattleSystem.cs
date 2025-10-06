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

public enum BattleStates { Start, PlayerTurn, EnemyTurn, Won, Loss }

public class TurnBasedBattleSystem : MonoBehaviour
{
    public ButtonManager buttonManager;
    public GameObject player;
    public GameObject enemy;
    public BattleStates State;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonManager = FindFirstObjectByType<ButtonManager>();
        State = BattleStates.Start;
        SetUpBattle();
    }

    IEnumerator SetUpBattle()
    {
        GameObject playerStart = Instantiate(player, playerBattleStation);
        playerStart.GetComponent<PlayerBehavior>();

        Instantiate(enemy, enemyBattleStation);

        State = BattleStates.PlayerTurn;
        PlayerTurn();

        yield return new WaitForSeconds(5f);

        State = BattleStates.EnemyTurn;
    }

    IEnumerator PlayerMove()
    {
        //Checks to see if player has moved
        if (buttonManager.confirmButtonClicked)
        {
            Debug.Log("PlayerHasMoved!");

            yield return new WaitForSeconds(5f);
        }
    }


    void PlayerTurn()
    {
        if (!buttonManager.playerIsGoingToMove)
        {
            buttonManager.playerIsGoingToMove = true;
        }
    }

}
