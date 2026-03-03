/*************************************************
Author Names : 		Clare
Date Created : 		2/28/2026
Date Last Modified : 3/2/2026
Brief Description : controls the out of combat menu stat box
***************************************************/
using TMPro;
using UnityEngine;

public class StatBox : MonoBehaviour
{
    [SerializeField] private TMP_Text windDamange;
    [SerializeField] private TMP_Text lightningDamage;
    [SerializeField] private TMP_Text meleeResist;
    [SerializeField] private TMP_Text rangeResist;


    /// <summary>
    /// subscribing to events
    /// </summary>
    private void OnEnable()
    {
        PublicEvents.AddToStatBox += AddToStatBox;   
        PublicEvents.RemoveFromStatBox += RemoveFromStatBox;
    }

    /// <summary>
    /// unsubscribing/ to events
    /// </summary>
    private void OnDisable()
    {
        PublicEvents.AddToStatBox -= AddToStatBox;
        PublicEvents.RemoveFromStatBox -= RemoveFromStatBox;
    }

    /// <summary>
    /// Updates text UI to 20% when equiping an artifact
    /// </summary>
    /// <param name="data"></param>
    private void AddToStatBox(string data)
    {
        string previousInfo;
        switch (data)
        {
            case "Wind +":
                previousInfo = windDamange.text.Split(':', System.StringSplitOptions.None)[0];
                windDamange.text = previousInfo + ": 20%";
                break;

            case "Lightning +":
                previousInfo = lightningDamage.text.Split(":", System.StringSplitOptions.None)[0];
                lightningDamage.text = previousInfo + ": 20%";
                break;
            case "Ranged Resist":
                previousInfo = rangeResist.text.Split(":", System.StringSplitOptions.None)[0];
                rangeResist.text = previousInfo + ": 20%";
                break;
            case "Melee Resist":
                previousInfo = meleeResist.text.Split(":", System.StringSplitOptions.None)[0];
                meleeResist.text = previousInfo + ": 20%";
                break;
            default:
                break;

        }
    }

    /// <summary>
    /// Resets the UI text to 0% when unequiping an artifact
    /// </summary>
    /// <param name="data"></param>
    private void RemoveFromStatBox(string data)
    {
        string previousInfo;
        switch (data)
        {
            case "Wind +":
                previousInfo = windDamange.text.Split(':', System.StringSplitOptions.None)[0];
                windDamange.text = previousInfo + ": 0%";
                break;

            case "Lightning +":
                previousInfo = lightningDamage.text.Split(":", System.StringSplitOptions.None)[0];
                lightningDamage.text = previousInfo + ": 0%";
                break;
            case "Ranged Resist":
                previousInfo = rangeResist.text.Split(":", System.StringSplitOptions.None)[0];
                rangeResist.text = previousInfo + ": 0%";
                break;
            case "Melee Resist":
                previousInfo = meleeResist.text.Split(":", System.StringSplitOptions.None)[0];
                meleeResist.text = previousInfo + ": 0%";
                break;
            default:
                break;

        }
    }
}
