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
using static UnityEditor.Rendering.InspectorCurveEditor;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] public Animator Anim;
    private string lastState;

    private Dictionary<string, AnimationClip> animClips;


    /// <summary>
    /// Checks if the animator is playing a specific clip
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    private bool CheckCurrentAnimatorState(string state)
    {
        return Anim.GetCurrentAnimatorStateInfo(0).IsName(state);
    }

    /// <summary>
    /// Called when the player takes damage. Waits before the last damage animation finished before calling another
    /// </summary>
    public async void Damage()
    {
        var animInfo = Anim.GetCurrentAnimatorClipInfo(0);
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
            currState = Anim.GetCurrentAnimatorStateInfo(0);
            wait = animInfo[0].clip.length * currState.normalizedTime;
            await Task.Delay((int)wait);
        }

        Anim.SetTrigger("Ouch");
    }

    /// <summary>
    /// Sets the player's state back to their last state
    /// </summary>
    public void ExitDamage()
    {
        if (lastState == "Player_Idle")
        {
            Anim.SetTrigger("Idle1");
        }
        else
        {
            Anim.SetTrigger("Walk1");
        }
    }


    /// <summary>
    /// Starts the Walk state
    /// </summary>
    public async void StartWalk()
    {
        var animInfo = Anim.GetCurrentAnimatorClipInfo(0);
        float wait = 10f;
        AnimatorStateInfo currState;
        if (CheckCurrentAnimatorState("Player_Damaged"))
        {
            currState = Anim.GetCurrentAnimatorStateInfo(0);
            wait = animInfo[0].clip.length * currState.normalizedTime;
            await Task.Delay((int)wait);
        }

        Anim.SetTrigger("Walk1");
    }

    /// <summary>
    /// Starts the Attack state
    /// </summary>
    public void StartAttack()
    {
        Anim.SetTrigger("Attack1");
    }

    /// <summary>
    /// Starts the Idle state
    /// </summary>
    public async void StartIdle()
    {
        var animInfo = Anim.GetCurrentAnimatorClipInfo(0);
        float wait = 10f;
        AnimatorStateInfo currState;
        if (CheckCurrentAnimatorState("Player_Damaged"))
        {
            currState = Anim.GetCurrentAnimatorStateInfo(0);
            wait = animInfo[0].clip.length * currState.normalizedTime;
            await Task.Delay((int)wait);
        }

        Anim.SetTrigger("Idle1");
    }
}
