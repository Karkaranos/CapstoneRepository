using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEditor.Rendering;
using UnityEngine;


public class CameraManager : MonoBehaviour
{

    private enum Cameras
    {
        Cameras,
        Refs
    }

    private EquippedSpellNode equipSpellNode;

    [SerializeField] private Cameras Cams;

    [SerializeField] public CinemachineCamera level1cam;
    [SerializeField] public CinemachineCamera level1playcam;

    [SerializeField] public bool transitionStarted;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEnable()
    {
        PublicEvents.StartBattle += SwitchesCamerasFromOutOfCombat;
    }

    public void OnDisable()
    {
        PublicEvents.StartBattle -= SwitchesCamerasFromOutOfCombat;
    }

    void SwitchesCamerasFromOutOfCombat()
    {
        SwitchCamera(level1cam);
    }

    void SwitchCamera(CinemachineCamera newActiveCamera)
    {
        // Deactivate all cameras
        level1cam.Priority = 10; // Default priority
        level1playcam.Priority = 10;

        // Activate the new camera by setting its priority higher
        newActiveCamera.Priority = 20; // A higher priority makes it the active camera
    }
}
