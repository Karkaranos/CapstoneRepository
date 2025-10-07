/*************************************************
Author Names : 		Tyler Hayes 
Date Created : 		09/28/2025
Date Last Modified : 09/28/2025
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

    //triggered whenever the player turn ends 
    public static Action EnemyTurnStarted;
}
