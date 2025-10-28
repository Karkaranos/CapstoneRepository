/*************************************************
Author Names : 	Jay Embry
Date Created : 	10/26/2025
Date Last Modified : 10/26/2025
Brief Description : Takes damage in place of the player if it occupies a tile
External Resources : 	
	***************************************************/
using UnityEngine;

public class ShieldBehavior : MonoBehaviour
{

    int shieldPoints;
    GameObject shield;

    /// <summary>
    /// runs whenever this script is generated from RuneEvents
    /// </summary>
    /// <param name="target"> transform of the player's tile </param>
    /// <param name="vfx"> vfx associated with the player's wind shield </param>
    public void OnShieldGenerated(Transform target, GameObject vfx)
    {

        shieldPoints = 2;
        shield = Instantiate(vfx, target);

    }

    /// <summary>
    /// call this if an enemy targets a tile with this script on it
    /// we can work out the specifics later i think
    /// depends on if we're getting TakeDamage() to work for PlayerStats??
    /// <summary>
    public void TakeDamage()
    {

        shieldPoints -= 1;

        if(shieldPoints <= 0)
        {

            Destroy(this);
            Destroy(shield);

        }

    }    

}
