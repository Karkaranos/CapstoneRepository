/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    10/5/2025
Date Last Modified : 	10/22/2025
Brief Description : 	Data container for all types of Artifacts, or Equipment                       
External Resources : 	N/A
***************************************************/
using NaughtyAttributes;
using UnityEngine;

#region Enum Setup

public enum Effects
{
    LightningAttackMultiplier, WindAttackMultiplier, AttackMultiplier, TotalDamageTakenMultiplier, RangedDamageTakenMultiplier, MeleeDamageTakenMultiplier, SpellSlotsChange, ActionPointChange, HealthChange, ResistanceMultiplier, Vampiric, Dodge, ChanceToAvoidUsingPoints, Instakill, Miss, Luck
}


public enum ArtifactTriggerCondition
{
    OnEquip, OnAttack, SpellCount
}


#endregion

[CreateAssetMenu(fileName = "ArtifactData", menuName = "ArtifactData")]
public class ArtifactData : ScriptableObject
{
    public string Name;
    public string Description;

    [Tooltip("When the Artifact effects occur")] public ArtifactTriggerCondition TriggerCondition;
    [Tooltip("How many spells this effect triggers after"), ShowIf(nameof(TriggerCondition), ArtifactTriggerCondition.SpellCount)] public int TriggerCount;
    [HideInInspector] public int Counter = 0;
    [Tooltip("All effects")] public ArtifactEffects[] Effects;
    [Tooltip("Used for set combinations")] public MarkType Mark;
    //[Tooltip("Takes 1 point away per fight it's used in. Set it to less than 0 to not use this")] public int Durability;
    [Tooltip("How many slots it takes up")] public int ArtifactSize;
    public Sprite ArtifactSprite;

    /// <summary>
    /// Constructor for ArtifactData in case some Artifacts are generated at runtime
    /// </summary>
    /// <param name="name">Name of the Artifact</param>
    /// <param name="description">Description for the Artifact</param>
    /// <param name="type">Type of Artifact</param>
    /// <param name="effects">What stats this affects and their value</param>
    /// <param name="size">Optional size stat. Defaulted paramater sets it to 1y</param>
    public ArtifactData(string name, string description, MarkType mark, ArtifactEffects[] effects, int size = 1)
    {
        Name = name;
        Description = description;
        Mark = mark;
        Effects = effects;
        ArtifactSize = size;
    }

    /// <summary>
    /// Copy constructor for Artifact Data
    /// </summary>
    /// <param name="ad">Existing Artifact Data</param>
    public ArtifactData(ArtifactData ad)
    {
        Name = ad.Name;
        Description = ad.Description;
        Mark = ad.Mark;
        Effects = ad.Effects;
        ArtifactSize = ad.ArtifactSize;
    }

}
