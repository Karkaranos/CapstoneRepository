/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    10/5/2025
Date Last Modified : 	10/7/2025
Brief Description : 	Controls what artifacts and effects are actively applied                     
External Resources : 	Length of Enum fron Unity Forums: https://discussions.unity.com/t/enum-count/78841?clickref=1101lBLKDGKd&utm_source=partnerize&utm_medium=affiliate&utm_campaign=unity_affiliate
***************************************************/
using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ArtifactManager : MonoBehaviour
{
    // Stores all currently applied Artifacts
    private List<ArtifactData> currentArtifacts = new List<ArtifactData>();

    // Stores all Artifacts not currently in use
    private List<ArtifactData> inventoryArtifacts = new List<ArtifactData>();

    [SerializeField, Tooltip("How many Artifacts can be applied at once")] private int maxArtifacts = 3;

    [SerializeField, Tooltip("Artifact Testing. Will be removed later")] private ArtifactData[] testData;

    /// <summary>
    /// Called on the first frame
    /// </summary>
    private void Start()
    {
        TestArtifacts();
    }
    
    /// <summary>
    /// Hardcoded function to show adding/removing
    /// Applies the first three items, then tries to apply a fourth
    /// Removes an item then applies the fourth
    private void TestArtifacts()
    {
        ApplyArtifact(testData[0]);
        ApplyArtifact(testData[1]);
        ApplyArtifact(testData[2]);
        ApplyArtifact(testData[3]);
        RemoveArtifact(testData[0]);
        ApplyArtifact(testData[3]);

    }

    public void ObtainArtifact(ArtifactData artifact)
    {
        inventoryArtifacts.Add(artifact);
    }

    /// <summary>
    /// Takes and adds a new Artifact to the currently stored Artifacts
    /// </summary>
    /// <param name="artifact">The artifact to add</param>
    public void ApplyArtifact(ArtifactData artifact)
    {
        if (currentArtifacts.Count < maxArtifacts)
        {
            currentArtifacts.Add(artifact);
            ChangeEffect(artifact, true);
            inventoryArtifacts.Remove(artifact);
        }
        else
        {
            Debug.LogWarning("Too many Artifacts applied");
        }
    }

    /// <summary>
    /// Removes an artifact
    /// Reverses its effects
    /// </summary>
    /// <param name="artifact">The artifact to remove</param>
    public void RemoveArtifact(ArtifactData artifact)
    {
        if (currentArtifacts.Contains(artifact))
        {
            inventoryArtifacts.Add(artifact);
            ChangeEffect(artifact, false);
            currentArtifacts.Remove(artifact);
        }
        else
        {
            Debug.LogWarning("Could not find Artifact");
        }
    }

    /// <summary>
    /// Handles what effects are applied at a given time
    /// Pass in true when applying effects, false when removing them
    /// Commented lines are commented out because the stats do not exist in this branch
    /// </summary>
    /// <param name="artifact">The artifact to be considered</param>
    /// <param name="adding">Whether the artifact is being applied or not</param>
    private void ChangeEffect(ArtifactData artifact, bool adding = true)
    {
        //Debug.LogWarning("Implement Artifact Stat Effects");
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
                    Debug.LogWarning("Effect " + e.Effect + " fell through cases");
                    break;
            }
            s += e.StatChangeAmount + " | ";
        }

        if(adding)
        {
            Debug.Log(s);
        }
        else
        {
            Debug.Log("Removed " + artifact.Name);
        }

    }
}
