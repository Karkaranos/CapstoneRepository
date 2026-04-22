/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		11/19/2025
Date Last Modified : 	12/2/2025
Brief Description : 		Ranged Enemy Wait State
External Resources : 	
***************************************************/
using UnityEngine;

public class RangedEnemyWaitState : RangedEnemyState
{
    public RangedEnemyWaitState(RangedEnemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    { }

    /// <summary>
    /// Sets the enemy variable references 
    /// </summary>
    /// <param name="enemy"></param>
    /// <param name="enemyStateMachine"></param>
    public void SetVariables(RangedEnemy enemy, EnemyStateMachine enemyStateMachine)
    {
        this.enemy = enemy;
        this.enemyStateMachine = enemyStateMachine;
    }

    /// <summary>
    /// Enter wait state logic
    /// </summary>
    public override void EnterState()
    {
        if (enemy == null)
        {
            return;
        }
        enemy.logText.text = "W";
        base.EnterState();
    }

    /// <summary>
    /// Exit wait state logic 
    /// </summary>
    public override void ExitState()
    {
        base.ExitState();
    }
}
