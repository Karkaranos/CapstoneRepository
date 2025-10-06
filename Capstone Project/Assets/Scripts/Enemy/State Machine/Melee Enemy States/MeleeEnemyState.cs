/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/1/2025
Date Last Modified : 	10/6/2025
Brief Description : 		Base class for all Melee Enemy States
External Resources : 	
***************************************************/

using UnityEngine;
using System.Collections;
public class MeleeEnemyState : EnemyState
{
    protected MeleeEnemy enemy;
    protected EnemyStateMachine enemyStateMachine;

    //constructor that specifically passes a Melee enemy 
    //allows for access to all Melee enemy specific variables that you don't get if you just use Enemy
    public MeleeEnemyState(MeleeEnemy enemy, EnemyStateMachine enemyStateMachine)
    {
        this.enemy = enemy;
        this.enemyStateMachine = enemyStateMachine;
    }
}
