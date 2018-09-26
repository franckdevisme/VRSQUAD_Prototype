using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CrazyMinnow.SALSA;

// TODO : CLEAN CE CODE FAIT A L'ARRACHE UN SOIR A 21H
public class DialogueExec : StateMachineBehaviour {

    public int npc_Index;
    
    public AudioClip audioclip;
    public DialogueContainer[] dialogue;
    private Text npcDialogueText;
    private Salsa3D salsaCompo;
    private AudioSource audiosource;
    private DialogueManager npcDialogueManager;

    private int dialogueIndex;
    private float dialogueDisplayTime;

    private float audioClipLength;
    private float timeSinceStateStart;

    public float pauseDelay;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Stockage de la durée du clip
        audioClipLength = audioclip.length + pauseDelay;



        dialogueIndex = 0;
        dialogueDisplayTime = 0;

        npcDialogueManager = animator.gameObject.GetComponent<DialogueManager>();
        npcDialogueManager.subtitlesParent.SetActive(true);
       
        for(int i = 0; i < npcDialogueManager.NPC_DialogueText.Length; i++)
        {
            npcDialogueManager.NPC_DialogueText[i].transform.parent.gameObject.SetActive(false);
        }

        npcDialogueText = npcDialogueManager.NPC_DialogueText[npc_Index];
        npcDialogueText.gameObject.transform.parent.gameObject.SetActive(true);
       

        salsaCompo = npcDialogueManager.salsaComponent[npc_Index];
        audiosource = salsaCompo.GetComponent<AudioSource>();

        salsaCompo.audioClip = audioclip;
        audiosource.clip = audioclip;

        salsaCompo.Play();
        audiosource.Play();
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

       


        dialogueDisplayTime += Time.deltaTime;
        if(dialogueIndex < dialogue.Length)
        {
            npcDialogueText.text = dialogue[dialogueIndex].textToDisplay;

            if (dialogueDisplayTime >= dialogue[dialogueIndex].displayDelay)
            {
                dialogueDisplayTime = 0;
                dialogueIndex++;
            }
        }


        timeSinceStateStart += Time.deltaTime;
        if(timeSinceStateStart >= audioClipLength)
        {
            animator.SetTrigger("NextDialogue");
            timeSinceStateStart = 0;
        }


    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      //  npcDialogueText.gameObject.transform.parent.gameObject.SetActive(false);
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
