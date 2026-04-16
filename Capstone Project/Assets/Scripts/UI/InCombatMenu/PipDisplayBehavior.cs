/*************************************************
Author Names : 		Tyler Bouchard
Date Created : 		2/10/2026
Date Last Modified : 3/25/2026
Brief Description : displays the pips you have
***************************************************/
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PipDisplayBehavior : MonoBehaviour
{
    public GameObject[] pipIndicators;
    public bool usesImageDisplay;
    public TextMeshProUGUI pipDisplayText;


    /// <summary>
    /// display the amount of pips you want
    /// </summary>
    /// <param name="amount"></param>
    public void DisplayPips(int amount) {
        if (usesImageDisplay)
        {
            if (amount > pipIndicators.Length)
            {
                amount = pipIndicators.Length;
            }

            foreach (GameObject pip in pipIndicators)
            {
                pip.GetComponent<Image>().color = new Color(1,1,1,0.25f);
            }

            for (int i = 0; i < amount; i++)
            {
                pipIndicators[i].GetComponent<Image>().color = new Color(1, 1, 1, 1f);
            }
        }
        else {
            pipDisplayText.text = "" + amount;
        }
    }
}
