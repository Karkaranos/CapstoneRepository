/*************************************************
Author Names : 	Jay Embry
Date Created : 	09/30/2025
Date Last Modified : 09/30/2025
Brief Description : The in-combat menus for rune selection.
                Generates and displays buttons.
				Displays submenus for the different tiers of spells.
External Resources : 	
	***************************************************/


using System.Runtime.CompilerServices;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Unity.VisualScripting;

public class RuneSelectionMenu : MonoBehaviour
{

    #region SETUP

    [System.Serializable] public enum Variables
    {

        Runes,

    }

    [SerializeField] private Variables currentInspectorShowing;

    #endregion SETUP


    #region RUNES

    [HorizontalLine(3, EColor.Red)]

    [ShowIf(nameof(currentInspectorShowing), Variables.Runes), SerializeField]
    private List<RuneSelectionButton> runeButtons;

    #endregion RUNES

    #region SPAWN RUNES

    private void Start()
    {
        
        foreach (var rune in runeButtons)
        {

            //Button button = this.gameObject.AddComponent<Button>();

        }

    }

    #endregion SPAWN RUNES

}
