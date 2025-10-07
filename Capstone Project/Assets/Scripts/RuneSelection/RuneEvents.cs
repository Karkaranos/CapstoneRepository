/*************************************************
Author Names : 	Jay Embry
Date Created : 	10/07/2025
Date Last Modified : 10/07/2025
Brief Description : Contains rune types and effects
External Resources : 	
	***************************************************/

using UnityEngine;

public class RuneEvents : MonoBehaviour
{

    #region INITIALIZATION

    /// <summary>
    /// Runs whenever this script is loaded into a scene
    /// </summary>
    private void OnEnable()
    {

        PublicEvents.LightningRuneSelected += LightningRune;
        PublicEvents.WindRuneSelected += WindRune;

    }

    /// <summary>
    /// Runs whenever this script is destroyed
    /// </summary>
    private void OnDisable()
    {

        PublicEvents.LightningRuneSelected -= LightningRune;
        PublicEvents.WindRuneSelected -= WindRune;

    }

    #endregion INITIALIZATION


    #region RUNE EVENTS

    /// <summary>
    /// Calls lightning rune effect
    /// </summary>
    /// <param name="runeNumber"> Grabs where this rune is on the skill tree </param>
    public void LightningRune(int runeNumber)
    {

        Debug.Log("You used Lightning " + runeNumber + "!");

        if(PublicEvents.EnemyTurnStarted != null)
        {

            PublicEvents.EnemyTurnStarted();

        }

    }

    /// <summary>
    /// Calls wind rune effect
    /// </summary>
    /// <param name="runeNumber"> Grabs where this rune is on the skill tree </param>
    public void WindRune(int runeNumber)
    {

        Debug.Log("You used Wind " + runeNumber + "!");

        if (PublicEvents.EnemyTurnStarted != null)
        {

            PublicEvents.EnemyTurnStarted();

        }

    }

    #endregion RUNE EVENTS

}
