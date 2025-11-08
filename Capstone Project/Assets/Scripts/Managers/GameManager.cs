/*************************************************
Author Names : 		Cade Naylor, Tyler Bouchard
Date Created : 		???/2025
Date Last Modified : 11/7/2025 (Clare Grady)
Brief Description : Game Manager
                    Creates and holds static references to other managers
External Resources : 	
	***************************************************/
using NaughtyAttributes;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    //inspector enums
    public enum Settings
    {
        None, Prefabs, ArtifactManager, ConsoleCommands, MarkManager
    }

    public Settings settings;
    [HideInInspector] public ArtifactManager ArtifactManager;
    [HideInInspector] public static GameObject CommandConsoleRef;
    public static PlayerStats PlayerStats;
    public static MarkManager MarkManager;
    public bool allowArtifacts = false;

    #region Prefabs
    [HorizontalLine(4, EColor.Red)]
    [Required, Tooltip("The Command Console Prefab"), ShowIf(nameof(settings), Settings.Prefabs), SerializeField]
        private GameObject CommandConsole;
    #endregion

    #region ArtifactManager
    [HorizontalLine(4, EColor.Orange)]
    [SerializeField, ShowIf(nameof(settings), Settings.ArtifactManager)] private int maxArtifacts;
    [SerializeField, ShowIf(nameof(settings), Settings.ArtifactManager)] private bool allowArtifactTesting;
    [SerializeField, Tooltip("Enemies may drop one of these at random upon death"),
    ShowIf(nameof(settings), Settings.ArtifactManager)]
    private ArtifactData[] randomArtifactPool;
    [SerializeField, Tooltip("Obtained upon level completion. Index + 1 is the level number."),
        ShowIf(nameof(settings), Settings.ArtifactManager)] private ArtifactData[] setArtifactPool;
    [SerializeField, Tooltip("Artifact Testing. Will be removed later"),
       ShowIf(EConditionOperator.And, nameof(allowArtifactTesting), nameof(TestForArtifactState))]
    private ArtifactData[] testData;
    private bool TestForArtifactState => TestSettingValue(Settings.ArtifactManager);

    #endregion

    #region Console Commands
    [HorizontalLine(4, EColor.Yellow)]
    [SerializeField, ShowIf(nameof(settings), Settings.ConsoleCommands), 
    Tooltip("Can you use the console?")] private bool consoleEnabled = true;
    [SerializeField, ShowIf(EConditionOperator.And, nameof(consoleEnabled), nameof(TestForConsoleState)),
    Tooltip("Does the console start enabled?")] private bool consoleEnabledOnLoad = true;
    [SerializeField, ShowIf(nameof(settings), Settings.ConsoleCommands),
    Tooltip("Can the Command Console be moved?")]
    private bool moveConsoleEnabled;
    [SerializeField, ShowIf(nameof(settings), Settings.ConsoleCommands),
    Tooltip("Can the Command Console greet the user?")]
    private bool greetEnabled;
    [SerializeField, ShowIf(nameof(settings), Settings.ConsoleCommands),
    Tooltip("Can the Command Console affect Enemies?")]
    private bool enemiesEnabled;
    private bool TestForConsoleState => TestSettingValue(Settings.ConsoleCommands);
    #endregion

    #region MarkManager
    [HorizontalLine(4, EColor.Green)]
    [SerializeField, ShowIf(nameof(settings), Settings.MarkManager),
    Tooltip("All Currently Enabled Marks")]
    private List<MarkData> validMarks = new List<MarkData>();
    #endregion

    // Should be relocated to PlayerBehavior
    #region ActionPoints
    [SerializeField] public TMP_Text ActionPointVisualizer;
    public int CurrentActionPoints;
    public int MoveActionPoints = 2;
    public int ActionPointsPerTurn = 3;
    #endregion

    /// <summary>
    /// updated the action points, right now its called from ActionPointManager
    /// </summary>
    /// <param name="amount"></param>
    public void UpdateActionPoints(int amount) {
        CurrentActionPoints -= amount;
        //ActionPointVisualizer.text = "Action Points: " + CurrentActionPoints;
        print("called");
        if (CurrentActionPoints <= 0)
        {
            TurnPublicEvents.ForceEndCurrentPhase();
        }
    }

    /// <summary>
    /// sets the current action points baclk to the max
    /// </summary>
    public void ResetActionPoints() {
        CurrentActionPoints = ActionPointsPerTurn;
        //ActionPointVisualizer.text = "Action Points: " + CurrentActionPoints;
    }

    /// <summary>
    /// Inspector function
    /// Checks if the given enum state is active
    /// Used for multiconditional show if
    /// </summary>
    /// <returns></returns>
    private bool TestSettingValue(Settings val)
    {
        return settings == val;
    }
    
    /// <summary>
    /// Called on the first frame update
    /// Creates static references
    /// </summary>
    void Start()
    {
        // Console Commands need to be before pretty much everything else
        CommandConsoleRef = Instantiate(CommandConsole, transform.position, Quaternion.identity);
        CommandConsoleRef.GetComponent<CommandConsoleBehavior>().Initialize(moveConsoleEnabled, greetEnabled, enemiesEnabled, consoleEnabled, consoleEnabledOnLoad);

        PlayerStats = GetComponent<PlayerStats>();

        MarkManager = new MarkManager(validMarks, this);

        ArtifactManager = new ArtifactManager(randomArtifactPool, setArtifactPool, maxArtifacts, this, allowArtifactTesting, testData);

        ResetActionPoints();
    }
}
