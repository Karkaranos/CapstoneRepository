/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		2/12/2026
Date Last Modified : 	2/12/2026
Brief Description : 		Handles collection of pips 
External Resources : 	
***************************************************/
using UnityEngine;

public class Pip : MonoBehaviour
{
    /// <summary>
    /// If collider is player destory pip 
    /// Change current Pip on field count
    /// Increment action points
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        --PipManager.Instance.currentPipsOnField;
        Destroy(this.gameObject);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
