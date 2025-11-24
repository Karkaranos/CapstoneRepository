/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    10/27/2025
Date Last Modified : 	11/23/2025
Brief Description : 	Handles Mark functionality 
External Resources : 	N/A
***************************************************/
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;


public class MarkManager
{
    private static List<MarkData> Marks;
    private static Dictionary<MarkType, int> MarkCount = new Dictionary<MarkType, int>();

    private static GameManager gm;
    private static PlayerStats player;
    private static int turnCount = 0;
    public MarkManager(List<MarkData> marks, GameManager game)
    {
        Marks = marks;
        gm = game;

        for(int i=0; i < MarkType.GetNames(typeof(MarkType)).Length; i++)
        {
            MarkType currentMark = (MarkType)i; 
            if(!MarkCount.ContainsKey(currentMark))
            {
                MarkCount.Add(currentMark, 0);
            }
        }
        TurnPublicEvents.BeginPlayerTurn += TurnStart;
        PublicEvents.EndBattle += BattleCleanup;
    }


    /// <summary>
    /// Unsubscribes from the BeginPlayerTurn event
    /// </summary>
    public void OnDisable()
    {
        TurnPublicEvents.BeginPlayerTurn -= TurnStart;
        PublicEvents.EndBattle -= BattleCleanup;
    }

