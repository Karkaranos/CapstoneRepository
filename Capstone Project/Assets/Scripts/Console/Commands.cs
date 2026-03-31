/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    9/28/2025
Date Last Modified : 	2/12/2026
Brief Description : 	Static Commands
                        Calls the appropriate functions given inputted Commands                       
External Resources : 	N/A
***************************************************/
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;
using System.Security.Cryptography;
using System.Linq;

public class Commands
{
    // Categories for commands. Used to control what commands are active at any given point
    public enum CommandGroup
    {
        MoveConsole, Greet, Enemies, Player, Navigation, None
    }

    // Links all commands with no variables to their command group
    public static Dictionary<string, CommandGroup> CommandDictionary = new Dictionary<string, CommandGroup>()
    {
        {"1", CommandGroup.MoveConsole},
        {"2", CommandGroup.MoveConsole},
        {"3", CommandGroup.MoveConsole},
        {"4", CommandGroup.MoveConsole},
        {"hi", CommandGroup.Greet},
        {"kill-enemies", CommandGroup.Enemies },
        {"enemies-godmode", CommandGroup.Enemies},
        //{"drop", CommandGroup.Enemies },
        {"kill-enemy-#", CommandGroup.Enemies},
        {"enemies-health-#", CommandGroup.Enemies},
        {"enemy-#-health-$", CommandGroup.Enemies},
        //{"no-cost", CommandGroup.Player},
        //{"max-xp", CommandGroup.Player},
        //{"unlock-all-spells", CommandGroup.Player},
        {"godmode", CommandGroup.Player},
        {"hp-#", CommandGroup.Player},
         {"light-dmg-#", CommandGroup.Player},
        {"wind-dmg-#", CommandGroup.Player},
        {"lvl-#", CommandGroup.Navigation},
        {"r", CommandGroup.Navigation},
        {"menu", CommandGroup.None },
        {"help", CommandGroup.None},
        {"skipcut", CommandGroup.None },

    };

    public static List<CommandGroup> AvailableCommandTypes = new List<CommandGroup>();

    // links all commands with 1 variable to their command group
    public static Dictionary<string, CommandGroup> PartialCommands1= new Dictionary<string, CommandGroup>()
    {
        {"kill-enemy-", CommandGroup.Enemies},
        {"enemies-health-", CommandGroup.Enemies},
        {"hp-", CommandGroup.Player},
        {"light-dmg-", CommandGroup.Player},
        {"wind-dmg-", CommandGroup.Player},
        {"lvl-", CommandGroup.Navigation}
    };

    // links all commands with 2 variables to their command group
    public static Dictionary<string, CommandGroup> PartialCommands2 = new Dictionary<string, CommandGroup>()
    {
        {"enemy-.*-health-.*", CommandGroup.Enemies}
    };

#region Command Groups
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

    /// <summary>
    /// Greets the user
    /// </summary>
    public static void Greet()
    {
        Logger.Log("Char says hi");
    }

    /// <summary>
    /// Applies a command to all enemies in the scene
    /// Currently can kill them or make them force drop an item
    /// </summary>
    /// <param name="command"></param>
    public static void Enemies(string command)
    {
        Enemy[] AllEnemies = GameObject.FindObjectsByType<Enemy>(findObjectsInactive:FindObjectsInactive.Exclude, sortMode:FindObjectsSortMode.None);
        
        // Commands that affect all enemies
        if(command.Contains("enemies") || command.Contains("drop"))
        {
            foreach(Enemy e in AllEnemies)
            {
                switch(command)
                {
                    case "kill-enemies":
                        e.Damage(9999999);
                        break;
                    case "enemies-godmode":
                        e.ToggleInvincibility();
                        Logger.Log("Enemy invincibility has been toggled");
                        break;
                    // Cases that use variables
                    default:
                        if(command.Contains("enemies-health-"))
                        {
                            float newVal = ConvertToNumber(command.Substring(15, command.Length-15));
                            e.SetHealth(newVal);
                        }
                        else
                        {
                            Logger.Warning("Invalid Enemy tag. Fell through Switch.");
                            return;
                        }
                        break;
                }
            }
        }
        // Commands that affect a single enemy
        else
        {
            if(command.Contains("kill-enemy-"))
            {
                int index = (int)ConvertToNumber(command.Substring(11, command.Length-11))-1;
                if(index >= AllEnemies.Length || index < 0)
                {
                    Logger.Warning("Enemy out of range");
                    return;
                }
                AllEnemies[index].Damage(99999999);
            }
            // I don't feel like using regex again lol
            else if (command.Contains("enemy-") && command.Contains("-health-"))
            {
                int[] allIndexes = GetAllIndexes(command, '-');
                int index = (int)ConvertToNumber(command.Substring(allIndexes[0]+1, allIndexes[1]-allIndexes[0]-1))-1;
                if(index >= AllEnemies.Length || index < 0)
                {
                    Logger.Warning("Enemy out of range");
                    return;
                }
                float health = ConvertToNumber(command.Substring(allIndexes[2]+1, command.Length-allIndexes[2]-1));
                AllEnemies[index].SetHealth(health);
            }

        }
    }

    
    /// <summary>
    /// Handles commands that are always available to the player
    /// </summary>
    /// <param name="command">The user-entered commnd</param>
    public static void AlwaysAvailable(string command, CameraManager cs = null)
    {
        switch(command)
        {
            // Displays all commands
            case "menu":
                string sb = "Available Commands: \n";
                foreach(string key in CommandDictionary.Keys)
                {
                    if(AvailableCommandTypes.Contains(CommandDictionary[key]))
                    {
                        sb += " - " + key + "\n";
                    }
                }
                Logger.Info(sb);
                break;
            case "skipcut":
                cs.SkipCutscene();
                break;
            case "help":
                Logger.Info("Commands you type here have an effect on the game\nTo see all commands, type 'menu'\n" +
                    "Any '#' or '$' should be replaced by numbers\nThe system will convert it to the correct type");
                break;
            default:
                Logger.Warning("Fell through Switch.");
                return;
        }
    }

