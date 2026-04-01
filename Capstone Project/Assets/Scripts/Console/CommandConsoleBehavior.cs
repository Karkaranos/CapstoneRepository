/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    9/26/2025
Date Last Modified : 	3/31/2026
Brief Description : 	Handles behavior for the Command Console
                        - Reads Value
                        - Calls appropriate static functions
                        - Controls what commands are available in the current scene
External Resources : 	https://www.programiz.com/csharp-programming/regex
***************************************************/

using TMPro;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Text.RegularExpressions;
using System.Collections;
using System;

public class CommandConsoleBehavior : MonoBehaviour
{
    // Controls what elements are visible in the inspector
    private enum InspectorOption
    {
        StaticValues, ValidCommands
    }

    [SerializeField] private InspectorOption options;
    [SerializeField] private bool consoleEnabled = true;
    [SerializeField] private bool consoleEnabledOnLoad = true;

    #region Logger Static Values
    [HorizontalLine(4, EColor.Red)]
    [SerializeField, ShowIf("options", InspectorOption.StaticValues)] private TMP_Text consoleTextbox;
    [Foldout("Log Colors"), SerializeField, ShowIf("options", InspectorOption.StaticValues), 
        Tooltip("CSS Tag to set Log Message Display Color")] private string logColor;
    [Foldout("Log Colors"), SerializeField, ShowIf("options", InspectorOption.StaticValues), 
        Tooltip("CSS Tag to set Log Warning Display Color")] private string warningColor;
    [Foldout("Log Colors"), SerializeField, ShowIf("options", InspectorOption.StaticValues), 
        Tooltip("CSS Tag to set Log Error Display Color")] private string errorColor;
    [Foldout("Log Colors"), SerializeField, ShowIf("options", InspectorOption.StaticValues),
    Tooltip("CSS Tag to set Log Input Display Color")]
    private string inputColor;
    [Foldout("Log Colors"), SerializeField, ShowIf("options", InspectorOption.StaticValues),
    Tooltip("CSS Tag to set Information Display Color")]
    private string infoColor;
    #endregion

    #region Command Groups
    [HorizontalLine(4, EColor.Orange)]
    [SerializeField, ShowIf("options", InspectorOption.ValidCommands),
        Tooltip("Can the Command Console be moved?")] private bool moveConsoleEnabled;
    [SerializeField, ShowIf("options", InspectorOption.ValidCommands),
    Tooltip("Can the Command Console greet the user?")] private bool greetEnabled;
    [SerializeField, ShowIf("options", InspectorOption.ValidCommands),
    Tooltip("Can the Command Console affect Enemies?")]
    private bool enemiesEnabled = true;
    private bool playerEnabled = true;
    private bool navigationEnabled = true;
    #endregion

    [Foldout("References"), Required, SerializeField] private TMP_Text consoleInputBox;
    [Foldout("References"), Required, SerializeField] private TMP_InputField consoleInputField;
    [Foldout("References"), Required, SerializeField] private RectTransform consoleRectTransform;
    
    private GameObject consoleGameObject;
    private static RectTransform rectTransform;

    private InputActionMap actionMap;
    private InputAction toggleConsole;

    public static RectTransform RectTransform { get => rectTransform; set => rectTransform = value; }

