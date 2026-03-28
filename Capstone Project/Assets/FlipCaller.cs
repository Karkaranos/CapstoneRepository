/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    3/28/2026
Date Last Modified : 	3/28/2026
Brief Description : 	Quick bandaid solution to get objects to pop up when ready is clicked
External Resources : 	N/A
***************************************************/
using UnityEngine;

public class FlipCaller : MonoBehaviour
{
    /// <summary>
    /// Gets a reference to the popup script and calls flipping
    /// </summary>
    public void Call()
    {
        FindFirstObjectByType<PopUpScript>()?.StartFlip();
    }
}
