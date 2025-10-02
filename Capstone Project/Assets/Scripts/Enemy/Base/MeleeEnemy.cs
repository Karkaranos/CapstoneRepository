/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/1/2025
Date Last Modified : 	10/1/2025
Brief Description : 		Base class for melee enemies
                    This is a seperate class from Enemy for 
                 sublogic of each enemy. 
External Resources : 	
***************************************************/
using Unity.VisualScripting;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    private MeleeEnemyWaitState enemyWaitState;
    private MeleeEnemyRunState enemyRunState; 
    private MeleeEnemyMoveToPlayerState moveToPlayerState;

    private void Awake()
    {
        enemyWaitState = new MeleeEnemyWaitState(this, enemyStateMachine);
        enemyRunState = new MeleeEnemyRunState(this,enemyStateMachine);
        moveToPlayerState = new MeleeEnemyMoveToPlayerState(this,enemyStateMachine);
    }

    private void OnEnable()
    {
        PublicEvents.EnemyTurnStarted += StartEnemyTurn; 
    }

    private void StartEnemyTurn()
    {
        if(LowHealthDetection())
        {
            enemyStateMachine.ChangeState(enemyRunState);
            return;
        }

        if (!PlayerInAgroRange())
        {
            //TODO wait on design
        }

        enemyStateMachine.ChangeState(moveToPlayerState);
    }

    private bool LowHealthDetection()
    {
        return (currentHealth / maxHealth <= lowHealthPercentage);
    }

    private bool PlayerInAgroRange()
    {
        return false;
    }
}
