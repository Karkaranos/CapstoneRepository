/*************************************************
Author Names : 		Tyler Hayes 
Date Created : 		09/28/2025
Date Last Modified : 09/28/2025
Brief Description : This manages the player's skill points
                    and stores the data of their unlocked nodes
External Resources : 	
	***************************************************/

using System.Collections.Generic;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    public int SkillPoints;

    public List<RuneData> unlockedRunes;

    //add in the data the node is storing as a parameter here
    //so we can store all the nodes the player's unlocked in a list
    /// <summary>
    /// Checks to see if the node can be unlocked and unlocks it if it can be
    /// will also store the data when we create the class for the data
    /// </summary>
    /// <param name="cost"></param>
    /// <returns></returns>
    public bool CanPurchaseNode(int cost)
    {
        //checks to see if you can purchase the node
        if (cost <= SkillPoints)
        {
            //purchases it
            SkillPoints -= cost;

            //tells the node to buy itself
            return true;
        }

        //tells the node you cant buy it
        return false;
    }

    /// <summary>
    /// This is where we'll put the saving of the data in the 
    /// skill tree node. Currently blank because character team
    /// has not talked about how we are doing this
    /// </summary>
    public void UpdatePurchasedNodes(RuneData runePurchased)
    {
        //Makes sure that theres only ever one copy of each rune data in the list
        if (!unlockedRunes.Contains(runePurchased))
        {
            unlockedRunes.Add(runePurchased);
        }
        else
        {
            //idk what would trigger this but its important to have just in case
            //maybe messing around in the inspector?
            throw new System.Exception("Player already owns the rune " + runePurchased.RuneName + " but tried to add it to the runes purchased");
        }
    }
}
