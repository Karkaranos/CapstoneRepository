/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    10/5/2025
Date Last Modified : 	10/5/2025
Brief Description : 	Data container for all types of Artifacts, or Equipment                       
External Resources : 	N/A
***************************************************/
using NaughtyAttributes;
using UnityEngine;

#region Enum Setup
public enum ArtifactType
{
    Staff, Tunic, Amulet, Potion, Cannon
}

public enum Effects
{
    LightningAttackMultiplier, WindAttackMultiplier, AttackMultiplier, DamageTakenMultiplier, SpellSlotsChange, ActionPointChange, HealthChange, ResistanceMultiplier, MovementSpeedMultiplier
}

#endregion

[CreateAssetMenu(fileName = "ArtifactData", menuName = "ArtifactData")]
public class ArtifactData : ScriptableObject
{
    public string Name;
    public string Description;

    [Tooltip("The type of Artifact")] public ArtifactType Type;
    [Tooltip("All effects")] public ArtifactEffects[] Effects;
    [Tooltip("Takes 1 point away per fight it's used in. Set it to less than 0 to not use this")] public int Durability;

    /// <summary>
    /// Constructor for ArtifactData in case some Artifacts are generated at runtime
    /// </summary>
    /// <param name="name">Name of the Artifact</param>
    /// <param name="description">Description for the Artifact</param>
    /// <param name="type">Type of Artifact</param>
    /// <param name="effects">What stats this affects and their value</param>
    /// <param name="durability">Optional durability stat. Defaulted paramater sets it to not lose durability</param>
    public ArtifactData(string name, string description, ArtifactType type, ArtifactEffects[] effects, int durability = -1)
    {
        Name = name;
        Description = description;
        Type = type;
        Effects = effects;
        Durability = durability;
    }

    /// <summary>
    /// Copy constructor for Artifact Data
    /// </summary>
    /// <param name="ad">Existing Artifact Data</param>
    public ArtifactData(ArtifactData ad)
    {
        Name = ad.Name;
        Description = ad.Description;
        Type = ad.Type;
        Effects = ad.Effects;
        Durability = ad.Durability;
    }

}
