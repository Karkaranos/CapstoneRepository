using TMPro;
using UnityEngine;

public class NotebookDescriptionBoxBehavior : MonoBehaviour
{ 
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private GameObject[] pips;
    [SerializeField] private TextMeshProUGUI rangeText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    public void SetupTextBox(NotebookSpellNodeBehavior node) {

        print("hear me hear me");

        titleText.text = node.runeData.name;
        rangeText.text = "Range: " + node.runeData.RuneRange;
        damageText.text = "Damage: " + node.runeData.RuneDamage;
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
}
