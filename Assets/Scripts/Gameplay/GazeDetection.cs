using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class GazeDetection : MonoBehaviour
{

    public static GazeDetection gazeDetectInst;

    public float currentGazePercentage;
    
    public int successfulRaysNumber;
    public int missedRaysNumber = 1;

    private int layer_mask;


    private Camera myCamera;

    private byte[] cameraScreenshot;
    public int screenshotResolution;

    public GameObject[] gazeTracers;
    private int tracerLayer;

    void Awake()
    {
        layer_mask = LayerMask.GetMask("GazeDetection");

        myCamera = GetComponent<Camera>();

        gazeDetectInst = this;

        
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(transform.forward), out hit, 10, layer_mask))
        {
         //   Debug.Log("Did Hit : " + hit.collider.name);
            successfulRaysNumber++;
        }
        else
        {
          //  Debug.Log("Did not Hit");
            missedRaysNumber++;
        }

        currentGazePercentage = ((float) successfulRaysNumber) / (((float) missedRaysNumber) + ((float) successfulRaysNumber));
        currentGazePercentage *= 100;
        currentGazePercentage = Mathf.Round(currentGazePercentage);

        if(Input.GetKey(KeyCode.L))
        {
            TakeScreenshot();
            Debug.Log("cameraScreenshot taken");
        }
    
    }

   
    public void TakeScreenshot()
    {
        tracerLayer = gazeTracers[0].layer;
        foreach (GameObject obj in gazeTracers)
        {           
            // Change the layer of the Tracer to Default so it can be visible by the camera;
            obj.layer = 0;
        }

        
        Debug.Log("Gaze Screenshot taken !");
        cameraScreenshot = I360Render.Capture(screenshotResolution, false, myCamera);

      
        string path = Path.Combine(Application.persistentDataPath, "ScreenGaze.png");
        
        File.WriteAllBytes(path, cameraScreenshot);

        StartCoroutine(RevertTracerLayer());
    }

    IEnumerator RevertTracerLayer()
    {
        yield return new WaitForEndOfFrame();

        foreach (GameObject obj in gazeTracers)
        {
            // Change back the layer of the Tracer to be invisible
            obj.layer = tracerLayer;
            Debug.Log(obj.layer);
        }
    }
}
