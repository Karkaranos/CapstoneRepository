using UnityEngine;
using NaughtyAttributes;
using System.Collections;

public class PopUpScript : MonoBehaviour
{
    [SerializeField] private GameObject backgroundObj;
    [SerializeField] private GameObject[] backObjs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //scan for closest animators
        
        if (backObjs != null)
        {
            backObjs = GameObject.FindGameObjectsWithTag("Background");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Flip()
    {
        foreach (GameObject backgroundObj in backObjs)
        {
            backgroundObj.GetComponent<Animator>().SetTrigger("Flip");
        }
    }

    void UnFlip()
    {
        foreach (GameObject backgroundObj in backObjs)
        {
            backgroundObj.GetComponent<Animator>().SetTrigger("UnFlip");
        }
    }

    IEnumerator Flipping()

    //when the book opens to a level begin flipping object upright
    //grab a list of all items 
}
