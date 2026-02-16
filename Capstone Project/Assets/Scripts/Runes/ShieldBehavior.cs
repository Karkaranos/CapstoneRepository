/*************************************************
Author Names : 	Jay Embry
Date Created : 	10/26/2025
Date Last Modified : 2/15/2026
Brief Description : Takes damage in place of the player if it occupies a tile
                    EDIT: Will get destroyed at the start of the player's turn
External Resources : 	
	***************************************************/
using UnityEngine;

public class ShieldBehavior : MonoBehaviour
{

    GameObject shield;

    /// <summary>
    /// runs whenever this script is generated from RuneEvents
    /// </summary>
    /// <param name="target"> transform of the player's tile </param>
    /// <param name="vfx"> vfx associated with the player's wind shield </param>
    public void OnShieldGenerated(Transform target, GameObject vfx)
    {

        shield = Instantiate(vfx, target);

    }

    public void TakeDamage()
    {

        //TODO: delete later

    }

    
    public void GetDestroyed()
    {

        Destroy(this);
        Destroy(shield);

    }

}
