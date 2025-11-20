using System;
using UnityEngine;

public static class TurnPublicEvents
{
    public static Action BeginStartTurn;

    public static Action BeginPlayerTurn;

    public static Action BeginEnemyTurn;

    public static Action BeginEndTurn;

    public static Action ForceEndCurrentPhase;

    public static Action TurnActionComplete;
}
