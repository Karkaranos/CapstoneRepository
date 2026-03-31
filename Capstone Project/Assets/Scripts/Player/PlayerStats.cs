/*************************************************
Author Names : 	    	Tyler Bouchard, Cade Naylor, Clare Grady, Brad Dixon
Date Created : 		    10/16/2025
Date Last Modified : 	3/10/2026 (Brad)
Brief Description : 	This class controls the player stats like health 
                        resistance and baseDamage
External Resources : 
***************************************************/
using NaughtyAttributes;
using System;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    #region Variables
    public enum Settings
    {
        GeneralStats, DamageTaken, Attack
    }

    public enum DamageSource
    {
        Ranged, Melee, Environmental, None
    }

    [SerializeField] private Settings settings;
    [SerializeField] private Slider healthBar;


    #region GeneralStats
    [HorizontalLine(4, EColor.Red)]
    [Tooltip("The player's current health"), ShowIf(nameof(settings), Settings.GeneralStats)] public int CurrentHealth = 100;
    [Tooltip("The player's maximum health at a given point"), ShowIf(nameof(settings), Settings.GeneralStats)] public int MaxHealth = 100;
    [Tooltip("Multiplies how much the player can heal/buff"), ShowIf(nameof(settings), Settings.GeneralStats)] public float HealBuffModifier = 1f;
    [Tooltip("The chance a player dodges the attack"), ShowIf(nameof(settings), Settings.GeneralStats), Range(0f, 1f)] public float DodgeChance = 0f;
    [Tooltip("The player's luck multiplier"), ShowIf(nameof(settings), Settings.GeneralStats)] public float LuckModifier = 1f;
    [Tooltip("What XP is multiplied by when an enemy dies"), ShowIf(nameof(settings), Settings.GeneralStats)] public float XPMultiplier = 1f;
    [Tooltip("A multiplier for RAP drop chance"), ShowIf(nameof(settings), Settings.GeneralStats)] public float RAPChanceModifier = 1f;

    [Header("References to different canvases or objects")]
    [Tooltip("UI Object for the turn indicator"), ShowIf(nameof(settings), Settings.GeneralStats)] public GameObject turnIndicator;
    private Transform playerCanvas;
    private SpriteRenderer playerSprite;
    [Tooltip("In-Combat Stat Update prefab. Has a text and image component"), ShowIf(nameof(settings), Settings.GeneralStats), SerializeField]
    private GameObject statChange;
    [SerializeField] private GameObject UICanvas;

    private int tempHealth;

    #endregion

    #region DamageTaken
    [HorizontalLine(4, EColor.Green)]
    [Tooltip("What percentage of damage the player can resist"), Range(0f, 1f),
        ShowIf(nameof(settings), Settings.DamageTaken)]
    public float Resistance = 0;
    [Tooltip("Multiplies how much damage the player takes from any source"), ShowIf(nameof(settings), Settings.DamageTaken)]
    public float DamageTakenMultiplier = 1;
    [Tooltip("Multiplies how much damage the player takes from Ranged Enemies"), ShowIf(nameof(settings), Settings.DamageTaken)]
    public float RangedDamageTakenMultiplier = 1;
    [Tooltip("Multiplies how much damage the player takes from Melee Enemies"), ShowIf(nameof(settings), Settings.DamageTaken)]
    public float MeleeDamageTakenMultiplier = 1;
    [Tooltip("Reflects this percent of damage taken back to the enemy who dealt it"), ShowIf(nameof(settings), Settings.DamageTaken)]
    public float Thorns = 0f;
    [Tooltip("Whether the player can take damage at all or not. True if they can"), ShowIf(nameof(settings), Settings.DamageTaken)]
    public bool TakesDamage = true;

    [SerializeField,ShowIf(nameof(settings), Settings.DamageTaken), Tooltip("Damage flash material")]
    protected Material flashColor;
    [SerializeField, ShowIf(nameof(settings), Settings.DamageTaken), Tooltip("How long before the material resets to normal, in milliseconds")]
    protected int flashTime = 1000;
    private Material baseMat;
    #endregion

    #region Attack Stats
