/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    9/28/2025
Date Last Modified : 	9/28/2025
Brief Description : 	Static Logger
                        Displays text in the CommandConsole and Unity Console                        
External Resources : 	N/A
***************************************************/
using System;
using TMPro;
using UnityEngine;

public class Logger
{
    private static TMP_Text consoleTextLog;
    public static string DebugColor;
    public static string WarningColor;
    public static string ErrorColor;
    public static TMP_Text ConsoleTextLog { get => consoleTextLog; set => consoleTextLog = value; }

    /// <summary>
    /// Initializes the display colors and location for the Logger
    /// </summary>
    /// <param name="consoleTextLog">The text box to output lines to</param>
    /// <param name="debugColor">What color standard messages appear. Default is white.</param>
    /// <param name="warningColor">What color warning messages appear. Default is yellow.</param>
    /// <param name="errorColor">What color error messages appear. Default is red.</param>
    public static void Initialize(TMP_Text consoleTextLog, string debugColor = "<color=white>", string warningColor = "<color=yellow>", string errorColor = "<color=red>")
    {
        ConsoleTextLog = consoleTextLog;
        DebugColor = debugColor;
        WarningColor = warningColor;
        ErrorColor = errorColor;
    }

    /// <summary>
    /// Logs a message in the Command Console and Unity Console
    /// </summary>
    /// <param name="text">The message to display</param>
    public static void Log(string text)
    {
        ConsoleTextLog.text += DebugColor + text + "</color>\n";
        Debug.Log(text);
    }

    /// <summary>
    /// Logs a warning in the Command Console and Unity Console
    /// </summary>
    /// <param name="text">The message to display</param>
    public static void Warning(string text)
    {
        ConsoleTextLog.text += WarningColor + text + "</color>\n";
        Debug.LogWarning(text);
    }

    /// <summary>
    /// Logs a message in the Command Console and Unity Console
    /// </summary>
    /// <param name="text">The message to display</param>
    public static void Error(string text)
    {
        ConsoleTextLog.text += ErrorColor + text + "</color>\n";
        Debug.LogError(text);
    }
}
