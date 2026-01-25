/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/1/2025
Date Last Modified : 	12/4/2025 Clare Grady
Brief Description : 		Melee Enemy Move State
External Resources : 	
***************************************************/
using System.Threading.Tasks;
using UnityEngine;

public class MeleeEnemyMoveToPlayerState : MeleeEnemyState
{

    public MeleeEnemyMoveToPlayerState(MeleeEnemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    { }

    /// <summary>
    /// Enter Move State Logic
    /// Set moveForTurn. If player in range attack else end turn 
    /// </summary>
    public async override void EnterState()
    {
        Debug.Log("Entered Move State");
        enemy.logText.text = "M";

        enemy.targetingBehaviour.FindTarget();
       // enemy.gridPathfinding.PathfindThroughGrid();
        enemy.gridPathfinding.StartMoveCoroutine();

        enemy.hasMovedForTurn = true;

        //delay in milliseconds for the grid to update
        //Based on move coroutine and how many steps an enemy takes per turn 
        await Task.Delay((int)enemy.moveStateDelay * 1000);

        if (enemy.gridPathfinding.MyPosition == enemy.gridPathfinding.GetTargetPosition())
        {
            Logger.Log("Enemy State: Move -> Attack");
            CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemy.GetAttackState()));
        }
        else
        {
            Logger.Log("Enemy State: Move -> EndTurn");
            CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemy.GetEndTurnState()));
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
