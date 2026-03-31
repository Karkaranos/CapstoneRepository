/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/19/2025
Date Last Modified : 	12/2/2025
Brief Description : 		End state for Melee Enemy
External Resources : 	
***************************************************/
using UnityEngine;

public class MeleeEnemyEndTurnState : MeleeEnemyState
{
    public MeleeEnemyEndTurnState(MeleeEnemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    { }

    /// <summary>
    /// Enter state logic
    /// Tells the enemy handler to start the next enemy turn
    /// </summary>
    public override void EnterState()
    {
        if (enemy == null)
        {
            return;
        }
        try
        {
            enemy.anim.SetBool("IsWalking", false);
        }
        catch { }
        Debug.Log("Enter End Turn");
        enemy.DelayedTurnStatus(false);
        EnemyHandler.Instance.RunNextEnemyTurn();
        CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemy.GetWaitState(), 0f));
        Debug.Log("End -> Wait");
    }

    /// <summary>
    /// Exit state logic
    /// Currently only calls the base which is not implemented 
    /// </summary>
    public override void ExitState()
    {
        base.ExitState();
    }
}
