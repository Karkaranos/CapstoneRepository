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
    public RuneType TypeOfRune;

    //stores which version of the element it is
    public int NumberOnSkillTree;

    //name of the rune
    public string RuneName;

    //Description of the rune
    public string RuneDescription;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="typeOfRune"> The element of rune this is </param>
    /// <param name="numberOnSkillTree"> which version of the element it is </param>
    public RuneData(RuneType typeOfRune, int numberOnSkillTree, string RuneName, string RuneDescription)
    {
        this.TypeOfRune = typeOfRune;
        this.NumberOnSkillTree = numberOnSkillTree;
        this.RuneName = RuneName;
        this.RuneDescription = RuneDescription;
    }
}