    /// <summary>
    /// Will be implemented later
    /// Handles Player commands
    /// </summary>
    /// <param name="command">the entered command</param>
    public static void Player(string command, PlayerStats p)
    {
        if(command == "godmode")
        {
            p.TakesDamage = !p.TakesDamage;
            Logger.Log("Player invincibility toggled");
        }
        else if (command.Contains("hp"))
        {
            int val = (int)ConvertToNumber(command.Substring(3, command.Length - 3));
            Logger.Log("Player health set to " + val);
            p.CurrentHealth = val;
        }
        else if (command.Contains("dmg"))
        {
            if(command.Contains("light"))
            {
                float val = ConvertToNumber(command.Substring(10, command.Length - 10));
                Logger.Log("Light Damage Multiplier set to " + val);
                p.LightningAttackMultiplier = val;
            }
            else if (command.Contains("wind"))
            {
                float val = ConvertToNumber(command.Substring(9, command.Length - 9));
                Logger.Log("Light Damage Multiplier set to " + val);
                p.WindAttackMultiplier = val;
            }
        }
    }

    /// <summary>
    /// Will be implemented later
    /// Handles Navigation commands
    /// </summary>
    /// <param name="command">the entered command</param>
    public static void Navigation(string command, EndLevelMenu e)
    {
        if(command.Contains("lvl"))
        {
            int val = (int)ConvertToNumber(command.Substring(4, command.Length - 4));
            Logger.Log("Loading level " + val);
            e.LoadSpecificLevel(val);
        }
        else if (command == "r")
        {
            Logger.Log("Reloading level");
            e.RestartLevel();
        }

    }

    #endregion

#region  Helper Functions
    /// <summary>
    /// Converts a string into a floating point number
    /// Is there an existing function to do this? probably
    /// I am in pain and cannot concentrate on searching
    /// </summary>
    /// <param name="s">String to convert</param>
    /// <returns>Floating point representation</returns>
    private static float ConvertToNumber(string s)
    {
        float result = 0;
        bool wholeNum = true;
        float multiplier = .1f;

        for(int i=0; i<s.Length; i++)
        {
            // Handle decimal points
            if(s[i] == '.')
            {
                wholeNum = false;
            }
            // Handle integer values
            else if(wholeNum)
            {
                result*=10;
                result += CharToInt(s[i]);
            }
            // handle floating point values
            else if (!wholeNum)
            {
                result += CharToInt(s[i])*multiplier;
                multiplier /= 10;
            }
        }
        return result;
    }

    /// <summary>
    /// Converts a character into a decimal amount, not ASCII
    /// Is there an existing function for this? probably
    /// I am in pain and cannot focus on searching online
    /// </summary>
    /// <param name="c">Character to convert</param>
    /// <returns>Integer representation</returns>
    private static int CharToInt(char c)
    {
        switch(c)
                {
                    case '1':
                        return 1;
                    case '2':
                        return 2;
                    case '3':
                        return 3;
                    case '4':
                        return 4;
                    case '5':
                        return 5;
                    case '6':
                        return 6;
                    case '7':
                        return 7;
                    case '8':
                        return 8;
                    case '9':
                        return 9;
                    default:
                        return 0;
                }
    }

    /// <summary>
    /// Gets all indexes of a specified character in a string
    /// </summary>
    /// <param name="s">String to search through</param>
    /// <param name="c">Character to find</param>
    /// <returns>All known indexes as an int array </returns>
    private static int[] GetAllIndexes(string s, char c)
    {
        int[] temp = new int[s.Length];
        int numHits = 0;
        for(int i=0; i<s.Length-1; i++)
        {
            if(s[i] == c)
            {
                temp[numHits] = i;
                numHits ++;
            }
        }
        

        int[] result = new int[numHits];
        Array.Copy(temp, result, numHits);
        return result;
    }

    #endregion

} 