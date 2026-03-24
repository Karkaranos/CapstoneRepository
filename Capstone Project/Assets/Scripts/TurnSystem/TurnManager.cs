/*************************************************
Author Names : 		Tyler Hayes 
Date Created : 		10/9/2025
Date Last Modified : 11/7/2025
Brief Description : Handles the changing of the phases of the turn
External Resources : 	
***************************************************/

using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
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
    [ShowIf(nameof(shownSettings), ShownSettings.Debug)] public static TurnStates currentStatus;
    [ShowIf(nameof(shownSettings), ShownSettings.Debug)] public TurnStates debugState;

    //how many instances this script has heard back from after 
    //sending out a new phase public event
    [ShowIf(nameof(shownSettings), ShownSettings.Debug), SerializeField] private int currentHearBackNum;

    //how many instances this script needs to hear back from
    //before sending out the next public event
    [ShowIf(nameof(shownSettings), ShownSettings.Debug), SerializeField] private int targetHearBackNum;

    [ShowIf(nameof(shownSettings), ShownSettings.Debug), SerializeField] private bool AutomaticallyGoToNextPhase = true;

    [ShowIf(nameof(shownSettings), ShownSettings.Debug), SerializeField] private bool breakInfLoop = false;

    #endregion
    #region Refs

    [HorizontalLine(4, EColor.Gray)]

    [ShowIf(nameof(shownSettings), ShownSettings.Refs), SerializeField] private GameObject playerCanvas;
    [ShowIf(nameof(shownSettings), ShownSettings.Refs), SerializeField] private GameObject turnIndicatorPrefab;

    private GameObject playerBanner;
    private GameObject enemyBanner; 

    #endregion


    /// <summary>
    /// subscribes to all needed events
    /// </summary>
    private void OnEnable()
    {
        TurnPublicEvents.TurnActionComplete += ProcessTurnActionComplete;
        PublicEvents.StartBattle += StartCombat;
        TurnPublicEvents.ForceEndCurrentPhase += NextPhase;
        PublicEvents.NewLevel += NewLevelOpened;
    }

    /// <summary>
    /// unsubscribes from all needed events
    /// </summary>
    private void OnDisable()
    {
        TurnPublicEvents.TurnActionComplete -= ProcessTurnActionComplete;
        PublicEvents.StartBattle -= StartCombat;
        TurnPublicEvents.ForceEndCurrentPhase -= NextPhase;
        PublicEvents.NewLevel -= NewLevelOpened;
    }

    private void NewLevelOpened()
    {
        StartCoroutine(delayedNewPlayerTurn());
        
    }

    private IEnumerator delayedNewPlayerTurn()
    {
        yield return null;
        targetHearBackNum = 0;
        currentHearBackNum = 0;
        SetPhase(TurnStates.PlayerTurn);
    }

    /// <summary>
    /// starts the phase on start
    /// </summary>
    private void Start()
    {
        breakInfLoop = false;
        currentStatus = TurnStates.Start;
        debugState = currentStatus;
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

        //attaching the turn indicator to the button manager because its on the main canvas and doesnt get disabled
        GameObject turnIndic = Instantiate(turnIndicatorPrefab, FindFirstObjectByType<ButtonManager>().transform);
        //turnIndicatorText = turnIndic.GetComponentInChildren<TMP_Text>();
        playerBanner = turnIndic.transform.GetChild(0).gameObject;
        enemyBanner = turnIndic.transform.GetChild(1).gameObject;
        enemyBanner.SetActive(false);

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

        currentStatus = phaseToSetTo;
        debugState = phaseToSetTo;

        switch (phaseToSetTo)
        {
            case TurnStates.Start:

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

                //throws out the public event to start the phase
                TurnPublicEvents.BeginStartTurn?.Invoke();

                break;
            case TurnStates.PlayerTurn:

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

                //throws out the public event to start the phase
                TurnPublicEvents.BeginPlayerTurn?.Invoke();

                break;
            case TurnStates.EnemyTurn:



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

                //throws out the public event to start the phase
                TurnPublicEvents.BeginEnemyTurn?.Invoke();

                break;
            case TurnStates.End:



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

                //throws out the public event to start the phase
                TurnPublicEvents.BeginEndTurn?.Invoke();

                break;
            default:
                throw new System.Exception("Check NextPhase() in TurnManager, the switch statement is broken or is missing cases");
        }

        SetTurnBanner();
    }

    /// <summary>
    /// processes when an instance tells this that its action is complete
    /// </summary>
    private void ProcessTurnActionComplete()
    {
        //Debug.Log("Called");

        //ups the number of instances this has heard back from
        ++currentHearBackNum;

        //checks to see if its heard back from everything
        if (currentHearBackNum >= targetHearBackNum)
        {
            //goes to next phase if it has

            if (AutomaticallyGoToNextPhase)
            {
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
        debugState = currentStatus;

        //sends out a diff public event for each diff state
        switch (currentStatus)
        {
            case TurnStates.Start:



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

                //throws out the public event to start the phase
                TurnPublicEvents.BeginStartTurn?.Invoke();

                break;
            case TurnStates.PlayerTurn:



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

                //throws out the public event to start the phase
                TurnPublicEvents.BeginPlayerTurn?.Invoke();

                break;
            case TurnStates.EnemyTurn:



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

                //throws out the public event to start the phase
                TurnPublicEvents.BeginEnemyTurn?.Invoke();

                break;
            case TurnStates.End:



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

                //throws out the public event to start the phase
                TurnPublicEvents.BeginEndTurn?.Invoke();

                break;
            default:
                throw new System.Exception("Check NextPhase() in TurnManager, the switch statement is broken or is missing cases");
        }

        SetTurnBanner();
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

    /// <summary>
    /// Updates the text for the turn indicator. Will also call other between turn stuff eventually
    /// </summary>
    /// <exception cref="System.Exception"></exception>
    private void SetTurnBanner()
    {
        switch (currentStatus)
        {
            case TurnStates.Start:
                playerBanner.SetActive(true);
                enemyBanner.SetActive(false);
                break;
            case TurnStates.PlayerTurn:
                playerBanner.SetActive(true);
                enemyBanner.SetActive(false);
                break;
            case TurnStates.EnemyTurn:
                enemyBanner.SetActive(true);
                playerBanner.SetActive(false);
                break;
            case TurnStates.End:
                enemyBanner.SetActive(true);
                playerBanner.SetActive(false);
                break;
            default:
                throw new System.Exception("Check UpdateText() in TurnManager, the switch statement is broken or is missing cases");
        }
    }

    #endregion
}