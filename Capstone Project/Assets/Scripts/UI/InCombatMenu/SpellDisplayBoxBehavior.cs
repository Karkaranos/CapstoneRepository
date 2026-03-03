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
    public GameObject[] pips = new GameObject[3];
    public float moveAmount;

    /// <summary>
    /// sets up the spellInfoBox
    /// </summary>
    /// <param name="stb"></param>
    public void SetupInfoBox(InCombatSpellSlotBehavior icsb) {
        spellName.text = icsb.rune.name;
        spellDescription.text = icsb.rune.RuneDescription;

        int attackPoints = icsb.rune.RuneActionPoints;
        foreach (GameObject p in pips )
        {
            p.SetActive(true);
        }

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
