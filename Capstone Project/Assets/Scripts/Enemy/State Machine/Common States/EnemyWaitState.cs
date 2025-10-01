/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/1/2025
Date Last Modified : 	10/1/2025
Brief Description : 		Wait state for enemies 
                    This is the state they are in durring player turn 
External Resources : 	
***************************************************/
using UnityEngine;

public class EnemyWaitState : EnemyState
{
    public EnemyWaitState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    { }

    
}
