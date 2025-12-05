/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/1/2025
Date Last Modified : 	12/4/2025
Brief Description : 		Base class for all enemies
External Resources : 	
***************************************************/
using System.Collections.Generic;
using System.Threading.Tasks;
using NaughtyAttributes;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

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
        Tooltip("Max health of enemy")]
    protected Slider healthBarSlider;

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
        Tooltip("Range player must be in for the enemy to detect them")]
    protected int aggroRange;

    [SerializeField,
        ShowIf(nameof(currentSettings), Settings.Combat),
        Tooltip("Range the player must be in for the enemy to hit them")]
    protected int attackRange;

    [SerializeField,
        ShowIf(nameof(currentSettings), Settings.Combat),
        Tooltip("Amout of damage the enemy does to the player")]
    public int damage;

    [SerializeField,
        ShowIf(nameof(currentSettings), Settings.Combat),
        Tooltip("Chance Enemy will drop an Artifact On Death"), Range(0f, 1f)]
    protected float artifactDropChance = .5f;

    [ShowIf(nameof(currentSettings), Settings.Combat),
        Tooltip("Gameobject for turn indicator element")]
    public GameObject turnIndicator;

    // Hidden Vars
    [HideInInspector] public PlayerStats playerStats;
    [HideInInspector] protected bool turnDelayed;

    [HideInInspector] protected bool invincible = false;

    [HideInInspector] public bool HasStatusEffect = false;
    [HideInInspector] public RuneType RuneStatusEffect;
    [HideInInspector] public int RuneStatusEffectNumber;

    #endregion

    #region MOVEMENT VARS

    [HorizontalLine(4, EColor.Blue)]

    [SerializeField,
        ShowIf(nameof(currentSettings), Settings.Movement),
        Tooltip("Movement range of enemy")] protected int movementRange;
    [SerializeField,
        ShowIf(nameof(currentSettings), Settings.Movement),
        Tooltip("Speed enemy slides to next tile")]
    protected float movementSpeed;
    [HideInInspector] public GridPathfinding gridPathfinding;
    [HideInInspector] public TargetingBehaviour targetingBehaviour;

    #endregion

    #region TEST VARS

    [HorizontalLine(4, EColor.Green)]

    [SerializeField, ShowIf(nameof(currentSettings), Settings.Testing)] public TextMeshPro logText;
    [ShowIf(nameof(currentSettings), Settings.Testing), SerializeField,
        Tooltip("Time in seconds that will delay the enemy move logic after changing to move state")]
    public int MoveStateDelay;

    #endregion

    #region STATE MACHINE VARS

    [HorizontalLine(4, EColor.White)]

    [SerializeField,
        ShowIf(nameof(currentSettings), Settings.StateMachine),
        Tooltip("Delay between each state transition")]protected float secondsBetweenStateTransitions = 1f;

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

    /// <summary>
    /// Start function
    /// </summary>
    public virtual void Start()
    {
        currentHealth = maxHealth;
        healthBarSlider.maxValue = maxHealth;
        gridPathfinding = GetComponent<GridPathfinding>();
        targetingBehaviour = GetComponent<TargetingBehaviour>();
        gridPathfinding.SetMovementRange(movementRange);
        gridPathfinding.SetAggroRange(aggroRange);
        gridPathfinding.SetMovementSpeed(movementSpeed);
        playerStats = FindFirstObjectByType<PlayerStats>();
        turnIndicator.SetActive(false);
    }

    /// <summary>
    /// Damage function for enemy. Public so states can call it
    /// </summary>
    /// <param name="damage"></param>
    public async void Damage(float damage)
    {
        if(invincible)
        {
            return;
        }

        //int casting truncates instead of rounds so this if there is extra damage it rounds up
        if(damage % 1 != 0)
        {
            damage += 1;
        }

        currentHealth -= (int)damage;
        print("Enemy takes damage");
        healthBarSlider.value = currentHealth;
        if (currentHealth < 0)
        {
            await Task.Delay(500);
            Die();
            if (FindFirstObjectByType<GameManager>().allowArtifacts)
            {
                TryDropItem();
            }
        }
        logText.text = damage + " dmg";
        print(currentHealth);
        
    }

    /// <summary>
    /// Die function for enemy
    /// </summary>
    private void Die()
    {
        EnemyHandler.Instance.RemoveEnemy(this);

        GridManager.RemoveEntity(gridPathfinding.MyPosition);

        Destroy(this.gameObject);
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

    /// <summary>
    /// Sets the Enemy's health
    /// If the new value is greater than the max health, sets the max health as well
    /// </summary>
    /// <param name="health">New health value</param>
    public void SetHealth(float health)
    {
        if(health > maxHealth)
        {
            maxHealth = health;
            healthBarSlider.maxValue = health;
        }
        currentHealth = health;
        healthBarSlider.value = currentHealth;
        if (currentHealth < 0)
        {
            Die();
            if (FindFirstObjectByType<GameManager>().allowArtifacts)
            {
                TryDropItem();
            }
        }
    }

    /// <summary>
    /// Toggles whether the enemy can take damage or not
    /// </summary>
    public void ToggleInvincibility()
    {
        invincible = !invincible;
    }

    /// <summary>
    /// Virtual method that all specific enemies will define
    /// that will start their individual state machine
    /// </summary>
    public virtual void StartEnemyTurn() {  }
    #endregion

    #region GETTERS AND SETTERS
    
    /// <summary>
    /// Getter for if the player is in the attack range of the enemy
    /// Needs to be overriden for each type of enemy 
    /// </summary>
    /// <returns></returns>
    public virtual bool GetPlayerInAttackRange()
    { 
        return false;
    }

    /// <summary>
    /// Sets whether or not the enemy's turn has been delayed 
    /// </summary>
    public void DelayedTurnStatus(bool isTurnDelayed)
    {

        turnDelayed = isTurnDelayed;

    }

    #endregion
}