    /// <summary>
    /// Occurs on the first frame update. Initializes Logger static class
    /// </summary>
    public void Initialize(bool moveConsole = true, bool greet = true, bool enemy = true, bool player = true, bool nav = true, bool enabled = false, bool enabledAtStart = false)
    {
        Commands.AvailableCommandTypes.Clear();

        Logger.Initialize(consoleTextbox, logColor, warningColor, errorColor, inputColor, infoColor);
        rectTransform = consoleRectTransform;

        PublicEvents.ToggleConsole += ToggleConsole;

        consoleGameObject = consoleRectTransform.gameObject;
        consoleGameObject.SetActive(enabledAtStart);
        consoleEnabled = enabled;
        consoleEnabledOnLoad = enabledAtStart;

        moveConsoleEnabled = moveConsole;
        greetEnabled = greet;
        enemiesEnabled = enemy;
        playerEnabled = player;
        navigationEnabled = nav;

        if(moveConsole)
        {
            Commands.AvailableCommandTypes.Add(Commands.CommandGroup.MoveConsole);
        }
        if (greetEnabled)
        {
            Commands.AvailableCommandTypes.Add(Commands.CommandGroup.Greet);
        }
        if (enemiesEnabled)
        {
            Commands.AvailableCommandTypes.Add(Commands.CommandGroup.Enemies);
        }
        if (playerEnabled)
        {
            Commands.AvailableCommandTypes.Add(Commands.CommandGroup.Player);
        }
        if (navigationEnabled)
        {
            Commands.AvailableCommandTypes.Add(Commands.CommandGroup.Navigation);
        }

        Commands.AvailableCommandTypes.Add(Commands.CommandGroup.None);
    }

    /// <summary>
    /// unsubscribes from events
    /// </summary>
    private void OnDestroy()
    {
        PublicEvents.ToggleConsole -= ToggleConsole;
    }

