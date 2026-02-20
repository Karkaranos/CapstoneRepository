/*************************************************
Author Names : 		Tyler Bouchard
Date Created : 		2/10/2026
Date Last Modified : 2/19/2026
Brief Description : This is the behavior for the spell display, It sets it up and pops it out
***************************************************/
using TMPro;
using UnityEngine;

public class SpellDisplayBoxBehavior : MonoBehaviour
{
    public TextMeshProUGUI spellName;
    public TextMeshProUGUI spellDescription;
    public float moveAmount;

    public void SetupInfoBox(SpellTabBehavior stb) {
        spellName.text = stb.runeData.name;
        spellDescription.text = stb.runeData.RuneDescription;
    }

    /// <summary>
    /// moves the spell tab to the right (called with an event trigger on the spell tab)
    /// </summary>
    public void PopOut()
    {
        GetComponent<RectTransform>().anchoredPosition += new Vector2(0f, moveAmount);
    }

    /// <summary>
    /// moves the spell tab back to its start pos (called with an event trigger on the spell tab)
    /// </summary>
    public void Retact()
    {
        GetComponent<RectTransform>().anchoredPosition -= new Vector2(0f, moveAmount);
    }
}
