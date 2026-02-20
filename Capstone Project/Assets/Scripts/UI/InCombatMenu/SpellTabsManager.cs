/*************************************************
Author Names : 		Tyler Bouchard
Date Created : 		2/10/2026
Date Last Modified : 2/19/2026
Brief Description : manages the spell tabs, sets them up and is resopnsible for selecting them
***************************************************/
using System.Collections.Generic;
using UnityEngine;

public class SpellTabsManager : MonoBehaviour
{
    public SpellTabBehavior[] spellTabs;
    public SpellDisplayBoxBehavior spellInfoBox;

    /// <summary>
    /// this is what gets called to set up the spell tabs in the in combat menu
    /// </summary>
    public void SetUpSpellTabs() {
        List<RuneData> spells = new List<RuneData>();

        //resets all of the spell tabs so there is no overlap in setup
        foreach (SpellTabBehavior spellTab in spellTabs)
        {
            spellTab.Deactivate();
        }

        //finds all of the equiped spells 
        foreach (RuneData rune in EquipedRunesAndArtifacts.runes) {
            if (rune != null)
            {
                spells.Add(rune);
            }
        }

        //sets up the spell tabs with the spell that they will cast
        for (int i = 0; i < spells.Count; i++) {
            spellTabs[i].SetUp(spells[i]);
            //InCombatMenuManager.spellTabs.Add(spellTabs[i]);
        }

        //deactivates the spell tabs that dont have a rune data stored in them
        foreach (SpellTabBehavior spellTab in spellTabs) {
            if (spellTab.runeData == null) {
                spellTab.Deactivate();
            }
        }
    }

    /// <summary>
    /// selects the spell tab
    /// </summary>
    /// <param name="stb"></param>
    public void SelectTab(SpellTabBehavior stb) {
        DeselectAllTabs();
        stb.PopOut();
        stb.selected = true;
    }

    /// <summary>
    /// deselects every spell tab
    /// </summary>
    public void DeselectAllTabs()
    {
        foreach (SpellTabBehavior spellTab in spellTabs)
        {
            spellTab.selected = false;
            if (spellTab.poppedOut)
            {
                spellTab.Retact();
            }
        }
    }
}