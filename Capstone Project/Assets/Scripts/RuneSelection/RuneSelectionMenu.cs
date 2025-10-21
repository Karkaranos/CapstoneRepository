/*************************************************
Author Names : 	Jay Embry
Date Created : 	09/30/2025
Date Last Modified : 10/07/2025
Brief Description : The in-combat menus for rune selection.
                    Generates and displays buttons.
				    Displays submenus for the different tiers of spells.
External Resources : 	
	***************************************************/


using System.Runtime.CompilerServices;
using NaughtyAttributes;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    SkillAndEquipManager skillAndEquipManager;

    private void Start()
    {

        skillAndEquipManager = GameObject.FindFirstObjectByType<SkillAndEquipManager>();

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
            buttons[index].GetComponentInChildren<Button>().GetComponentInChildren<TMP_Text>().text = skillAndEquipManager.equippedSpells[index].RuneName;
            buttons[index].SetActive(true);

            //Unnecessary but it'll make upcoming lines of code a bit easier to read
            int runeNumber = skillAndEquipManager.equippedSpells[index].NumberOnSkillTree;
            float runeDamage = skillAndEquipManager.equippedSpells[index].RuneDamage;
            int runeRange = skillAndEquipManager.equippedSpells[index].RuneRange;


            //Links rune effect to button based on rune type
            switch (skillAndEquipManager.equippedSpells[index].TypeOfRune)
            {

                case (RuneType.Lightning):

                    buttons[index].GetComponentInChildren<Button>().onClick.AddListener(() => PublicEvents.RuneSelected.Invoke(RuneType.Lightning, runeNumber, runeDamage, runeRange));
                    break;

                case (RuneType.Wind):

                    buttons[index].GetComponentInChildren<Button>().onClick.AddListener(() => PublicEvents.RuneSelected.Invoke(RuneType.Wind, runeNumber, runeDamage, runeRange));
                    break;

            }

        }

    }

    #endregion ENABLE BUTTONS

}
