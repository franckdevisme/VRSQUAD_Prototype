using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MicrophoneUI : MonoBehaviour {

    public int recordTime;

    public Slider recordSlider;
    public Image micImage;
    public Text micText;

    private float timeElapsedRecording;

    public bool isRecording;

	// Use this for initialization
	void Start ()
    {
        gameObject.SetActive(false);		
	}
	
	// Update is called once per frame
	void Update ()
    {     
        if(isRecording)
        {
            // Update the slider value
            timeElapsedRecording += Time.deltaTime;
            recordSlider.value = timeElapsedRecording;
            // When recording, ping pong the mic img color value
            micImage.color = new Color(Mathf.PingPong(timeElapsedRecording * 0.4f, 0.5f) + 0.5f, 1, Mathf.PingPong(timeElapsedRecording * 0.4f, 0.5f) + 0.5f);
            micText.color = new Color(Mathf.PingPong(timeElapsedRecording * 0.4f, 0.5f) + 0.5f, 1, Mathf.PingPong(timeElapsedRecording * 0.4f, 0.5f) + 0.5f);

            if (timeElapsedRecording >= recordTime)
            {
                EndRecord();
            }
        }

        
	}

    public void StartRecord(int duration)
    {
        recordTime = duration;
        recordSlider.value = 0f;
        isRecording = true;
        recordSlider.maxValue = recordTime;
    }

    public void EndRecord()
    {
        isRecording = false;
        timeElapsedRecording = 0f;
        micImage.color = Color.white;
        micText.color = Color.white;
    }


}
