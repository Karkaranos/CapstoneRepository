/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    9/28/2025
Date Last Modified : 	9/28/2025
Brief Description : 	Static Commands
                        Calls the appropriate functions given inputted Commands                       
External Resources : 	N/A
***************************************************/
using System.Collections.Generic;
using UnityEngine;

public class Commands
{
    // Categories for commands. Used to control what commands are active at any given point
    public enum CommandGroup
    {
        MOVE_CONSOLE, GREET
    }

    // Links all valid commands with their appropriate category
    public static Dictionary<string, CommandGroup> CommandDictionary = new Dictionary<string, CommandGroup>()
    {
        {"1", CommandGroup.MOVE_CONSOLE},
        {"2", CommandGroup.MOVE_CONSOLE},
        {"3", CommandGroup.MOVE_CONSOLE},
        {"4", CommandGroup.MOVE_CONSOLE},
        {"hi", CommandGroup.GREET}
    };

    /// <summary>
    /// Greets the user
    /// </summary>
    public static void Greet()
    {
        Logger.Log("Hello User!");
    }

    /// <summary>
    /// Sets the Console location to one of four pre-set locations
    /// </summary>
    /// <param name="location"></param>
    public static void SetConsoleLocation(string location)
    {
        if (CommandConsoleBehavior.RectTransform == null)
        {
            Logger.Error("Console Rect Transform Cannot Be Found");
            return;
        }

        RectTransform rectTransform = CommandConsoleBehavior.RectTransform;
        Vector2 newValues = Vector2.zero;
        switch (location)
        {
            case "1":
                // Top Left
                newValues = new Vector2(0, 1);
                break;
            case "2":
                // Top Right
                newValues = new Vector2(1, 1);
                break;
            case "3":
                // Bottom Right
                newValues = new Vector2(1, 0);
                break;
            case "4":
                // Bottom Left
                // This would be Vector2.zero. As newValues is already initialized to that, 
                // it would be redundant to set it again. Case exists to ensure proper handling. 
                break;
            default:
                Logger.Warning("Invalid Location Set. Fell through switch.");
                return;
        }

        // Sets the pivot, anchor points, and new position
        rectTransform.pivot = newValues;
        rectTransform.anchorMin = newValues;
        rectTransform.anchorMax = newValues;
        rectTransform.anchoredPosition3D = Vector3.zero;
    }

}
