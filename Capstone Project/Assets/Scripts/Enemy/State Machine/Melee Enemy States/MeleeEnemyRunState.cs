/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/1/2025
Date Last Modified : 	12/2/2025
Brief Description : 		Run state for Melee Enemy
External Resources : 	
***************************************************/

using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class MeleeEnemyRunState : MeleeEnemyState
{
    public MeleeEnemyRunState(MeleeEnemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    { }

    /// <summary>
    /// Enter Run State logic
    /// </summary>
    public override void EnterState()
    {
        Debug.Log("Enter Run State");
        enemy.anim.SetBool("IsWalking", true);
        enemy.logText.text = "R";
        //TODO run logic 
        CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemy.GetEndTurnState()));
    }

    /// <summary>
    /// Exit Run State Logic
    /// </summary>
    public override void ExitState()
    {
        Debug.Log("Run -> End Turn");
        enemy.anim.SetBool("IsWalking", false);
    }
}
