/*************************************************
Author Names : 		Cade Naylor
Date Created : 		10/22/2025
Date Last Modified : 10/22/2025
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
        gm.CurrentActionPoints -= gm.MoveActionPoints;
        if (gm.CurrentActionPoints <= 0)
        {
            TurnPublicEvents.ForceEndCurrentPhase();
        }
    }

    /// <summary>
    /// Sets the available points at the start of a turn
    /// </summary>
    private void AllocatePoints()
    {
        gm.CurrentActionPoints = gm.ActionPointsPerTurn;
    }

    /// <summary>
    /// Removes cost when the player casts a spell
    /// Ends the player turn if costs are low
    /// </summary>
    /// <param name="cost"></param>
    public void PlayerCastSpell(int cost)
    {
        gm.CurrentActionPoints -= cost;
        if (gm.CurrentActionPoints <= 0)
        {
            TurnPublicEvents.ForceEndCurrentPhase();
        }
    }

}
