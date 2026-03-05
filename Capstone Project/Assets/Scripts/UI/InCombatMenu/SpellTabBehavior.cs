/*************************************************
Author Names : 		Tyler Bouchard
Date Created : 		2/10/2026
Date Last Modified : 2/19/2026
Brief Description : Behavior for the spell tabs, controls how the pop out, sets them up with the data they need
***************************************************/
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellTabBehavior : MonoBehaviour
{
    public RuneData runeData = null;
    [SerializeField] private float moveAmount = 20f;
    public GameObject[] pips;
    public TextMeshProUGUI spellName;
    public Image runeImage;
    public bool selected = false;

    public bool poppedOut = false;

    /// <summary>
    /// moves the spell tab to the right (called with an event trigger on the spell tab)
    /// </summary>
    public void PopOut() {
        if (!poppedOut)
        {
            GetComponent<RectTransform>().anchoredPosition += new Vector2(moveAmount, 0f);
            poppedOut = true;
        }
    }

    /// <summary>
    /// moves the spell tab back to its start pos (called with an event trigger on the spell tab)
    /// </summary>
    public void Retact() {
        if (poppedOut && !selected) {
            GetComponent<RectTransform>().anchoredPosition -= new Vector2(moveAmount, 0f);
            poppedOut = false;
        }
    }

    /// <summary>
    /// sets up the spell tab
    /// </summary>
    public void SelectSpellTab() { 
        GetComponentInParent<SpellTabsManager>().SelectTab(this);
    }
}