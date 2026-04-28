/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/1/2025
Date Last Modified : 	10/1/2025
Brief Description : 		The actual state machine for each enemy 
External Resources : 	https://www.youtube.com/watch?v=RQd44qSaqww
***************************************************/
using System.Collections;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    private EnemyState currentState;
    private float secondsBetweenStateTransitions;

    /// <summary>
    /// When you initialize the statemachine you tell it what state to start in and how long between state transitions 
    /// </summary>
    /// <param name="startingState"></param>
    /// <param name="secondsBetweenStateTransitions"></param>
    public void Initialized(EnemyState startingState, float secondsBetweenStateTransitions)
    {
        currentState = startingState;
        this.secondsBetweenStateTransitions = secondsBetweenStateTransitions;
        currentState.EnterState();
    }

    /// <summary>
    /// //Change the state by playing a states exit behaviour and the new states enter behavior 
    ///IEnumerator so this doesn't go at ligth speed 
    /// </summary>
    /// <param name="newState"></param>
    /// <returns></returns>
    public IEnumerator ChangeState(EnemyState newState)
    {
        currentState.ExitState();
        yield return new WaitForSecondsRealtime(secondsBetweenStateTransitions);
        currentState = newState;
        currentState.EnterState();
    }

    /// <summary>
    /// Change state function that uses chosen seconds rather than the enemy state tranistion time 
    /// Mainly so the Endstate doesn't need to wait to go to the wait state 
    /// </summary>
    /// <param name="newState"></param>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public IEnumerator ChangeState(EnemyState newState, float seconds)
    {
        currentState.ExitState();
        yield return new WaitForSecondsRealtime(seconds);
        currentState = newState;
        currentState.EnterState();
    }
}
