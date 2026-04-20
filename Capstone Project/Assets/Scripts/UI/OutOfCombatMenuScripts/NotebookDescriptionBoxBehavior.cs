/*************************************************
Author Names : 		Tyler Bouchard, Cade Naylor
Date Created : 		3/5/2026
Date Last Modified : 4/20/2026
Brief Description : for updating the description box in the notebook in the in combat menu
***************************************************/
using TMPro;
using UnityEngine;

public class NotebookDescriptionBoxBehavior : MonoBehaviour
{ 
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private GameObject[] pips;
    [SerializeField] private TextMeshProUGUI rangeText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private NotebookSpellNodeBehavior nsnb;

    /// <summary>
    /// Assigns listeners to public evemts
    /// </summary>
    public void OnEnable()
    {
        PublicEvents.ArtifactChanged += UpdateDamageNumber;
    }

    /// <summary>
    /// Unassigns listeners from public events
    /// </summary>
    public void OnDisable()
    {
        PublicEvents.ArtifactChanged -= UpdateDamageNumber;
    }



    /// <summary>
    /// updates the text
    /// </summary>
    /// <param name="node"></param>
    public void SetupTextBox(NotebookSpellNodeBehavior node) {
        nsnb = node;
        titleText.text = node.runeData.name;
        rangeText.text = "Range: " + node.runeData.RuneRange;
        damageText.text = "Damage: " + (int)(node.runeData.RuneDamage * (node.runeData.TypeOfRune == RuneType.Lightning ? FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier
                : FindFirstObjectByType<PlayerStats>().WindAttackMultiplier));
        descriptionText.text = node.runeData.RuneDescription;

        // setting up the pips
        foreach (GameObject pip in pips) { 
            pip.SetActive(false);
        }

        for (int i = 0; i < node.runeData.RuneActionPoints; i++)
        {
            pips[i].SetActive(true);
        }
    }

    /// <summary>
    /// Updates the spell description damage number to match artifacts
    /// </summary>
    /// <param name="ad"></param>
    public void UpdateDamageNumber()
    {
        if (nsnb != null)
        {
            damageText.text = "Damage: " + (int)(nsnb.runeData.RuneDamage * (nsnb.runeData.TypeOfRune == RuneType.Lightning ? FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier
                   : FindFirstObjectByType<PlayerStats>().WindAttackMultiplier));
        }
    }

}
