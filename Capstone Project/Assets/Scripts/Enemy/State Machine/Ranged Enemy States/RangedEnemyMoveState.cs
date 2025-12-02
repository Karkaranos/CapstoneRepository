/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		11/19/2025
Date Last Modified : 	12/2/2025
Brief Description : 		Ranged enemy move state
External Resources : 	
***************************************************/
using System.Threading.Tasks;
using UnityEngine;

public class RangedEnemyMoveState : RangedEnemyState
{
    public RangedEnemyMoveState(RangedEnemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {  }

    /// <summary>
    /// Enter move state logic
    /// </summary>
    public async override void EnterState()
    {
        enemy.logText.text = "Moving";

        /*
         * Check if player to close 
         * Pathfinding Logic HERE (move to or away from player) 
         * Move Coroutine 
         * await delay 
         */

        enemy.targetingBehaviour.FindTarget();
        enemy.gridPathfinding.StartMoveCoroutine();

        await Task.Delay(enemy.moveStateDelay * 1000);

        if (enemy.GetPlayerInAttackRange() && enemy.GetPlayerInLineOfSight())
        {
            Debug.Log("Move -> Attack");
            CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemy.GetAttackState()));
        }
        else
        {
            CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemy.GetEndTurnState()));
        }
    }

    /// <summary>
    /// Exit move state logic 
    /// </summary>
    public override void ExitState()
    {
        base.ExitState();
    }
}