[HorizontalLine(4, EColor.Blue)]
    [Tooltip("Multiplies how much damage the player deals across all elements"), ShowIf(nameof(settings), Settings.Attack)]
    public float BaseAttackMultiplier = 1f;
    [Tooltip("Multiplies how much damage the player deals from lightning spells"), ShowIf(nameof(settings), Settings.Attack)]
    public float LightningAttackMultiplier = 1f;
    [Tooltip("Multiplies how much damage the player deals from wind spells"), ShowIf(nameof(settings), Settings.Attack)]
    public float WindAttackMultiplier = 1f;
    [Tooltip("Multiplies how much damage the player deals from fire spells"), ShowIf(nameof(settings), Settings.Attack)]
    public float FireAttackMultiplier = 1f;
    [Tooltip("Multiplies how much damage the player deals from water spells"), ShowIf(nameof(settings), Settings.Attack)]
    public float WaterAttackMultiplier = 1f;
    [Tooltip("Multiplies how much damage the player deals from Tier 1 spells"), ShowIf(nameof(settings), Settings.Attack)]
    public float Tier1AttackMultiplier = 1f;
    [Tooltip("Multiplier for the damage the first spell cast on this turn deals"), ShowIf(nameof(settings), Settings.Attack)]
    public float FirstSpellMultiplier = 1f;
    [Tooltip("Multiplier for the damage the first spell cast on this turn deals"), ShowIf(nameof(settings), Settings.Attack)]
    public float SecondSpellMultiplier = 1f;
    [HideInInspector] public int SpellsCastThisTurn = 0;
    [Tooltip("How likely the player is to miss their attack. Currently does not function"), ShowIf(nameof(settings), Settings.Attack)]
    public float MissChance = 0f;
    [Tooltip("How likely the player is to instakill an enemy when attacking. Currently does not function"), ShowIf(nameof(settings), Settings.Attack)]
    public float InstaKillChance = 0f;
    [Tooltip("How likely the player is to not use Action Points when attcking. Currently does not function"), ShowIf(nameof(settings), Settings.Attack)]
    public float NoActionPointCostChance = 0f;
    #endregion

    private GameManager gm;

    #endregion

    /// <summary>
    /// Called on the first frame update
    /// Assigns initial health value
    /// </summary>
    private async void Start()
    {
        CurrentHealth = MaxHealth;

        ArtifactManager.SetPlayerReference(this);
        MarkManager.SetPlayer(this);

        await Task.Delay(1000);
        GameObject player = FindFirstObjectByType<PlayerBehavior>().gameObject;
        turnIndicator = player.transform.GetChild(2).GetChild(1).gameObject;
        turnIndicator.SetActive(true);
    }

    private void OnEnable()
    {
        PublicEvents.NewLevel += SetTurnIndicator;
        PublicEvents.NewPlayerCreated += PlayerSpawned;
    }

    private void OnDisable()
    {
        PublicEvents.NewLevel -= SetTurnIndicator;
    }

    /// <summary>
    /// Sets the player's health to max
    /// </summary>
    public void FullHeal()
    {
        CurrentHealth = MaxHealth;
        UpdateHealthBar();
    }

    /// <summary>
    /// Sets player variables when a new player is created
    /// </summary>
    /// <param name="pCanvas">The player's canvas</param>
    /// <param name="pSprite">The player's sprite renderer</param>
    public void PlayerSpawned(Transform pCanvas, SpriteRenderer pSprite)
    {
        playerCanvas = pCanvas;
        playerSprite = pSprite;
        baseMat = playerSprite.material;
    
    }

    /// <summary>
    /// damages the player, takes the playersresistance into account
    /// Also takes damage source into accoubt
    /// </summary>
    /// <param name="amount">Base amount of damage</param>
    /// <param name="source">Where the damage came from</param>
    public async void TakeDamage(int amount, DamageSource source = DamageSource.None, Enemy e = null)
    {

        if (!TakesDamage)
        {
            return;
        }

        // Check if the player dodges the attack
        // Return before dealing damage
        float dodgeCheck = UnityEngine.Random.Range(0f, 1f) * LuckModifier;
        if (dodgeCheck <= DodgeChance && DodgeChance > 0f)
        {
            return;
        }

        //i could move this to a different script if that would be more efficient
        //for now, this checks if the player's tile will "take damage" for them
        if (FindFirstObjectByType<PlayerBehavior>().GetComponentInParent<ShieldBehavior>() != null)
        {

            FindFirstObjectByType<PlayerBehavior>().GetComponentInParent<ShieldBehavior>().TakeDamage();

            //how much damage is this negating?? is this negating damage, or simply taking a hit for the player??
            //discuss this more later
            //for now, it's eating a hit for the player
            return;

        }


        if (playerSprite != null)
        {
            playerSprite.material = flashColor;
            FMODUnity.RuntimeManager.PlayOneShot("event:/EnemyDamage");
        }

        float damageToTake = (amount * (1 - Resistance) * DamageTakenMultiplier);
        switch (source)
        {
            case DamageSource.Ranged:
                damageToTake *= RangedDamageTakenMultiplier;
                break;
            case DamageSource.Melee:
                damageToTake *= MeleeDamageTakenMultiplier;
                break;
            case DamageSource.Environmental:
                break;
            case DamageSource.None:
                break;
            default:
                break;
        }
        if ((int)damageToTake < 0) { damageToTake = 0; }

        if (tempHealth > 0)
        {

            tempHealth -= (int)damageToTake;

            if (tempHealth < 0)
            {

                damageToTake = Mathf.Abs(tempHealth);

            }

        }

        CurrentHealth -= (int)damageToTake;

        if (statChange != null)
        {
            GameObject g = Instantiate(statChange, playerCanvas);
            g.GetComponent<StatusIndicator>()?.Initialize("-" + (int)damageToTake + " HP ", false);
        }


        MarkManager.HealthValueChanged(((float)CurrentHealth) / ((float)MaxHealth));

        if (tempHealth < 0)
        {

            tempHealth = 0;

        }


        // Damage the enemy if the player has thorns
        if (Thorns > 0 && e != null)
        {
            e.Damage(amount * Thorns);
        }

        //if player dead end level pop up 
        if (CurrentHealth <= 0)
        {
            EndLevelPopup();
            return;
        }

        UpdateHealthBar();


        await Task.Delay(flashTime);
        if (playerSprite != null)
        {
            playerSprite.material = baseMat;
        }
    }

    /// <summary>
    /// heals the player
    /// </summary>
    /// <param name="amount"></param>
    public void Heal(int amount)
    {

        float conditionalMultipliers = 1f;
        if (SpellsCastThisTurn == 0)
        {
            conditionalMultipliers *= FirstSpellMultiplier;
        }
        else if (SpellsCastThisTurn == 1)
        {
            conditionalMultipliers *= SecondSpellMultiplier;
        }
        CurrentHealth += (int)(amount * HealBuffModifier * conditionalMultipliers);

        if (statChange != null)
        {
            GameObject g = Instantiate(statChange, playerCanvas);
            g.GetComponent<StatusIndicator>()?.Initialize("-" + (int)(amount * HealBuffModifier * conditionalMultipliers) + " HP ", true);
        }

        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }

        UpdateHealthBar();
    }

    /// <summary>
    /// adds temp health to the player whenver it's triggered by a combo
    /// could be used for other things
    /// </summary>
    /// <param name="tempHealthAmount"> how much temp health that the player gets </param>
    public void AddTempHealth(int tempHealthAmount)
    {

        //shouldn't be capped by max health iirc
        tempHealth += tempHealthAmount;

    }

    /// <summary>
    /// updates the health bar slider
    /// </summary>
    private void UpdateHealthBar()
    {
        //for the healthbar over the players head
        healthBar = FindFirstObjectByType<PlayerBehavior>().GetComponentInChildren<Slider>();
        healthBar.maxValue = MaxHealth;
        healthBar.value = CurrentHealth;

        //for the health bar in the player profile menu in the top left of the incombat menu
        healthBar = FindFirstObjectByType<PlayerProfileDisplayBehavior>().GetComponentInChildren<Slider>();
        healthBar.maxValue = MaxHealth;
        healthBar.value = CurrentHealth;
    }

    /// <summary>
    /// Sets the text for the end level when player dies 
    /// called by TakeDamage 
    /// </summary>
    private void EndLevelPopup()
    {
        //UICanvas.SetActive(false);
        EndLevelMenu endLevelMenu = FindFirstObjectByType<EndLevelMenu>();
        //endLevelMenu.SetText("You Died");
        //endLevelMenu.SetNextLevelButton(false);
        endLevelMenu.EnableEndMenuUi(false);
    }

    private void SetTurnIndicator()
    {
        GameObject player = FindFirstObjectByType<PlayerBehavior>().gameObject;
        turnIndicator = player.transform.GetChild(2).GetChild(1).gameObject;
        turnIndicator.SetActive(true);
    }
}