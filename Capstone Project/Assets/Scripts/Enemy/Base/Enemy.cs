/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/1/2025
Date Last Modified : 	1/27/2026
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

    public enum DamageType
    {
        Lightning, Wind, None
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
        ReadOnly]public float currentHealth = 5f;
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
    public int attackRange;

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

    [SerializeField,
        ShowIf(nameof(currentSettings), Settings.Combat),
        Tooltip("Damage flash material")]
    protected Material flashColor;

    [SerializeField,
        ShowIf(nameof(currentSettings), Settings.Combat),
        Tooltip("Sprite Renderer")]
    protected SpriteRenderer spriteRen;

    [SerializeField,
    ShowIf(nameof(currentSettings), Settings.Combat),
    Tooltip("How long before the material resets to normal, in milliseconds")]
    protected int flashTime = 1000;

    [Tooltip("The player's canvas"), ShowIf(nameof(currentSettings), Settings.Combat), SerializeField]
    protected Transform enemyCanvas;
    [Tooltip("In-Combat Stat Update prefab. Has a text and image component"), ShowIf(nameof(currentSettings), Settings.Combat), SerializeField]
    protected GameObject statChange;
    [Tooltip("Damage indicator for lightning"), ShowIf(nameof(currentSettings), Settings.Combat), SerializeField]
    protected Sprite lightningSprite;
    [Tooltip("Damage indicator for wind"), ShowIf(nameof(currentSettings), Settings.Combat), SerializeField]
    protected Sprite windSprite;


    // Hidden Vars
    [HideInInspector] public PlayerStats playerStats;
    [HideInInspector] protected bool turnDelayed;

    [HideInInspector] protected bool invincible = false;

    [HideInInspector] public bool HasStatusEffect = false;
    [HideInInspector] public RuneType RuneStatusEffect;
    [HideInInspector] public int RuneStatusEffectNumber;
    [HideInInspector] public bool isRangedEnemy = false;
    protected Material baseMat; 

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
    [ShowIf(nameof(currentSettings), Settings.Movement), SerializeField,
        Tooltip("Time in seconds that will delay the enemy move logic after changing to move state")]
    public float moveStateDelay = 0.5f;
    [HideInInspector] public GridPathfinding gridPathfinding;
    [HideInInspector] public TargetingBehaviour targetingBehaviour;

    #endregion

    #region TEST VARS

    [HorizontalLine(4, EColor.Green)]

    [SerializeField, ShowIf(nameof(currentSettings), Settings.Testing)] public TextMeshPro logText;
    

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
        baseMat = spriteRen.material;
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
        gridPathfinding.SetMoveCoroSpeed(moveStateDelay);
        playerStats = FindFirstObjectByType<PlayerStats>();
        turnIndicator.SetActive(false);
    }

    private void OnEnable()
    {
        PublicEvents.NewLevel += SetPlayerStats; 
    }

    private void OnDisable()
    {
        PublicEvents.NewLevel -= SetPlayerStats;
    }

    /// <summary>
    /// Damage function for enemy. Public so states can call it
    /// </summary>
    /// <param name="damage"></param>
    public async void Damage(float damage, DamageType dType = DamageType.None)
    {
        if(invincible)
        {
            return;
        }

        if (spriteRen != null)
        {
            spriteRen.material = flashColor;
        }

        //int casting truncates instead of rounds so this if there is extra damage it rounds up
        if(damage % 1 != 0)
        {
            damage += 1;
        }

        if (currentHealth > 0)
        {
            currentHealth -= (int)damage;

            if (statChange != null && (int)damage > 0)
            {
                GameObject g = Instantiate(statChange, enemyCanvas);
                Sprite s;
                switch (dType)
                {
                    case DamageType.Lightning:
                        s = lightningSprite;
                        break;
                    case DamageType.Wind:
                        s = windSprite;
                        break;
                    default:
                        s = null;
                        break;
                }
                g.GetComponent<StatusIndicator>()?.Initialize("-" + (int)damage + " HP ", false, s);
            }

            print("Enemy takes damage");
            healthBarSlider.value = currentHealth;
            if (currentHealth <= 0)
            {
                EnemyHandler.Instance.RemoveEnemy(this);
                await Task.Delay(500);
                Die();
                if (FindFirstObjectByType<GameManager>().allowArtifacts)
                {
                    TryDropItem();
                }
            }
            logText.text = damage + " dmg";
            print(currentHealth);


            await Task.Delay(flashTime);
            if (spriteRen != null)
            {
                spriteRen.material = baseMat;
            }
        }
    }

    /// <summary>
    /// Die function for enemy
    /// </summary>
    private void Die()
    {
        GridManager.RemoveEntity(gridPathfinding.MyPosition);

        PlayerBehavior pb = FindFirstObjectByType<PlayerBehavior>();
        pb.RemoveEnemyPosition(gridPathfinding.MyPosition);

        if (gameObject != null)
        {
            Destroy(this.gameObject);
        }
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

    /// <summary>
    /// Shows the damage preview 
    /// </summary>
    /// <param name="damage"></param>
    public void ShowDamagePreview(float damage)
    {

    }
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

    private void SetPlayerStats()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
    }

    #endregion

    #region GETTERS/SETTERS
    public float GetMovementSpeed()
    {
        return movementRange;
    }
    #endregion
}
