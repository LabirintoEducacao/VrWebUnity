using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAnimationParameter : StateMachineBehaviour
{
    public string parameterBoolToSet;
    public bool boolValue;
    public AudioClip playOnEnd;
    public AudioClip playOnStart;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playOnStart != null) {
            AudioSource source = animator.GetComponent<AudioSource>();
            if (source != null) {
                source.clip = playOnStart;
                animator.GetComponent<AudioSource>().Play();
            }
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (System.String.IsNullOrEmpty(parameterBoolToSet)) {
            return;
        }
        animator.SetBool(parameterBoolToSet, boolValue);
        
        if (playOnEnd != null) {
            AudioSource source = animator.GetComponent<AudioSource>();
            if (source != null) {
                source.clip = playOnEnd;
                animator.GetComponent<AudioSource>().Play();
            }
        }
    }
}
