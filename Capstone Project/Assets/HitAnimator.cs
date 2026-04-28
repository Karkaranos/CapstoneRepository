/*************************************************
Author Names : 		Jake Gorski, 
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

    public void ShockHit() //forces the enemy to play the shocked animation.
    {
        animator.SetTrigger("Shocked");
    }
    public void WindHit() //forces the enemy to play the Damaged animation.
    {
        animator.SetTrigger("Damaged");
    }
}
