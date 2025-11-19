/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		11/18/2025
Date Last Modified : 	11/18/2025
Brief Description : 		Base class for Range enemies
                    This is a seperate class from Enemy for 
                 sublogic of each enemy. 
External Resources : 	
***************************************************/
using UnityEngine;
using NaughtyAttributes;

public class RangedEnemy : Enemy
{
    #region VARS

    #region COMBAT VARS
    [Header("Ranged Enemy Specfic")]
    [ShowIf(nameof(currentSettings), Settings.Combat),
        SerializeField,
        Tooltip("The minimum amount of tiles away from the enemy the player must be to be attacked")]
    private int minimumAttackDistance;

    [ShowIf(nameof(currentSettings), Settings.Combat)] public bool canAttackTwice = true;

    #endregion

    #region TEST VARS

    [ShowIf(nameof(currentSettings), Settings.Testing), SerializeField] private bool playerInAttackRange;
    [ShowIf(nameof(currentSettings), Settings.Testing), SerializeField] private bool playerInLineOfSight;

    #endregion

    #region STATE MACHINE VARS

    private RangedEnemyAttackState attackState;
    private RangedEnemyWaitState waitState;
    private RangedEnemyMoveState moveState;
    private RangedEnemyEndTurnState endTurnState;

    #endregion

    #region OTHER NON INSPECTOR VARS 

    [HideInInspector] public bool hasMovedForTurn = false;
    [HideInInspector] public bool hasAttackedTwice = false;

    #endregion

    #endregion

    #region FUNCTIONS
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        enemyStateMachine = new EnemyStateMachine();
        waitState = new RangedEnemyWaitState(this, enemyStateMachine);
        moveState = new RangedEnemyMoveState(this, enemyStateMachine);
        attackState = new RangedEnemyAttackState(this, enemyStateMachine);
        endTurnState = new RangedEnemyEndTurnState(this, enemyStateMachine);
        enemyStateMachine.Initialized(waitState, secondsBetweenStateTransitions);
        base.Start();
        //targetingBehaviour.behaviours = Ranged (Wait till Brad is done)
    }

    /// <summary>
    /// Defines what path the state machine should take
    /// at the start of its turn
    /// </summary>
    [Button("Start Enemy Turn")]
    public override void StartEnemyTurn()
    {
        if(turnDelayed)
        {
            CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(endTurnState));
            return;
        }

        if(GetPlayerInAttackRange() && GetPlayerInLineOfSight())
        {
            CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(attackState));
            return;
        }
        else
        {
            CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(moveState));
            return;
        }
        
    }
    #endregion

    #region GETTERS AND SETTERS

    /// <summary>
    /// Getters for the states accessed by other states 
    /// </summary>
    /// <returns></returns>
    public RangedEnemyWaitState GetWaitState() {  return waitState; }
    public RangedEnemyAttackState GetAttackState() { return attackState; }
    public RangedEnemyEndTurnState GetEndTurnState() { return endTurnState; }    

    /// <summary>
    /// Returns if the player is in attack range
    /// </summary>
    /// <returns></returns>
    public override bool GetPlayerInAttackRange()
    {
        return playerInAttackRange;
    }

    /// <summary>
    /// Returns if the player is in Line of Sight
    /// </summary>
    /// <returns></returns>
    public bool GetPlayerInLineOfSight()
    {
        return playerInLineOfSight;
    }
    #endregion
}
