/*************************************************
Author Names : 	Jay Embry
Date Created : 	10/22/2025
Date Last Modified : 10/22/2025
Brief Description : Blows up the GameObject used to signal that an enemy's been attacked
External Resources : 	
	***************************************************/
using UnityEngine;

public class TempVFXManager : MonoBehaviour
{

    /// <summary>
    /// calls whenever this game object is instantiated
    /// </summary>
    void Start()
    {

        Invoke("DestroyGameObject", 1);
        
    }

    /// <summary>
    /// destroys this game object
    /// can be replaced later when we have actual vfx implemented
    /// </summary>
    void DestroyGameObject()
    {

        Destroy(this.gameObject);

    }

}
