using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {

    private PlayerManager playerManagerInstance;

    // Use this for initialization
    void Start ()
    {
        playerManagerInstance = PlayerManager.playerManagerInst;
	}

    public void ChangeScene(int index)
    {
        playerManagerInstance.SceneChange(index);
    }
}
