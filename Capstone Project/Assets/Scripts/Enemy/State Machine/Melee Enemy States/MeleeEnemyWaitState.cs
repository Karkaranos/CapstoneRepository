using UnityEngine;

public class MeleeEnemyWaitState : MeleeEnemyState
{
    public MeleeEnemyWaitState(MeleeEnemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {}

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ExitState()
    {
        enemy.hasMovedForTurn = false;
        enemy.hasAttackedTwice = false;
    }
}
