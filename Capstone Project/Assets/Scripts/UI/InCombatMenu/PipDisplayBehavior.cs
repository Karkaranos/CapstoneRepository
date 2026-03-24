/*************************************************
Author Names : 		Tyler Bouchard
Date Created : 		2/10/2026
Date Last Modified : 3/9/2026
Brief Description : displays the pips you have
***************************************************/
using TMPro;
using UnityEngine;

public class PipDisplayBehavior : MonoBehaviour
{
    public GameObject[] pipIndicators;
    public bool dislpaysNumber = false;
    public TextMeshProUGUI pipDisplayText;

    /// <summary>
    /// display the amount of pips you want
    /// </summary>
    /// <param name="amount"></param>
    public void DisplayPips(int amount) {
        if (dislpaysNumber)
        {
            pipDisplayText.text = "" + amount;
        }
        else {
            if (amount > pipIndicators.Length)
            {
                amount = pipIndicators.Length;
            }

            foreach (GameObject pip in pipIndicators)
            {
                pip.SetActive(false);
            }

            for (int i = 0; i < amount; i++)
            {
                pipIndicators[i].SetActive(true);
            }
        } 
    }
}
