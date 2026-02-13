/*************************************************
Author Names : 		Tyler Hayes, Jay Embry, Tyler Bouchard
Date Created : 		09/30/2025
Date Last Modified : 2/12/2026
Brief Description : This is the data container for what nodes have
                    been unlocked on the skill tree
External Resources : 	
	***************************************************/

using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

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
    //Determines the secondary damage done by a rune if at all
    public float SecondaryRuneDamage;

    [ShowIf(nameof(currentInspectorShowing), Data.Stats)]
    //How far the rune will reach
    public int RuneRange;

    [ShowIf(nameof(currentInspectorShowing), Data.Stats)]
    //How many action points this rune will cost to play in combat
    public int RuneActionPoints;

    [ShowIf(nameof(currentInspectorShowing), Data.Stats)]
    [MinValue(0), MaxValue(1)]
    //The chance that a rune has to trigger its secondary effect
    //This should be a float somewhere in between 0 and 1, with 1 being a 100% gurantee
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

    [ShowIf(nameof(currentInspectorShowing), Data.VFX)]
    //Drop the image for the rune in here
    public Sprite runeImage;

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
