/*************************************************
Author Names : 		    Jake Gorski
Date Created : 		    10/30/2025
Date Last Modified : 	10/30/2025
Brief Description : 	When an object with the "TreeMainPoint" animator and labeled with the "Background" can be set to either Flip or Unflip based on the function activated in the inspector. 
External Resources : 	N/A
***************************************************/
using UnityEngine;
using NaughtyAttributes;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class PopUpScript : MonoBehaviour
{
    [SerializeField] private string targetTag = "Background"; //change tag to whatever is needed to affect all background objects.
    [SerializeField] private float delayMultiplier = 0.1f; //controls how much distance affects delay.

    private List<GameObject> backgrounds; //lists of all background objects.
    void Start()
    {
       GetReferences();

        //StartCoroutine(Flip());
    }

    /// <summary>
    /// Gets a reference to any popup background objects
    /// </summary>
    private void GetReferences()
    {
        backgrounds = GameObject.FindGameObjectsWithTag(targetTag).ToList();//grabs all "background" objects

        backgrounds = backgrounds.OrderBy(obj => Vector3.Distance(transform.position, obj.transform.position)).ToList();//orders by distance.
    }

    /// <summary>
    /// Calls the Flip coroutine
    /// </summary>
    public void StartFlip()
    {
        GetReferences();
        StartCoroutine(Flip());
    }

    [Button]
    public IEnumerator Flip()//objects that are laying down get flipped up. 
    {
        yield return new WaitForSeconds(1f);
        Debug.LogError(backgrounds.Count);
        foreach (var obj in backgrounds)
        {
            Animator anim = obj.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetTrigger("Flip");
            }

            float distance = Vector3.Distance(transform.position, obj.transform.position);
            float delay = distance * delayMultiplier;

            yield return new WaitForSeconds(delay);
            //Debug.Log("Flipping " + obj.name + " at " + delay + " seconds");
        }
    }

    [Button]
    public IEnumerator UnFlip()//takes the objects that are currently popped up and flattens them back out.
    {
        float maxDistance = backgrounds.Max(obj => Vector3.Distance(transform.position, obj.transform.position));

        foreach (var obj in backgrounds)
        {
            Animator anim = obj.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetTrigger("UnFlip");
            }

            float distance = Vector3.Distance(transform.position, obj.transform.position);
            float invertedDelay = (maxDistance - distance) * delayMultiplier;

            yield return new WaitForSeconds(invertedDelay);
            Debug.Log("Flipping " + obj.name + " at " + invertedDelay + " seconds");
        }
    }
}
