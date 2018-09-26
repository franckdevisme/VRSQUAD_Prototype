using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrazyMinnow.SALSA;

public class GazeAttractor : MonoBehaviour {
   
    public static GazeAttractor gazeAttractInst;

    // Array of all gazing npc
    public HeadLookController[] headLookArray;
    private RandomEyes3D randomEyesComp;

    private void Awake()
    {
        gazeAttractInst = this;
    }
    void Start()
    {
        foreach (HeadLookController headlook in headLookArray)
        {
            NewGazeTarget(headlook, gameObject);
        }
    }

    void Update ()
    {	
       Debug.DrawLine(transform.position, randomEyesComp.eyePosition.transform.position, Color.red);
	}

    public void NewGazeTarget(HeadLookController headlook, GameObject newTarget)
    {
        headlook.target = newTarget.transform.position;
        randomEyesComp = headlook.gameObject.GetComponent<RandomEyes3D>();
        randomEyesComp.lookTarget = newTarget;
        randomEyesComp.lookTargetTracking = newTarget;
    }
}
