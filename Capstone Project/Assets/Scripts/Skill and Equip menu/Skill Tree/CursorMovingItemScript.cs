/*************************************************
Author Names : 		Tyler Hayes 
Date Created : 		10/6/2025
Date Last Modified : 10/6/2025
Brief Description : Short script that just moves the box to the
                    mouse cursor. 
External Resources : 	
	***************************************************/

using UnityEngine;
using UnityEngine.InputSystem;

public class CursorMovingItemScript : MonoBehaviour
{
    PlayerInput input;
    InputAction mouseMove;
    InputAction TrashSpell;

    private void Awake()
    {
        //sets up inputactions
        input = GetComponent<PlayerInput>();
        input.currentActionMap.Enable();
        mouseMove = input.currentActionMap.FindAction("MousePos");
        TrashSpell = input.currentActionMap.FindAction("RightClick");
    }

    private void OnEnable()
    {
        //subscribes to needed funcs
        TrashSpell.started += TrashSpell_started;
    }

    private void OnDisable()
    {
        //unsubscribes
        TrashSpell.started -= TrashSpell_started;
    }
    /// <summary>
    /// Trashes held item when right clicked while holding item
    /// </summary>
    /// <param name="obj"></param>
    private void TrashSpell_started(InputAction.CallbackContext obj)
    {
        FindFirstObjectByType<SkillTreeManager>().ConfirmEquipSpell();
    }

    void Update()
    {
        //always sets pos to mousepos
        transform.position = mouseMove.ReadValue<Vector2>();
    }
}
