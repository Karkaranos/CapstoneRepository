using UnityEngine;
using NaughtyAttributes;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PopUpScript : MonoBehaviour
{
    [SerializeField] private string targetTag = "Background"; //change tag to whatever is needed to affect all background objects.
    [SerializeField] private float delayMultiplier = 0.1f; //controls how much distance affects delay.

    private List<GameObject> backgrounds; //lists of all background objects.
    void Start()
    {
        backgrounds = GameObject.FindGameObjectsWithTag(targetTag).ToList();

        backgrounds = backgrounds.OrderBy(obj => Vector3.Distance(transform.position, obj.transform.position)).ToList();

        StartCoroutine(Flip());
    }
    [Button]
    public IEnumerator Flip()
    {
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
            Debug.Log("Flipping " + obj.name + " at " + delay + " seconds");
        }
    }
    [Button]
    public IEnumerator UnFlip()
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

   
    //when the book opens to a level begin flipping object upright
    //grab a list of all items 
}
