/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/1/2025
Date Last Modified : 	10/6/2025
Brief Description : 		Wait State for melee enemy
External Resources : 	
***************************************************/
using UnityEngine;

public class MeleeEnemyWaitState : MeleeEnemyState
{
    public MeleeEnemyWaitState(MeleeEnemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {}

    /// <summary>
    /// logic for entering the wait state
    /// </summary>
    public override void EnterState()
    {
        base.EnterState();
        Debug.Log("Enter Wait State");
    }

    /// <summary>
    /// logic for exiting the wait state 
    ///reset enemies per turn variables 
    /// </summary>
    public override void ExitState()
    {
        enemy.hasMovedForTurn = false;
        enemy.hasAttackedTwice = false;
    }
}
