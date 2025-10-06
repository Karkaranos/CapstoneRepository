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
    InputAction trashSpell;

    private void Awake()
    {
        //sets up inputactions
        input = GetComponent<PlayerInput>();
        input.currentActionMap.Enable();
        mouseMove = input.currentActionMap.FindAction("MousePos");
        trashSpell = input.currentActionMap.FindAction("RightClick");
    }

    /// <summary>
    /// subscribes to everything when enabled
    /// </summary>
    private void OnEnable()
    {
        //subscribes to needed funcs
        trashSpell.started += TrashSpell_started;
    }

    /// <summary>
    /// unsubscribes from everything when disabled
    /// </summary>
    private void OnDisable()
    {
        //unsubscribes
        trashSpell.started -= TrashSpell_started;
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
