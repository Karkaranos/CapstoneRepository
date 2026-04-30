/*************************************************
Author Names : 		 
Date Created : 		4/28/2026
Date Last Modified : 	4/28/2026
Brief Description : 		An intermediate between Enemy.CS and the animator of the enemies.
External Resources : 	
***************************************************/

using UnityEngine;

public class HitAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    /// <summary>
    /// Forces the enemy to play the shocked animation.
    /// </summary>
    public void ShockHit()
    {
        animator.SetTrigger("Shocked");
    }
    /// <summary>
    /// Forces the enemy to play the Damaged animation.
    /// </summary>
    public void WindHit() 
    {
        animator.SetTrigger("Damaged");
    }
}
