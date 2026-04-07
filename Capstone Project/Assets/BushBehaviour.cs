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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bAnimator = GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// When a player or enemy collides with the bush, the animation will play
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("I should be triggering");
            bAnimator.SetBool("BushAnim" , true);
        }
        else
        {
            bAnimator.SetBool("BushAnim", false);
        }
    }
    #endregion

}
