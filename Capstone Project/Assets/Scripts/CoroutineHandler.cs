/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/6/2025
Date Last Modified : 	10/6/2025
Brief Description : 		Coroutine Handler singleton needed for state machine
External Resources : 	
***************************************************/
using System.Collections;
using UnityEngine;

public class CoroutineHandler : MonoBehaviour
{
    public static CoroutineHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RunCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}