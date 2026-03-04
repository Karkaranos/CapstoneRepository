/******************************************************************************
 * Author: Brad Dixon
 * Creation Date: 3/2/2026
 * Last Modified: 3/3/2026
 * Brief: Handles the transitions between scenes and levels
 * External Resources: https://www.youtube.com/watch?v=HBEStd96UzI Used this as a starting point
 * ***************************************************************************/
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    [SerializeField] private Animator transitionAnimator;
    [SerializeField, Scene] private string gameplayScene;
    public static TransitionManager instance;

    /// <summary>
    /// Ensures there is only one instance of this manager and allows it to persist through scenes
    /// </summary>
    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// Public call so we can tell it when we want a scene transition to play
    /// </summary>
    public void StartSceneTransition()
    {
        transitionAnimator.SetTrigger("SceneStart");
    }

    /// <summary>
    /// Used for a animation event to load the next scene while the transition is happening
    /// </summary>
    public void EndSceneTransition()
    {
        SceneManager.LoadScene(gameplayScene);
        transitionAnimator.SetTrigger("SceneEnd");
    }

    /// <summary>
    /// Public call for when the cutscene ends naturally
    /// </summary>
    public void CutsceneTransition()
    {
        transitionAnimator.SetTrigger("GameBegin");
    }

    /// <summary>
    /// Public call for when the player skips the cutscene
    /// </summary>
    public void SkipButtonTransition()
    {
        transitionAnimator.SetTrigger("SkipTransition");
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
    }

    /// <summary>
    /// Public call for the transition that takes you to the win screen when you kill all the enemies.
    /// </summary>
    public void LevelToEndScreen()
    {
        transitionAnimator.SetTrigger("EndScreen");
    }

    /// <summary>
    /// Animation event that shows the end screen UI
    /// </summary>
    public void ShowEndUI()
    {
        FindFirstObjectByType<EndLevelMenu>().ShowTheEndMenuUI();
    }

    /// <summary>
    /// Public call for the transition that takes you to the equip menu when you continue from the end screen.
    /// </summary>
    public void EndScreenToEquipMenu()
    {
        transitionAnimator.SetTrigger("EquipMenu");
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
