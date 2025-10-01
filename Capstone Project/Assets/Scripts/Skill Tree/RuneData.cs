/*************************************************
Author Names : 		Tyler Hayes 
Date Created : 		09/30/2025
Date Last Modified : 09/30/2025
Brief Description : This is the data container for what nodes have
                    been unlocked on the skill tree
External Resources : 	
	***************************************************/

using UnityEngine;

//this is the general enum for the rune type
public enum RuneType
{
    Lightning,
    Wind
}

[System.Serializable]
public class RuneData
{
    //stores the type of rune
    public RuneType typeOfRune;

    //stores which version of the element it is
    public int numberOnSkillTree;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="typeOfRune"> The element of rune this is </param>
    /// <param name="numberOnSkillTree"> which version of the element it is </param>
    public RuneData(RuneType typeOfRune, int numberOnSkillTree)
    {
        this.typeOfRune = typeOfRune;
        this.numberOnSkillTree = numberOnSkillTree;
    }
}
