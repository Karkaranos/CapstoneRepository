/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/1/2025
Date Last Modified : 	10/7/2025
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

        enemy.logText.text = "Attacking";
        //TODO ATTACK LOGIC
        
        //If enemy can attack twice and hasn't yet call Attack state again 
        if(enemy.canAttackTwice && !enemy.hasAttackedTwice)
        {
            Logger.Log("Enemy State: Attack -> Attack");
            enemy.hasAttackedTwice = true;
            CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemy.GetAttackState()));
            enemy.gridTesting.Pathfind();
            return;
        }

        //Trigger Enemy end turn
        Logger.Log("Enemy State: Attack -> EndTurn");
        CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemy.GetEndTurnState()));
        enemy.gridTesting.Pathfind();
    }

    /// <summary>
    /// Exit attack state logic 
    /// </summary>
    public override void ExitState()
    {
        
    }
}
