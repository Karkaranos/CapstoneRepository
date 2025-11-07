/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/6/2025
Date Last Modified : 	11/7/2025
Brief Description : 		Coroutine Handler singleton needed for state machine
External Resources : 	
***************************************************/
using System.Collections;
using UnityEngine;

public class CoroutineHandler : MonoBehaviour
{
    public static CoroutineHandler Instance { get; private set; }

    /// <summary>
    /// Make sure that this is a Singleton 
    /// </summary>
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Calls the start Coroutine function from this script so it can be used 
    /// in non mono behaviour scripts 
    /// </summary>
    /// <param name="coroutine"></param>
    public void RunCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}