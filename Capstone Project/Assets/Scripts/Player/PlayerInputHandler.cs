/*************************************************
Author Names : 		Tyler Hayes 
Date Created : 		10/27/2025
Date Last Modified : 10/27/2025
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

    //stores if the mouse has been rightclicked because this shit has to be done in fixedupdate for whatever fucking reason
    private bool mousePressed = false;

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

        mousePressed = false;
    }


    /// <summary>
    /// subscribes to all needed functions
    /// </summary>
    private void OnEnable()
    {
        PublicEvents.EnablePlayersInputs += EnableOrDisablePlayersInputs;
        rightClick.started += RightClick_started;
        leftClick.started += LeftClick_started;
    }

    /// <summary>
    /// unsubscribes from all needed functions
    /// </summary>
    private void OnDisable()
    {
        PublicEvents.EnablePlayersInputs -= EnableOrDisablePlayersInputs;
        rightClick.started -= RightClick_started;
        leftClick.started -= LeftClick_started;
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
    /// detects if the player clicked on a tile and sends out the tile clicked on if true
    /// apparently this only works in update i fucking hate this >:C
    /// </summary>
    private void FixedUpdate()
    {
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
                    PublicEvents.SelectTile?.Invoke(hit.transform.gameObject.GetComponentInParent<TileBehaviour>());

                    if (hit.transform.gameObject.GetComponent<Enemy>() != null)
                    {

                        PublicEvents.SelectTarget?.Invoke(hit.transform.gameObject.GetComponentInParent<TileBehaviour>(),
                            hit.transform.gameObject.GetComponent<Enemy>(), null);

                    }
                    else if (hit.transform.gameObject.GetComponent<PlayerBehavior>() != null)
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
    }

    #endregion PLAYERINPUTHANDLERS
}
