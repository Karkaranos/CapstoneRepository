using UnityEngine;

public class MeleeEnemyAttackState : MeleeEnemyState
{
    public MeleeEnemyAttackState(MeleeEnemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    { }

    public override void EnterState()
    {
        Debug.Log("Entered Attacking state");
        //Attack Logic 
        
        if(enemy.canAttackTwice && !enemy.hasAttackedTwice)
        {
            Debug.Log("Attack -> Attack");
            enemy.hasAttackedTwice = true;
            enemyStateMachine.ChangeState(enemy.GetAttackState());
            return;
        }

        //Trigger Enemy end turn
        Debug.Log("Attack -> Wait");
        enemyStateMachine.ChangeState(enemy.GetWaitState());
    }

    public override void ExitState()
    {
        
    }
}
