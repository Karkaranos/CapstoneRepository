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

    //calls the function that destroys this game object a second after it has popped up
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
