/*************************************************
Author Names : 		Cade Naylor, Tyler Bouchard
Date Created : 		10/22/2025
Date Last Modified : 11/3/2025 (Tyler Bouchard)
Brief Description : Controls action points. This is a temporary script while all player scripts are checked out
External Resources : 	
	***************************************************/
using System;
using UnityEngine;

public class ActionPointManager : MonoBehaviour
{
    private GameManager gm;
    /// <summary>
    /// Start is called on the first frame update
    /// </summary>
    void Start()
    {
        gm = FindFirstObjectByType<GameManager>();
    }

    /// <summary>
    /// Subscribes to public public events
    /// </summary>
    private void OnEnable()
    {
        PublicEvents.PlayerMove += PlayerHasMoved;
        TurnPublicEvents.BeginPlayerTurn += AllocatePoints;
        PublicEvents.RuneCast += PlayerCastSpell;
    }

    /// <summary>
    /// unsubscribes from all public events
    /// </summary>
    private void OnDisable()
    {
        PublicEvents.PlayerMove -= PlayerHasMoved;
        TurnPublicEvents.BeginPlayerTurn -= AllocatePoints;
        PublicEvents.RuneCast -= PlayerCastSpell;
    }

    /// <summary>
    /// Removes points if the player has moved 
    /// </summary>
    private void PlayerHasMoved()
    {
        gm.UpdateActionPoints(gm.MoveActionPoints);
    }

    /// <summary>
    /// Sets the available points at the start of a turn
    /// </summary>
    private void AllocatePoints()
    {
        gm.ResetActionPoints();
    }

    /// <summary>
    /// Removes cost when the player casts a spell
    /// Ends the player turn if costs are low
    /// </summary>
    /// <param name="cost"></param>
    public void PlayerCastSpell(int cost)
    {
        Debug.Log("Cost of Spell is " + cost);
        gm.UpdateActionPoints(cost);
    }
}
