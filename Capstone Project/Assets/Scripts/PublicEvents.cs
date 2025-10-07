/*************************************************
Author Names : 		Tyler Hayes, Jay Embry 
Date Created : 		09/28/2025
Date Last Modified : 10/07/2025
Brief Description : Stores all of the public events used.
External Resources : 	
	***************************************************/

using System;
using UnityEditor.PackageManager;
using UnityEngine;

public static class PublicEvents
{
    //triggered whenever a node on the skill tree is purchased.
    //this is what triggers the other skill tree nodes to unlock themselves
    //on purchase of a node.
    public static Action SkillTreeNodePurchased;

    public static Action StartBattle;

    //Triggers whenever the player is done playing a spell
    public static Action EndPlayerTurn;


    #region RUNE EVENTS

    //Contains the list of runes equipped from SkillAndEquipManager
    //This just needs to be called after a rune is equipped in the pre-combat menu and everything else should work
    public static Action<RuneData> EquipRunesToCombatMenu;

    //Triggered when a button from the in-combat menu is clicked
    //Rune types are assigned from RuneSelectionMenu 
    public static Action<int> LightningRuneSelected;
    public static Action<int> WindRuneSelected;

    #endregion RUNE EVENTS
}
