using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSkybox : MonoBehaviour {

    private Material skyboxMaterial;

    public Texture newTexture = null;

    private void Awake()
    {

        skyboxMaterial = GetComponent<MeshRenderer>().material;

        newTexture = Resources.Load("texturetoload", typeof(Texture)) as Texture;
        Debug.Log(newTexture);
    }
    void Start ()
    {
        if(newTexture != null)
        {
            skyboxMaterial.SetTexture("_Tex", newTexture);
            Debug.Log("Loaded");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            skyboxMaterial.SetTexture("_Tex", newTexture);
            Debug.Log(newTexture);
        }
    }
}
