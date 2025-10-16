/*************************************************
Author Names : 		Tyler Hayes 
Date Created : 		10/9/2025
Date Last Modified : 10/9/2025
Brief Description : Handles the changing of the phases of the turn
External Resources : 	
***************************************************/

using NaughtyAttributes;
using System.Collections;
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
    public enum ShownSettings
    {
        Debug,
        None
    }

    public ShownSettings shownSettings;

    [HorizontalLine(4, EColor.Indigo)]

    //current state of the turn
    [ShowIf(nameof(shownSettings), ShownSettings.Debug)] public TurnStates currentStatus;

    //how many instances this script has heard back from after 
    //sending out a new phase public event
    [ShowIf(nameof(shownSettings), ShownSettings.Debug), SerializeField] private int currentHearBackNum;

    //how many instances this script needs to hear back from
    //before sending out the next public event
    [ShowIf(nameof(shownSettings), ShownSettings.Debug), SerializeField] private int targetHearBackNum;

    [ShowIf(nameof(shownSettings), ShownSettings.Debug), SerializeField] private bool breakInfLoop = false;
    [ShowIf(nameof(shownSettings), ShownSettings.Debug), SerializeField] private bool gameHasStarted = false;

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
        breakInfLoop = false;
        StartCoroutine(StartGame());

    }

    /// <summary>
    /// waits a frame before starting the game
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartGame()
    {
        //temp hardcoded delay - will be removed and fixed post-milestone
        yield return new WaitForSecondsRealtime(5f);
        gameHasStarted = true;
        NextPhase();
    }

    /// <summary>
    /// processes when an instance tells this that its action is complete
    /// </summary>
    private void ProcessTurnActionComplete()
    {
        Debug.Log("Called");
        if (gameHasStarted)
        {
            //ups the number of instances this has heard back from
            currentHearBackNum++;

            //checks to see if its heard back from everything
            if (currentHearBackNum >= targetHearBackNum)
            {
                //goes to next phase if it has

                NextPhase();
            }
        }
        
    }

    /// <summary>
    /// Proceeds to the next phase
    /// </summary>
    /// <exception cref="System.Exception"></exception>
    [Button("Next Phase")]
    private void NextPhase()
    {
        //resets the number of things its heard back from
        currentHearBackNum = 0;

        //Debug.Log(currentHearBackNum);

        //determines what phase it is going to next
        currentStatus = DetermineNextState();

        //sends out a diff public event for each diff state
        switch (currentStatus)
        {
            case TurnStates.Start:

                //throws out the public event to start the phase
                TurnPublicEvents.BeginStartTurn?.Invoke();

                //sets the target number of instances to hear back from equal to the number of listeners
                //on the event
                if (TurnPublicEvents.BeginStartTurn?.GetInvocationList().Length > 0)
                {
                    targetHearBackNum = TurnPublicEvents.BeginStartTurn.GetInvocationList().Length;
                    breakInfLoop = false;
                }
                else
                {
                    NextPhase();
                }

                break;
            case TurnStates.PlayerTurn:
                
                //throws out the public event to start the phase
                TurnPublicEvents.BeginPlayerTurn?.Invoke();

                //sets the target number of instances to hear back from equal to the number of listeners
                //on the event
                if (TurnPublicEvents.BeginPlayerTurn?.GetInvocationList().Length > 0)
                {
                    targetHearBackNum = TurnPublicEvents.BeginPlayerTurn.GetInvocationList().Length;

                    breakInfLoop = false;
                }
                else
                {
                    NextPhase();
                }



                break;
            case TurnStates.EnemyTurn:

                //throws out the public event to start the phase
                TurnPublicEvents.BeginEnemyTurn?.Invoke();

                //sets the target number of instances to hear back from equal to the number of listeners
                //on the event
                if (TurnPublicEvents.BeginEnemyTurn?.GetInvocationList().Length > 0)
                {
                    targetHearBackNum = TurnPublicEvents.BeginEnemyTurn.GetInvocationList().Length;
                    breakInfLoop = false;
                }
                else
                {
                    NextPhase();
                }

                    break;
            case TurnStates.End:

                //throws out the public event to start the phase
                TurnPublicEvents.BeginEndTurn?.Invoke();

                //sets the target number of instances to hear back from equal to the number of listeners
                //on the event
                if (TurnPublicEvents.BeginEndTurn?.GetInvocationList().Length > 0)
                {
                    targetHearBackNum = TurnPublicEvents.BeginEndTurn.GetInvocationList().Length;
                    breakInfLoop = false;
                }
                else
                {
                    if (breakInfLoop)
                    {
                        Debug.Log("No listeners to the turnmanager, had to break an inf loop");
                        break;
                    }
                    breakInfLoop = true;
                    NextPhase();
                }


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