    /// <summary>
    /// Called when enter is pressed in the input box
    /// Reads input then carries out the appropriate command if the command group is enabled
    /// </summary>
    /// <param name="command">The command entered into the input field</param>
    public void EnterCommand(string command)
    {
        Logger.Input(command);
        command = command.ToLower();
        if (Commands.CommandDictionary.ContainsKey(command))
        {
            switch (Commands.CommandDictionary[command])
            {
                case Commands.CommandGroup.MoveConsole:
                    if (moveConsoleEnabled)
                    {
                        Commands.SetConsoleLocation(command);
                    }
                    break;
                case Commands.CommandGroup.Greet:
                    if (greetEnabled)
                    {
                        Commands.Greet();
                    }
                    break;
                case Commands.CommandGroup.Enemies:
                    {
                        if (enemiesEnabled)
                        {
                            Commands.Enemies(command);
                        }
                        break;
                    }
                case Commands.CommandGroup.Player:
                    {
                        if(playerEnabled)
                        {
                            Commands.Player(command, FindFirstObjectByType<PlayerStats>());
                        }
                        break;
                    }
                case Commands.CommandGroup.Navigation:
                    {
                        if(navigationEnabled)
                        {
                            Pip[] pips = FindObjectsByType<Pip>(FindObjectsSortMode.None);
                            foreach (Pip pip in pips)
                            {
                                pip.RemovePip();
                            }

                            
                            foreach(TileBehaviour d in PipManager.Instance.hazardTiles)
                            {
                                d.RemoveHazard();
                            }

                            Commands.Navigation(command, FindFirstObjectByType<EndLevelMenu>(FindObjectsInactive.Include));
                        }
                        break;
                    }
                case Commands.CommandGroup.None:
                    Commands.AlwaysAvailable(command, FindFirstObjectByType<CameraManager>());
                    break;
                default:
                    Logger.Warning("Command Group Not Implemented");
                    break;
            }
        }
        else
        {
            // Hardcoded cases with variables for now; will look into
            // how to do this more efficiently
            if(command.Contains('-'))
            {
                int lastIndex = command.LastIndexOf('-')+1;
                if(Commands.PartialCommands1.ContainsKey(command.Substring(0,lastIndex)))
                {
                    switch (Commands.PartialCommands1[command.Substring(0,lastIndex)])
                    {
                        case Commands.CommandGroup.MoveConsole:
                            if (moveConsoleEnabled)
                            {
                                Commands.SetConsoleLocation(command);
                            }
                            break;
                        case Commands.CommandGroup.Greet:
                            if (greetEnabled)
                            {
                                Commands.Greet();
                            }
                            break;
                        case Commands.CommandGroup.Enemies:
                            {
                                if (enemiesEnabled)
                                {
                                    Commands.Enemies(command);
                                }
                                break;
                            }
                        case Commands.CommandGroup.Player:
                            {
                                if(playerEnabled)
                                {
                                    Commands.Player(command, FindFirstObjectByType<PlayerStats>());
                                }
                                break;
                            }
                        case Commands.CommandGroup.Navigation:
                            {
                                if(navigationEnabled)
                                {
                                    Pip[] pips = FindObjectsByType<Pip>(FindObjectsSortMode.None);
                                    foreach (Pip pip in pips)
                                    {
                                        pip.RemovePip();
                                    }
                                    foreach (TileBehaviour d in PipManager.Instance.hazardTiles)
                                    {
                                        d.RemoveHazard();
                                    }
                                    Commands.Navigation(command, FindFirstObjectByType<EndLevelMenu>(FindObjectsInactive.Include));
                                }
                                break;
                            }
                        case Commands.CommandGroup.None:
                            Commands.AlwaysAvailable(command, FindFirstObjectByType<CameraManager>());
                            break;
                        default:
                            Logger.Warning("Command Group Not Implemented");
                            break;
                    }
                }
                else
                {
                    bool matchFound = false;
                    foreach(string key in Commands.PartialCommands2.Keys)
                    {
                        // pattern matching key
                        Regex r = new Regex(key);
                        if(!matchFound && r.IsMatch(command))
                        {
                            switch (Commands.PartialCommands2[key])
                            {
                                case Commands.CommandGroup.MoveConsole:
                                    if (moveConsoleEnabled)
                                    {
                                        Commands.SetConsoleLocation(command);
                                    }
                                    break;
                                case Commands.CommandGroup.Greet:
                                    if (greetEnabled)
                                    {
                                        Commands.Greet();
                                    }
                                    break;
                                case Commands.CommandGroup.Enemies:
                                    {
                                        if (enemiesEnabled)
                                        {
                                            Commands.Enemies(command);
                                        }
                                        break;
                                    }
                                case Commands.CommandGroup.Player:
                                    {
                                        if(playerEnabled)
                                        {
                                            Commands.Player(command, FindFirstObjectByType<PlayerStats>());
                                        }
                                        break;
                                    }
                                case Commands.CommandGroup.Navigation:
                                    {
                                        if(navigationEnabled)
                                        {
                                            Pip[] pips = FindObjectsByType<Pip>(FindObjectsSortMode.None);
                                            foreach (Pip pip in pips)
                                            {
                                                pip.RemovePip();
                                            }
                                            foreach (TileBehaviour d in PipManager.Instance.hazardTiles)
                                            {
                                                d.RemoveHazard();
                                            }
                                            Commands.Navigation(command, FindFirstObjectByType<EndLevelMenu>(FindObjectsInactive.Include));
                                        }
                                        break;
                                    }
                                case Commands.CommandGroup.None:
                                    Commands.AlwaysAvailable(command, FindFirstObjectByType<CameraManager>());
                                    break;
                                default:
                                    Logger.Warning("Command Group Not Implemented", false);
                                    break;
                            }
                            matchFound = true;
                            break;
                        }
                    }
                    if(!matchFound)
                        Logger.Error("Could not find partial key by regex", false);
                }
            }
            else
            {   
                Logger.Error("Could not find key" + command, false);
            }
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
        consoleInputField.text = "";
        consoleInputBox.text = "";
    }

    /// <summary>
    /// Controls whether the console is enabled or not
    /// </summary>
    public void ToggleConsole()
    {
        if (consoleEnabled)
        {
            consoleGameObject.SetActive(!consoleGameObject.activeInHierarchy);
            if (consoleGameObject.activeInHierarchy)
            {
                Logger.Info("Type 'menu' for a list of all commands");
            }
        }
    }
}
