/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/1/2025
Date Last Modified : 	10/7/2025
Brief Description : 		Melee Enemy Move State
External Resources : 	
***************************************************/
using UnityEngine;

public class MeleeEnemyMoveToPlayerState : MeleeEnemyState
{

    public MeleeEnemyMoveToPlayerState(MeleeEnemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    { }

    /// <summary>
    /// Enter Move State Logic
    /// Set moveForTurn. If player in range attack else end turn 
    /// </summary>
    public override void EnterState()
    {
        Debug.Log("Entered Move State");
        //Move Logic 
        enemy.hasMovedForTurn = true;

        if(enemy.PlayerInAttackRange())
        {
            Debug.Log("Move -> Attack");
            enemy.logText.text = "Attacking";
            CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemy.GetAttackState()));
        }
        else
        {
            Debug.Log("Move -> Wait");
            enemy.logText.text = "Waiting";
            CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemy.GetWaitState()));
        }
    }

    /// <summary>
    /// Exit Move state logic 
    /// </summary>
    public override void ExitState()
    {
        base.ExitState();
    }
}
