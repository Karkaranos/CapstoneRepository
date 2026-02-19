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

    public void SetUpInfoBox(TileBehaviour tile) {

        print("called");
        MeleeEnemy meleeEnemyScript = tile?.entityObject?.GetComponent<MeleeEnemy>();
        RangedEnemy rangedEnemyScript = tile?.entityObject?.GetComponent<RangedEnemy>();

        if (meleeEnemyScript != null) {
            print(meleeEnemyScript.gameObject.name);
        }

        if (rangedEnemyScript != null)
        {
            print(rangedEnemyScript.gameObject.name);
        }
    }
}
