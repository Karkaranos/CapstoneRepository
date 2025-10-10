/*************************************************
Author Names : 		    Aidan Ratcliffe, Cade Naylor
Date Created : 		    10/6/2025
Date Last Modified : 	10/10/2025
Brief Description : 	A script to hold the BattleSystem
External Resources : 	https://youtu.be/_1pz_ohupPs
***************************************************/
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public enum battleStates { Start, PlayerTurn, EnemyTurn, Won, Loss }

public class TurnBasedBattleSystem : MonoBehaviour
{
    public ButtonManager buttonManager;
    public GameObject player;
    public GameObject enemy;
    public battleStates State;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;
    [SerializeField] private List<GameObject> playerMenus = new List<GameObject>(); //should be removed and relocated elsewhere

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonManager = FindFirstObjectByType<ButtonManager>();
        State = battleStates.Start;
        //SetUpBattle();
    }

    private void OnEnable()
    {
        TurnPublicEvents.BeginPlayerTurn += PlayerTurn;
    }

    private void OnDisable()
    {
        TurnPublicEvents.BeginPlayerTurn -= PlayerTurn;
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
        yield return null;
        //yield return new WaitForSeconds(5f);

        //State = battleStates.EnemyTurn;
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
        foreach (GameObject g in playerMenus)
        {
            g.SetActive(true);
        }


        if (State != battleStates.PlayerTurn)
        {
           // StartCoroutine(PlayerChoice());
        }
    }


    public bool EnemyTurn()
    {
       // buttonManager.playerCanvas.gameObject.SetActive(false);
       
        

        //ok i know hardcoding is abd. this should be ripped out and removed elsewhere. i just want the canvas to disappear for now
        foreach (GameObject g in playerMenus)
        {
            g.SetActive(false);
        }
        return true;
    }

    public bool PlayerTurnTime()
    {
        foreach (GameObject g in playerMenus)
        {
            g.SetActive(true);
        }
        return true;
    }

}
