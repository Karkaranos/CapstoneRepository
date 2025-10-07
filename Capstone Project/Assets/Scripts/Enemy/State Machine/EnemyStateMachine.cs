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
        Debug.Log("Changing State...");
        currentState = newState;
        currentState.EnterState();
    }
}
