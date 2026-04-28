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
    [Tooltip("The size of the font.")]
    public int FontSize;
    [Tooltip("How big the textbox should be horizontally.")]
    public float HorizontalSize;
    [Tooltip("How big the textbox should be vertically.")]
    public float VerticleSize;
    [Tooltip("Where the center of the textbox will be located.")]
    public Vector2 TextboxLocation;
    [Tooltip("If true, will show the next textbox after this.")]
    public bool GoToNextTextbox;
    [Tooltip("If true, will keep the text box present while preventing it from going forward")]
    public bool disableClick;
}
