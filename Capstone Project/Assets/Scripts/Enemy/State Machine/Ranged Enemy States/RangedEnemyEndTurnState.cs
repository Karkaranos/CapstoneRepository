/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		11/19/2025
Date Last Modified : 	11/21/2025
Brief Description : 		Ranged Enemy end turn state
External Resources : 	
***************************************************/
using UnityEngine;

public class RangedEnemyEndTurnState : RangedEnemyState
{
    public RangedEnemyEndTurnState(RangedEnemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    { }

    /// <summary>
    /// Enter end turn logic
    /// </summary>
    public override void EnterState()
    {
        if (enemy == null)
        {
            return;
        }
        try
        {
            enemy.rangedAnimator.SetBool("IsWalking", false);
        }
        catch { }
        enemy.DelayedTurnStatus(false);
        EnemyHandler.Instance.RunNextEnemyTurn();
        CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemy.GetWaitState(), 0f));
    }

    /// <summary>
    /// Exit end turn logic 
    /// </summary>
    public override void ExitState()
    {
        base.ExitState();
    }
}
