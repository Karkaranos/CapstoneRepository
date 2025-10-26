/*************************************************
Author Names : 	    	Tyler Bouchard 
Date Created : 		    10/16/2025
Date Last Modified : 	10/16/2025
Brief Description : 	This class controls the player stats like health 
                        resistance and baseDamage
External Resources : 
***************************************************/
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Health Stats")]
    public int health = 100;
    public float resistance = 0;
    public float damageTakenMultiplier = 1;
    public float rangedDamageTakenMultiplier = 1;
    public float meleeDamageTakenMultiplier = 1;

    [Header("Attack Stats")]
    public float baseAttackMultiplier = 1;
    public float lightningAttackMultiplier = 1;
    public float windAttackMultiplier = 1;

    private GameManager gm;

    public enum DamageSource
    {
        Ranged, Melee, Environmental, None
    }

    /// <summary>
    /// damages the player, takes the playersresistance into account
    /// Also takes damage source into accoubt
    /// </summary>
    /// <param name="amount">Base amount of damage</param>
    /// <param name="source">Where the damage came from</param>
    public void TakeDamage(int amount, DamageSource source = DamageSource.None)
    {
        float damageToTake = (amount * (1 - resistance) * damageTakenMultiplier);
        switch (source)
        {
            case DamageSource.Ranged:
                damageToTake *= rangedDamageTakenMultiplier;
                break;
            case DamageSource.Melee:
                damageToTake *= meleeDamageTakenMultiplier;
                break;
            case DamageSource.Environmental:
                break;
            case DamageSource.None:
                break;
            default:
                break;
        }
        if ((int)damageToTake < 0) { damageToTake = 0; }
        health -= (int)damageToTake;
    }

    /// <summary>
    /// heals the player
    /// </summary>
    /// <param name="amount"></param>
    public void Heal(int amount)
    {
        health += amount;
    }
}