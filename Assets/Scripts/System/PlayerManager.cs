using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour {

    public static PlayerManager playerManagerInst;

    public OVRPlayerController playerController;

    public Image blackScreen;
    public Slider loadingSlider;
    public float fadeSpeed = 0.05f;
    public float fadeStrength = 0.025f;
    private bool isSceneLoading = false;

    AsyncOperation asyncLoadLevel;


    // Use this for initialization
    void Awake ()
    {

        loadingSlider.gameObject.SetActive(false);

        if (playerManagerInst == null)
        {
            playerManagerInst = this; // In first scene, make us the singleton.
            DontDestroyOnLoad(gameObject);
        }
        else if (playerManagerInst != this)
            Destroy(gameObject); // On reload, singleton already set, so destroy duplicate.

        playerController = GetComponentInChildren<OVRPlayerController>();
    }

    public void SceneChange(int index)
    {
        if (!isSceneLoading)
        {
            isSceneLoading = true;
            StartCoroutine(FadeToBlack(index));
        }
    }

    IEnumerator FadeToBlack(int index)
    {
        while (blackScreen.color.a < 1)
        {
            var tempcolor = blackScreen.color;
            tempcolor.a += fadeStrength;
            blackScreen.color = tempcolor;
            yield return new WaitForSeconds(fadeSpeed);
        }


        loadingSlider.gameObject.SetActive(true);
        

        // On attends que la scène se load
        asyncLoadLevel = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
        while (!asyncLoadLevel.isDone)
        {
            // Update de la barre de chargement
           loadingSlider.value = asyncLoadLevel.progress;
           yield return null;
        }


        /*
        // Si PlayerSpawnPoint existe, TP à sa position, sinon TP en 0,0,0
        playerController.gameObject.transform.position = GameObject.Find("PlayerSpawnPoint") ? GameObject.Find("PlayerSpawnPoint").transform.position : Vector3.zero;

        if (GameObject.Find("PlayerSpawnPoint"))
            Debug.Log(GameObject.Find("PlayerSpawnPoint").transform.position);

         */
        while (blackScreen.color.a > 0)
        {
            var tempcolor = blackScreen.color;
            tempcolor.a -= fadeStrength;
            blackScreen.color = tempcolor;
            yield return new WaitForSeconds(fadeSpeed);
        }
        isSceneLoading = false;
        loadingSlider.gameObject.SetActive(false);
        yield break;
    }

}
