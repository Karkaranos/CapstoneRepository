/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		11/18/2025
Date Last Modified : 	12/02/2025 (Clare)
Brief Description : 		Base class for Range enemies
                    This is a seperate class from Enemy for 
                 sublogic of each enemy. 
External Resources : 	
***************************************************/
using NaughtyAttributes;
using UnityEngine;
using static TargetingBehaviour;

public class RangedEnemy : Enemy
{
    #region VARS

    #region COMBAT VARS
    [Header("Ranged Enemy Specfic")]
    [ShowIf(nameof(currentSettings), Settings.Combat),
        SerializeField,
        Tooltip("The minimum amount of tiles away from the enemy the player must be to be attacked")]
    public int minimumAttackDistance;

    [ShowIf(nameof(currentSettings), Settings.Combat),
        SerializeField,
        Tooltip("The maximum amount of tiles away from the enemy the player must be to be attacked")]
    public int maxAttackDistance;

    [ShowIf(nameof(currentSettings), Settings.Combat)] public bool canAttackTwice = true;

    #endregion

    #region TEST VARS

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
    /// <summary>
    /// Initialize the Ranged state machine 
    /// call Enemy.Start()
    /// TODO: set targeting behaviour to ranged 
    /// </summary>
    public override void Start()
    {
        enemyStateMachine = new EnemyStateMachine();
        waitState = new RangedEnemyWaitState(this, enemyStateMachine);
        moveState = new RangedEnemyMoveState(this, enemyStateMachine);
        attackState = new RangedEnemyAttackState(this, enemyStateMachine);
        endTurnState = new RangedEnemyEndTurnState(this, enemyStateMachine);
        enemyStateMachine.Initialized(waitState, secondsBetweenStateTransitions);
        base.Start();
        targetingBehaviour.behaviours = TargetingBehaviour.TargetingBehaviours.ranged;
        isRangedEnemy = true;
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
            Debug.Log("Turn Delayed");
            CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(endTurnState));
            return;
        }

        if(GetPlayerInAttackRange() && GetPlayerInLineOfSight())
        {
            Debug.Log("Wait -> Attack");
            CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(attackState));
            return;
        }
        else
        {
            Debug.Log("Wait -> Move");
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
        Debug.Log("PATHFIND CALLED");
        targetingBehaviour.FindTarget();
        gridPathfinding.PathfindThroughGrid();
        Debug.Log("My Pos: " + gridPathfinding.MyPosition.ToString());
        Debug.Log("Target Pos: " + gridPathfinding.GetTargetPosition().ToString());

        if (gridPathfinding.MyPosition == gridPathfinding.GetTargetPosition()) { Debug.Log("In Range"); }
        return gridPathfinding.MyPosition == gridPathfinding.GetTargetPosition();
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
