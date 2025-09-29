/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    9/28/2025
Date Last Modified : 	9/29/2025
Brief Description : 	Static Logger
                        Displays text in the CommandConsole and Unity Console                        
External Resources : 	https://docs.unity3d.com/ScriptReference/ILogHandler.html
***************************************************/
using System;
using TMPro;
using UnityEngine;

public class Logger : ILogHandler
{
    private static TMP_Text consoleTextLog;
    private static string DebugColor;
    private static string WarningColor;
    private static string ErrorColor;
    private static string InputColor;
    public static TMP_Text ConsoleTextLog { get => consoleTextLog; set => consoleTextLog = value; }
    public static ILogHandler Default { get; }

    /// <summary>
    /// Constructor for Logger
    /// Saves a reference to Unity's default logger
    /// </summary>
    static Logger()
    {
        Default = Debug.unityLogger.logHandler;
    }

    /// <summary>
    /// Initializes the display colors and location for the Logger
    /// </summary>
    /// <param name="consoleTextLog">The text box to output lines to</param>
    /// <param name="debugColor">What color standard messages appear. Default is white.</param>
    /// <param name="warningColor">What color warning messages appear. Default is yellow.</param>
    /// <param name="errorColor">What color error messages appear. Default is red.</param>
    public static void Initialize(TMP_Text consoleTextLog, string debugColor = "<color=white>", string warningColor = "<color=yellow>", 
        string errorColor = "<color=red>", string inputColor = "<color=gray>")
    {
        ConsoleTextLog = consoleTextLog;
        DebugColor = debugColor;
        WarningColor = warningColor;
        ErrorColor = errorColor;
        InputColor = inputColor;
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
    /// Logs a message in the Command Console and Unity Console
    /// Call when the User enters a command
    /// </summary>
    /// <param name="text">The command the User entered</param>
    public static void Input(string text)
    {
        ConsoleTextLog.text += InputColor + ">> " + text + "</color>\n";
        Debug.Log("User Entered " + text);
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

    /// <summary>
    /// Intercepts all log messages
    /// May throw some issues if you directly try to use Debug.Log from other methods
    /// </summary>
    /// <param name="logType">The type of log</param>
    /// <param name="context">What object the log applies to</param>
    /// <param name="format">Format string</param>
    /// <param name="args">Arguments for formatting</param>
    public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
        switch (logType)
        {
            case LogType.Log:
                Log(String.Format(format, args));
                break;
            case LogType.Error:
                Error(String.Format(format, args));
                break;
            case LogType.Warning:
                Warning(String.Format(format, args));
                break;
            case LogType.Exception:
                Error(String.Format(format, args));
                break;
            case LogType.Assert:
                Error(String.Format(format, args));
                break;
            default:
                Warning("LogFormat fell through cases");
                break;
        }
    }

    /// <summary>
    /// Intercepts Exception messages
    /// May throw some issues if you try to directly use Debug.Log from other scripts
    /// </summary>
    /// <param name="exception">Runtime exception</param>
    /// <param name="context">What object the exception applies to</param>
    public void LogException(Exception exception, UnityEngine.Object context)
    {
        Warning(exception.ToString());
    }
}
