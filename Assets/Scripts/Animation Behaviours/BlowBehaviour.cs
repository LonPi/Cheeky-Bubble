using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlowBehaviour : StateMachineBehaviour
{
    // for debugging purpose
    bool enteredUpdateLoop;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (BlowBubble.Instance.animatorChanged)
        {
            BlowBubble.Instance.ReleaseBubble();
            BlowBubble.Instance.animatorChanged = false;
        }
        BlowBubble.Instance.IsInBlowState = true;
        BlowBubble.Instance.InstantiateBubble();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enteredUpdateLoop = true;
        BlowBubble.Instance.ChangeBubbleSize();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // for debugging purpose
        if (enteredUpdateLoop == false)
        {
            float val = BlowBubble.Instance.instantiatedBubble.transform.localScale.x;
            Debug.Log("something wrong happened, bubble released prematurely... scale: " + val);
        }
        
        BlowBubble.Instance.IsInBlowState = false;
        BlowBubble.Instance.ReleaseBubble();
        
        // for debugging
        enteredUpdateLoop = false;
    }
}
