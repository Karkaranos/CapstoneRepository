/*************************************************
Author Names : 		Tyler Hayes, Jay Embry
Date Created : 		10/27/2025
Date Last Modified : 2/15/2026 (Jay Embry)
Brief Description : Handles all of the player's inputs
External Resources : 	
***************************************************/

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputHandler : MonoBehaviour
{
    #region VARS

    //all of the player's inputactions
    private PlayerInput pInput;
    private InputAction mousePos;
    private InputAction rightClick;
    private InputAction leftClick;
    private InputAction toggleConsole;
    private InputAction panCam;
    private InputAction movePlayer;

    //stores if the mouse has been rightclicked because this shit has to be done in fixedupdate for whatever fucking reason
    private bool mousePressed = false;
    [HideInInspector] public bool enableMovement;
    private bool isMoving;

    //used to determine whenever the player is pathing an attack
    [HideInInspector] public bool IsPathing;

    private Vector2 movementDirection;

    #endregion VARS

    #region INITIALIZATION

    /// <summary>
    /// sets up actionmap
    /// </summary>
    private void Awake()
    { 
        pInput = GetComponent<PlayerInput>();
        pInput.currentActionMap.Enable();
        mousePos = pInput.currentActionMap.FindAction("MousePos");
        rightClick = pInput.currentActionMap.FindAction("RightClick");
        leftClick = pInput.currentActionMap.FindAction("LeftClick");
        toggleConsole = pInput.currentActionMap.FindAction ("ToggleConsole");
        panCam = pInput.currentActionMap.FindAction("PanCamera");
        movePlayer = pInput.currentActionMap.FindAction("Move");


        mousePressed = false;
        enableMovement = false;
        isMoving = false;
    }


    /// <summary>
    /// subscribes to all needed functions
    /// </summary>
    private void OnEnable()
    {
        PublicEvents.EnablePlayersInputs += EnableOrDisablePlayersInputs;
        rightClick.started += RightClick_started;
        leftClick.started += LeftClick_started;
        leftClick.canceled += LeftClick_canceled;
        toggleConsole.started += Toggle_started;
        panCam.performed += PanCam_performed;
        movePlayer.performed += MovePlayer_performed;
        movePlayer.canceled += MovePlayer_canceled;

        pInput.onControlsChanged += PInput_onControlsChanged;
    }

    private void LeftClick_canceled(InputAction.CallbackContext obj)
    {
        PublicEvents.LeftClickReleased?.Invoke();
    }

    /// <summary>
    /// unsubscribes from all needed functions
    /// </summary>
    private void OnDisable()
    {
        PublicEvents.EnablePlayersInputs -= EnableOrDisablePlayersInputs;
        rightClick.started -= RightClick_started;
        leftClick.started -= LeftClick_started;
        leftClick.canceled -= LeftClick_canceled;
        panCam.performed -= PanCam_performed;
        movePlayer.performed -= MovePlayer_performed;
        movePlayer.canceled -= MovePlayer_canceled;

        pInput.onControlsChanged -= PInput_onControlsChanged;
    }

    /// <summary>
    /// call this via the publicevent EnablePlayersInputs to enable/disable the player's actionmap 
    /// </summary>
    /// <param name="isEnabled"></param>
    private void EnableOrDisablePlayersInputs(bool isEnabled)
    {
        if (isEnabled)
        {
            pInput.currentActionMap.Enable();
        }
        else
        {
            pInput.currentActionMap.Disable();
        }
    }

    private void PInput_onControlsChanged(PlayerInput obj)
    {

        if (obj.currentControlScheme == "KeyboardAndMouse")
        {
            PublicEvents.ControllerDisabled?.Invoke();
        }
        else
        {
            if (obj.currentControlScheme == "Controller")
            {
                PublicEvents.ControllerEnabled?.Invoke();
            }
        }
    }

    #endregion INITIALIZATION

    #region PLAYERINPUTHANDLERS

    /// <summary>
    /// called whenever the player left clicks
    /// </summary>
    /// <param name="obj"></param>
    private void LeftClick_started(InputAction.CallbackContext obj)
    {
        mousePressed = true;

        PublicEvents.LeftClicked?.Invoke();
    }

    /// <summary>
    /// called whenever the player rightclicks
    /// </summary>
    /// <param name="obj"></param>
    private void RightClick_started(InputAction.CallbackContext obj)
    {
        PublicEvents.RightClicked?.Invoke();
    }

    /// <summary>
    /// toggles the console
    /// </summary>
    /// <param name="obj"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    private void Toggle_started(InputAction.CallbackContext obj)
    {
        Debug.Log("sending public event");
        PublicEvents.ToggleConsole?.Invoke();
    }


    /// <summary>
    /// sends out the public event to pan the camera
    /// </summary>
    /// <param name="obj"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    private void PanCam_performed(InputAction.CallbackContext obj)
    {
        PublicEvents.PanCamera?.Invoke(obj.ReadValue<Vector2>());
    }

    /// <summary>
    /// Sends out a public event to move the player in the grid
    /// </summary>
    /// <param name="obj"></param>
    private void MovePlayer_performed(InputAction.CallbackContext obj)
    {

        if (FindFirstObjectByType<RuneEvents>())
        {

            if (FindFirstObjectByType<RuneEvents>().WaitingOnPath)
            {

                IsPathing = true;

            }

        }

        movementDirection = obj.ReadValue<Vector2>();

        isMoving = true;
    }

    /// <summary>
    /// Changes a bool for fixed update so we can tell it the player has stopped moving
    /// </summary>
    /// <param name="obj"></param>
    private void MovePlayer_canceled(InputAction.CallbackContext obj)
    {
        isMoving = false;

        IsPathing = false;

    }

    /// <summary>
    /// detects if the player clicked on a tile and sends out the tile clicked on if true
    /// apparently this only works in update i fucking hate this >:C
    /// Also used to continuously send an event for moving becuase just having it in a performed call doesn't work
    /// </summary>
    private void FixedUpdate()
    {
        PublicEvents.MousePosition?.Invoke(mousePos.ReadValue<Vector2>());

        //only triggers when leftclicked
        if (mousePressed)
        {
            mousePressed = false;

            //creates a raycast based on where the mouse is
            Ray ray = Camera.main.ScreenPointToRay(mousePos.ReadValue<Vector2>());
            RaycastHit hit;

            //if it hits something
            if (Physics.Raycast(ray, out hit))
            {
                //if it hits a tilebehavior, sends the publicevent
                if (hit.transform.gameObject.GetComponentInParent<TileBehaviour>() != null)
                {

                    if (hit.transform.gameObject.GetComponent<Enemy>() != null || hit.transform.gameObject.GetComponentInChildren<Enemy>() != null)
                    {

                        PublicEvents.SelectTarget?.Invoke(hit.transform.gameObject.GetComponentInParent<TileBehaviour>(),
                            hit.transform.gameObject.GetComponent<Enemy>(), null);

                    }
                    else if (hit.transform.gameObject.GetComponent<PlayerBehavior>() != null || hit.transform.gameObject.GetComponentInChildren<PlayerBehavior>() != null)
                    {

                        PublicEvents.SelectTarget?.Invoke(hit.transform.gameObject.GetComponentInParent<TileBehaviour>(),
                           null, hit.transform.gameObject.GetComponent<PlayerBehavior>());

                    }
                    else
                    {

                        PublicEvents.SelectTarget?.Invoke(hit.transform.gameObject.GetComponentInParent<TileBehaviour>(), null, null);

                    }

                }

            }
        }

        if(isMoving || IsPathing)
        {
            //Using an if statement in case we want to call another event when not trying to move
            if (enableMovement)
            {
                PublicEvents.MovementDirection?.Invoke(movementDirection);
            }
        }
    }

    #endregion PLAYERINPUTHANDLERS
}
