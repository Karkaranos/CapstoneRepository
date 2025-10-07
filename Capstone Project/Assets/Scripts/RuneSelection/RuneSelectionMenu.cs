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

        Runes,
        Buttons

    }

    [SerializeField] private Variables currentInspectorShowing;

    #endregion SETUP


    #region RUNES

    [HorizontalLine(3, EColor.Red)]

    //this should be public so that it can be added onto based on the player's prep?
    [ShowIf(nameof(currentInspectorShowing), Variables.Runes), SerializeField, Expandable]
    public List<RuneData> runeData;

    public void OnEnable()
    {

        PublicEvents.EquipRunesToCombatMenu += EquipRunes;

    }


    /// <summary>
    /// Grabs what runes the player has equipped from SkillAndEquipManager
    /// </summary>
    /// <param name="rune"> Rune equipped </param>
    public void EquipRunes(RuneData rune)
    {

        runeData.Add(rune);
        EquipRunesToButtons(runeData.IndexOf(rune));

    }

    public void OnDisable()
    {

        PublicEvents.EquipRunesToCombatMenu -= EquipRunes;

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

        //Activates button and updates button text
        buttons[index].GetComponentInChildren<Button>().GetComponentInChildren<TMP_Text>().text = runeData[index].RuneName;
        buttons[index].SetActive(true);

        //Unnecessary but it'll make upcoming lines of code a bit easier to read
        int runeNumber = runeData[index].NumberOnSkillTree;


        //Links rune effect to button based on rune type
        switch (runeData[index].TypeOfRune)
        {

            case (RuneType.Lightning):

                buttons[index].GetComponentInChildren<Button>().onClick.AddListener(() => PublicEvents.LightningRuneSelected.Invoke(runeNumber));
                break;

            case (RuneType.Wind):

                buttons[index].GetComponentInChildren<Button>().onClick.AddListener(() => PublicEvents.WindRuneSelected.Invoke(runeNumber));
                break;

        }

    }

    #endregion ENABLE BUTTONS

}
