using NaughtyAttributes;
using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class CameraPanning : MonoBehaviour
{
    private enum Cameras
    {
        Cameras,
        Refs
    }

    public Input playerInput;
    [SerializeField] private Cameras Cams;
    [SerializeField, ShowIf(nameof(Cams), Cameras.Refs)] private Transform CameraTarget;
    [SerializeField, ShowIf(nameof(Cams), Cameras.Refs)] Vector2 moveInput;
    [SerializeField, ShowIf(nameof(Cams), Cameras.Refs)] private float panSpeed = 20f;


    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // Update is called once per frame
    private void Update()
    {
        float deltaTime = Time.unscaledDeltaTime;

        UpdateMovement(deltaTime);
    }


    void UpdateMovement(float deltaTime)
    {
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = Camera.main.transform.right;
        forward.y = 0f;
        forward.Normalize();

        Vector3 targetVelocity = new Vector3(moveInput.x, 0, moveInput.y) * panSpeed;


        Vector3 motion = targetVelocity * deltaTime;

        CameraTarget.position += forward * motion.z + right * moveInput.x;
    }
}
