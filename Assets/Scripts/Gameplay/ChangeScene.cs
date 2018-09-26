using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeScene : MonoBehaviour {


    public Text fpsCount;
    public Image blackScreen;
    public float fadeSpeed = 0.05f;
    public float fadeStrength = 0.025f;

    private bool isSceneLoading = false;
    private OVRPlayerController playerController;

    public GameObject UIPanel;

    public bool showMenuOnStart;

    public OVRPlayerController player;
    public Transform[] waypoints;

    private bool firstTap = false; // Truewhen the player taps for the first time;


    private float tapTimer;

    AsyncOperation asyncLoadLevel;

    // Use this for initialization
    void Awake ()
    {
        DontDestroyOnLoad(gameObject);
        playerController = GetComponentInChildren<OVRPlayerController>();


        UIPanel.SetActive(showMenuOnStart);

        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        StartCoroutine(DisplayFPSeverySecond());
    }


    void Update()
    {
        if(!UIPanel.activeSelf)
        {
           
            UIPanel.transform.position = playerController.transform.position + playerController.transform.forward * 1f;           
        }

        UIPanel.transform.LookAt(new Vector3(playerController.transform.position.x, UIPanel.transform.position.y,playerController.transform.position.z));
        UIPanel.transform.Rotate(0, 180, 0);


        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || Input.GetKeyDown(KeyCode.M))
        // if (Input.GetMouseButtonUp(0) && !firstTap)
        {
          //  StartCoroutine(DoubleTap());
            UIPanel.SetActive(!UIPanel.activeSelf);
        }
    }

    private IEnumerator DoubleTap()
    {
        yield return new WaitForEndOfFrame();
        firstTap = true;
        while(tapTimer < 0.2f)
        {
            if(Input.GetMouseButtonUp(0))
            {
                Debug.Log("doubletap");

                UIPanel.SetActive(!UIPanel.activeSelf);
                break;
            }
            tapTimer += Time.deltaTime;
            yield return null;
        }
        firstTap = false;
        tapTimer = 0.0f;
    }

    public void LaunchCutscene(int index)
    {
        if (!isSceneLoading)
        {
            Debug.Log("Loading Next Scene");
            isSceneLoading = true;
            StartCoroutine(FadeToBlack(index));
        }
    }

    IEnumerator FadeToBlack(int index)
    {        
        while(blackScreen.color.a < 1)
        {
            var tempcolor = blackScreen.color;
            tempcolor.a += fadeStrength;
            blackScreen.color = tempcolor;
            yield return new WaitForSeconds(fadeSpeed);
        }

        // On attends que la scène se load
        asyncLoadLevel = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }

        // Si PlayerSpawnPoint existe, TP à sa position, sinon TP en 0,0,0
        playerController.gameObject.transform.position = GameObject.Find("PlayerSpawnPoint") ? GameObject.Find("PlayerSpawnPoint").transform.position : Vector3.zero;

        if (GameObject.Find("PlayerSpawnPoint"))
            Debug.Log(GameObject.Find("PlayerSpawnPoint").transform.position);

        while (blackScreen.color.a > 0)
        {
            var tempcolor = blackScreen.color;
            tempcolor.a -= fadeStrength;
            blackScreen.color = tempcolor;
            yield return new WaitForSeconds(fadeSpeed);
        }

        UIPanel.SetActive(false);
        isSceneLoading = false;
        yield break;
    }

    IEnumerator DisplayFPSeverySecond()
    {
        while(true)
        {
            float fps = 1.0f / Time.deltaTime;
            fps = Mathf.Round(fps);            
            fpsCount.text = "FPS : " + fps.ToString();
            yield return new WaitForSeconds(1f);
        }      
    }

    public void TeleportToWaypoint(int index)
    {
        player.transform.position = waypoints[index].position;
    }
}
