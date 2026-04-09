/*************************************************
Author Names : 		    Aidan Ratcliffe
Date Created : 		    4/6/2026
Date Last Modified : 	4/6/2026
Brief Description : 	Triggers bush animations
External Resources : 	N/A
***************************************************/
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Animations;
using System.Runtime.CompilerServices;

public class BushBehaviour : MonoBehaviour
{
    #region player variables
    [SerializeField, Tooltip("A reference to the object the Animator is on")] private GameObject bushAnim;
    [Tooltip("A reference to the bush's Sprite Renderer"), SerializeField, Required]
    private SpriteRenderer bushSprite;
    [Tooltip("A reference to the bush's Animator"), SerializeField]
    private Animator bAnimator;
    #endregion

    #region functions
    /// <summary>
    /// Start is called once before the first execution of Update after the MonoBehaviour is created
    /// Grabs the Animator from the Bush Prefab and defines it
    /// </summary>
    void Start()
    {
        bAnimator = GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// When a player or enemy collides with the bush, the animation will play
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || (other.gameObject.CompareTag("Enemy")))
        {
            Debug.Log("I should be triggering");
            bAnimator.SetBool("BushAnim", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || (other.gameObject.CompareTag("Enemy")))
        {
            bAnimator.SetBool("BushAnim", false);
        }
    }
    #endregion

}
