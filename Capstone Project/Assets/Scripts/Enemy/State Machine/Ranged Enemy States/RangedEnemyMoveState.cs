/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		11/19/2025
Date Last Modified : 	12/4/2025
Brief Description : 		Ranged enemy move state
External Resources : 	
***************************************************/
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class RangedEnemyMoveState : RangedEnemyState
{
    private bool isActiveMoveState = false;
    public RangedEnemyMoveState(RangedEnemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
        isActiveMoveState = false;
    }

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
    /// Subscribe to events
    /// </summary>
    private void OnEnable()
    {
        PublicEvents.MoveCoroFinsihed += FindNextState;
    }

    /// <summary>
    /// Unsubscribe from events
    /// </summary>
    private void OnDisable()
    {
        PublicEvents.MoveCoroFinsihed -= FindNextState;
    }

    /// <summary>
    /// Enter move state logic
    /// </summary>
    public override void EnterState()
    {
        isActiveMoveState = true;
        if (enemy == null)
        {
            return;
        }
        enemy.logText.text = "M";
        enemy.rangedAnimator.SetBool("IsWalking", true);
        enemy.targetingBehaviour.FindTarget();
        enemy.gridPathfinding.StartMoveCoroutine();

    }

    /// <summary>
    /// After told by the gridpathfinding that move is done determine the next state the enemy goes to 
    /// </summary>
    private void FindNextState()
    {
        if(isActiveMoveState)
        {
            if (enemy.gridPathfinding.MyPosition == enemy.gridPathfinding.GetTargetPosition() && enemy.GetPlayerInLineOfSight())
            {
                CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemy.GetAttackState()));
            }
            else
            {
                CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemy.GetEndTurnState()));
            }
        }
    }

    /// <summary>
    /// Exit move state logic 
    /// </summary>
    public override void ExitState()
    {
        isActiveMoveState = false;
        base.ExitState();
    }
}
