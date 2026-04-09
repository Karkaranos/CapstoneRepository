/*************************************************
Author Names : 		Clare
Date Created : 		3/5/2026
Date Last Modified : 3/5/2026
Brief Description : controls the wasd appear and disapear
External Resources : 	N/A
***************************************************/
using UnityEngine;

public class WasdUIController : MonoBehaviour
{
    [SerializeField] private GameObject wasdObject;
    [SerializeField] private CanvasGroup canvasGroup;

    private GridTesting gridTesting;

    /// <summary>
    /// On start initialization
    /// </summary>
    private void Start()
    {
        gridTesting = FindFirstObjectByType<GridTesting>();
    }

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
    /// && its the tutorial level
    /// </summary>
    private void ToggleWasd()
    {
        if(gridTesting.GetGridIndex() == 0)
        {
            canvasGroup.alpha = canvasGroup.alpha == 0 ? 1 : 0;
        }
        
    }
}
