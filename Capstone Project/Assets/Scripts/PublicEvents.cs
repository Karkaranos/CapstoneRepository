/*************************************************
Author Names : 		Tyler Hayes, Jay Embry , Cade Naylor
Date Created : 		09/28/2025
Date Last Modified : 10/22/2025
Brief Description : Stores all of the public events used.
External Resources : 	
	***************************************************/

using System;
using UnityEngine;

public static class PublicEvents
{
    //triggered whenever a node on the skill tree is purchased.
    //this is what triggers the other skill tree nodes to unlock themselves
    //on purchase of a node.
    public static Action SkillTreeNodePurchased;

    public static Action StartBattle;

    //Triggers whenever the player is done playing a spell
    public static Action EnemyTurnStarted;

    //Triggers whenever the player confirms a move
    public static Action PlayerMove;

    //Triggers whenever a tile is selected while in wait mode(?)
    public static Action<TileBehaviour> SelectTile;


    #region RUNE EVENTS

    //Contains the list of runes equipped from SkillAndEquipManager
    //This just needs to be called after a rune is equipped in the pre-combat menu and everything else should work
    public static Action<int> EquipRunesToCombatMenu;

    //Triggered when a button from the in-combat menu is clicked
    //Rune types are assigned from RuneSelectionMenu 
    public static Action<RuneType, int, float, int, GameObject> RuneSelected;

    #endregion RUNE EVENTS
}
