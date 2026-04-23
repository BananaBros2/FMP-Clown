using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private Animator animator;

    public enum TargetAnimation
    {
        IDLE,
        RUNNING,
        JUMP,
        FALL,
        FASTFALL,
        WALLSLIDE,
        WALLHOOK,
        WHIP,
        BALLRIDE,
        BALLFALL,
        SWIM,
        DEATH
    }

    Dictionary<TargetAnimation, string> animDict = new Dictionary<TargetAnimation, string>();

    private bool switchAnimation = false;



    private void Awake()
    {
        animator = transform.GetComponent<Animator>(); // Set animator component

        SetupAnimationDictionary();

    }


    private void SetupAnimationDictionary()
    {
        animDict.Add(TargetAnimation.IDLE, "Do_Idle");
        animDict.Add(TargetAnimation.RUNNING, "Do_Run");
        animDict.Add(TargetAnimation.JUMP, "Do_Jump");
        animDict.Add(TargetAnimation.FALL, "Do_Fall");
        animDict.Add(TargetAnimation.FASTFALL, "Do_FastFall");
        animDict.Add(TargetAnimation.WALLSLIDE, "Do_WallSlide");
        animDict.Add(TargetAnimation.WALLHOOK, "Do_WallHook");
        animDict.Add(TargetAnimation.WHIP, "Do_Whip");
        animDict.Add(TargetAnimation.BALLRIDE, "Do_BallRide");
        animDict.Add(TargetAnimation.SWIM, "Do_Swim");
        animDict.Add(TargetAnimation.DEATH, "Do_Death");
    }


    public void ChangeAnimation(TargetAnimation newAnim, int animValue = 0)
    {
        switchAnimation = true;

        // Switch to new animation (if not done already)
        if (!animator.GetBool(animDict[newAnim]))
        {
            //print("Chang9ing animation");
            animator.SetBool("Do_Idle", false);
            animator.SetBool("Do_Run", false);
            animator.SetBool("Do_Jump", false);
            animator.SetBool("Do_Fall", false);
            //animator.SetBool("Do_FastFall", false);
            animator.SetBool("Do_WallSlide", false);
            animator.SetInteger("Do_WallHook", 0);
            animator.SetBool("Do_Whip", false);
            animator.SetBool("Do_BallRide", false);
            animator.SetBool("Do_Swim", false);
            animator.SetBool("Do_Death", false);

            if (newAnim != TargetAnimation.WALLHOOK)
            {
                animator.SetBool(animDict[newAnim], true);
            }
            else
            {
                animator.SetInteger("Do_WallHook", animValue);
            }
                
        }

    }


}
