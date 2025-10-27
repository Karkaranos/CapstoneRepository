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
    public GameObject OutOfCombatCanvas;
    #endregion

    #region FUNCTIONS

    /// <summary>
    /// OnDisable for when the public event StartBattle is called
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
    /// Function to switch the desired camera to "cutscene" camera
    /// </summary>
    void SwitchesCamerasFromOutOfCombat()
    {
        SwitchCamera(level1playcam);
    }

    IEnumerator SwitchesToOutOfCombatCanvas()
    {
        Debug.Log("This happening before the delay");
        yield return new WaitForSeconds(3f);
        Debug.Log("This is happening after delay");
        OutOfCombatCanvas.SetActive(true);
    }

    /// <summary>
    /// Funtion to switch camera depending on priority.
    /// 10 = default priority
    /// 20 = highest priority
    /// </summary>
    /// <param name="newActiveCamera"></param>
    void SwitchCamera(CinemachineCamera ActiveCamera)
    {
        level1cam.Priority = 20; 
        level1playcam.Priority = 10;

        ActiveCamera.Priority = 10; 
    }
    #endregion
}
