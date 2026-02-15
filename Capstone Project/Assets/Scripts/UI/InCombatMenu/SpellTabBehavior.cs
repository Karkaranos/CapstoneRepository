using UnityEngine;
using UnityEngine.UI;

public class SpellTabBehavior : MonoBehaviour
{
    public RuneData runeData = null;
    public Image runeImage;

    /// <summary>
    /// sets up the spell tab with the runeData that it will be responsible for
    /// </summary>
    /// <param name="rd"></param>
    public void SetUp(RuneData rd) {
        gameObject.SetActive(true);
        runeData = rd;
        runeImage.sprite = runeData.runeImage;
    }

    /// <summary>
    /// deactiveates the spell tab and resets what it is storing
    /// </summary>
    public void Deactivate() {
        gameObject.SetActive(false);
        runeImage.sprite = null;
        runeData = null;
    }
}