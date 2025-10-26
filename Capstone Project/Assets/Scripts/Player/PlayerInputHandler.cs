using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput pInput;
    private InputAction mousePos;
    private InputAction rightClick;
    private InputAction leftClick;

    #region INITIALIZATION

    private void Awake()
    {
        pInput = GetComponent<PlayerInput>();
        pInput.currentActionMap.Enable();
        mousePos = pInput.currentActionMap.FindAction("MousePos");
        rightClick = pInput.currentActionMap.FindAction("RightClick");
        leftClick = pInput.currentActionMap.FindAction("LeftClick");

        
    }

    

    private void OnEnable()
    {
        PublicEvents.EnablePlayersInputs += EnableOrDisablePlayersInputs;
        rightClick.started += RightClick_started;
        leftClick.started += LeftClick_started;


        PublicEvents.RightClicked += RightClicked;
        PublicEvents.LeftClicked += LeftClicked;
        PublicEvents.SelectTile += TestingTile;
    }

    private void OnDisable()
    {
        PublicEvents.EnablePlayersInputs -= EnableOrDisablePlayersInputs;
        rightClick.started -= RightClick_started;
        leftClick.started -= LeftClick_started;
    }

    private void TestingTile(TileBehaviour obj)
    {
        Debug.Log("Tile Clicked On: " + obj.name);
    }

    private void RightClicked()
    {
        Debug.Log("Right Clicked");
    }

    private void LeftClicked()
    {
        Debug.Log("Left Clicked");
    }


    #endregion INITIALIZATION

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

    #region PLAYERINPUTHANDLERS

    private void LeftClick_started(InputAction.CallbackContext obj)
    {
        Debug.Log("LeftClicked");
        PublicEvents.LeftClicked?.Invoke();
    }

    private void RightClick_started(InputAction.CallbackContext obj)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePos.ReadValue<Vector2>());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 temp = hit.transform.gameObject.transform.position;
            if (hit.transform.gameObject.GetComponentInParent<TileBehaviour>() == null)
            {
                Debug.Log("is null" + hit.transform.gameObject.name);
            }
            else
            {
                PublicEvents.SelectTile(hit.transform.gameObject.GetComponentInParent<TileBehaviour>());
            }
        }

        
        PublicEvents.RightClicked?.Invoke();
    }

    private void FixedUpdate()
    {
        
    }


    #endregion PLAYERINPUTHANDLERS


}
