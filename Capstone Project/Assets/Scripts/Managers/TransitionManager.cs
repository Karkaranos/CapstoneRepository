/******************************************************************************
 * Author: Brad Dixon
 * Creation Date: 3/2/2026
 * Last Modified: 3/11/2026
 * Brief: Handles the transitions between scenes and levels
 * External Resources: https://www.youtube.com/watch?v=HBEStd96UzI Used this as a starting point
 * ***************************************************************************/
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.SceneManagement;
using FMODUnity;
using System;

public class TransitionManager : MonoBehaviour
{
    [SerializeField] private Animator transitionAnimator;
    private int sceneToLoad;
    public static TransitionManager instance;

    [SerializeField] private EventReference bgmEventRefSFX;
    [SerializeField] private EventReference ambienceEventRefSFX;
    [SerializeField] private GameObject audioListenerObject;
    [SerializeField] private EventReference EquipmnetEventRefSFX;

    [SerializeField] private FMOD.Studio.Bus masterBus;
   [SerializeField] private FMOD.Studio.Bus bgmBus;
    [SerializeField] private FMOD.Studio.Bus sfxBus;
    [SerializeField] private FMOD.Studio.Bus ambBus;


    public bool TransitioningBetweenLevels = false;


    /// <summary>
    /// Ensures there is only one instance of this manager and allows it to persist through scenes
    /// </summary>
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);

        masterBus = RuntimeManager.GetBus("bus:/");
        bgmBus = RuntimeManager.GetBus("bus:/BGM");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
        ambBus = RuntimeManager.GetBus("bus:/Ambience");
    }

    /// <summary>
    /// Public call so we can tell it when we want a scene transition to play
    /// </summary>
    public void SceneTransition(int s)
    {
        TransitioningBetweenLevels = true;
        sceneToLoad = s;
        transitionAnimator.SetTrigger("SceneTransition");
    }

    /// <summary>
    /// Used for a animation event to load the next scene while the transition is happening
    /// </summary>
    public void LoadSceneDuringTransition()
    {
        SceneManager.LoadScene(sceneToLoad);
        TransitioningBetweenLevels = false;
    }

    /// <summary>
    /// Public call for when the cutscene ends naturally
    /// </summary>
    public void CutsceneTransition()
    {
        transitionAnimator.SetTrigger("GameBegin");
        TransitioningBetweenLevels = true;
    }

    /// <summary>
    /// Public call for when the player skips the cutscene
    /// </summary>
    public void SkipButtonTransition()
    {
        transitionAnimator.SetTrigger("SkipTransition");
        TransitioningBetweenLevels = true;
    }

    /// <summary>
    /// Public call to disable the video in the button manager. Used specifically for skipping
    /// </summary>
    public void DisableCutscene()
    {
        FindFirstObjectByType<ButtonManager>().DisableVideoCanvas();
    }

    /// <summary>
    /// Public call for the default level transition
    /// </summary>
    public void LevelTransition()
    {
        transitionAnimator.SetTrigger("LevelTransition");

        AudioManager.instance.CreateEventInstance(ambienceEventRefSFX);
        AudioManager.instance.PlayOneShot(ambienceEventRefSFX, audioListenerObject.transform.position);

        AudioManager.instance.CreateEventInstance(bgmEventRefSFX);
        AudioManager.instance.PlayOneShot(bgmEventRefSFX, audioListenerObject.transform.position);
    }

    /// <summary>
    /// Public call for the transition that takes you to the win screen when you kill all the enemies.
    /// </summary>
    public void LevelToEndScreen()
    {
        transitionAnimator.SetTrigger("EndScreen");
        bgmBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
        sfxBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
        ambBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    /// <summary>
    /// Animation event that shows the end screen UI
    /// </summary>
    public void ShowEndUI()
    {
        FindFirstObjectByType<EndLevelMenu>().ShowTheEndMenuUI();

        bgmBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
        sfxBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
        ambBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);

    }

    /// <summary>
    /// Public call for the transition that takes you to the equip menu when you continue from the end screen.
    /// </summary>
    public void EndScreenToEquipMenu()
    {
        transitionAnimator.SetTrigger("EquipMenu");
        FindFirstObjectByType<SkillAndArtifactManager>(FindObjectsInactive.Include)?.SetButton();

        masterBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);

        AudioManager.instance.CreateEventInstance(EquipmnetEventRefSFX);
        AudioManager.instance.PlayOneShot(EquipmnetEventRefSFX, audioListenerObject.transform.position);
    }

    /// <summary>
    /// Animation event that loads the equip menu for the next level
    /// </summary>
    public void ShowEquipMenu()
    {
        FindFirstObjectByType<EndLevelMenu>().LoadNextLevel();
    }

    /// <summary>
    /// Animation event to execute the code that loads the level
    /// </summary>
    public void LoadLevel()
    {
        FindFirstObjectByType<SkillAndArtifactManager>().ShowNextLevel();
    }
}
