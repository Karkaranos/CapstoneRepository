/*************************************************
Author Names : 		Clare
Date Created : 		3/5/2026
Date Last Modified : 3/5/2026
Brief Description : controls the wasd appear and disapear
***************************************************/
using UnityEngine;

public class WasdUIController : MonoBehaviour
{
    [SerializeField] private GameObject wasdObject;
    [SerializeField] private CanvasGroup canvasGroup;

    private void OnEnable()
    {
        PublicEvents.MoveButton += ToggleWasd;
    }

    private void OnDisable()
    {
        PublicEvents.MoveButton -= ToggleWasd;
    }

    private void ToggleWasd()
    {
        canvasGroup.alpha = canvasGroup.alpha == 0 ? 1 : 0; 
    }
}
