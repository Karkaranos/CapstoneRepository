/*************************************************
Author Names : 	Jay Embry
Date Created : 	09/30/2025
Date Last Modified : 10/22/2025
Brief Description : The in-combat menus for rune selection.
                    Generates and displays buttons.
				    Displays submenus for the different tiers of spells.
External Resources : 	
	***************************************************/

using NaughtyAttributes;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class RuneSelectionMenu : MonoBehaviour
{

    #region SETUP

    [System.Serializable] public enum Variables
    {

        Buttons

    }

    [SerializeField] private Variables currentInspectorShowing;

    SkillAndArtifactManager skillAndEquipManager;

    private void Start()
    {

        skillAndEquipManager = GameObject.FindFirstObjectByType<SkillAndArtifactManager>();

    }

    #endregion SETUP


    #region RUNES

    public void OnEnable()
    {

        PublicEvents.EquipRunesToCombatMenu += EquipRunesToButtons;

    }

    public void OnDisable()
    {

        PublicEvents.EquipRunesToCombatMenu -= EquipRunesToButtons;

    }


    #endregion RUNES


    #region BUTTONS

    [HorizontalLine(3, EColor.Blue)]

    [ShowIf(nameof(currentInspectorShowing), Variables.Buttons), SerializeField]
    private List<GameObject> buttons;

    #endregion BUTTONS


    #region ENABLE BUTTONS

    /// <summary>
    /// Links buttons to rune data
    /// </summary>
    /// <param name="index"> Index of the rune being referenced from runeData </param>
    void EquipRunesToButtons(int index)
    {

        if (skillAndEquipManager.equippedSpells[index] == null)
        {

            buttons[index].SetActive(false);

        }
        else
        {


            //Activates button and updates button text
            //buttons and text will have to be adjusted to fit all of this in but that's already something that's being worked on i think
            buttons[index].GetComponentInChildren<Button>().GetComponentInChildren<TMP_Text>().text =
                skillAndEquipManager.equippedSpells[index].RuneName + " (AP: " + skillAndEquipManager.equippedSpells[index].RuneActionPoints + ")";

            buttons[index].SetActive(true);

            //Unnecessary but it'll make upcoming lines of code a bit easier to read
            RuneType runeType = skillAndEquipManager.equippedSpells[index].TypeOfRune;
            int runeNumber = skillAndEquipManager.equippedSpells[index].NumberOnSkillTree;
            float runeDamage = skillAndEquipManager.equippedSpells[index].RuneDamage;
            int runeRange = skillAndEquipManager.equippedSpells[index].RuneRange;
            GameObject runeVFX = skillAndEquipManager.equippedSpells[index].RuneVFX;
            int runeCost = skillAndEquipManager.equippedSpells[index].RuneActionPoints;


            //Links rune effect to button based on rune type
            buttons[index].GetComponentInChildren<Button>().onClick.AddListener(() => PublicEvents.RuneSelected.Invoke
            (skillAndEquipManager.equippedSpells[index]));

        }

    }

    #endregion ENABLE BUTTONS

}
