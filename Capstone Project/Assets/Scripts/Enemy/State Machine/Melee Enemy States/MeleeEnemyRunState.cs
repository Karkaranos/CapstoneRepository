using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class MeleeEnemyRunState : MeleeEnemyState
{
    public MeleeEnemyRunState(MeleeEnemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    { }

    public override void EnterState()
    {
        Debug.Log("Enemy is at low health. Running from player");
        //TODO run logic 
        //Trigger Enemy End turn 
        enemyStateMachine.ChangeState(enemy.GetWaitState()); 
    }

    public override void ExitState()
    {
        Debug.Log("Run state -> wait state");
    }
}
