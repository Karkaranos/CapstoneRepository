/*************************************************
Author Names : 	Jay Embry
Date Created : 	10/26/2025
Date Last Modified : 10/26/2025
Brief Description : Delays enemies' turns and enables some combos
                    This may not be necessary. We'll find out.
External Resources : 	
	***************************************************/
using UnityEngine;

public class TornadoBehavior : MonoBehaviour

{

    GameObject tornado;


    /// <summary>
    /// runs whenever this script is generated from RuneEvents
    /// </summary>
    /// <param name="target"> transform of the player's tile </param>
    /// <param name="vfx"> vfx associated with the player's tornado </param>
    public void OnTornadoGenerated(Transform target, GameObject vfx)
    {

        tornado = Instantiate(vfx, target);

    }
}
