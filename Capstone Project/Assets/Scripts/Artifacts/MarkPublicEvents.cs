/*************************************************
Author Names : 		Cade Naylor
Date Created : 		10/27/2025
Date Last Modified : 10/27/2025
Brief Description : Stores all of the Mark trigger events
External Resources : 	
	***************************************************/

using System;
using UnityEngine;

public static class MarkPublicEvents
{
    // Right now, OnEquip is the only one fully linked up

    // will need to be linked with PlayerStats for heal and take damage
    // Triggered whenever the player's health changes
    public static Action<float> PlayerHealthUpdated;

    // Triggers when the item is equipped
    public static Action OnEquip;

    // Will need to be linked with Enemy for enemy death
    // Triggers when an enemy is killed
    public static Action EnemyDeath;
}
