using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : GridMovement
{
    private PlayerControls inputActions;


    private void Awake()
    {
        inputActions = new PlayerControls();
    }
    private void MovePerformed(InputAction.CallbackContext context)
    {
        Vector3 moveDirection = context.ReadValue<Vector3>();
        
        if (moveDirection.x == -1)
        {
            MoveTo(TylersGridManager.GetUpperLeftNeighbor(currentTile));
        }
        if (moveDirection.x == 1)
        {
            MoveTo(TylersGridManager.GetUpperRightNeighbor(currentTile));
        }
        if (moveDirection.y == 1)
        {
            MoveTo(TylersGridManager.GetUpperNeighbor(currentTile));
        }
        if (moveDirection.y == -1)
        {
            MoveTo(TylersGridManager.GetLowerNeighbor(currentTile));
        }
        if (moveDirection.z == 1)
        {
            MoveTo(TylersGridManager.GetLowerLeftNeighbor(currentTile));
        }
        if (moveDirection.z == -1)
        {
            MoveTo(TylersGridManager.GetLowerRightNeighbor(currentTile));
        }
    }
    
    private void OnEnable()
    {
        inputActions.PlayerActions.Enable();
        inputActions.PlayerActions.Move.performed += MovePerformed;
    }

    private void OnDisable()
    {
        inputActions.PlayerActions.Disable();
        inputActions.PlayerActions.Move.performed -= MovePerformed;
    }
}
