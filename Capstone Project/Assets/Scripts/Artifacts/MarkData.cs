/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    10/27/2025
Date Last Modified : 	10/27/2025
Brief Description : 	Data container for all Marks   
                        Anything commented out is not needed for VS and will be returned to
External Resources : 	N/A
***************************************************/
using NaughtyAttributes;
using UnityEngine;


public enum MarkType
{
    Strength, Speed, Risk, Luck, Restoration, Conquest, Victory, None
}

public enum MarkTriggerCondition
{
    OnEquip, HealthPercent, LevelStart, TurnStart, TurnCount, EnemyDeath
}

public enum MarkEffects
{
    AttackMultiplier, DamageTaken, MovementCost, Heal, APOnEnemyDeath, IncreasedXP, IncreasedRAPDrop, Luck
}

[System.Serializable]
public struct MarkEffectsLinked
{
    [AllowNesting] public MarkEffects Effect;
    [Tooltip("Use small values for a percentage change and whole numbers for a flat change"), AllowNesting] public float valueChange;
}

[CreateAssetMenu(fileName = "MarkData", menuName = "MarkData")]
public class MarkData : ScriptableObject
{
    public MarkType Name;
    [Tooltip("Describes when Mark Effects are triggered")] public MarkTriggerCondition TriggerCondition;

    [Tooltip("True if Effects trigger if the percent is above the provided value, false if Effects trigger when percent is below"),
        ShowIf(nameof(TriggerCondition), MarkTriggerCondition.HealthPercent), AllowNesting]
    public bool TriggerIfAbove;
    [Tooltip("What percent the effect triggers around"), ShowIf(nameof(TriggerCondition), MarkTriggerCondition.HealthPercent), AllowNesting] public float Percent;

    /*
    [Tooltip("What turn Effects change on"), ShowIf(nameof(TriggerCondition), MarkTriggerCondition.TurnCount), AllowNesting]
        public float EffectChangeTurn;*/

    [Tooltip("Effects When 2 of this Mark is equipped"), AllowNesting] public MarkEffectsLinked[] EffectsWith2;
    [Tooltip("Effects When 3 of this Mark is equipped"), AllowNesting] public MarkEffectsLinked[] EffectsWith3;

    [HideInInspector] public bool EffectCanTrigger = false;

    public MarkType MarkType { get; internal set; }
}