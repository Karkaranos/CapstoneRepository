/*************************************************
Author Names : 		Cade Naylor
Date Created : 		10/24/2025
Date Last Modified : 10/24/2025
Brief Description : Stores FMOD event references
External Resources : 	
	***************************************************/
using UnityEngine;
using FMODUnity;
using NaughtyAttributes;

public class FMODEventsManager : MonoBehaviour
{
    #region Variables
    private enum Settings
    {
        music, one_shot_sounds, looping_sounds
    }

    [SerializeField] private Settings inspectorSettings;
    public static FMODEventsManager instance { get; private set; }

    #region Music
    [HorizontalLine(4, EColor.Red)]
    [SerializeField, ShowIf(nameof(inspectorSettings), Settings.music)] private EventReference lvl1BGM;
    #endregion

    #region One Shots
    [HorizontalLine(4, EColor.Yellow)]
    [SerializeField, ShowIf(nameof(inspectorSettings), Settings.one_shot_sounds), Tooltip("Array index should match the spell's number -1")] 
        private EventReference[] lightningSounds;
    #endregion

    #region Looping SFX
    [HorizontalLine(4, EColor.Blue)]
    [SerializeField, ShowIf(nameof(inspectorSettings), Settings.one_shot_sounds)] private EventReference testIG;
    #endregion

    #region Getters and Setters
    public EventReference Lvl1BGM { get; private set; }
    public EventReference[] LightningSounds { get; private set; }

    #endregion
    #endregion

    /// <summary>
    /// Called upon the first frame update
    /// Ensures there is only one FMOD instance
    /// </summary>
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("There is more than one FMODEventsManager in the scene");
            Destroy(instance);
        }
        instance = this;
    }
}
