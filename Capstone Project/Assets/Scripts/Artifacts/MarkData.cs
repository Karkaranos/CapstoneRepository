/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    10/27/2025
Date Last Modified : 	11/18/2025
Brief Description : 	Data container for all Marks
External Resources : 	N/A
***************************************************/
using NaughtyAttributes;
using UnityEngine;


public enum MarkType
{
    Strength, Risk, Luck, Restoration, Conquest, Victory, None
}

public enum MarkTriggerCondition
{
    OnEquip, HealthPercent, TurnStart, TurnCount, EnemyDeath
}

public enum MarkEffects
{
    AttackMultiplier, DamageTaken, Luck, Heal, APOnEnemyDeath, IncreasedXP, IncreasedRAPDrop
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
    [ShowAssetPreview] public Sprite MarkVisual;
    [Space(30)]
    [Tooltip("Describes when Mark Effects are triggered")] public MarkTriggerCondition TriggerCondition;

    [Tooltip("True if Effects trigger if the percent is above the provided value, false if Effects trigger when percent is below"),
        ShowIf(nameof(TriggerCondition), MarkTriggerCondition.HealthPercent), AllowNesting]
    public bool TriggerIfAbove;
    [Tooltip("What percent the effect triggers around"), ShowIf(nameof(TriggerCondition), MarkTriggerCondition.HealthPercent), AllowNesting] public float Percent;
    [Tooltip("How many turns before the effect switches with 2 marks"), ShowIf(nameof(TriggerCondition), MarkTriggerCondition.TurnCount), AllowNesting] public int TwoMarkTurnChange;
    [Tooltip("How many turns before the effect switches with 3 marks"), ShowIf(nameof(TriggerCondition), MarkTriggerCondition.TurnCount), AllowNesting] public int ThreeMarkTurnChange;

    [Tooltip("Effects When 2 of this Mark is equipped"), AllowNesting] public MarkEffectsLinked[] EffectsWith2;
    [Tooltip("Post-Turn Condition Effects When 2 of this Mark is equipped"), AllowNesting, ShowIf(nameof(TriggerCondition), MarkTriggerCondition.TurnCount)] public MarkEffectsLinked[] PostTurnEffectsWith2;
    [Tooltip("Effects When 3 of this Mark is equipped"), AllowNesting] public MarkEffectsLinked[] EffectsWith3;
    [Tooltip("Post-Turn Condition Effects When 3 of this Mark is equipped"), AllowNesting, ShowIf(nameof(TriggerCondition), MarkTriggerCondition.TurnCount)] public MarkEffectsLinked[] PostTurnEffectsWith3;
    [Tooltip("How many times this effect can trigger with 2 of this Mark. Ignore for non AP"), AllowNesting, ShowIf(nameof(TriggerCondition), MarkTriggerCondition.EnemyDeath)] public int maxTriggerWith2;
     [Tooltip("How many times this effect can trigger with 3 of this Mark. Ignore for non AP"), AllowNesting, ShowIf(nameof(TriggerCondition), MarkTriggerCondition.EnemyDeath)] public int maxTriggerWith3;

    [HideInInspector] public bool EffectCanTrigger = false;
    [HideInInspector] public bool TwoConditionTrigger = true;
    [HideInInspector] public bool ThreeConditionTrigger = true;
    [HideInInspector] public int TimesTriggered = 0;

    public MarkType MarkType { get; internal set; }
}