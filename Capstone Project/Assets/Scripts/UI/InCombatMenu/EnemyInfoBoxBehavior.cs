/*************************************************
Author Names : 		Tyler Bouchard
Date Created : 		2/10/2026
Date Last Modified : 2/19/2026
Brief Description : This is for displaying the enemy info, reads a tile behavior and if that tile behavior has an enemy 
it displays its info
***************************************************/
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInfoBoxBehavior : MonoBehaviour
{
    public TextMeshProUGUI moveRangeText;
    public TextMeshProUGUI attackRangeText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI attackText;

    public Image LightningEffectIndicatorImage;
    public Image WindEffectIndicatorImage;

    public float popOutAmount;
    private bool poppedOut;

    /// <summary>
    /// moves the spell tab to the right (called with an event trigger on the spell tab)
    /// </summary>
    public void PopOut()
    {
        if (!poppedOut)
        {
            GetComponent<RectTransform>().anchoredPosition += new Vector2(-popOutAmount, 0f);
            poppedOut = true;
        }
    }

    /// <summary>
    /// moves the spell tab back to its start pos (called with an event trigger on the spell tab)
    /// </summary>
    public void Retact()
    {
        if (poppedOut)
        {
            GetComponent<RectTransform>().anchoredPosition -= new Vector2(popOutAmount, 0f);
            poppedOut = false;
        }
    }


}
