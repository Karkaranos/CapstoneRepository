using UnityEngine;

public class SpellTabsManager : MonoBehaviour
{
    public SpellTabBehavior[] spellTabs;
    public void SetUpSpellTabs() {
        for (int i = 0; i < EquipedRunesAndArtifacts.runes.Count; i++) {
            spellTabs[i].SetUp(EquipedRunesAndArtifacts.runes[i]);
        }
    }
}
