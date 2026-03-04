/*************************************************
Author Names : 		    Aidan Ratcliffe
Date Created : 		    10/22/2025
Date Last Modified : 	1/26/2026
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

    [SerializeField, ShowIf(nameof(Cams), Cameras.Refs)] public GameObject player;
    [SerializeField, ShowIf(nameof(Cams), Cameras.Refs)] public CinemachineCamera Level1cam;
    [SerializeField, ShowIf(nameof(Cams), Cameras.Refs)] public CinemachineCamera Level1playcam;
    //[SerializeField, ShowIf(nameof(Cams), Cameras.Refs)] public CinemachineCamera PlayerZcam;
    [SerializeField, ShowIf(nameof(Cams), Cameras.Refs)] private CinemachineCamera activeCam;
    [SerializeField, ShowIf(nameof(Cams), Cameras.Refs)] public PopUpScript PopUpScript;
    [SerializeField, ShowIf(nameof(Cams), Cameras.Refs)] public VideoPlayer VideoPlayer;
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
        PopUpScript.StartCoroutine(PopUpScript.Flip());
        SwitchCamera(Level1playcam);
        //level1cam.Priority = 10;
    }

    /// <summary>
    /// A coroutine to play the cutscene and then switch to the spell canvas
    /// </summary>
    /// <returns></returns>
    IEnumerator SwitchesToOutOfCombatCanvas()
    {
        VideoPlayer.Prepare();
        while (!VideoPlayer.isPrepared)
        {
            yield return null; // Wait until preparation is complete
        }

        // Play the video
        VideoPlayer.Play();

        // Wait until the video finishes playing
        while (VideoPlayer.isPlaying)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        OutOfCombatCanvas.SetActive(true);
        VideoCanvas.SetActive(false);
    }

    /// <summary>
    /// Funtion to switch camera depending on priority.
    /// Level1cutscenecam needs to be the highest priority on start for this to work
    /// 10 = default priority
    /// 20 = highest priority
    /// </summary>
    /// <param name="newActiveCamera"></param>
    public void SwitchCamera(CinemachineCamera ActiveCamera)
    {
        //Level1cam.Priority = 20; 
        Level1playcam.Priority = 10;
        //PlayerZcam.Priority = 10;
        player = GameObject.FindGameObjectWithTag("Player");
        //PlayerZcam.LookAt = player.transform;
        //PlayerZcam.Follow = player.transform;
        Debug.LogWarning(ActiveCamera.name);
        if(activeCam != null)
        {
            activeCam.Priority = 10;
        }

        activeCam = ActiveCamera;
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
