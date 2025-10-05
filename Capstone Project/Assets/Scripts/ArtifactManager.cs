/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    10/5/2025
Date Last Modified : 	10/5/2025
Brief Description : 	Data container for all types of Artifacts, or Equipment                       
External Resources : 	Length of Enum fron Unity Forums: https://discussions.unity.com/t/enum-count/78841?clickref=1101lBLKDGKd&utm_source=partnerize&utm_medium=affiliate&utm_campaign=unity_affiliate
***************************************************/
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactManager : MonoBehaviour
{
    // Stores all currently applied Artifacts
    private List<ArtifactData> currentArtifacts = new List<ArtifactData>();

    // Stores all Artifacts not currently in use
    private List<ArtifactData> inventoryArtifacts = new List<ArtifactData>();

    [SerializeField, Tooltip("How many Artifacts can be applied at once")] private int maxArtifacts = 3;

    [SerializeField, Tooltip("Artifact Testing. Will be removed later")] private ArtifactData[] testData;

    private void Start()
    {
        //TestArtifacts();
    }
    
    /// <summary>
    /// Hardcoded function to show adding/removing
    /// The first two should add two hats.
    /// The third item, an orb, should be added
    /// It should remove the previous orb before adding the fourth artifact
    /// </summary>
    private void TestArtifacts()
    {
        ApplyArtifact(testData[0], false);
        ApplyArtifact(testData[1], false);
        ApplyArtifact(testData[2], true);
        ApplyArtifact(testData[3], true);

    }

    public void ObtainArtifact(ArtifactData artifact)
    {
        inventoryArtifacts.Add(artifact);
    }

    /// <summary>
    /// Takes and adds a new Artifact to the currently stored Artifacts
    /// </summary>
    /// <param name="artifact">The artifact to add</param>
    /// <param name="clearExisting">True if this artifact removes any existing artifacts of the same type, false if it does not</param>
    public void ApplyArtifact(ArtifactData artifact, bool clearExisting = true)
    {
        if (clearExisting)
        {
            // Assumes only one of each type can be used at a time
            // Searches all existing artifacts and removes any of the same type as the incoming artifact
            foreach (ArtifactData ad in currentArtifacts)
            {
                if (ad.Type == artifact.Type)
                {
                    RemoveArtifact(ad);
                    inventoryArtifacts.Add(ad);
                    currentArtifacts.Remove(ad);
                    break;
                }
            }
        }

        if (currentArtifacts.Count < maxArtifacts)
        {
            currentArtifacts.Add(artifact);
            AddArtifact(artifact);
            inventoryArtifacts.Remove(artifact);
        }
    }

    private void AddArtifact(ArtifactData artifact)
    {
        Debug.LogWarning("Implement Artifact Stat Effects");
        string s = "";
        foreach(ArtifactEffects e in artifact.Effects)
        {
            s += e.Effect.ToString() + " by " + e.StatChangeAmount.ToString() + " ";
        }
        Debug.Log("Added an artifact that changes " + s);
    }

    private void RemoveArtifact(ArtifactData artifact)
    {
        Debug.LogWarning("Implement Artifact Stat Effects");
        string s = "";
        foreach (ArtifactEffects e in artifact.Effects)
        {
            s += e.Effect.ToString() + " by " + e.StatChangeAmount.ToString() + " ";
        }
        Debug.Log("Removed an artifact that changed " + s);
    }
}
