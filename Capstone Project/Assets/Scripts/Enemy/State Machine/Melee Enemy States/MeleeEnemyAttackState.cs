/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/1/2025
Date Last Modified : 	10/6/2025
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
    /// else return to wait state
    /// </summary>
    public override void EnterState()
    {
        Debug.Log("Entered Attacking state");
        
        //TODO ATTACK LOGIC
        
        //If enemy can attack twice and hasn't yet call Attack state again 
        if(enemy.canAttackTwice && !enemy.hasAttackedTwice)
        {
            Debug.Log("Attack -> Attack");
            enemy.hasAttackedTwice = true;
            CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemy.GetAttackState()));
            return;
        }

        //Trigger Enemy end turn
        Debug.Log("Attack -> Wait");
        CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemy.GetWaitState()));
    }

    /// <summary>
    /// Exit attack state logic 
    /// </summary>
    public override void ExitState()
    {
        
    }
}
