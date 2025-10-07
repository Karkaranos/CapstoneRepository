/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/1/2025
Date Last Modified : 	10/6/2025
Brief Description : 		Base class for all enemies
External Resources : 	
***************************************************/
using UnityEngine;
using NaughtyAttributes;

public class Enemy : MonoBehaviour
{
    #region VARS
    
    //inspector enums
    public enum Settings
    {
        Health,
        Combat,
        Movement,
        Testing,
        StateMachine
    }


    [SerializeField, Tooltip("Changes what settings are shown in the inspector")] protected Settings currentSettings;
    #region HEALTH VARS

    [HorizontalLine(4, EColor.Red)]

    [SerializeField, 
        ShowIf(nameof(currentSettings), Settings.Health),
        Tooltip("Max health of enemy")] protected float maxHealth;
    
    [SerializeField,
        ShowIf(nameof(currentSettings), Settings.Health),
        Min(0),
        MaxValue(1),
        Tooltip("Percentage of health the enemy needs to be at to trigger low health status")] protected float lowHealthPercentage = 0.20f;

    [SerializeField, ReadOnly]protected float currentHealth = 5f;
    #endregion

    #region STATE MACHINE VARS

    [HorizontalLine(4, EColor.White)]

    [SerializeField,
        ShowIf(nameof(currentSettings), Settings.StateMachine),
        Tooltip("Delay between each state transition")]protected float secondsBetweenStateTransitions = 5f;

    protected EnemyStateMachine enemyStateMachine;

    #endregion
    #endregion

    #region FUNCTIONS

    /// <summary>
    /// Make state machine for enemy
    /// </summary>
    private void Awake()
    {
        enemyStateMachine = new EnemyStateMachine();
    }

    //Start function
    private void Start()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Damage function for enemy. Public so states can call it
    /// </summary>
    /// <param name="damage"></param>
    public void Damage(float damage)
    {
        currentHealth -= damage;
        print("Enemy takes damage");

        if(currentHealth < 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Die function for enemy
    /// </summary>
    private void Die()
    {
        print("Enemy is dead!");
    }
    #endregion
}
