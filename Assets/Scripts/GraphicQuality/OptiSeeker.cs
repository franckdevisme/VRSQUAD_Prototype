using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptiSeeker : MonoBehaviour {

	// Use this for initialization
	void Start () {
        DisplayBadMeshes();
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    void DisplayBadMeshes()
    {
        MeshFilter[] meshArray = GetComponentsInChildren<MeshFilter>();
        foreach(MeshFilter mesh in meshArray)
        {
            if(mesh.sharedMesh.vertexCount > 8000)
            {
                Debug.Log(mesh.sharedMesh.vertexCount);
                mesh.transform.GetComponent<Renderer>().material.color = Color.red;
            }
        }
    }
}
