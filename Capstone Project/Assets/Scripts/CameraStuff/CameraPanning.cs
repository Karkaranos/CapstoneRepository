/*************************************************
Author Names : 		    Aidan Ratcliffe
Date Created : 		    1/29/2026
Date Last Modified : 	2/4/2026 
Brief Description : 	Controls the camera panning
External Resources : 	N/A
***************************************************/
using NaughtyAttributes;
using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class CameraPanning : MonoBehaviour
{

    #region variables
    private enum Cameras
    {
        Cameras,
        Refs
    }

    public Input playerInput;
    [SerializeField] private Cameras Cams;
    [SerializeField, ShowIf(nameof(Cams), Cameras.Refs)] private Transform CameraTarget;
    [SerializeField, ShowIf(nameof(Cams), Cameras.Refs)] Vector2 moveInput;
    [SerializeField, ShowIf(nameof(Cams), Cameras.Refs)] public float panSpeed;
    #endregion

    #region functions

    /// <summary>
    /// Enables public events
    /// </summary>
    private void OnEnable()
    {
        PublicEvents.PanCamera += OnMove;
    }

    /// <summary>
    /// Disables public event
    /// </summary>
    private void OnDisable()
    {
        PublicEvents.PanCamera -= OnMove;
    }


    /// <summary>
    /// Grabs the OnMove function from Player Input
    /// </summary>
    /// <param name="value"></param>
    public void OnMove(Vector2 value)
    {
        moveInput = value;
    }

    // Update is called once per frame
    /// <summary>
    /// Simply creating a float variable to represent deltatime/frames in real time
    /// </summary>
    private void Update()
    {
        float deltaTime = Time.unscaledDeltaTime;

        UpdateMovement(deltaTime);
    }

    /// <summary>
    /// UpdateMovement takes in the float and multiplies deltaTime by the various Vector3s
    /// to represent the different directions the camera will move
    /// </summary>
    /// <param name="deltaTime"></param>
    void UpdateMovement(float deltaTime)
    {
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = Camera.main.transform.right;
        right.y = 0f;
        right.Normalize();

        Vector3 targetVelocity = new Vector3(moveInput.x, 0, moveInput.y) * panSpeed;


        Vector3 motion = transform.forward * moveInput.y + transform.right * moveInput.x;

        CameraTarget.position += motion * panSpeed * deltaTime;
    }

    #endregion

}
