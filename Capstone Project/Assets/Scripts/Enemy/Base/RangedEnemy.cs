/*************************************************
Author Names : 		Clare Grady, Brad Dixon
Date Created : 		11/18/2025
Date Last Modified : 	3/30/2026 (Brad)
Brief Description : 		Base class for Range enemies
                    This is a seperate class from Enemy for 
                 sublogic of each enemy. 
External Resources : 	
***************************************************/
using NaughtyAttributes;
using System.Threading.Tasks;
using UnityEngine;
using static TargetingBehaviour;

public class RangedEnemy : Enemy
{
    #region VARS
    public Animator rangedAnimator;

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

    [ShowIf(nameof(currentSettings), Settings.Combat)] [SerializeField] GameObject attackPrefab;

    #endregion

    #region TEST VARS

    [ShowIf(nameof(currentSettings), Settings.Testing), SerializeField] private bool playerInLineOfSight;


    #endregion

    #region STATE MACHINE VARS

    [ShowIf(nameof(currentSettings), Settings.StateMachine), SerializeField] private RangedEnemyAttackState attackState;
    [ShowIf(nameof(currentSettings), Settings.StateMachine), SerializeField] private RangedEnemyWaitState waitState;
    [ShowIf(nameof(currentSettings), Settings.StateMachine), SerializeField] private RangedEnemyMoveState moveState;
    [ShowIf(nameof(currentSettings), Settings.StateMachine), SerializeField] private RangedEnemyEndTurnState endTurnState;

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
        moveState.SetVariables(this, enemyStateMachine);
        attackState.SetVariables(this, enemyStateMachine);
        waitState.SetVariables(this, enemyStateMachine);
        endTurnState.SetVariables(this, enemyStateMachine);
        enemyStateMachine.Initialized(waitState, secondsBetweenStateTransitions);
        base.Start();
        targetingBehaviour.behaviours = TargetingBehaviour.TargetingBehaviours.ranged;
        isRangedEnemy = true;
        if(rangedAnimator == null)
        {
            rangedAnimator = GetComponent<Animator>();
        }
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

    /// <summary>
    /// Unity event that spawns the attack prefab containing the attack animation
    /// </summary>
    public void SpawnAttack()
    {
        Vector3 spawnPos = FindFirstObjectByType<PlayerBehavior>().gameObject.transform.position;
        GameObject g = Instantiate(attackPrefab, (spawnPos + new Vector3(0, 1, -.2f)), Quaternion.identity);
        g.GetComponent<RangedEnemyAttackBehaviour>().SetDamage(damage);
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
        targetingBehaviour.FindTarget();

        GridManager.DisplayGridAsText();
 
        gridPathfinding.PathfindThroughGrid();

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
