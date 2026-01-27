/*************************************************
Author Names : 	Jay Embry
Date Created : 	09/30/2025
Date Last Modified : 10/02/2025
Brief Description : Stores button data?
                    Kind of unnecessary after RuneData's been made but could be used for stylization
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
        Sprites

    }

    [SerializeField] private Properties currentInspectorShowing;

    #endregion SETUP


    #region TEXT PROPERTIES

    [HorizontalLine(3, EColor.Red)]

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

}
