/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/1/2025
Date Last Modified : 	10/8/2025
Brief Description : 		Melee Enemy Move State
External Resources : 	
***************************************************/
using UnityEngine;

public class MeleeEnemyMoveToPlayerState : MeleeEnemyState
{
    private GridPathfinding gridPathfinding;

    public MeleeEnemyMoveToPlayerState(MeleeEnemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    { 
        gridPathfinding = enemy.GetComponent<GridPathfinding>();
    }

    /// <summary>
    /// Enter Move State Logic
    /// Set moveForTurn. If player in range attack else end turn 
    /// </summary>
    public override void EnterState()
    {
        Debug.Log("Entered Move State");
        enemy.logText.text = "Moving";
        gridPathfinding.SetTarget();
        gridPathfinding.PathfindThroughGrid();

        enemy.hasMovedForTurn = true;

        //TEMP LINE FOR MILESTONE
        enemy.playerInAttackRange = true;

        if(enemy.PlayerInAttackRange())
        {
            Debug.Log("Move -> Attack");
            CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemy.GetAttackState()));
        }
        else
        {
            Debug.Log("Move -> Wait");
            CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemy.GetWaitState()));
            enemy.gridTesting.Pathfind();
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
