/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    10/5/2025
Date Last Modified : 	10/26/2025
Brief Description : 	Controls what artifacts and effects are actively applied                     
External Resources : 	https://stackoverflow.com/questions/1420186/references-to-variables-in-c
***************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;

public class ArtifactManager
{
    #region Variables
    [SerializeField] private bool inTestMode;

    #region Artifacts
    // Stores all currently applied Artifacts
    private static List<ArtifactData> currentArtifacts = new List<ArtifactData>();

    // Stores all Artifacts not currently in use
    private static List<ArtifactData> inventoryArtifacts = new List<ArtifactData>();

    [SerializeField, Tooltip("How many Artifacts can be applied at once")] private static int maxArtifacts = 3;


    private static ArtifactData[] testData;
    private static ArtifactData[] randomArtifactPool;
    private static ArtifactData[] setArtifactPool;

    public static int MaxArtifacts { get => maxArtifacts; set => maxArtifacts = value; }

    #endregion Artifacts

    #region Stamps
    private int markOfSpeedCount = 0;
    private int markOfStrengthCount = 0;
    private int markOfRiskCount = 0;

    #endregion Stamps

    private static PlayerStats player;
    private static GameManager gm;

    #endregion Variables

    /// <summary>
    /// Constructor for Artifact Manager
    /// </summary>
    /// <param name="rap">Random Artifact Pool</param>
    /// <param name="sap">Set Artifact Pool</param>
    /// <param name="maxArtifact">Maximum Number of Artifacts</param>
    /// <param name="testing">True if testing functionality</param>
    /// <param name="testInfo">Data for testing. Please have a minimum length of 4</param>
    public ArtifactManager(ArtifactData[] rap, ArtifactData[] sap, int maxArtifact, PlayerStats p, bool testing = false, ArtifactData[] testInfo = null)
    {
        randomArtifactPool = rap;
        setArtifactPool = sap;
        maxArtifacts = maxArtifact;
        player = p;
        if(testing)
        {
            inTestMode = true;
            testData = testInfo;
            TestArtifacts();
        }

    }

    /// <summary>
    /// Sets a reference to the PlayerStats script
    /// </summary>
    /// <param name="p">PlayerStats</param>
    /// <returns>Whether the operaation was sucessful or not</returns>
    public static void SetPlayerReference(PlayerStats p)
    {
        player = p;
    }

    /// <summary>
    /// Hardcoded function to show adding/removing
    /// Applies the first three items, then tries to apply a fourth
    /// Removes an item then applies the fourth
    public static void TestArtifacts()
    {
        ApplyArtifact(testData[0]);
    }

    /// <summary>
    /// Gets an artifact from the Set Artifact Pool
    /// Returns an artifact based on current level
    /// </summary>
    /// <param name="level">Current level, 0-level count-1</param>
    /// <returns>related ArtifactData</returns>
    /// <exception cref="System.Exception">Throws out of index exception</exception>
    public static ArtifactData GetArtifactFromSAP(int level)
    {
        if (level <= setArtifactPool.Length)
        {
            return setArtifactPool[level];
        }
        throw new System.Exception("Cannot access indexes outside of the SAP Array");
    }

    /// <summary>
    /// Returns an artifact from the RandomArtifactPool
    /// </summary>
    /// <returns>Random ArtifactData</returns>
    public static ArtifactData GetArtifactFromRAP()
    {
        return randomArtifactPool[UnityEngine.Random.Range(0, randomArtifactPool.Length)];
    }

    /// <summary>
    /// Adds the ArtifactData to the player's pool
    /// </summary>
    /// <param name="artifact">ArtifactData to add</param>
    public static void ObtainArtifact(ArtifactData artifact)
    {
        inventoryArtifacts.Add(artifact);
        Logger.Log("Added " + artifact.Name + " to inventory");
    }

    /// <summary>
    /// Equips an Artifact and updates the stats accordingly
    /// </summary>
    /// <param name="artifact">The artifact to add</param>
    public static void ApplyArtifact(ArtifactData artifact)
    {
        if (currentArtifacts.Count < MaxArtifacts)
        {
            currentArtifacts.Add(artifact);
            ChangeEffect(artifact, true);
            inventoryArtifacts.Remove(artifact);
        }
        else
        {
            Logger.Warning("Too many Artifacts applied");
        }
    }

