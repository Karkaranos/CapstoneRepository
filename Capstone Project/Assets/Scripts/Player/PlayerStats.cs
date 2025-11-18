/*************************************************
Author Names : 	    	Tyler Bouchard, Cade Naylor, Clare Grady
Date Created : 		    10/16/2025
Date Last Modified : 	11/18/2025 (Cade Naylor)
Brief Description : 	This class controls the player stats like health 
                        resistance and baseDamage
                        Also it seems like the stats have to be public to work with refs and encapsulation doesn't work :(
External Resources : 
***************************************************/
using UnityEngine;
using System;
using NaughtyAttributes;
using UnityEngine.UI;
using Unity.VisualScripting;

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
    [Tooltip("The chance a player dodges the attack"), ShowIf(nameof(settings), Settings.GeneralStats), Range(0f,1f)] public float DodgeChance = 0f;
    [Tooltip("The player's luck modifier"), ShowIf(nameof(settings), Settings.GeneralStats), Range(0f, 1f)] public float LuckModifier = 0f;

    private int tempHealth;
    #endregion

    #region DamageTaken
    [HorizontalLine(4, EColor.Green)]
    [Tooltip("What percentage of damage the player can resist"), Range(0f,1f), 
        ShowIf(nameof(settings), Settings.DamageTaken)] public float Resistance = 0;
    [Tooltip("Multiplies how much damage the player takes from any source"), ShowIf(nameof(settings), Settings.DamageTaken)] 
        public float DamageTakenMultiplier = 1;
    [Tooltip("Multiplies how much damage the player takes from Ranged Enemies"), ShowIf(nameof(settings), Settings.DamageTaken)] 
        public float RangedDamageTakenMultiplier = 1;
    [Tooltip("Multiplies how much damage the player takes from Melee Enemies"), ShowIf(nameof(settings), Settings.DamageTaken)] 
        public float MeleeDamageTakenMultiplier = 1;
    #endregion

    #region Attack Stats
    [HorizontalLine(4, EColor.Blue)]
    [Tooltip("Multiplies how much damage the player deals across all elements"), ShowIf(nameof(settings), Settings.Attack)] 
        public float BaseAttackMultiplier = 1;
    [Tooltip("Multiplies how much damage the player deals from lightning spells"), ShowIf(nameof(settings), Settings.Attack)]
        public float LightningAttackMultiplier = 1;
    [Tooltip("Multiplies how much damage the player deals from wind spells"), ShowIf(nameof(settings), Settings.Attack)]
        public float WindAttackMultiplier = 1;
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
    private void Start()
    {
        CurrentHealth = MaxHealth;
        healthBar.maxValue = MaxHealth;
        ArtifactManager.SetPlayerReference(this);
        MarkManager.SetPlayer(this);
    }

    /// <summary>
    /// damages the player, takes the playersresistance into account
    /// Also takes damage source into accoubt
    /// </summary>
    /// <param name="amount">Base amount of damage</param>
    /// <param name="source">Where the damage came from</param>
    public void TakeDamage(int amount, DamageSource source = DamageSource.None)
    {

        // Check if the player dodges the attack
        // Return before dealing damage
        float dodgeCheck = UnityEngine.Random.Range(0f, 1f);
        if(dodgeCheck <= DodgeChance && DodgeChance > 0f)
        {
            return;
        }

        //i could move this to a different script if that would be more efficient
        //for now, this checks if the player's tile will "take damage" for them
        if (this.gameObject.GetComponentInParent<ShieldBehavior>() != null)
        {

            this.gameObject.GetComponentInParent<ShieldBehavior>().TakeDamage();

            //how much damage is this negating?? is this negating damage, or simply taking a hit for the player??
            //discuss this more later
            //for now, it's eating a hit for the player
            return;

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
        CurrentHealth -= (int)damageToTake;
        tempHealth -= (int)damageToTake;

        if (tempHealth < 0)
        {

            tempHealth = 0;
            Debug.Log("No more extra health! ");

        }

        MarkManager.HealthValueChanged(CurrentHealth/MaxHealth);
        //if player dead end level pop up 
        if(CurrentHealth <= 0)
        {
            EndLevelPopup();
            return;
        }

        UpdateHealthBar();
    }

    /// <summary>
    /// heals the player
    /// </summary>
    /// <param name="amount"></param>
    public void Heal(int amount)
    {
        CurrentHealth += amount;
        if(CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
        
        //if the player gets healed while having temp health, i don't want it to get taken away from them
        if(tempHealth > 0)
        {

            CurrentHealth += tempHealth;

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
        CurrentHealth += tempHealthAmount;
        tempHealth += tempHealthAmount;

        Debug.Log(tempHealth + " hit points added to the player!");

    }

    /// <summary>
    /// updates the health bar slider
    /// </summary>
    private void UpdateHealthBar()
    {
        healthBar.value = (CurrentHealth - tempHealth);
    }

    /// <summary>
    /// Sets the text for the end level when player dies 
    /// called by TakeDamage 
    /// </summary>
    private void EndLevelPopup()
    {
        EndLevelMenu endLevelMenu = FindFirstObjectByType<EndLevelMenu>();
        endLevelMenu.SetText("You Died");
        endLevelMenu.EnableEndMenuUi();
    }
}