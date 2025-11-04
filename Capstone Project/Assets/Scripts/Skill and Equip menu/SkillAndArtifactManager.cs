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
using UnityEngine.UI;

public class SkillAndArtifactManager : MonoBehaviour
{
    #region VARS
    private enum Settings
    {
        References,
        TestingAndDebug
    }

    [SerializeField] private Settings InspectorSettings;

    #region REFS

    [HorizontalLine(4, EColor.Red)]

    //all the containers for the different menus
    [ShowIf(nameof(InspectorSettings), Settings.References), SerializeField] private GameObject SkillTreeContainer;
    [ShowIf(nameof(InspectorSettings), Settings.References), SerializeField] private GameObject EquipMenuContainer;
    [ShowIf(nameof(InspectorSettings), Settings.References), SerializeField]
    private GameObject OutOfCombatMenuContainer;

    [SerializeField, ShowIf(nameof(InspectorSettings), Settings.References)]
    private Button continueButton;

    [SerializeField, ShowIf(nameof(InspectorSettings), Settings.References)]
    private GameObject cursorBoxPrefab;

    #endregion

    #region TESTING AND DEBUG

    //number of spell slots the player has
    [ShowIf(nameof(InspectorSettings), Settings.TestingAndDebug), SerializeField] public int NumOfSpellSlots;

    //master list of all equipped spells the player has
    [ShowIf(nameof(InspectorSettings), Settings.TestingAndDebug), SerializeField, Expandable,
        OnValueChanged(nameof(UpdateContinueButton))] public List<RuneData> equippedSpells;


    #endregion

    
    private GameObject spawnedCursorBox;

    #endregion

    /// <summary>
    /// Updates whether or not the player can move on from the menu
    /// </summary>
    /// <returns></returns>
    private bool UpdateContinueButton()
    {
        foreach (RuneData d in equippedSpells)
        {
            if (d != null)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// sets up the number of spell slots
    /// </summary>
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

        if (spawnedCursorBox != null)
        {
            Destroy(spawnedCursorBox);
            spawnedCursorBox = null;
        }
    }

    /// <summary>
    /// moves on to the next level
    /// </summary>
    public void ContinueToNextLevel()
    {
        OutOfCombatMenuContainer.SetActive(false);
        FindFirstObjectByType<RuneEvents>().gameObject.SetActive(false);

        PublicEvents.StartBattle.Invoke();
        //GameObject.Find("Move Confirmation").SetActive(false);
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
        continueButton.interactable = UpdateContinueButton();
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

    /// <summary>
    /// controls the temporary box to follow the cursor
    /// </summary>
    public void SpawnCursorBox()
    {
        if (spawnedCursorBox == null)
        {
            spawnedCursorBox = Instantiate(cursorBoxPrefab, transform);
        }
    }

    /// <summary>
    /// deletes the box
    /// </summary>
    public void DeleteCursorBox()
    {
        if (spawnedCursorBox != null)
        {
            Destroy(spawnedCursorBox);
            spawnedCursorBox = null;
        }
    }

}
