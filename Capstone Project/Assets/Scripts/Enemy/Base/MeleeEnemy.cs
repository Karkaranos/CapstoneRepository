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

    [SerializeField] private Animator enemyAtk;      //check during Beta

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
        enemyStateMachine = new EnemyStateMachine();
        enemyWaitState = new MeleeEnemyWaitState(this, enemyStateMachine);
        enemyRunState = new MeleeEnemyRunState(this, enemyStateMachine);
        moveToPlayerState = new MeleeEnemyMoveToPlayerState(this, enemyStateMachine);
        attackState = new MeleeEnemyAttackState(this, enemyStateMachine);
        endTurnState = new MeleeEnemyEndTurnState(this, enemyStateMachine);
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
       // enemyAtk.SetBool("enemyIdle", true); //check for beta


        if (turnDelayed)
        {
            CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(endTurnState));
            return;

        }

        //if low health go to run state
        if(LowHealthDetection())
        {
            Debug.Log("Wait -> Run");
            CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemyRunState));
            return;
        }

        //Attack if player in range otherwise move towards player
        if(GetPlayerInAttackRange())
        {
            Debug.Log("Wait -> Attack");
            enemyAtk.SetBool("enemyATKing", true);      //check for beta
            CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(attackState));
            return;
        }
        else
        {
            Debug.Log("Wait -> Move");
            hasMovedForTurn = true;
          //  enemyAtk.SetBool("enemyIdle", true);   //check for beta
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
        Debug.Log("PATHFIND CALLED");
        targetingBehaviour.FindTarget();
        gridPathfinding.PathfindThroughGrid();
        Debug.Log("My Pos: " + gridPathfinding.MyPosition.ToString());
        Debug.Log("Target Pos: " + gridPathfinding.GetTargetPosition().ToString());
        
        if (gridPathfinding.MyPosition == gridPathfinding.GetTargetPosition()) { Debug.Log("In Range");  }
        return gridPathfinding.MyPosition == gridPathfinding.GetTargetPosition();
    }

    #endregion
}
