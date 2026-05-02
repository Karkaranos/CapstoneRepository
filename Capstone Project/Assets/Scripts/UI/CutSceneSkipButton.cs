/******************************************************************************
 * Author: Gabriel Rodriguez
 * Creation Date: 03/03/2026
 * Last Modified: 03/03/2026 (Gabriel Rodriguez)
 * Brief: Allows skipping of the book opening animation and storybook cutscene
 * External Resources: N/A
 * ***************************************************************************/
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class CutSceneSkipButton : MonoBehaviour
{
    [SerializeField] private GameObject videoCanvas;

    /// <summary>
    /// Closes cutscene and enters out-of-combat menu
    /// </summary>
    public void SkipCutscene()
    {
        //videoCanvas.SetActive(false);
        FindAnyObjectByType<TransitionManager>().SkipButtonTransition();
        RuntimeManager.GetBus("Bus:/").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    /// <summary>
    /// Loads combat scene
    /// </summary>
    public void SkipBook()
    {
        gameObject.GetComponent<Button>().interactable = false;
        FindFirstObjectByType<TransitionManager>().SceneTransition(1);
        RuntimeManager.GetBus("Bus:/").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}