    /// <summary>
    /// Sets a reference to the player
    /// </summary>
    /// <param name="p"></param>
    public static void SetPlayer(PlayerStats p)
    {
        player = p;
    }

#region  Trigger Conditions
    /// <summary>
    /// Clear any effects from battle-related marks
    ///     HealthPercent
    ///     EnemyDeath
    /// </summary>
    public static void BattleCleanup()
    {
        turnCount = 0;
        foreach(MarkData m in Marks)
        {
            if(m.TriggerCondition == MarkTriggerCondition.HealthPercent || m.TriggerCondition == MarkTriggerCondition.EnemyDeath)
            {
                if(MarkCount[m.Name]==2)
                {
                    foreach(MarkEffectsLinked me in m.EffectsWith2)
                    {
                        UpdateEffect(me.Effect, me.valueChange, m, false);
                    }
                }
                else if(MarkCount[m.Name]==3)
                {
                    foreach(MarkEffectsLinked me in m.EffectsWith3)
                    {
                        UpdateEffect(me.Effect, me.valueChange, m, false);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Called when the player's health changes
    /// Removes the effects if the condition is no longer met
    /// Shouldn't trigger each turn the condition is met
    /// </summary>
    /// <param name="percent">Player's health value, as a percent</param>
    public static void HealthValueChanged(float percent)
    {
        foreach(MarkData m in Marks)
        {
            // Currently mark of Strength- buffs after a certain health percentage
            if(m.TriggerCondition == MarkTriggerCondition.HealthPercent)
            {
                bool add = true;
                // Allows the effect to trigger the next time the condition is met
                if(!m.EffectCanTrigger && ((percent < m.Percent && m.TriggerIfAbove) || (percent > m.Percent && !m.TriggerIfAbove)))
                {
                    m.EffectCanTrigger = true;
                    if(m.TimesTriggered == 0)
                    if(m.TimesTriggered%2 == 0)
                    {
                        return;
                    }
                    add = false;
                }
                else if(!m.EffectCanTrigger || (m.EffectCanTrigger && ((percent < m.Percent && m.TriggerIfAbove) || (percent > m.Percent && !m.TriggerIfAbove))))
                {
                    return;
                }
                else if (m.EffectCanTrigger && ((percent <= m.Percent && !m.TriggerIfAbove) || (percent >= m.Percent && m.TriggerIfAbove)))
                {
                    m.EffectCanTrigger= false;
                    add = true;
                }
                else if(m.EffectCanTrigger && ((percent < m.Percent && m.TriggerIfAbove) || (percent > m.Percent && !m.TriggerIfAbove)))
                {
                }
                else
                {
                    return;
                }
                // This should cause the function to return if the percentage stays below or above the required amount




                {
                    foreach(MarkEffectsLinked e in m.EffectsWith2)
                    {
                        UpdateEffect(e.Effect, e.valueChange, m, add);
                    }
                }
                else if(MarkCount[m.Name] == 3)
                {
                    foreach (MarkEffectsLinked e in m.EffectsWith3)
                    {
                        UpdateEffect(e.Effect, e.valueChange, m, add);
                    }
                }
                m.EffectCanTrigger = false;
                m.TimesTriggered++;
            }
        }
    }

    /// <summary>
    /// Handles triggering Mark conditions for TurnStart and TurnCount
    /// Right now works for Restoration and Risk
    /// </summary>
    public static void TurnStart()
    {
        turnCount++;
        foreach(MarkData m in Marks)
        {
            if(turnCount == 1)
            {
                m.TimesTriggered = 0;
            }
            // Currently Mark of Restoration- heals
            if(m.TriggerCondition == MarkTriggerCondition.TurnStart)
            {
                if(MarkCount[m.Name] == 2)
                {
                    foreach (MarkEffectsLinked e in m.EffectsWith2)
                    {
                        UpdateEffect(e.Effect, e.valueChange, m, true);
                    }
                }
                else if (MarkCount[m.Name]==3)
                {
                    foreach (MarkEffectsLinked e in m.EffectsWith2)
                    {
                        UpdateEffect(e.Effect, e.valueChange, m, true);
                    }
                }
            }
            // Currently Mark of Risk- changes damage taken and damage dealt
            else if (m.TriggerCondition == MarkTriggerCondition.TurnCount)
            {
                if(turnCount == 1)
                {
                    if(MarkCount[m.Name] == 2)
                    {
                        foreach (MarkEffectsLinked e in m.EffectsWith2)
                        {
                            UpdateEffect(e.Effect, e.valueChange, m, true);
                        }
                    }
                    else if (MarkCount[m.Name] == 3)
                    {
                        foreach (MarkEffectsLinked e in m.EffectsWith3)
                        {
                            UpdateEffect(e.Effect, e.valueChange, m, true);
                        }
                    }
                }
                else if(MarkCount[m.Name]==2 && turnCount == m.TwoMarkTurnChange)
                {
                    foreach (MarkEffectsLinked e in m.EffectsWith2)
                    {
                        UpdateEffect(e.Effect, e.valueChange, m, false);
                    }
                    foreach (MarkEffectsLinked e in m.PostTurnEffectsWith2)
                    {
                        UpdateEffect(e.Effect, e.valueChange, m, true);
                    }
                }
                else if(MarkCount[m.Name]==3 && turnCount == m.ThreeMarkTurnChange)
                {
                    foreach (MarkEffectsLinked e in m.EffectsWith3)
                    {
                        UpdateEffect(e.Effect, e.valueChange, m, false);
                    }
                    foreach (MarkEffectsLinked e in m.PostTurnEffectsWith3)
                    {
                        UpdateEffect(e.Effect, e.valueChange, m, true);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Called when adding or removing Artifacts
    /// </summary>
    /// <param name="markCount"></param>
    /// <param name="adding"></param>
    public static void EquipValueChanged(bool adding, MarkType mark)
    {
        foreach (MarkData m in Marks)
        {
            if(m.Name != mark)
            {
                continue;
            }
            if(m.TriggerCondition == MarkTriggerCondition.OnEquip)
            {
                if (MarkCount[m.Name] < 2 && !adding)
                {
                    foreach (MarkEffectsLinked e in m.EffectsWith2)
                    {
                        UpdateEffect(e.Effect, e.valueChange, m, false);
                        m.TwoConditionTrigger = true;
                    }
                }
                else if (MarkCount[m.Name] == 2 && adding && m.TwoConditionTrigger)
                {
                    foreach (MarkEffectsLinked e in m.EffectsWith2)
                    {
                        UpdateEffect(e.Effect, e.valueChange, m, true);
                    }
                    m.TwoConditionTrigger = false;
                }
                else if(MarkCount[m.Name] == 2 && !adding && !m.ThreeConditionTrigger)
                {

                    foreach (MarkEffectsLinked e in m.EffectsWith3)
                    {
                        UpdateEffect(e.Effect, e.valueChange, m, false);
                    }
                    foreach (MarkEffectsLinked e in m.EffectsWith2)
                    {
                        UpdateEffect(e.Effect, e.valueChange, m, true);
                    }
                    m.ThreeConditionTrigger = true;
                    m.TwoConditionTrigger = false;
                }
                else if (MarkCount[m.Name] == 3 && adding && m.ThreeConditionTrigger)
                {
                    foreach (MarkEffectsLinked e in m.EffectsWith2)
                    {
                        UpdateEffect(e.Effect, e.valueChange, m, false);
                    }
                    foreach (MarkEffectsLinked e in m.EffectsWith3)
                    {
                        UpdateEffect(e.Effect, e.valueChange, m, true);
                    }
                    m.ThreeConditionTrigger = false;
                }
                
            }
        }
    }

    /// <summary>
    /// Called when 
    /// </summary>
    public static void EnemyKilled()
    {
        foreach(MarkData m in Marks)
        {
            // Currently Mark of Conquest- adds AP
            if(m.TriggerCondition == MarkTriggerCondition.EnemyDeath)
            {
                if(MarkCount[m.Name] == 2)
                {
                    foreach (MarkEffectsLinked e in m.EffectsWith2)
                    {
                        UpdateEffect(e.Effect, e.valueChange, m, true);
                    }
                }
                else if (MarkCount[m.Name]==3)
                {
                    foreach (MarkEffectsLinked e in m.EffectsWith2)
                    {
                        UpdateEffect(e.Effect, e.valueChange, m, true);
                    }
                }
            }
        }
    }

    #endregion

    /// <summary>
    /// Updates what effects marks are causing
    /// </summary>
    /// <param name="e">Mark Effect</param>
    /// <param name="val">Value change</param>
    /// <param name="player">Reference to PlayerStats</param>
    /// <param name="adding">Whether the effect is being added or removed</param>
    /// <param name="markCount">How many marks of this type</param>
    private static void UpdateEffect(MarkEffects e, float val, MarkData m, bool adding = true)
    {
        switch (e)
        {
            case MarkEffects.AttackMultiplier:
                AdjustValueGeometrically(ref player.BaseAttackMultiplier, val, adding);
                break;
            case MarkEffects.DamageTaken:
                AdjustValueGeometrically(ref player.DamageTakenMultiplier, val, adding);
                break;
            case MarkEffects.Heal:
                player.Heal((int)(val));
                break;
            case MarkEffects.APOnEnemyDeath:
                if((MarkCount[m.Name] == 2 && m.TimesTriggered < m.maxTriggerWith2) ||
                    (MarkCount[m.Name]==3 && m.TimesTriggered < m.maxTriggerWith3))
                {
                    gm.CurrentActionPoints++;
                    m.TimesTriggered++;
                }
                break;
            case MarkEffects.IncreasedXP:
                AdjustValueGeometrically(ref player.XPMultiplier, val, adding);
                break;
            case MarkEffects.IncreasedRAPDrop:
                AdjustValueGeometrically(ref player.RAPChanceModifier, val, adding);
                break;
            case MarkEffects.Luck:
                AdjustValueGeometrically(ref player.LuckModifier, val, adding);
                break;
            default:
                break;
        }
    }


    /// <summary>
    /// Update the internal Mark count to align with ArtifactManager
    /// </summary>
    /// <param name="key">Key to update</param>
    /// <param name="val">Stored val</param>
    public static void UpdateDictionary(MarkType key, int val)
    {
        MarkCount[key] = val;
        Debug.LogWarning("Updated " + key.ToString() + " to " + val.ToString());
    }

    /// <summary>
    /// Used for multiplicative or divisive adjustments to values
    /// </summary>
    /// <param name="x">The value to be adjusted</param>
    /// <param name="y">The value to be multiplied/divided by</param>
    /// <param name="b">True for adding an artifact</param>
    private static void AdjustValueGeometrically(ref float x, float y, bool b)
    {
        x = (b ? x * y : x / y);
    }

    /// <summary>
    /// Used for additive or subtractive adjustments to values using ints
    /// </summary>
    /// <param name="x">The value to be adjusted</param>
    /// <param name="y">The value being added/subtracted</param>
    /// <param name="b">True for adding an artifact</param>
    private static void AdjustValueArithmetically(ref float x, float y, bool b)
    {
        x = x + (b ? y : y * -1);
    }

    private static void AdjustValueAOrG(ref float x, float y, bool b)
    {
        if (x > 0f)
        {
            AdjustValueGeometrically(ref x, y, b);
        }
        else
        {
            AdjustValueArithmetically(ref x, y, b);
        }
    }
}