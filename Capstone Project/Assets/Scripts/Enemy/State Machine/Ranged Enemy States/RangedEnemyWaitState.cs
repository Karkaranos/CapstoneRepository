/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		11/19/2025
Date Last Modified : 	11/19/2025
Brief Description : 		Ranged Enemy Wait State
External Resources : 	
***************************************************/
using UnityEngine;

public class RangedEnemyWaitState : RangedEnemyState
{
    public RangedEnemyWaitState(RangedEnemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    { }

    /// <summary>
    /// Enter wait state logic
    /// </summary>
    public override void EnterState()
    {
        enemy.logText.text = "Waiting";
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
