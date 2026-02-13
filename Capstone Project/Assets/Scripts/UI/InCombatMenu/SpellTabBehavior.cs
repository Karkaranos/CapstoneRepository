using UnityEngine;
using UnityEngine.UI;

public class SpellTabBehavior : MonoBehaviour
{
    RuneData runeData = null;
    public Image runeImage;
    public void SetUp(RuneData rd) {
        print("alled");
        runeData = rd;
        runeImage.sprite = runeData.runeImage;
        // show the pips.
    }
}
