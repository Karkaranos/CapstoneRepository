/*************************************************
Author Names : 	    	Tyler Bouchard, Cade Naylor
Date Created : 		    10/16/2025
Date Last Modified : 	10/28/2025 (Tyler Bouchard)
Brief Description : 	This class controls the player stats like health 
                        resistance and baseDamage
                        Also it seems like the stats have to be public to work with refs and encapsulation doesn't work :(
External Resources : 
***************************************************/
using UnityEngine;
using System;
using NaughtyAttributes;
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
    [Tooltip("The chance a player dodges the attack"), ShowIf(nameof(settings), Settings.GeneralStats), Range(0f,1f)] public float DodgeChance = 0f;
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
        UpdateHealthBar();
    }

    /// <summary>
    /// updates the health bar slider
    /// </summary>
    private void UpdateHealthBar()
    {
        healthBar.value = CurrentHealth;
    }
}