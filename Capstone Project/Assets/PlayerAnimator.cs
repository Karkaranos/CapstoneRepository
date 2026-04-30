/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    4/30/2026
Date Last Modified : 	4/30/2026
Brief Description : 	Controls the player's Animation state
External Resources : 	N/A
***************************************************/
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator anim;
    private string lastState;

    private Dictionary<string, AnimationClip> animClips;

    private void Start()
    {
        anim = GetComponent<Animator>();
        var clips = anim.GetCurrentAnimatorClipInfo(0);
        foreach (var clip in clips)
        {
            Debug.Log(clip.clip.name);
        }

    }

    /// <summary>
    /// Checks if the animator is playing a specific clip
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    private bool CheckCurrentAnimatorState(string state)
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName(state);
    }

    /// <summary>
    /// Called when the player takes damage. Waits before the last damage animation finished before calling another
    /// </summary>
    public async void Damage()
    {
        var animInfo = anim.GetCurrentAnimatorClipInfo(0);
        float wait = 10f;
        AnimatorStateInfo currState;
        if(!CheckCurrentAnimatorState("Player_Damaged"))
        {
            if (CheckCurrentAnimatorState(animInfo[0].clip.name))
            {
                lastState = animInfo[0].clip.name;
                Debug.Log(lastState);
            }
        }
        else
        {
            currState = anim.GetCurrentAnimatorStateInfo(0);
            wait = animInfo[0].clip.length * currState.normalizedTime;
            await Task.Delay((int)wait);
        }

        anim.SetTrigger("Ouch");
    }

    /// <summary>
    /// Sets the player's state back to their last state
    /// </summary>
    public void ExitDamage()
    {
        if (lastState == "Player_Idle")
        {
            anim.SetTrigger("Idle1");
        }
        else
        {
            anim.SetTrigger("Walk1");
        }
    }

    public void StartWalk()
    {

    }

    public void StartAttack()
    {

    }

    public void StartIdle()
    {
        anim.SetTrigger("Idle");
    }
}
