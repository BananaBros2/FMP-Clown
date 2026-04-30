using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [Tooltip("Animator component reference")]
    private Animator animator;

    /// <summary>
    /// Animation state enumerator used to request animations
    /// </summary>
    public enum TargetAnimation
    {
        IDLE, RUNNING,
        JUMP, FALL, FASTFALL,
        WALLSLIDE, WALLHOOK,
        WHIP,
        BALLRIDE, BALLFALL,
        SWIM,
        DEATH
    }

    [Tooltip("Dictionary for converting animation target into a string readable by animator")]
    Dictionary<TargetAnimation, string> animDict = new Dictionary<TargetAnimation, string>();


    private void Awake()
    {
        animator = transform.GetComponent<Animator>(); // Set animator component
        SetupAnimationDictionary();
    }

    /// <summary>
    /// Setup animation dictionary
    /// </summary>
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

    /// <summary>
    /// Set animation of player
    /// </summary>
    /// <param name="newAnim">Target animation to switch to</param>
    /// <param name="animValue">Integer for animations that require values</param>
    public void ChangeAnimation(TargetAnimation newAnim, int animValue = 0)
    {
        // Switch to new animation
        if (!animator.GetBool(animDict[newAnim])) // Check if already switched, (doesn't check for integers, still seems to work?)
        {
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

            if (newAnim != TargetAnimation.WALLHOOK) // Update animation 
            {
                animator.SetBool(animDict[newAnim], true);
            }
            else // Update wall hook animation
            {
                animator.SetInteger("Do_WallHook", animValue);
            }
                
        }

    }


}
