/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    10/27/2025
Date Last Modified : 	10/27/2025
Brief Description : 	Handles Mark functionality 
                        Should eventually be created and held in the GameManager
External Resources : 	N/A
***************************************************/
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;


public class MarkManager
{
    private static List<MarkData> Marks;
    private static Dictionary<MarkType, int> MarkCount;

    private static GameManager gm;
    private static PlayerStats player;
    public MarkManager(List<MarkData> marks, GameManager game)
    {
        Marks = marks;
        gm = game;
    }

    /// <summary>
    /// Sets a reference to the player
    /// </summary>
    /// <param name="p"></param>
    public static void SetPlayer(PlayerStats p)
    {
        player = p;
    }
    /// <summary>
    /// Clear any effects from battle-related marks
    ///     HealthPercent
    ///     EnemyDeath
    /// </summary>
    /// <param name="mark">Mark</param>
    /// <param name="markCount">How many of the mark are at play</param>
    /// <param name="player">Reference to playerStats</param>
    public static void ClearBattleMarkEffects(MarkData mark, int markCount)
    {
        // Add the enem
        if(mark.TriggerCondition == MarkTriggerCondition.HealthPercent || mark.TriggerCondition == MarkTriggerCondition.EnemyDeath)
        {
            //Yes, there is a way to simplify this into one line of code by having the effects inherit from a parent
            //It's not worth it
            if (markCount == 3)
            {
                foreach (MarkEffectsLinked me in mark.EffectsWith3)
                {
                    UpdateEffect(me.Effect, me.valueChange, false, 3);
                }
            }
            else if (markCount == 2)
            {
                foreach (MarkEffectsLinked me in mark.EffectsWith2)
                {
                    UpdateEffect(me.Effect, me.valueChange, false, 2);
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
    /// <param name="mark">The mark</param>
    /// <param name="markCount">How many of the mark there are</param>
    /// <param name="player">Reference to PlayerStats</param>
    public static void HealthValueChanged(float percent)
    {
        Debug.Log("Called health change");
        MarkData mark = null;
        foreach(MarkData m in Marks)
        {
            // Hardcoded
            if(m.MarkType == MarkType.Strength)
            {
                mark = m;
            }
        }

        if(mark=null)
        {
            return;
        }

        bool add = true;
        // Allows the effect to trigger the next time the condition is met
        if((mark.Percent < percent && mark.TriggerIfAbove || (mark.Percent > percent && !mark.TriggerIfAbove)))
        {
            mark.EffectCanTrigger = true;
            add = false;
        }

        // This should cause the function to return if the percentage stays below or above the required amount
        if(!mark.EffectCanTrigger)
        {
            return;
        }

        if(MarkCount[mark.MarkType] ==2)
        {
            foreach(MarkEffectsLinked e in mark.EffectsWith2)
            {
                UpdateEffect(e.Effect, e.valueChange, add);
            }
        }
        else if(MarkCount[mark.MarkType] == 3)
        {
            foreach (MarkEffectsLinked e in mark.EffectsWith3)
            {
                UpdateEffect(e.Effect, e.valueChange, add);
            }
        }
    }

    /// <summary>
    /// Called when adding or removing Artifacts
    /// This is messy af but it works
    /// </summary>
    /// <param name="mark"></param>
    /// <param name="markCount"></param>
    /// <param name="adding"></param>
    /// <param name="player"></param>
    public static void EquipValueChanged(MarkType mark, int markCount, bool adding)
    {
        if (markCount == 0 || (markCount == 1 && adding))
        {
            return;
        }
        foreach (MarkData md in Marks)
        {
            if(md.MarkType == mark)
            {
                if (markCount < 2)
                {
                    foreach (MarkEffectsLinked e in md.EffectsWith2)
                    {
                        UpdateEffect(e.Effect, e.valueChange, false);
                    }
                }
                else if (markCount == 2 && adding)
                {
                    foreach (MarkEffectsLinked e in md.EffectsWith2)
                    {
                        UpdateEffect(e.Effect, e.valueChange, true);
                    }
                }
                else if (markCount == 3)
                {
                    foreach (MarkEffectsLinked e in md.EffectsWith2)
                    {
                        UpdateEffect(e.Effect, e.valueChange, false);
                    }
                    foreach (MarkEffectsLinked e in md.EffectsWith3)
                    {
                        UpdateEffect(e.Effect, e.valueChange, true);
                    }
                }
                else if(markCount == 2 && !adding)
                {

                    foreach (MarkEffectsLinked e in md.EffectsWith3)
                    {
                        UpdateEffect(e.Effect, e.valueChange, false);
                    }
                    foreach (MarkEffectsLinked e in md.EffectsWith2)
                    {
                        UpdateEffect(e.Effect, e.valueChange, true);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Updates what effects marks are causing
    /// </summary>
    /// <param name="e">Mark Effect</param>
    /// <param name="val">Value change</param>
    /// <param name="player">Reference to PlayerStats</param>
    /// <param name="adding">Whether the effect is being added or removed</param>
    /// <param name="markCount">How many marks of this type</param>
    private static void UpdateEffect(MarkEffects e, float val, bool adding = true, int? markCount = 2, int? turnCount = 0)
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
                // since it occurs on death it shouldnt be affected when removed
                gm.CurrentActionPoints++;
                break;
            case MarkEffects.IncreasedXP:
                Logger.Warning("Implement XP Drop on Enemy Death");
                break;
            case MarkEffects.IncreasedRAPDrop:
                Logger.Warning("Implement RAP Drop on Enemy Death");
                break;
            case MarkEffects.Luck:
                AdjustValueAOrG(ref player.LuckModifier, val, adding);
                break;
            default:
                break;
        }
    }

    public static void UpdateTurnCondition()
    {
        
    }

    /// <summary>
    /// Update the internal Mark count to align with ArtifactManager
    /// </summary>
    /// <param name="key">Key to update</param>
    /// <param name="val">Stored val</param>
    public static void UpdateDictionary(MarkType key, int val)
    {
        MarkCount[key] = val;
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