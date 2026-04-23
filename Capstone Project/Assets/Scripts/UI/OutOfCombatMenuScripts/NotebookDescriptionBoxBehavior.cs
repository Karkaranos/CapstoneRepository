/*************************************************
Author Names : 		Tyler Bouchard, Cade Naylor
Date Created : 		3/5/2026
Date Last Modified : 4/20/2026
Brief Description : for updating the description box in the notebook in the in combat menu
***************************************************/
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotebookDescriptionBoxBehavior : MonoBehaviour
{ 
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private GameObject[] pips;
    [SerializeField] private TextMeshProUGUI rangeText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image spellExampleImage;

    [SerializeField] private NotebookSpellNodeBehavior nsnb;

    /// <summary>
    /// Initializes the Notebook Page
    /// </summary>
    private void Start()
    {
        SetupTextBox(nsnb);
    }

    /// <summary>
    /// updates the text
    /// </summary>
    /// <param name="node"></param>
    public void SetupTextBox(NotebookSpellNodeBehavior node) {
        nsnb = node;
        titleText.text = nsnb.runeData.name;
        rangeText.text = "Range: " + nsnb.runeData.RuneRange;
        damageText.text = "Damage: " + (int)(nsnb.runeData.RuneDamage * (nsnb.runeData.TypeOfRune == RuneType.Lightning ? FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier
                : FindFirstObjectByType<PlayerStats>().WindAttackMultiplier));

        costText.text = "Cost: " + nsnb.runeData.RuneActionPoints;
        descriptionText.text = nsnb.runeData.RuneDescription;
        spellExampleImage.sprite = nsnb.runeData.runeExampleImage;


        // setting up the pips
        foreach (GameObject pip in pips) { 
            pip.SetActive(false);
        }

        for (int i = 0; i < nsnb.runeData.RuneActionPoints; i++)
        {
            pips[i].SetActive(true);
        }
    }

    /// <summary>
    /// Overloaded function if using the same node
    /// </summary>
    public void SetupTextBox()
    {
        SetupTextBox(nsnb);
    }

}
