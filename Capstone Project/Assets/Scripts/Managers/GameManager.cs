using NaughtyAttributes;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //inspector enums
    public enum Settings
    {
        None, Prefabs, ArtifactManager, ConsoleCommands
    }

    public Settings settings;
    [HideInInspector] public static ArtifactManager ArtifactManager;
    [HideInInspector] public static GameObject CommandConsoleRef;
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
    void Awake()
    {
        // Console Commands need to be before pretty much everything else
        CommandConsoleRef = Instantiate(CommandConsole, transform.position, Quaternion.identity);
        CommandConsoleRef.GetComponent<CommandConsoleBehavior>().Initialize(moveConsoleEnabled, greetEnabled, enemiesEnabled);

        ArtifactManager = new ArtifactManager(randomArtifactPool, setArtifactPool, maxArtifacts, allowArtifactTesting, testData);

    }
}
