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
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    [ShowIf(nameof(currentInspectorShowing), Variables.Buttons), SerializeField]
    private List<GameObject> containers;

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
            buttons[index].GetComponent<Image>().sprite = skillAndEquipManager.equippedSpells[index].runeImage;
            buttons[index].GetComponent<InCombatSpellSlotBehavior>().rune = skillAndEquipManager.equippedSpells[index];

            buttons[index].SetActive(true);


            //Links rune effect to button based on rune type
            //creates a new event trigger to add to the container
            EventTrigger trigger = containers[index].GetComponent<EventTrigger>();
            EventTrigger.Entry tree = new()
            {
                eventID = EventTriggerType.PointerClick,
                callback = new EventTrigger.TriggerEvent()
            };

            //stops weird garbage collection
            int i = index;

            //i fucking hate this syntax
            tree.callback.AddListener((eventData) => 
            {
                PublicEvents.RuneSelected.Invoke(skillAndEquipManager.equippedSpells[i]);
                FindAnyObjectByType<UIAudioManager>().PlayUIClick();
            });

            trigger.triggers.Add(tree);

        }

    }

    /// <summary>
    /// unhighlights all of the spell slots;
    /// </summary>
    public void RemoveAllContainerHighlights() { 
        foreach (GameObject container in containers) {
            container.GetComponent<SpellSlotHighlightBehavior>().RemoveHighlight();
        }
    }

    #endregion ENABLE BUTTONS

}
