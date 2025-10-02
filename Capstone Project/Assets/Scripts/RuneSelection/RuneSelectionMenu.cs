/*************************************************
Author Names : 	Jay Embry
Date Created : 	09/30/2025
Date Last Modified : 10/02/2025
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
    [ShowIf(nameof(currentInspectorShowing), Variables.Runes), SerializeField]
    public List<RuneData> runeData;

    #endregion RUNES


    #region BUTTONS

    [HorizontalLine(3, EColor.Blue)]

    [ShowIf(nameof(currentInspectorShowing), Variables.Buttons), SerializeField]
    private List<GameObject> buttons;

    #endregion BUTTONS


    #region ENABLE BUTTONS

    private void Start()
    {

        //the button should store the rune's effect function eventually
        for(int i = 0; i < runeData.Count; i++)
        {

            buttons[i].GetComponentInChildren<Button>().GetComponentInChildren<TMP_Text>().text = runeData[i].RuneName;
            buttons[i].SetActive(true);

        }

    }

    #endregion ENABLE BUTTONS

}
