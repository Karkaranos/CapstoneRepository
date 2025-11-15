/*************************************************
Author Names : 		    Aidan Ratcliffe
Date Created : 		    10/22/2025
Date Last Modified : 	10/23/2025
Brief Description : 	Manages Camera Transitions
External Resources : 	N/A
***************************************************/
using NaughtyAttributes;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Video;


public class CameraManager : MonoBehaviour
{
    #region VARS
    
    private enum Cameras
    {
        Cameras,
        Refs
    }

    [SerializeField] private Cameras Cams;

    [SerializeField, ShowIf(nameof(Cams), Cameras.Refs)] public CinemachineCamera level1cam;
    [SerializeField, ShowIf(nameof(Cams), Cameras.Refs)] public CinemachineCamera level1playcam;
    [SerializeField, ShowIf(nameof(Cams), Cameras.Refs)] public PopUpScript popUpScript;
    [SerializeField, ShowIf(nameof(Cams), Cameras.Refs)] public VideoPlayer videoPlayer;
    public GameObject OutOfCombatCanvas;
    public GameObject VideoCanvas;
    public static GameObject VideoCanvasStatic;
    #endregion

    #region FUNCTIONS

    /// <summary>
    /// OnEnable for when the public event StartBattle is called
    /// </summary>
    public void OnEnable()
    {
        StartCoroutine(SwitchesToOutOfCombatCanvas());
        PublicEvents.StartBattle += SwitchesCamerasFromOutOfCombat;
        VideoCanvasStatic = VideoCanvas;
    }

    /// <summary>
    /// OnDisable for when the public event StartBattle is called
    /// </summary>
    public void OnDisable()
    {
        PublicEvents.StartBattle -= SwitchesCamerasFromOutOfCombat;
    }

    /// <summary>
    /// Function to switch the desired camera from spell choosing to playcam
    /// Switches playcam to active cam
    /// </summary>
    void SwitchesCamerasFromOutOfCombat()
    {
        VideoCanvas.SetActive(false);
        popUpScript.StartCoroutine(popUpScript.Flip());
        SwitchCamera(level1playcam);
        //level1cam.Priority = 10;
    }

    /// <summary>
    /// A coroutine to play the cutscene and then switch to the spell canvas
    /// </summary>
    /// <returns></returns>
    IEnumerator SwitchesToOutOfCombatCanvas()
    {
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
        {
            yield return null; // Wait until preparation is complete
        }

        // Play the video
        videoPlayer.Play();

        // Wait until the video finishes playing
        while (videoPlayer.isPlaying)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        OutOfCombatCanvas.SetActive(true);
    }

    /// <summary>
    /// Funtion to switch camera depending on priority.
    /// Level1cutscenecam needs to be the highest priority on start for this to work
    /// 10 = default priority
    /// 20 = highest priority
    /// </summary>
    /// <param name="newActiveCamera"></param>
    void SwitchCamera(CinemachineCamera ActiveCamera)
    {
        level1cam.Priority = 20; 
        level1playcam.Priority = 10;

        ActiveCamera.Priority = 20; 
    }

    /// <summary>
    /// Static function to skip the cutscene
    /// </summary>
    public static void SkipCutsceneStatic()
    {
        VideoCanvasStatic.SetActive(false);
    }

    /// <summary>
    /// function to skip the cutscene
    /// </summary>
    public void SkipCutscene()
    {
        VideoCanvas.SetActive(false);
    }
    #endregion
}
