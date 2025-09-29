/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    9/26/2025
Date Last Modified : 	9/29/2025
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
    // Controls what elements are visible in the inspector
    private enum InspectorOption
    {
        STATIC_VALUES, VALID_COMMANDS
    }

    [SerializeField] private InspectorOption _options;
    [SerializeField] private bool _consoleEnabled = true;
    [SerializeField] private bool _consoleEnabledOnLoad = true;

    #region Logger Static Values
    [HorizontalLine(4, EColor.Red)]
    [SerializeField, ShowIf("_options", InspectorOption.STATIC_VALUES)] private TMP_Text _consoleTextbox;
    [Foldout("Log Colors"), SerializeField, ShowIf("_options", InspectorOption.STATIC_VALUES), 
        Tooltip("CSS Tag to set Log Message Display Color")] private string _logColor;
    [Foldout("Log Colors"), SerializeField, ShowIf("_options", InspectorOption.STATIC_VALUES), 
        Tooltip("CSS Tag to set Log Warning Display Color")] private string _warningColor;
    [Foldout("Log Colors"), SerializeField, ShowIf("_options", InspectorOption.STATIC_VALUES), 
        Tooltip("CSS Tag to set Log Error Display Color")] private string _errorColor;
    [Foldout("Log Colors"), SerializeField, ShowIf("_options", InspectorOption.STATIC_VALUES),
    Tooltip("CSS Tag to set Log Input Display Color")]
    private string _inputColor;
    #endregion

    #region Command Groups
    [HorizontalLine(4, EColor.Orange)]
    [SerializeField, ShowIf("_options", InspectorOption.VALID_COMMANDS),
        Tooltip("Can the Command Console be moved?")] private bool _moveConsoleEnabled;
    [SerializeField, ShowIf("_options", InspectorOption.VALID_COMMANDS),
    Tooltip("Can the Command Console greet the user?")] private bool _greetEnabled;
    #endregion

    [Foldout("References"), Required, SerializeField] private TMP_Text _consoleInputBox;
    [Foldout("References"), Required, SerializeField] private TMP_InputField _consoleInputField;
    [Foldout("References"), Required, SerializeField] private RectTransform _consoleRectTransform;
    private GameObject _consoleGameObject;

    // must be public as it is static
    [HideInInspector] public static RectTransform RectTransform;

    /// <summary>
    /// Occurs on the first frame update. Initializes Logger static class
    /// </summary>
    void Start()
    {
        Logger.Initialize(_consoleTextbox, _logColor, _warningColor, _errorColor, _inputColor);
        RectTransform = _consoleRectTransform;
        Logger.Log("Testing");
        Logger.Warning("Test Warning");
        Logger.Error("Test Error");

        _consoleGameObject = _consoleRectTransform.gameObject;
        ToggleConsole(_consoleEnabledOnLoad);
    }

    /// <summary>
    /// Called when enter is pressed in the input box
    /// Reads input then carries out the appropriate command if the command group is enabled
    /// </summary>
    /// <param name="command">The command entered into the input field</param>
    public void EnterCommand(string command)
    {
        Logger.Input(command);
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
        _consoleInputField.text = "";
        _consoleInputBox.text = "";
    }

    public void ToggleConsole(bool toggle)
    {
        _consoleGameObject.SetActive(toggle);
    }
}
