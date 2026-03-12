/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/1/2025
Date Last Modified : 	12/2/2025
Brief Description : 		Base class for all states
External Resources : 	
***************************************************/
using UnityEngine;

public class MeleeEnemyAttackState : MeleeEnemyState
{
    public MeleeEnemyAttackState(MeleeEnemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    { }

    /// <summary>
    /// Enter attack state logic 
    /// If can attack twice and hasn't yet calls state again
    /// else go to end turn state
    /// </summary>
    public override void EnterState()
    {
        Debug.Log("Entered Attacking state");
        enemy.anim.SetTrigger("Attack");
        enemy.logText.text = "A";

        //damage player
        enemy.playerStats.TakeDamage(enemy.damage);


        //If enemy can attack twice and hasn't yet call Attack state again 
        if (enemy.canAttackTwice && !enemy.hasAttackedTwice)
        {
            Logger.Log("Enemy State: Attack -> Attack");
            enemy.hasAttackedTwice = true;
            CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemy.GetAttackState()));
            return;
        }

        //Trigger Enemy end turn
        Logger.Log("Enemy State: Attack -> EndTurn");
        CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemy.GetEndTurnState()));
    }

    /// <summary>
    /// Exit attack state logic 
    /// </summary>
    public override void ExitState()
    {
        base.ExitState();
        enemy.anim.SetBool("IsAttacking", false);
    }
}
