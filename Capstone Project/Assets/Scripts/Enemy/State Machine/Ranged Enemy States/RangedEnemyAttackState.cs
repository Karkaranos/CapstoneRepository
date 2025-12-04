/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		11/19/2025
Date Last Modified : 	12/2/2025
Brief Description : 		Ranged enemy attack state
External Resources : 	
***************************************************/
using UnityEngine;

public class RangedEnemyAttackState : RangedEnemyState
{
    public RangedEnemyAttackState(RangedEnemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    { }

    /// <summary>
    /// Enter attack state logic 
    /// </summary>
    public override void EnterState()
    {
        enemy.logText.text = "A";

        enemy.playerStats.TakeDamage(enemy.damage);

        if (enemy.canAttackTwice && !enemy.hasAttackedTwice)
        {
            enemy.hasAttackedTwice = true;
            Debug.Log("Attack -> Attack");
            CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemy.GetAttackState()));
            return;
        }

        //Trigger Enemy end turn
        CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemy.GetEndTurnState()));
    }

    /// <summary>
    /// Exit attack state logic
    /// </summary>
    public override void ExitState()
    {
        base.ExitState();
    }
}
