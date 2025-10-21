/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/1/2025
Date Last Modified : 	10/20/2025
Brief Description : 		Base class for melee enemies
                    This is a seperate class from Enemy for 
                 sublogic of each enemy. 
External Resources : 	
***************************************************/

using UnityEngine;
using NaughtyAttributes;
using TMPro;
using Unity.VisualScripting;

public class MeleeEnemy : Enemy
{
    #region VARS

    //Vars used to show functionality without implementation. TEMPORARY
    #region TEST VARS

    [ShowIf(nameof(currentSettings), Settings.Testing)]public bool canAttackTwice = true;
    [ShowIf(nameof(currentSettings), Settings.Testing)] public bool isLowHealth = false;
    [ShowIf(nameof(currentSettings), Settings.Testing)] public bool playerInAttackRange = true;

    #endregion

    //Vars related to the state machine
    #region STATE MACHINE VARS

    private MeleeEnemyWaitState enemyWaitState;
    private MeleeEnemyRunState enemyRunState; 
    private MeleeEnemyMoveToPlayerState moveToPlayerState;
    private MeleeEnemyAttackState attackState;
    private MeleeEnemyEndTurnState endTurnState;

    #endregion

    //Other vars needed 
    #region OTHER NON INSPECTOR VARS
    [HideInInspector]public bool hasMovedForTurn = false;
    [HideInInspector]public bool hasAttackedTwice = false;

    #endregion
    #endregion

    #region FUNCTIONS

    /// <summary>
    /// Initialize all states of the state machine, link them to the state machine, 
    /// then tell the state machine to start in the wait state
    /// </summary>
    private void Awake()
    {
        enemyStateMachine = new EnemyStateMachine();
        enemyWaitState = new MeleeEnemyWaitState(this, enemyStateMachine);
        enemyRunState = new MeleeEnemyRunState(this,enemyStateMachine);
        moveToPlayerState = new MeleeEnemyMoveToPlayerState(this,enemyStateMachine);
        attackState = new MeleeEnemyAttackState(this,enemyStateMachine);
        endTurnState = new MeleeEnemyEndTurnState(this,enemyStateMachine);
        enemyStateMachine.Initialized(enemyWaitState, secondsBetweenStateTransitions);
    }

    //TODO: Come back and seeif needed rn
    private void Start()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Defines under what path the state machine should take 
    /// under what conditions at the start of the 
    /// enemies turn 
    /// </summary>
    [Button("Start Enemy Turn")]
    public override void StartEnemyTurn()
    {
        //if low health go to run state
        if(LowHealthDetection())
        {
            Debug.Log("Wait -> Run");
            CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemyRunState));
            return;
        }

        //Attack if player in range otherwise move towards player
        if(PlayerInAttackRange())
        {
            Debug.Log("Wait -> Attack");
            CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(attackState));
            return;
        }
        else
        {
            Debug.Log("Wait -> Move");
            hasMovedForTurn = true;
            CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(moveToPlayerState));
            return;
        }
    }

    /// <summary>
    /// Are we at low health. Will filled more for actual functionality
    /// </summary>
    /// <returns></returns>
    private bool LowHealthDetection()
    {
        return isLowHealth;
    }

    /// <summary>
    /// Is the player in attack ranger 
    ///Will be filled out for actual functionality 
    /// </summary>
    /// <returns></returns>
    public bool PlayerInAttackRange()
    {
        return playerInAttackRange;
    }

    #endregion

    #region GETTER AND SETTERS

    /// <summary>
    /// Getters for the states to be accessed by other states (made as needed)
    /// </summary>
    /// <returns></returns>
    public MeleeEnemyWaitState GetWaitState() {  return enemyWaitState; }
    public MeleeEnemyAttackState GetAttackState() {  return attackState; }
    public MeleeEnemyEndTurnState GetEndTurnState() { return endTurnState; }

    #endregion
}
