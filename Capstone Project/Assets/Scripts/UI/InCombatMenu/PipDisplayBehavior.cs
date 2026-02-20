/*************************************************
Author Names : 		Tyler Bouchard
Date Created : 		2/10/2026
Date Last Modified : 2/19/2026
Brief Description : displays the pips you have
***************************************************/
using UnityEngine;

public class PipDisplayBehavior : MonoBehaviour
{
    public GameObject[] pipIndicators;

    /// <summary>
    /// display the amount of pips you want
    /// </summary>
    /// <param name="amount"></param>
    public void DisplayPips(int amount) {
        if (amount > pipIndicators.Length) 
        {
            amount = pipIndicators.Length;
        }
        for (int i = 0; i < amount; i++) {
            pipIndicators[pipIndicators.Length - i].gameObject.SetActive(false);
        }
    }
}
