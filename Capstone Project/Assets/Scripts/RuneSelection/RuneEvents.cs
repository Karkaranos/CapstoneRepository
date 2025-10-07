using UnityEngine;

public class RuneEvents : MonoBehaviour
{

    #region INITIALIZATION

    private void OnEnable()
    {

        PublicEvents.LightningRuneSelected += LightningRune;
        PublicEvents.WindRuneSelected += WindRune;

    }

    private void OnDisable()
    {

        PublicEvents.LightningRuneSelected -= LightningRune;
        PublicEvents.WindRuneSelected -= WindRune;

    }

    #endregion INITIALIZATION


    #region RUNE EVENTS

    public void LightningRune(int runeNumber)
    {

        Debug.Log("You used Lightning " + runeNumber + "!");

        if(PublicEvents.EndPlayerTurn != null)
        {

            PublicEvents.EndPlayerTurn();

        }

    }

    public void WindRune(int runeNumber)
    {

        Debug.Log("You used Wind " + runeNumber + "!");

        if (PublicEvents.EndPlayerTurn != null)
        {

            PublicEvents.EndPlayerTurn();

        }

    }

    #endregion RUNE EVENTS

}
