/*************************************************
Author Names : 		 
Date Created : 		4/28/2026
Date Last Modified : 	4/28/2026
Brief Description : 		An intermediate between Enemy.CS and the animator of the enemies.
External Resources : 	
***************************************************/

using UnityEngine;
using UnityEditor.Animations;
using System.Threading.Tasks;

public class HitAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private string lastState;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    /// <summary>
    /// Forces the enemy to play the shocked animation.
    /// </summary>
    public async void ShockHit()
    {
        var animInfo = animator.GetCurrentAnimatorClipInfo(0);
        float wait = 10f;
        AnimatorStateInfo currState;
        if (!CheckCurrentAnimatorState("ElecDamaged") && !CheckCurrentAnimatorState("Damaged"))
        {
            if (CheckCurrentAnimatorState(animInfo[0].clip.name))
            {
                lastState = animInfo[0].clip.name;
                Debug.Log(lastState);
            }
        }
        else
        {
            currState = animator.GetCurrentAnimatorStateInfo(0);
            wait = animInfo[0].clip.length * currState.normalizedTime;
            await Task.Delay((int)wait);
        }


        animator.SetTrigger("Shocked");

        currState = animator.GetCurrentAnimatorStateInfo(0);
        wait = animInfo[0].clip.length * currState.normalizedTime;
        await Task.Delay((int)wait);

        CallDeathCheck();
    }


    /// <summary>
    /// Checks if the animator is playing a specific clip
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    private bool CheckCurrentAnimatorState(string state)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(state);
    }

    /// <summary>
    /// Forces the enemy to play the Damaged animation.
    /// </summary>
    public async void WindHit() 
    {
        var animInfo = animator.GetCurrentAnimatorClipInfo(0);
        float wait = 10f;
        AnimatorStateInfo currState;
        if (!CheckCurrentAnimatorState("ElecDamaged") && !CheckCurrentAnimatorState("Damaged"))
        {
            if (CheckCurrentAnimatorState(animInfo[0].clip.name))
            {
                lastState = animInfo[0].clip.name;
                Debug.Log(lastState);
            }
        }
        else
        {
            currState = animator.GetCurrentAnimatorStateInfo(0);
            wait = animInfo[0].clip.length * currState.normalizedTime;
            await Task.Delay((int)wait);
        }

        animator.SetTrigger("Damaged");

        currState = animator.GetCurrentAnimatorStateInfo(0);
        wait = animInfo[0].clip.length * currState.normalizedTime;
        await Task.Delay((int)wait);

        CallDeathCheck();
    }


    /// <summary>
    /// Calls the damage function at the appropriate point in the animation
    /// </summary>
    public void CallPlayerDamage()
    {
        GetComponentInParent<MeleeEnemyAttackState>().DealDamage();
    }

    /// <summary>
    /// Used for the melee enemy
    /// </summary>
    public void CallDeathCheck()
    {
        GetComponentInParent<Enemy>()?.CallDie();
        GetComponent<Enemy>()?.CallDie();
    }
}
