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
    public TextMeshProUGUI spellDamage;
    public TextMeshProUGUI spellDescription;
    public GameObject[] pips;
    public float moveAmount;
    public Vector2 retractPosition;

    private void Start()
    {
        retractPosition = GetComponent<RectTransform>().anchoredPosition;
    }

    /// <summary>
    /// sets up the spellInfoBox
    /// </summary>
    /// <param name="stb"></param>
    public void SetupInfoBox(InCombatSpellSlotBehavior icsb) {
        if (icsb.rune != null) {
            PopOut();
            spellName.text = icsb.rune.name;
            spellDamage.text = "Damage: " + icsb.rune.RuneDamage;
            spellDescription.text = icsb.rune.RuneDescription;

            int attackPoints = icsb.rune.RuneActionPoints;
            foreach (GameObject p in pips)
            {
                p.SetActive(false);
            }

            for (int i = 0; i < attackPoints; i++)
            {
                pips[i].SetActive(true);
            }
        }
        else
        {
            Retact();
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
        GetComponent<RectTransform>().anchoredPosition = retractPosition;
    }
}
