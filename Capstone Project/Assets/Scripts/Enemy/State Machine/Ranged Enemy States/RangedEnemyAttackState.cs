/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		11/19/2025
Date Last Modified : 	12/2/2025
Brief Description : 		Ranged enemy attack state
External Resources : 	
***************************************************/
using UnityEngine;

public class RangedEnemyAttackState : RangedEnemyState
{

    [SerializeField] private FMOD.Studio.EventInstance rangedAttackSFX;
    private GameObject aPrefab;


    public RangedEnemyAttackState(RangedEnemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    { }

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
    /// Enter attack state logic 
    /// </summary>
    public override void EnterState()
    {
        if (enemy == null)
        {
            return;
        }
        enemy.logText.text = "A";

        enemy.rangedAnimator.SetTrigger("Attack");

        rangedAttackSFX = FMODUnity.RuntimeManager.CreateInstance("event:/RangedAttack");
        FMODUnity.RuntimeManager.PlayOneShot("event:/RangedAttack");

        if (enemy.canAttackTwice && !enemy.hasAttackedTwice)
        {
            enemy.hasAttackedTwice = true;
            CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemy.GetAttackState()));
            return;
        }

        //Trigger Enemy end turn
        CoroutineHandler.Instance.RunCoroutine(enemyStateMachine.ChangeState(enemy.GetEndTurnState()));
    }

    /// <summary>
    /// Exit attack state logic
    /// </summary>
    public override void ExitState()
    {
        base.ExitState();
    }
}
