/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/1/2025
Date Last Modified : 	10/19/2025
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
        enemy.logText.text = "Moving";

        enemy.targetingBehaviour.behaviours = TargetingBehaviour.TargetingBehaviours.melee;
        enemy.targetingBehaviour.FindTarget();
        enemy.gridPathfinding.PathfindThroughGrid();

        enemy.hasMovedForTurn = true;

        //TEMP LINE FOR MILESTONE
        enemy.playerInAttackRange = true;

        
        if(enemy.PlayerInAttackRange())
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
