/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/1/2025
Date Last Modified : 	10/6/2025
Brief Description : 		Base class for all enemies
External Resources : 	
***************************************************/
using UnityEngine;
using NaughtyAttributes;
using TMPro;

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

    [SerializeField, 
        ShowIf(nameof(currentSettings), Settings.Health),
        ReadOnly]protected float currentHealth = 5f;
    #endregion

    #region COMBAT VARS
    [HorizontalLine(4, EColor.Pink)]

    [SerializeField,
    ShowIf(nameof(currentSettings), Settings.Combat),
    Tooltip("Chance Enemy will drop an Artifact On Death"), Range(0f, 1f)]
    protected float artifactDropChance = .5f;

    #endregion

    #region STATE MACHINE VARS

    [HorizontalLine(4, EColor.White)]

    [SerializeField,
        ShowIf(nameof(currentSettings), Settings.StateMachine),
        Tooltip("Delay between each state transition")]protected float secondsBetweenStateTransitions = 1f;

    protected EnemyStateMachine enemyStateMachine;

    #endregion

    #region TEST VARS

    [HorizontalLine(4, EColor.Green)]

    [SerializeField, ShowIf(nameof(currentSettings), Settings.Testing)] public TextMeshPro logText;
    [HideInInspector] public GridTesting gridTesting;

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
            if (FindFirstObjectByType<GameManager>().allowArtifacts)
            {
                TryDropItem();
            }
        }
    }

    /// <summary>
    /// Die function for enemy
    /// </summary>
    private void Die()
    {
        print("Enemy is dead!");
    }

    /// <summary>
    /// Generates a random number and checks if an item will drop
    /// Optional overload to force drops
    /// </summary>
    public void TryDropItem(float overload = -1f)
    {
        if (FindFirstObjectByType<GameManager>().allowArtifacts)
        {

            float dropChance = (overload > 0f ? overload : artifactDropChance);
            Debug.Log(dropChance);
            float randValue = Random.Range(0f, 1f);
            if (randValue <= dropChance)
            {
                // this line should be replaced later. 
                // it generates an artifact from the pool and 
                ArtifactData ad = ArtifactManager.GetArtifactFromRAP();
                ArtifactManager.ObtainArtifact(ad);
                Logger.Log("Dropped " + ad.Name);
            }
        }
    }
    #endregion
}
