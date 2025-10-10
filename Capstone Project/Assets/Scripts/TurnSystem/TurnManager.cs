/*************************************************
Author Names : 		Tyler Hayes 
Date Created : 		10/9/2025
Date Last Modified : 10/9/2025
Brief Description : Handles the changing of the phases of the turn
External Resources : 	
***************************************************/

using NaughtyAttributes;
using UnityEngine;

//this is the enum that dictates what state the turn is in
public enum TurnStates
{
    Start,
    PlayerTurn,
    EnemyTurn,
    End
}
public class TurnManager : MonoBehaviour
{
    //current state of the turn
    public TurnStates currentStatus;

    //how many instances this script has heard back from after 
    //sending out a new phase public event
    public int currentHearBackNum;

    //how many instances this script needs to hear back from
    //before sending out the next public event
    public int targetHearBackNum;

    /// <summary>
    /// subscribes to all needed events
    /// </summary>
    private void OnEnable()
    {
        TurnPublicEvents.TurnActionComplete += ProcessTurnActionComplete;
    }

    /// <summary>
    /// unsubscribes from all needed events
    /// </summary>
    private void OnDisable()
    {
        TurnPublicEvents.TurnActionComplete -= ProcessTurnActionComplete;
    }

    /// <summary>
    /// starts the phase on start
    /// </summary>
    private void Start()
    {
        NextPhase();
    }

    /// <summary>
    /// processes when an instance tells this that its action is complete
    /// </summary>
    private void ProcessTurnActionComplete()
    {
        //ups the number of instances this has heard back from
        currentHearBackNum++;

        //checks to see if its heard back from everything
        if (currentHearBackNum >= targetHearBackNum)
        {
            //goes to next phase if it has
            currentHearBackNum = 0;
            NextPhase();
        }
    }

    /// <summary>
    /// Proceeds to the next phase
    /// </summary>
    /// <exception cref="System.Exception"></exception>
    [Button("Next Phase")]
    private void NextPhase()
    {
        //determines what phase it is going to next
        currentStatus = DetermineNextState();

        //sends out a diff public event for each diff state
        switch (currentStatus)
        {
            case TurnStates.Start:

                //throws out the public event to start the phase
                TurnPublicEvents.BeginStartTurn();

                //sets the target number of instances to hear back from equal to the number of listeners
                //on the event
                targetHearBackNum = TurnPublicEvents.BeginStartTurn.GetInvocationList().Length;
                break;
            case TurnStates.PlayerTurn:

                //throws out the public event to start the phase
                TurnPublicEvents.BeginPlayerTurn();

                //sets the target number of instances to hear back from equal to the number of listeners
                //on the event
                targetHearBackNum = TurnPublicEvents.BeginPlayerTurn.GetInvocationList().Length;
                break;
            case TurnStates.EnemyTurn:

                //throws out the public event to start the phase
                TurnPublicEvents.BeginEnemyTurn();

                //sets the target number of instances to hear back from equal to the number of listeners
                //on the event
                targetHearBackNum = TurnPublicEvents.BeginEnemyTurn.GetInvocationList().Length;
                break;
            case TurnStates.End:

                //throws out the public event to start the phase
                TurnPublicEvents.BeginEndTurn();

                //sets the target number of instances to hear back from equal to the number of listeners
                //on the event
                targetHearBackNum = TurnPublicEvents.BeginEndTurn.GetInvocationList().Length;
                break;
            default:
                throw new System.Exception("Check NextPhase() in TurnManager, the switch statement is broken or is missing cases");
        }
    }


    #region helperfuncs

    private TurnStates DetermineNextState()
    {
        //creates a null output
        TurnStates output;

        //sets the right next phase based on what the current phase is
        switch (currentStatus)
        {
            case TurnStates.Start:
                output = TurnStates.PlayerTurn;
                break;
            case TurnStates.PlayerTurn:
                output = TurnStates.EnemyTurn;
                break;
            case TurnStates.EnemyTurn:
                output = TurnStates.End;
                break;
            case TurnStates.End:
                output = TurnStates.Start;
                break;
            default:
                throw new System.Exception("Check DetermineNextState() in TurnManager, the switch statement is broken or is missing cases");
        }

        return output;
    }

    #endregion
}
