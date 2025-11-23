/*************************************************
Author Names : 		Tyler Hayes, Jay Embry
Date Created : 		09/30/2025
Date Last Modified : 10/22/2025
Brief Description : This is the data container for what nodes have
                    been unlocked on the skill tree
External Resources : 	
	***************************************************/

using NaughtyAttributes;
using UnityEngine;

//this is the general enum for the rune type
public enum RuneType
{
    Lightning,
    Wind
}

[CreateAssetMenu(fileName = "RuneData", menuName = "RuneData")]
public class RuneData : ScriptableObject
{

    #region SETUP

    public enum Data
    {

        ID,
        Stats,
        VFX

    }

    [SerializeField] private Data currentInspectorShowing;

    #endregion SETUP


    #region ID

    [HorizontalLine(4, EColor.Red)]

    [ShowIf(nameof(currentInspectorShowing), Data.ID)]
    //stores the type of rune
    public RuneType TypeOfRune;

    [ShowIf(nameof(currentInspectorShowing), Data.ID)]
    //stores which version of the element it is
    public int NumberOnSkillTree;

    [ShowIf(nameof(currentInspectorShowing), Data.ID)]
    //name of the rune
    public string RuneName;

    [ShowIf(nameof(currentInspectorShowing), Data.ID)]
    //Description of the rune
    public string RuneDescription;

    #endregion ID


    #region STATS

    [HorizontalLine(4, EColor.Orange)]

    [ShowIf(nameof(currentInspectorShowing), Data.Stats)]
    //Influences how much damage the rune will do
    public float RuneDamage;

    [ShowIf(nameof(currentInspectorShowing), Data.Stats)]
    //How far the rune will reach
    public int RuneRange;

    [ShowIf(nameof(currentInspectorShowing), Data.Stats)]
    //How many action points this rune will cost to play in combat
    public int RuneActionPoints;

    [ShowIf(nameof(currentInspectorShowing), Data.Stats)]
    //How many action points this rune will cost to play in combat
    public float RuneSecondaryEffectChance;

    #endregion STATS


    #region VFX

    [HorizontalLine(4, EColor.Yellow)]

    [ShowIf(nameof(currentInspectorShowing), Data.VFX)]
    //Drop the VFX here!
    public GameObject RuneVFX;

    [ShowIf(nameof(currentInspectorShowing), Data.VFX)]
    //for chain/burst lightning, but can be used for more in the future
    public GameObject SecondaryRuneVFX;

    #endregion VFX


    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="typeOfRune"> The element of rune this is </param>
    /// <param name="numberOnSkillTree"> which version of the element it is </param>
    public RuneData(RuneType typeOfRune, int numberOnSkillTree, string RuneName, string RuneDescription, float RuneDamage, int RuneRange, GameObject RuneVFX, GameObject SecondaryRuneVFX)
    {
        this.TypeOfRune = typeOfRune;
        this.NumberOnSkillTree = numberOnSkillTree;
        this.RuneName = RuneName;
        this.RuneDescription = RuneDescription;
        this.RuneDamage = RuneDamage;
        this.RuneRange = RuneRange;
        this.RuneVFX = RuneVFX;
        this.SecondaryRuneVFX = SecondaryRuneVFX;
    }
}
