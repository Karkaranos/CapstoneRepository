/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    9/26/2025
Date Last Modified : 	9/28/2025
Brief Description : 	Handles behavior for the Command Console
                        - Reads Value
                        - Calls appropriate static functions
                        - Controls what commands are available in the current scene
External Resources : 	N/A
***************************************************/

using TMPro;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;

public class CommandConsoleBehavior : MonoBehaviour
{
    private enum CommandConsoleInspectorOptions
    {
        STATIC_VALUES, VALID_COMMANDS
    }

    [SerializeField] private CommandConsoleInspectorOptions _options;

    [Foldout("Logger Static Values"), SerializeField, ShowIf("_options", CommandConsoleInspectorOptions.STATIC_VALUES)] private TMP_Text _consoleTextbox;
    [Foldout("Logger Static Values"), SerializeField, ShowIf("_options", CommandConsoleInspectorOptions.STATIC_VALUES), 
        Tooltip("CSS Tag to set Log Message Display Color")] private string _logColor;
    [Foldout("Logger Static Values"), SerializeField, ShowIf("_options", CommandConsoleInspectorOptions.STATIC_VALUES), 
        Tooltip("CSS Tag to set Log Warning Display Color")] private string _warningColor;
    [Foldout("Logger Static Values"), SerializeField, ShowIf("_options", CommandConsoleInspectorOptions.STATIC_VALUES), 
        Tooltip("CSS Tag to set Log Error Display Color")] private string _errorColor;

    [Foldout("CommandGroups"), SerializeField, ShowIf("_options", CommandConsoleInspectorOptions.VALID_COMMANDS),
        Tooltip("Can the Command Console be moved?")] private bool _moveConsoleEnabled;
    [Foldout("CommandGroups"), SerializeField, ShowIf("_options", CommandConsoleInspectorOptions.VALID_COMMANDS),
    Tooltip("Can the Command Console greet the user?")] private bool _greetEnabled;

    [Foldout("References"), Required, SerializeField] private TMP_Text _consoleInputBox;
    [Foldout("References"), Required, SerializeField] private RectTransform _consoleRectTransform;
    [HideInInspector] public static RectTransform RectTransform;

    /// <summary>
    /// Occurs on the first frame update. Initializes Logger static class
    /// </summary>
    void Start()
    {
        Logger.Initialize(_consoleTextbox, _logColor, _warningColor, _errorColor);
        RectTransform = _consoleRectTransform;
        Logger.Log("Testing");
        Logger.Warning("Test Warning");
        Logger.Error("Test Error");
    }

    /// <summary>
    /// Called when enter is pressed in the input box
    /// Reads input then carries out the appropriate command if the command group is enabled
    /// </summary>
    /// <param name="command">The command entered into the input field</param>
    public void EnterCommand(string command)
    {
        Logger.Log(">>" + command);
        if (Commands.CommandDictionary.ContainsKey(command.ToLower()))
        {
            switch (Commands.CommandDictionary[command.ToLower()])
            {
                case Commands.CommandGroup.MOVE_CONSOLE:
                    if (_moveConsoleEnabled)
                    {
                        Commands.SetConsoleLocation(command);
                    }
                    break;
                case Commands.CommandGroup.GREET:
                    if (_greetEnabled)
                    {
                        Commands.Greet();
                    }
                    break;
                default:
                    Logger.Warning("Command Group Not Implemented");
                    break;
            }
        }
        else
        {
            Logger.Error("Invalid Command Entered");
        }

        ClearCommand();
        
    }

    /// <summary>
    /// Called when the input box is deselected and from other functions
    /// Clears the input box
    /// </summary>
    /// <param name="s">Whatever is in the text box. Has a default value of empty.</param>
    public void ClearCommand(string command = "")
    {
        _consoleInputBox.text = string.Empty;
    }
}
