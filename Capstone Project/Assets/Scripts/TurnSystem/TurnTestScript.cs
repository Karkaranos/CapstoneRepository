/*************************************************
Author Names : 		Tyler Hayes 
Date Created : 		10/9/2025
Date Last Modified : 10/9/2025
Brief Description : Tests the turnmanager script
External Resources : 	
***************************************************/

using NaughtyAttributes;
using UnityEngine;

public class TurnTestScript : MonoBehaviour
{
    //bools to check if this script recieved the message that each phase of the turn has triggered
    public bool StartTurnTriggered;
    public bool PlayerTurnTriggered;
    public bool EnemyTurnTriggered;
    public bool EndTurnTriggered;

    /// <summary>
    /// subscribes to all needed publicevents
    /// </summary>
    private void OnEnable()
    {
        TurnPublicEvents.BeginStartTurn += OnTurnStart;
        TurnPublicEvents.BeginPlayerTurn += OnTurnPlayer;
        TurnPublicEvents.BeginEnemyTurn += OnTurnEnemy;
        TurnPublicEvents.BeginEndTurn += OnTurnEnd;
    }

    /// <summary>
    /// unsubscribes from all needed publicevents
    /// </summary>
    private void OnDisable()
    {
        TurnPublicEvents.BeginStartTurn -= OnTurnStart;
        TurnPublicEvents.BeginPlayerTurn -= OnTurnPlayer;
        TurnPublicEvents.BeginEnemyTurn -= OnTurnEnemy;
        TurnPublicEvents.BeginEndTurn -= OnTurnEnd;
    }

    /// <summary>
    /// Sends a message back to the turn manager that it's completed its task
    /// </summary>
    [Button("Complete Start Turn")]
    private void CompleteStartTurn()
    {
        if (StartTurnTriggered)
        {
            StartTurnTriggered = false;
            Debug.Log("Completing Start Turn");
            TurnPublicEvents.TurnActionComplete();
        }
    }

    /// <summary>
    /// logs when it recieves the turn start
    /// </summary>
    private void OnTurnStart()
    {
        if (!StartTurnTriggered)
        {
            StartTurnTriggered = true;
            Debug.Log("Turn Started");
        }
        
    }


    /// <summary>
    /// Sends back to the turnmanager when its action has completed
    /// </summary>
    [Button("Complete Player Turn")]
    private void CompletePlayerTurn()
    {
        if (PlayerTurnTriggered)
        {
            PlayerTurnTriggered = false;
            Debug.Log("Completing Player Turn");
            TurnPublicEvents.TurnActionComplete();
        }
    }

    /// <summary>
    /// Triggers when the players turn starts
    /// </summary>
    private void OnTurnPlayer()
    {
        if (!PlayerTurnTriggered)
        {
            PlayerTurnTriggered = true;
            Debug.Log("Player Turn Started");
        }

    }

    /// <summary>
    /// sends out when this scripts enemy turn stuff is complete
    /// </summary>
    [Button("Complete Enemy Turn")]
    private void CompleteEnemyTurn()
    {
        if (EnemyTurnTriggered)
        {
            EnemyTurnTriggered = false;
            Debug.Log("Completing Enemy Turn");
            TurnPublicEvents.TurnActionComplete();
        }
    }

    /// <summary>
    /// triggers when the enemy turn starts
    /// </summary>
    private void OnTurnEnemy()
    {
        if (!EnemyTurnTriggered)
        {
            EnemyTurnTriggered = true;
            Debug.Log("Enemy Turn Now");
        }

    }

    /// <summary>
    /// sends out when this scripts end turn stuff is complete
    /// </summary>
    [Button("Complete End Turn")]
    private void CompleteEndTurn()
    {
        if (EndTurnTriggered)
        {
            EndTurnTriggered = false;
            Debug.Log("Completing End Turn");
            TurnPublicEvents.TurnActionComplete();
        }
    }

    /// <summary>
    /// triggers when the end turn starts
    /// </summary>
    private void OnTurnEnd()
    {
        if (!EndTurnTriggered)
        {
            EndTurnTriggered = true;
            Debug.Log("Turn Ended");
        }

    }
}
