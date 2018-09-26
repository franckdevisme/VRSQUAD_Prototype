using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrazyMinnow.SALSA;

public class AttractGaze : StateMachineBehaviour {

    public int[] affectedNpcID;
    public int targetNpcID;

    private GameObject targetPos;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {


        targetPos = GazeAttractor.gazeAttractInst.headLookArray[targetNpcID].GetComponent<RandomEyes3D>().eyePosition;

        foreach (int i in affectedNpcID)
        {
            GazeAttractor.gazeAttractInst.NewGazeTarget(GazeAttractor.gazeAttractInst.headLookArray[i], targetPos);
        }     
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (int i in affectedNpcID)
        {
            GazeAttractor.gazeAttractInst.NewGazeTarget(GazeAttractor.gazeAttractInst.headLookArray[i], GazeAttractor.gazeAttractInst.gameObject);
        }

    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
