using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CrazyMinnow.SALSA;

public class DialogueManager : MonoBehaviour {

    [HideInInspector]
    public static DialogueManager dialogueManagerInstance;

    public Salsa3D[] salsaComponent;

    public Text[] NPC_DialogueText;
    public Text[] playerAnswers;
    public GameObject subtitlesParent;
    public GameObject playerAnswersParent;

    [Header("NPC Traits")]
    [Range(0,100)]
    public int humeur1;

    [Range(0, 100)]
    public int humeur2;

    [Range(0, 100)]
    public int humeur3;

    [Header("Microphone")]
    public GameObject micParent;
    public float microphoneVolume;


    private void Awake()
    {
        dialogueManagerInstance = this;
    }
    void Start()
    {
        playerAnswersParent.SetActive(false);
        subtitlesParent.SetActive(false);
    }



}

[System.Serializable]
public struct DialogueContainer
{
    [TextArea(2, 10)]
    public string textToDisplay;
    public float displayDelay;
}