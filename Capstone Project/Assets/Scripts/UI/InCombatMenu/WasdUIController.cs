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

    /// <summary>
    /// Subscribe to event
    /// </summary>
    private void OnEnable()
    {
        PublicEvents.MoveButton += ToggleWasd;
    }

    /// <summary>
    /// unsubscribe from event
    /// </summary>
    private void OnDisable()
    {
        PublicEvents.MoveButton -= ToggleWasd;
    }

    /// <summary>
    /// Toggle if the WASD prompt is showing 
    /// </summary>
    private void ToggleWasd()
    {
        canvasGroup.alpha = canvasGroup.alpha == 0 ? 1 : 0; 
    }
}
