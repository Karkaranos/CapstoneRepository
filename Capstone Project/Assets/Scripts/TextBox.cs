/******************************************************************************
 * Author: Brad Dixon
 * Creation Date: 4/2/2026
 * Last Modified: 4/7/2026
 * Brief: Container for text box data
 * External Resources: N/A
 * ***************************************************************************/
using UnityEngine;
using TMPro;
using NaughtyAttributes;

[System.Serializable]
public class TextBox
{
    [ResizableTextArea, Tooltip("What the textbox will say.")]
    public string TextboxContent;
    [Tooltip("How big the textbox should be horizontally.")]
    public float HorizontalSize;
    [Tooltip("How big the textbox should be Vertically.")]
    public float VerticalSize;
    [Tooltip("Where the center of the textbox will be located.")]
    public Vector2 TextboxLocation;
}
