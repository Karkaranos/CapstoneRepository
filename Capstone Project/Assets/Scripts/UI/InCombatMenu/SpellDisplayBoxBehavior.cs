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
