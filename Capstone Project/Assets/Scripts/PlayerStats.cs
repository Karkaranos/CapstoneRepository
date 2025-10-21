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

    [Header("Attack Stats")]
    public float baseAttackMultiplier = 1;
    public float lightningAttackMultiplier = 1;
    public float windAttackMultiplier = 1;

    /// <summary>
    /// damages the player, takes the playersresistance into account
    /// </summary>
    /// <param name="amount"></param>
    public void TakeDamage(int amount)
    {
        int damageToTake = (int)(amount * (1 - resistance));
        if (damageToTake < 0) { damageToTake = 0; }
        health -= damageToTake;
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