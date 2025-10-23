/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/1/2025
Date Last Modified : 	10/19/2025
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
        enemy.logText.text = "Moving";

        enemy.targetingBehaviour.FindTarget();
        enemy.gridPathfinding.PathfindThroughGrid();

        enemy.hasMovedForTurn = true;

        await Task.Delay(750);

        if(enemy.GetPlayerInAttackRange())
        {
            Logger.Log("Enemy State: Move -> Attack");
            CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemy.GetAttackState(), 0));
        }
        else
        {
            Logger.Log("Enemy State: Move -> EndTurn");
            CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemy.GetEndTurnState(), 0));
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
