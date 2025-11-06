/*************************************************
Author Names : 		    Aidan Ratcliffe
Date Created : 		    10/22/2025
Date Last Modified : 	10/23/2025
Brief Description : 	Manages Camera Transitions
External Resources : 	N/A
***************************************************/
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;


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
    public GameObject OutOfCombatCanvas;
    #endregion

    #region FUNCTIONS

    /// <summary>
    /// OnEnable for when the public event StartBattle is called
    /// </summary>
    public void OnEnable()
    {
        StartCoroutine(SwitchesToOutOfCombatCanvas());
        PublicEvents.StartBattle += SwitchesCamerasFromOutOfCombat;
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
        yield return new WaitForSeconds(3f);
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
    #endregion
}
