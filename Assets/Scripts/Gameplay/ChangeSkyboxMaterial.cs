using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSkyboxMaterial : MonoBehaviour {

    private MeshRenderer myMeshRend;
    public bool autoSwitch;

    public Text textIndex;

   
    public Texture[] skyboxArray;

    private int skyboxIndex = -1;

	// Use this for initialization
	void Start () {

        myMeshRend = GetComponent<MeshRenderer>();
       
        if(autoSwitch)
             InvokeRepeating("ChangeSkyboxCubemap", 5f, 5f);

    }
	
    public void ChangeSkyboxCubemap()
    {
       
        skyboxIndex++;
        if (skyboxIndex > skyboxArray.Length)
        {
            skyboxIndex = 0;
           
        }
        myMeshRend.material.SetTexture("_Tex", skyboxArray[skyboxIndex]);
        myMeshRend.material.SetFloat("_Exposure", 1f);
        Debug.Log("ChangingSkybox : " + skyboxIndex);
        textIndex.text = "" + skyboxIndex;
    }
}
