using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeRoom : MonoBehaviour
{

    public static ChangeRoom changeRoomInst;
    public GameObject toMove;

    public MeshRenderer[] skyboxPositionsList;

    private Image blackScreen;
    private DialogueManager dialogManag;

    // Use this for initialization
    void Awake()
    {
        dialogManag = DialogueManager.dialogueManagerInstance;

        changeRoomInst = this;

        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }


        skyboxPositionsList = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer skyB in skyboxPositionsList)
        {
            skyB.gameObject.SetActive(false);
        }
        skyboxPositionsList[0].gameObject.SetActive(true);

        blackScreen = PlayerManager.playerManagerInst.blackScreen;

    }

    public void GoToRoom(int i)
    {
        StartCoroutine(FadeInOut(i));
    }


   IEnumerator FadeInOut(int i)
    {
        Color tempcolor;

        // Fade In
        while (blackScreen.color.a < 1)
        {
            tempcolor = blackScreen.color;
            tempcolor.a += 0.025f;
            blackScreen.color = tempcolor;
            yield return new WaitForSeconds(0.025f);
        }

        // Téléportation
        foreach (MeshRenderer skyB in skyboxPositionsList)
        {
            skyB.gameObject.SetActive(false);
        }

        // Téléportation du NPC Teacher
        toMove = GameObject.Find("NPC_Teacher");
        Vector3 temp = skyboxPositionsList[i].transform.position;
        temp.x += 0.1f;
        temp.y += -0.85f;
        temp.z += 1.4f;
        toMove.transform.position = temp;

        //Téléportation du NPCGazeAttractor
        toMove = GameObject.Find("NPCGazeAttractor");
        temp = skyboxPositionsList[i].transform.position;
        toMove.transform.position = temp;

        //Téléportation de l'apprenant
        skyboxPositionsList[i].gameObject.SetActive(true);
        PlayerManager.playerManagerInst.playerController.transform.position = skyboxPositionsList[i].transform.position;
        dialogManag.transform.position = skyboxPositionsList[i].transform.position;

        // Fade Out
        while (blackScreen.color.a > 0)
        {
            tempcolor = blackScreen.color;
            tempcolor.a -= 0.025f;
            blackScreen.color = tempcolor;
            yield return new WaitForSeconds(0.025f);
        }

       
        yield break;
    }
}
