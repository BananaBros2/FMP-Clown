using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
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
        DEATH
    }

    private TargetAnimation targetAnimation;

    private Animator animator;

    private bool switchAnimation = false;


    Dictionary<TargetAnimation, string> animDict = new Dictionary<TargetAnimation, string>();



    private void Awake()
    {
        animator = transform.GetComponent<Animator>();

        animDict.Add(TargetAnimation.IDLE, "Do_Idle");
        animDict.Add(TargetAnimation.RUNNING, "Do_Run");
        animDict.Add(TargetAnimation.JUMP, "Do_Jump");
        animDict.Add(TargetAnimation.FALL, "Do_Fall");
        animDict.Add(TargetAnimation.WALLSLIDE, "Do_WallSlide");
        animDict.Add(TargetAnimation.WALLHOOK, "Do_WallHook");
        animDict.Add(TargetAnimation.WHIP, "Do_Whip");
        animDict.Add(TargetAnimation.BALLRIDE, "Do_BallRide");
        animDict.Add(TargetAnimation.DEATH, "Do_Death");


    }


    private void Update()
    {
        
    }

    public void ChangeAnimation(TargetAnimation newAnim)
    {
        switchAnimation = true;

        if (!animator.GetBool(animDict[newAnim]))
        {
            animator.SetBool("Do_Idle", false);
            animator.SetBool("Do_Run", false);
            animator.SetBool("Do_Jump", false);
            animator.SetBool("Do_Fall", false);
            animator.SetBool("Do_WallSlide", false);
            animator.SetBool("Do_WallHook", false);
            animator.SetBool("Do_Whip", false);
            animator.SetBool("Do_BallRide", false);
            //animator.SetBool("Do_Death", false);

            animator.SetBool(animDict[newAnim], true);
        }





    }
}
