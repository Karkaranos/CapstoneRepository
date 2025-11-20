/*************************************************
Author Names : 	Jay Embry
Date Created : 	11/18/2025
Date Last Modified : 11/18/2025
Brief Description : Should be put on spell animations.
                    For now, it just deletes the spell when its animation is over.
External Resources : 	
	***************************************************/
using UnityEngine;

public class VFXBehavior : MonoBehaviour
{

    /// <summary>
    /// destroys the effect
    /// should be called via a trigger at the end of an animation
    /// </summary>
    public void DestroyVFX()
    {

        Destroy(this.gameObject);

    }

}
