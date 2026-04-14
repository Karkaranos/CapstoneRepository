/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/1/2025
Date Last Modified : 	12/4/2025 Clare Grady
Brief Description : 		Melee Enemy Move State
External Resources : 	
***************************************************/
using System.Threading.Tasks;
using UnityEngine;

public class MeleeEnemyMoveToPlayerState : MeleeEnemyState
{
    private bool isActiveMoveState = false;

    public MeleeEnemyMoveToPlayerState(MeleeEnemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    { }

    /// <summary>
    /// Subscribe to event
    /// </summary>
    private void OnEnable()
    {
        PublicEvents.MoveCoroFinsihed += FindNextState;
    }

    /// <summary>
    /// Unsubscribe from event
    /// </summary>
    private void OnDisable()
    {
        PublicEvents.MoveCoroFinsihed -= FindNextState;
    }

    /// <summary>
    /// Sets the enemy variable references 
    /// </summary>
    /// <param name="enemy"></param>
    /// <param name="enemyStateMachine"></param>
    public void SetVariables(MeleeEnemy enemy, EnemyStateMachine enemyStateMachine)
    {
        this.enemy = enemy;
        this.enemyStateMachine = enemyStateMachine;
    }

    /// <summary>
    /// Enter Move State Logic
    /// Set moveForTurn. If player in range attack else end turn 
    /// </summary>
    public override void EnterState()
    {
        isActiveMoveState = true;
        if (enemy == null)
        {
            return;
        }
        enemy.logText.text = "M";
        enemy.anim.SetBool("IsWalking", true);
        enemy.targetingBehaviour.FindTarget();
       // enemy.gridPathfinding.PathfindThroughGrid();
        enemy.gridPathfinding.StartMoveCoroutine();

        enemy.hasMovedForTurn = true;

        //delay in milliseconds for the grid to update
        //Based on move coroutine and how many steps an enemy takes per turn 
    }

    /// <summary>
    /// Determine what state to go to next
    /// </summary>
    private void FindNextState()
    {
        if (isActiveMoveState)
        {
            if (enemy.gridPathfinding.MyPosition == enemy.gridPathfinding.GetTargetPosition())
            {
                Logger.Log("Enemy State: Move -> Attack");
                CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemy.GetAttackState()));
            }
            else
            {
                Logger.Log("Enemy State: Move -> EndTurn");
                CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemy.GetEndTurnState()));
            }
        }
    }
    /// <summary>
    /// Exit Move state logic 
    /// </summary>
    public override void ExitState()
    {
        isActiveMoveState = false;
        base.ExitState();
    }

}
