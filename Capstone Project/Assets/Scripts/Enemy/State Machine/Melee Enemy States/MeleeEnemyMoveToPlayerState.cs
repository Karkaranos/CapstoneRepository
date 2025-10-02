using UnityEngine;

public class MeleeEnemyMoveToPlayerState : EnemyState
{

    public MeleeEnemyMoveToPlayerState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    { 
        
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}
