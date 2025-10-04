using UnityEngine;

public class MeleeEnemyState : EnemyState
{
    protected MeleeEnemy enemy;
    protected EnemyStateMachine enemyStateMachine;

    public MeleeEnemyState(MeleeEnemy enemy, EnemyStateMachine enemyStateMachine)
    {
        this.enemy = enemy;
        this.enemyStateMachine = enemyStateMachine;
    }
}
