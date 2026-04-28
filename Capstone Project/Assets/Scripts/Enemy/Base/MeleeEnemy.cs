/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/1/2025
Date Last Modified : 	1/25/2026
Brief Description : 		Base class for melee enemies
                    This is a seperate class from Enemy for 
                 sublogic of each enemy. 
External Resources : 	
***************************************************/

using UnityEngine;
using NaughtyAttributes;
using TMPro;
using Unity.VisualScripting;
using System.Collections;

public class MeleeEnemy : Enemy
{
    #region VARS

    //Vars related to Melee enemy combat
    #region COMBAT VARS

    [Header("Melee Enemy Specfic")]
    [ShowIf(nameof(currentSettings), Settings.Combat)] public bool canAttackTwice = true;
    
    #endregion

    //Vars used to show functionality without implementation. TEMPORARY
    #region TEST VARS

    [ShowIf(nameof(currentSettings), Settings.Testing)] public bool isLowHealth = false;

    #endregion

    //Vars related to the state machine
    #region STATE MACHINE VARS

    [ShowIf(nameof(currentSettings), Settings.StateMachine), SerializeField] private MeleeEnemyWaitState enemyWaitState;
    [ShowIf(nameof(currentSettings), Settings.StateMachine), SerializeField] private MeleeEnemyMoveToPlayerState moveToPlayerState;
    [ShowIf(nameof(currentSettings), Settings.StateMachine), SerializeField] private MeleeEnemyAttackState attackState;
    [ShowIf(nameof(currentSettings), Settings.StateMachine), SerializeField] private MeleeEnemyEndTurnState endTurnState;

    #endregion

    //Other vars needed 
    #region OTHER NON INSPECTOR VARS
    [HideInInspector]public bool hasMovedForTurn = false;
    [HideInInspector]public bool hasAttackedTwice = false;
    public Animator anim;
    #endregion
    #endregion

    #region FUNCTIONS

    /// <summary>
    /// Currently calls Enemy.Start()
    /// if there is anything unique needed to be done in Melee start 
    /// it will be put here
    /// </summary>
    public override void Start()
    {
        enemyWaitState.SetVariables(this, enemyStateMachine);
        moveToPlayerState.SetVariables(this, enemyStateMachine);
        attackState.SetVariables(this, enemyStateMachine);
        endTurnState.SetVariables(this, enemyStateMachine);
        enemyStateMachine.Initialized(enemyWaitState, secondsBetweenStateTransitions);
        base.Start();
        targetingBehaviour.behaviours = TargetingBehaviour.TargetingBehaviours.melee;
        
    }

    /// <summary>
    /// Defines under what path the state machine should take 
    /// under what conditions at the start of the 
    /// enemies turn 
    /// </summary>
    [Button("Start Enemy Turn")]
    public override void StartEnemyTurn()
    {

        if(turnDelayed)
        {
            CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(endTurnState));
            return;

        }
        //Attack if player in range otherwise move towards player
        if(GetPlayerInAttackRange())
        {
            CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(attackState));
            return;
        }
        else
        {
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

    #endregion

    #region GETTER AND SETTERS

    /// <summary>
    /// Getters for the states to be accessed by other states (made as needed)
    /// </summary>
    /// <returns></returns>
    public MeleeEnemyWaitState GetWaitState() {  return enemyWaitState; }
    public MeleeEnemyAttackState GetAttackState() {  return attackState; }
    public MeleeEnemyEndTurnState GetEndTurnState() { return endTurnState; }

    /// <summary>
    /// Logic to determine if enemy is in attack range
    /// Overriden from Enemy.cs 
    /// </summary>
    /// <returns></returns>
    public override bool GetPlayerInAttackRange()
    {
        targetingBehaviour.FindTarget();
        gridPathfinding.PathfindThroughGrid();
        
        if (gridPathfinding.MyPosition == gridPathfinding.GetTargetPosition()) { }
        return gridPathfinding.MyPosition == gridPathfinding.GetTargetPosition();
    }

    #endregion
}
