/*************************************************
Author Names : 		Tyler Hayes 
Date Created : 		10/2/2025
Date Last Modified : 10/6/2025
Brief Description : This manages the player's current 
                    equipped spells. Does not hold the 
                    final list of spells, however
External Resources : 	
	***************************************************/

using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class SkillAndEquipManager : MonoBehaviour
{
    #region VARS
    private enum Settings
    {
        References,
        TestingAndDebug
    }

    [SerializeField] private Settings InspectorSettings;

    #region REFS

    //all the containers for the different menus
    [ShowIf(nameof(InspectorSettings), Settings.References), SerializeField] private GameObject SkillTreeContainer;
    [ShowIf(nameof(InspectorSettings), Settings.References), SerializeField] private GameObject EquipMenuContainer;

    #endregion

    #region TESTING AND DEBUG

    //number of spell slots the player has
    [ShowIf(nameof(InspectorSettings), Settings.TestingAndDebug), SerializeField] public int NumOfSpellSlots;

    //master list of all equipped spells the player has
    [ShowIf(nameof(InspectorSettings), Settings.TestingAndDebug), SerializeField, Expandable] public List<RuneData> equippedSpells;

    #endregion

    #endregion

    private void OnEnable()
    {
        //sets the spell menu active
        SkillTreeContainer.SetActive(true);
        EquipMenuContainer.SetActive(false);

        //sets the spell slots
        if (equippedSpells.Count < NumOfSpellSlots)
        {
            for (int i = equippedSpells.Count; i < NumOfSpellSlots; i++)
            {
                equippedSpells.Add(null);
            }
        }
    }

    /// <summary>
    /// swaps the menus between the spell and equip menu
    /// </summary>
    /// <param name="isSkill"> if youre turning on the spell menu </param>
    public void SwapMenus(bool isSkill)
    {
        if (isSkill)
        {
            SkillTreeContainer.SetActive(true);
            EquipMenuContainer.SetActive(false);
        }
        else
        {
            SkillTreeContainer.SetActive(false);
            EquipMenuContainer.SetActive(true);
        }
    }

    /// <summary>
    /// moves on to the next level
    /// </summary>
    public void ContinueToNextLevel()
    {
        Debug.Log("Next Level");
    }

    /// <summary>
    /// sets the given spell to the equipped master list
    /// </summary>
    /// <param name="index"> index to set </param>
    /// <param name="data"> data to set to that index </param>
    public void SetIndexOfEquippedSpells(int index, RuneData data)
    {
        equippedSpells[index] = data;
        PublicEvents.EquipRunesToCombatMenu(index);
    }
    
    /// <summary>
    /// gets the spell from the given index
    /// </summary>
    /// <param name="index"> index to get from </param>
    /// <returns></returns>
    public RuneData GetIndexOfEquippedSpells(int index)
    {
        return equippedSpells[index];
    }

}
