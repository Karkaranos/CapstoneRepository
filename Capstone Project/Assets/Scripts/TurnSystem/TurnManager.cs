/*************************************************
Author Names : 		Tyler Hayes 
Date Created : 		10/9/2025
Date Last Modified : 10/23/2025
Brief Description : Handles the changing of the phases of the turn
External Resources : 	
***************************************************/

using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
        Balancing,
        Debug,
        Refs,
        None
    }

    public ShownSettings shownSettings;

    #region Debug
    [HorizontalLine(4, EColor.Red)]

    //how much time the turn manager waits before actually starting
    [ShowIf(nameof(shownSettings), ShownSettings.Balancing), SerializeField, Range(0, 1),
        Tooltip("This is how long it waits before actually starting combat. Probably should be around 0.1")]
    private float StartCombatDelay;





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

    [ShowIf(nameof(shownSettings), ShownSettings.Debug), SerializeField] private System.Delegate[] WaitingOn;

    #endregion
    #region Refs

    [HorizontalLine(4, EColor.Gray)]

    [ShowIf(nameof(shownSettings), ShownSettings.Refs), SerializeField] private GameObject playerCanvas;

    #endregion

    /// <summary>
    /// subscribes to all needed events
    /// </summary>
    private void OnEnable()
    {
        TurnPublicEvents.TurnActionComplete += ProcessTurnActionComplete;
        PublicEvents.StartBattle += StartCombat;
        TurnPublicEvents.ForceEndCurrentPhase += NextPhase;
    }

    /// <summary>
    /// unsubscribes from all needed events
    /// </summary>
    private void OnDisable()
    {
        TurnPublicEvents.TurnActionComplete -= ProcessTurnActionComplete;
        PublicEvents.StartBattle -= StartCombat;
        TurnPublicEvents.ForceEndCurrentPhase -= NextPhase;
    }

    /// <summary>
    /// starts the phase on start
    /// </summary>
    private void Start()
    {
        breakInfLoop = false;
    }

    /// <summary>
    /// Starts combat after a set delay to let everything spawn in
    /// </summary>
    private void StartCombat()
    {
        StartCoroutine(DelayStartCombat());
    }

    /// <summary>
    /// Has the actual delay for starting combat
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayStartCombat()
    {
        yield return new WaitForSeconds(StartCombatDelay);
        SetPhase(TurnStates.Start);
    }


    /// <summary>
    /// Sets the phase to whatever the parameter is
    /// </summary>
    /// <param name="phaseToSetTo"> The phase to change to </param>
    /// <exception cref="System.Exception"></exception>
    private void SetPhase(TurnStates phaseToSetTo)
    {
        currentHearBackNum = 0;

        switch (phaseToSetTo)
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

    /// <summary>
    /// processes when an instance tells this that its action is complete
    /// </summary>
    private void ProcessTurnActionComplete()
    {
        Debug.Log("Called");

        //ups the number of instances this has heard back from
        currentHearBackNum++;

        //checks to see if its heard back from everything
        if (currentHearBackNum >= targetHearBackNum)
        {
            //goes to next phase if it has

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

    /// <summary>
    /// Determines the state to go to next.
    /// Called when the phase is over.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.Exception"></exception>
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
