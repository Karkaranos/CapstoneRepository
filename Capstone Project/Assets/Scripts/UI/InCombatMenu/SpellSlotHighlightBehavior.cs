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

    private void Start()
    {
        RSM = FindAnyObjectByType<RuneSelectionMenu>();
    }

    public void showHighlight()
    {
        print("Callleeed");
        RSM.RemoveAllContainerHighlights();
        slotImage.sprite = highlightSprite;
    }

    public void RemoveHighlight()
    {
        slotImage.sprite = noramlSprite;
    }
}
