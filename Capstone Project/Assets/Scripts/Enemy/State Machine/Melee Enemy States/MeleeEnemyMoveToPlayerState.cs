using UnityEngine;

public class MeleeEnemyMoveToPlayerState : MeleeEnemyState
{

    public MeleeEnemyMoveToPlayerState(MeleeEnemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    { }

    public override void EnterState()
    {
        Debug.Log("Entered Move State");
        //Move Logic 
        enemy.hasMovedForTurn = true;

        if(enemy.PlayerInAttackRange())
        {
            Debug.Log("Move -> Attack");
            enemyStateMachine.ChangeState(enemy.GetAttackState());
        }
        else
        {
            Debug.Log("Move -> Wait"); 
            enemyStateMachine.ChangeState(enemy.GetWaitState());
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}
