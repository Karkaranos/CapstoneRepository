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
using NaughtyAttributes;

public class MeleeEnemy : Enemy
{
    private MeleeEnemyWaitState enemyWaitState;
    private MeleeEnemyRunState enemyRunState; 
    private MeleeEnemyMoveToPlayerState moveToPlayerState;
    private MeleeEnemyAttackState attackState;

    public bool hasMovedForTurn = false;
    public bool canAttackTwice = true;
    public bool hasAttackedTwice = false;
    private void Awake()
    {
        enemyStateMachine = new EnemyStateMachine();
        enemyWaitState = new MeleeEnemyWaitState(this, enemyStateMachine);
        enemyRunState = new MeleeEnemyRunState(this,enemyStateMachine);
        moveToPlayerState = new MeleeEnemyMoveToPlayerState(this,enemyStateMachine);
        attackState = new MeleeEnemyAttackState(this,enemyStateMachine);
        enemyStateMachine.Initialized(enemyWaitState);
    }

    private void OnEnable()
    {
        PublicEvents.EnemyTurnStarted += StartEnemyTurn; 
    }

    [Button("Start Enemy Turn")]
    private void StartEnemyTurn()
    {
        if(LowHealthDetection())
        {
            Debug.Log("Wait -> Run");
            enemyStateMachine.ChangeState(enemyRunState);
            return;
        }

        if(PlayerInAttackRange())
        {
            Debug.Log("Wait -> Attack");
            enemyStateMachine.ChangeState(attackState);
            return;
        }
        else
        {
            Debug.Log("Wait -> Move");
            hasMovedForTurn = true;
            enemyStateMachine.ChangeState(moveToPlayerState);
            return;
        }
    }

    private bool LowHealthDetection()
    {
        return (currentHealth / maxHealth <= lowHealthPercentage);
    }

    public bool PlayerInAttackRange()
    {
        return true;
    }

    public MeleeEnemyWaitState GetWaitState() {  return enemyWaitState; }
    public MeleeEnemyAttackState GetAttackState() {  return attackState; }
}
