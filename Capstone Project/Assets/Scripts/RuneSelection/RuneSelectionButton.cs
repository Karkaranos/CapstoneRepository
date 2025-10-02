/*************************************************
Author Names : 	Jay Embry
Date Created : 	09/30/2025
Date Last Modified : 09/30/2025
Brief Description : Stores data for the rune selection menu's buttons.
				Stores submenus?
External Resources : 	
	***************************************************/

using NaughtyAttributes;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]

public class RuneSelectionButton
{

    #region SETUP

    [System.Serializable] public enum Properties
    {

        Text,
        Sprites,
        Function

    }

    [SerializeField] private Properties currentInspectorShowing;

    #endregion SETUP


    #region TEXT PROPERTIES

    [HorizontalLine(3, EColor.Red)]

    [ShowIf(nameof(currentInspectorShowing), Properties.Text), SerializeField, AllowNesting]
    [Tooltip("What is this rune called?")] private string runeName;

    [ShowIf(nameof(currentInspectorShowing), Properties.Text), SerializeField, AllowNesting]
    [Tooltip("What size is the text?")] private float textSize;

    [ShowIf(nameof(currentInspectorShowing), Properties.Text), SerializeField, AllowNesting]
    [Tooltip("What color is the text?")] private Color textColor;

    #endregion TEXT PROPERTIES


    #region SPRITE PROPERTIES

    [HorizontalLine(3, EColor.Blue)]

    [ShowIf(nameof(currentInspectorShowing), Properties.Sprites), SerializeField, AllowNesting]
    [Tooltip("How big is this button?")] private Vector3 buttonSize;

    [ShowIf(nameof(currentInspectorShowing), Properties.Sprites), SerializeField, AllowNesting]
    [Tooltip("What are the buttons supposed to look like?")] private Sprite highlightedSprite, pressedSprite, selectedSprite, disabledSprite;

    #endregion SPRITE PROPERTIES


    #region FUNCTIONS

    //[HorizontalLine(3, EColor.Indigo)]

    #endregion FUNCTIONS

}
