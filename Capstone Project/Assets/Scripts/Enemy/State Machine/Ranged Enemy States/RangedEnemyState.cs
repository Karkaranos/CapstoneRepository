/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		11/19/2025
Date Last Modified : 	12/2/2025
Brief Description : 		Base class for all Ranged Enemy States
External Resources : 	
***************************************************/
using UnityEngine;

public class RangedEnemyState : EnemyState
{
    protected RangedEnemy enemy;
    protected EnemyStateMachine enemyStateMachine;

    /// <summary>
    /// constructor that specifically passes a Ranged Enemy
    /// allows for access to all Ranged Enemy specific values
    /// </summary>
    /// <param name="enemy"></param>
    /// <param name="enemyStateMachine"></param>
    public RangedEnemyState(RangedEnemy enemy, EnemyStateMachine enemyStateMachine)
    {
        this.enemy = enemy;
        this.enemyStateMachine = enemyStateMachine;
    }
    
}
