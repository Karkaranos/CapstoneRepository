/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/1/2025
Date Last Modified : 	10/6/2025
Brief Description : 		Melee Enemy Move State
External Resources : 	
***************************************************/
using UnityEngine;

public class MeleeEnemyMoveToPlayerState : MeleeEnemyState
{

    public MeleeEnemyMoveToPlayerState(MeleeEnemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    { }

    //Set moveForTurn. If player in range attack else end turn 
    public override void EnterState()
    {
        Debug.Log("Entered Move State");
        //Move Logic 
        enemy.hasMovedForTurn = true;

        if(enemy.PlayerInAttackRange())
        {
            Debug.Log("Move -> Attack");
            CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemy.GetAttackState()));
        }
        else
        {
            Debug.Log("Move -> Wait"); 
            CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemy.GetWaitState()));
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}
