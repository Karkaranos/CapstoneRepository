/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		11/19/2025
Date Last Modified : 	12/4/2025
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
        if (enemy == null)
        {
            return;
        }
        enemy.logText.text = "M";
        enemy.rangedAnimator.SetBool("IsWalking", true);
        enemy.targetingBehaviour.FindTarget();
        enemy.gridPathfinding.StartMoveCoroutine();

        await Task.Delay((int)(enemy.GetMovementSpeed() * enemy.gridPathfinding.GetMoveCoroSpeed() * 1000 + 1000));

        if(enemy.gridPathfinding.MyPosition == enemy.gridPathfinding.GetTargetPosition() && enemy.GetPlayerInLineOfSight())
        {
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
