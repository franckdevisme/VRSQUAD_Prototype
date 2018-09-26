using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Billboard : MonoBehaviour {

    public OVRPlayerController playerController;

	// Use this for initialization
	void Start () {


    }
	
	// Update is called once per frame
	void Update ()
    {
     //   gameObject.transform.position = playerController.transform.position + playerController.transform.forward * 1f;


        gameObject.transform.LookAt(new Vector3(playerController.transform.position.x, gameObject.transform.position.y, playerController.transform.position.z));
        gameObject.transform.Rotate(0, 180, 0);
    }
}
