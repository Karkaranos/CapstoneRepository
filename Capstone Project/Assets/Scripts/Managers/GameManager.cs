using NaughtyAttributes;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //inspector enums
    public enum Settings
    {
        None, Prefabs, ArtifactManager
    }

    public Settings settings;
    [HideInInspector] public static ArtifactManager ArtifactManager;
    [HideInInspector] public static GameObject CommandConsoleRef;

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
       ShowIf(EConditionOperator.And, nameof(allowArtifactTesting), nameof(ArtifactMenu))]
    private ArtifactData[] testData;
    private bool ArtifactMenu => TestSettingValue();

    #endregion

    /// <summary>
    /// Inspector function
    /// Converts a specific enum state to true
    /// </summary>
    /// <returns></returns>
    private bool TestSettingValue()
    {
        return settings == Settings.ArtifactManager;
    }
    
    /// <summary>
    /// Called on the first frame update
    /// Creates static references
    /// </summary>
    void Awake()
    {
        //Assign a reference to ArtifactManager
        CommandConsoleRef = Instantiate(CommandConsole, transform.position, Quaternion.identity);
        CommandConsoleRef.GetComponent<CommandConsoleBehavior>().Initialize();
        ArtifactManager = new ArtifactManager(randomArtifactPool, setArtifactPool, maxArtifacts, allowArtifactTesting, testData);

    }
}
