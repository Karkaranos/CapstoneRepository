/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    10/5/2025
Date Last Modified : 	10/8/2025
Brief Description : 	Controls what artifacts and effects are actively applied                     
External Resources : 	N/A
***************************************************/
using System.Collections.Generic;
using UnityEngine;

public class ArtifactManager
{

    [SerializeField] private bool inTestMode;
    // Stores all currently applied Artifacts
    private static List<ArtifactData> currentArtifacts = new List<ArtifactData>();

    // Stores all Artifacts not currently in use
    private static List<ArtifactData> inventoryArtifacts = new List<ArtifactData>();

    [SerializeField, Tooltip("How many Artifacts can be applied at once")] private static int maxArtifacts = 3;


    private static ArtifactData[] testData;
    private static ArtifactData[] randomArtifactPool;
    private static ArtifactData[] setArtifactPool;

    public static int MaxArtifacts { get => maxArtifacts; set => maxArtifacts = value; }

    /// <summary>
    /// Constructor for Artifact Manager
    /// </summary>
    /// <param name="rap">Random Artifact Pool</param>
    /// <param name="sap">Set Artifact Pool</param>
    /// <param name="maxArtifact">Maximum Number of Artifacts</param>
    /// <param name="testing">True if testing functionality</param>
    /// <param name="testInfo">Data for testing. Please have a minimum length of 4</param>
    public ArtifactManager(ArtifactData[] rap, ArtifactData[] sap, int maxArtifact, bool testing = false, ArtifactData[] testInfo = null)
    {
        randomArtifactPool = rap;
        setArtifactPool = sap;
        maxArtifacts = maxArtifact;
        if(testing)
        {
            inTestMode = true;
            testData = testInfo;
            TestArtifacts();
        }

    }

    /// <summary>
    /// Hardcoded function to show adding/removing
    /// Applies the first three items, then tries to apply a fourth
    /// Removes an item then applies the fourth
    public static void TestArtifacts()
    {
        ApplyArtifact(testData[0]);
        ApplyArtifact(testData[1]);
        ApplyArtifact(testData[2]);
        ApplyArtifact(testData[3]);
        RemoveArtifact(testData[0]);
        ApplyArtifact(testData[3]);

    }

    public static ArtifactData GetArtifactFromSAP(int level)
    {
        if (level <= setArtifactPool.Length)
        {
            return setArtifactPool[level];
        }
        throw new System.Exception("Cannot access indexes outside of the SAP Array");
    }

    public static ArtifactData GetArtifactFromRAP()
    {
        return randomArtifactPool[Random.Range(0, randomArtifactPool.Length)];
    }

    public static void ObtainArtifact(ArtifactData artifact)
    {
        inventoryArtifacts.Add(artifact);
        Logger.Log("Added " + artifact.Name + " to inventory");
    }

    /// <summary>
    /// Takes and adds a new Artifact to the currently stored Artifacts
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
        //Logger.LogWarning("Implement Artifact Stat Effects");
        string s = artifact.Name + " Effects: ";
        foreach(ArtifactEffects e in artifact.Effects)
        {
            switch (e.Effect)
            {
                case Effects.LightningAttackMultiplier:
                    //Player.LightningAttack = (adding ? Player.LightningAttack * e.StatChangeAmount : Player.LightningAttack / e.StatChangeAmount);
                    s += "Lightning Attack multiplied by ";
                    break;
                case Effects.ActionPointChange:
                    //Player.ActionPoints = (adding ? Player.ActionPoints + e.StatChangeAmount : Player.ActionPoints - e.StatChangeAmount);
                    s += "Action Points changed by ";
                    break;
                case Effects.AttackMultiplier:
                    //Player.Attack = (adding ? Player.Attack * e.StatChangeAmount : Player.Attack / e.StatChangeAmount);
                    s += "Attack multiplied by ";
                    break;
                case Effects.WindAttackMultiplier:
                    //Player.WindAttack = (adding ? Player.WindAttack * e.StatChangeAmount : Player.WindAttack / e.StatChangeAmount);
                    s += "Wind Attack multiplied by ";
                    break;
                case Effects.ResistanceMultiplier:
                    //Player.Resistance = (adding ? Player.Resistance * e.StatChangeAmount : Player.Resistance / e.StatChangeAmount);
                    s += "Resistance multiplied by ";
                    break;
                case Effects.DamageTakenMultiplier:
                    //Player.DamageTaken = (adding ? Player.DamageTaken * e.StatChangeAmount : Player.DamageTaken / e.StatChangeAmount);
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
                case Effects.MovementSpeedMultiplier:
                    //Player.Speed = (adding ? PlayerAttack * e.StatChangeAmount : PlayerAttack / e.StatChangeAmount);
                    s += "Movement Speed multiplied by ";
                    break;
                default:
                    Logger.Warning("Effect " + e.Effect + " fell through cases");
                    break;
            }
            s += e.StatChangeAmount + " | ";
        }

        if(adding)
        {
            Logger.Log(s);
        }
        else
        {
            Logger.Log("Removed " + artifact.Name);
        }

    }
}
