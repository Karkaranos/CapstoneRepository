/*************************************************
Author Names : 		Tyler Bouchard
Date Created : 		4/1/2026
Date Last Modified : 4/7/2026
Brief Description : Highlights the spell slot when it is selected
***************************************************/
using UnityEngine;
using UnityEngine.UI;

public class SpellSlotHighlightBehavior : MonoBehaviour
{
    public Sprite highlightSprite;
    public Sprite noramlSprite;

    public Image slotImage;

    private RuneSelectionMenu RSM;

    /// <summary>
    /// finds the RuneSelectionMenu thing
    /// </summary>
    private void Start()
    {
        RSM = FindAnyObjectByType<RuneSelectionMenu>();
    }

    /// <summary>
    /// shows highlight
    /// </summary>
    public void showHighlight()
    {
        print("Callleeed");
        RSM.RemoveAllContainerHighlights();
        slotImage.sprite = highlightSprite;
    }

    /// <summary>
    /// removes highlight
    /// </summary>
    public void RemoveHighlight()
    {
        slotImage.sprite = noramlSprite;
    }
}
