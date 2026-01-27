/*************************************************
Author Names : 		Tyler Hayes, Jay Embry , Cade Naylor, Clare
Date Created : 		09/28/2025
Date Last Modified : 10/28/2025
Brief Description : Stores all of the public events used.
External Resources : 	
	***************************************************/

using System;
using NUnit.Framework;
using UnityEditor.PackageManager;
using UnityEngine;

public static class PublicEvents
{
    //triggered whenever a node on the skill tree is purchased.
    //this is what triggers the other skill tree nodes to unlock themselves
    //on purchase of a node.
    public static Action SkillTreeNodePurchased;
    public static Action<RuneType> MasteryRunePurchased;
    

    public static Action TrashHeldOOCObject;

    public static Action StartBattle;

    public static Action EndBattle;

    //Triggers whenever the player is done playing a spell
    public static Action EnemyTurnStarted;

    #region PLAYER  

    //Triggers whenever the player confirms a move
    public static Action PlayerMove;

    //triggers whenever the player selects a rune to use as an attack
    public static Action PlayerTryingToAttack;

    public static Action PlayerNoLongerTryingToAttack;

    #endregion PLAYER

    #region INPUTS

    //called whenever the player rightclicks
    public static Action RightClicked;

    //called whenever the player leftclicks
    public static Action LeftClicked;

    //call this to enable / disable the player's inputs
    public static Action<bool> EnablePlayersInputs;

    public static Action ToggleConsole;

    #endregion INPUTS

    //Triggers whenever a tile is selected while in wait mode(?)
    public static Action<TileBehaviour> SelectTile;

    public static Action<TileBehaviour, Enemy, PlayerBehavior> SelectTarget;


    #region RUNE EVENTS

    //Contains the list of runes equipped from SkillAndEquipManager
    //This just needs to be called after a rune is equipped in the pre-combat menu and everything else should work
    public static Action<int> EquipRunesToCombatMenu;

    //Triggered when a button from the in-combat menu is clicked
    //Rune types are assigned from RuneSelectionMenu 
    public static Action<RuneData> RuneSelected;

    //for calculating a spell's range
    public static Action<bool, int, TileBehaviour> CheckRange;

    //spell triggers
    public static Action<RuneData, TileBehaviour, Enemy, PlayerBehavior> LightningCast;
    public static Action<RuneData, TileBehaviour, Enemy, PlayerBehavior> WindCast;
    public static Action EndCast;


    //Triggered when a spell is cast
    public static Action<int> RuneCast;

    #endregion RUNE EVENTS
}