    /// <summary>
    /// Removes an artifact
    /// Reverses its effects
    /// </summary>
    /// <param name="artifact">The artifact to remove</param>
    public static void RemoveArtifact(ArtifactData artifact)
    {
        if (currentArtifacts.Contains(artifact))
        {
            inventoryArtifacts.Add(artifact);
            ChangeEffect(artifact, false);
            currentArtifacts.Remove(artifact);
        }
        else
        {
            Logger.Warning("Could not find Artifact");
        }
    }

    /// <summary>
    /// Handles what effects are applied at a given time
    /// Pass in true when applying effects, false when removing them
    /// Commented lines are commented out because the stats do not exist in this branch
    /// </summary>
    /// <param name="artifact">The artifact to be considered</param>
    /// <param name="adding">Whether the artifact is being applied or not</param>
    private static void ChangeEffect(ArtifactData artifact, bool adding = true)
    {
        string s = artifact.Name + " Effects: ";

        if(player==null)
        {
            Debug.LogWarning("Player is null");
            return;
        }
        foreach (ArtifactEffects e in artifact.Effects)
        {

            switch (e.Effect)
            {
                case Effects.LightningAttackMultiplier:
                    AdjustValue(ref player.lightningAttackMultiplier, e.StatChangeAmount, adding);
                    s += "Lightning Attack multiplied by ";
                    break;
                case Effects.ActionPointChange:
                    Logger.Warning("Action Point change not implemented");
                    //Player.ActionPoints = (adding ? Player.ActionPoints + e.StatChangeAmount : Player.ActionPoints - e.StatChangeAmount);
                    s += "Action Points changed by ";
                    break;
                case Effects.AttackMultiplier:
                    AdjustValue(ref player.baseAttackMultiplier, e.StatChangeAmount, adding);
                    s += "Attack multiplied by ";
                    break;
                case Effects.WindAttackMultiplier:
                    AdjustValue(ref player.windAttackMultiplier, e.StatChangeAmount, adding);
                    s += "Wind Attack multiplied by ";
                    break;
                case Effects.ResistanceMultiplier:
                    AdjustValue(ref player.resistance, e.StatChangeAmount, adding);
                    s += "Resistance multiplied by ";
                    break;
                case Effects.TotalDamageTakenMultiplier:
                    AdjustValue(ref player.damageTakenMultiplier, e.StatChangeAmount, adding);
                    s += "Damage Taken multiplied by ";
                    break;
                case Effects.HealthChange:
                    //Player.Health = (adding ? Player.Health + e.StatChangeAmount : Player.Health - e.StatChangeAmount);
                    s += "Health changed by ";
                    break;
                case Effects.SpellSlotsChange:
                    //Player.SpellSlots = (adding ? Player.SpellSlots * e.StatChangeAmount : Player.SpellSlots / e.StatChangeAmount);
                    s += "Spell Slot Count changed by ";
                    break;
                case Effects.MovementRadiusChange:
                    //Player.Speed = (adding ? PlayerAttack * e.StatChangeAmount : PlayerAttack / e.StatChangeAmount);
                    s += "Movement Speed multiplied by ";
                    break;
                case Effects.RangedDamageTakenMultiplier:
                    AdjustValue(ref player.rangedDamageTakenMultiplier, e.StatChangeAmount, adding);
                    s += "Ranged Damage Taken multiplied by ";
                    break;
                case Effects.MeleeDamageTakenMultiplier:
                    AdjustValue(ref player.meleeDamageTakenMultiplier, e.StatChangeAmount, adding);
                    s += "Melee Damage Taken multiplied by ";
                    break;
                case Effects.Vampiric:
                    s += "Valpiric effect- to be implemented ";
                    break;
                case Effects.Dodge:
                    s += "Valpiric effect- to be implemented ";
                    break;
                default:
                    Logger.Warning("Effect " + e.Effect + " fell through cases");
                    break;
            }
            s += e.StatChangeAmount + " | ";
        }

        if (adding)
        {
            Logger.Log(s);
        }
        else
        {
            Logger.Log("Removed " + artifact.Name);
        }

    }

    /// <summary>
    /// Used for multiplicative or divisive adjustments to values
    /// </summary>
    /// <param name="x">The value to be adjusted</param>
    /// <param name="y">The value to be multiplied/divided by</param>
    /// <param name="b">True for multiplication</param>
    private static void AdjustValue(ref float x, float y, bool b)
    {
        y = (y > 1 ? y + 1 : 1 - y);
        x = x + (b ? x * y : x / y);
    }
}
