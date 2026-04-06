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
    [SerializeField, Tooltip("A reference to the object the Animator is on")] private GameObject bushAnim;
    [Tooltip("A reference to the bush's Sprite Renderer"), SerializeField, Required]
    private SpriteRenderer bushSprite;
    [Tooltip("A reference to the bush's Animator"), SerializeField]
    private Animator bAnimator;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bAnimator = bushAnim.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
