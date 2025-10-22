/*************************************************
Author Names : 	Jay Embry
Date Created : 	10/22/2025
Date Last Modified : 10/22/2025
Brief Description : Selects tile
External Resources : 	
	***************************************************/

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class TileSelector : MonoBehaviour
{

    [SerializeField] PlayerInput playerInput;

    [SerializeField] InputAction playerClick;
    [SerializeField] InputAction playerClickPerformed;

    private void OnEnable()
    {

        playerInput.currentActionMap.Enable();
        playerClick.Enable();
        playerClickPerformed.started += playerClickedConfirmed;

    }

    /// <summary>
    /// Runs whenever this script is destroyed
    /// </summary>
    private void OnDisable()
    {

        playerInput.currentActionMap.Disable();
        playerClick.Disable();
        playerClickPerformed.started -= playerClickedConfirmed;

    }

    private void playerClickedConfirmed(InputAction.CallbackContext context)
    {

        Debug.Log("Clicked!");

        Ray ray = Camera.main.ScreenPointToRay(playerClick.ReadValue<Vector2>());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {

            if (hit.transform.gameObject.GetComponent<TileBehavior>() != null)
            {

                Debug.Log("Clicked a tile!");
                PublicEvents.SelectTile.Invoke(hit.transform.gameObject.GetComponent<TileBehavior>());

            }

        }

    }

}
