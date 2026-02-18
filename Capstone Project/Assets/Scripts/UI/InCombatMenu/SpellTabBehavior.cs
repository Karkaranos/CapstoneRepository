using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellTabBehavior : MonoBehaviour
{
    [HideInInspector] public RuneData runeData = null;
    [SerializeField] private float moveAmount = 20f;
    public GameObject[] pips;
    public TextMeshProUGUI spellName;
    public Image runeImage;
    public bool selected = false;

    public bool poppedOut = false;

    /// <summary>
    /// sets up the spell tab with the runeData that it will be responsible for
    /// </summary>
    /// <param name="rd"></param>
    public void SetUp(RuneData rd) {
        gameObject.SetActive(true);
        runeData = rd;
        
        runeImage.sprite = runeData.runeImage;
        spellName.text = runeData.name;

        //showing the pip cost value
        foreach (GameObject pip in pips)
        {
            pip.SetActive(true);
        }
        for (int i = 0; i < pips.Length - runeData.RuneActionPoints; i++)
        {
            pips[i].SetActive(false);
        }
    }

    /// <summary>
    /// deactiveates the spell tab and resets what it is storing
    /// </summary>
    public void Deactivate() {
        gameObject.SetActive(false);
        runeImage.sprite = null;
        runeData = null;
    }

    /// <summary>
    /// moves the spell tab to the right (called with an event trigger on the spell tab)
    /// </summary>
    public void PopOut() {
        if (!selected)
        {
            GetComponent<RectTransform>().anchoredPosition += new Vector2(moveAmount, 0f);
            poppedOut = true;
        }
    }

    /// <summary>
    /// moves the spell tab back to its start pos (called with an event trigger on the spell tab)
    /// </summary>
    public void Retact() {
        if (!selected) {
            GetComponent<RectTransform>().anchoredPosition -= new Vector2(moveAmount, 0f);
            poppedOut = false;
        }
    }

    public void SelectSpellTab() { 
        GetComponentInParent<SpellTabsManager>().SelectTab(this);
    }
}