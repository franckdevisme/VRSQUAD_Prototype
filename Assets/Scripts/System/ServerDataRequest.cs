using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class ServerDataRequest : MonoBehaviour
{
    public ProfileSelection profileSelec;
    public UserProfile profile;

    // http://192.168.43.254:8000/login?

    public string url;

    // Contenu des fields de la page de login

    private KeyboardInput keyboardScript;

    private void Awake()
    {
        keyboardScript = GetComponent<KeyboardInput>();
    }

    public void LoginValidate()
    {
        string fullUrl = url + "/" + keyboardScript.inputFields[0].text + "/" + keyboardScript.inputFields[1].text;
        Debug.Log(fullUrl);

        StartCoroutine(GetRequest(fullUrl));
    }


    IEnumerator GetRequest(string uri)
    {

        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);

            profile.name = uwr.GetResponseHeader("nom");
            profile.surname = uwr.GetResponseHeader("prénom");
            profile.username = uwr.GetResponseHeader("username");
        }

        UnityWebRequest uwr_img = UnityWebRequestTexture.GetTexture(uri);
        yield return uwr_img.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            // Get downloaded asset bundle
            profile.profileImg = DownloadHandlerTexture.GetContent(uwr);
        }

        profileSelec.UpdateProfileInfo(profile);

        gameObject.SetActive(false);
    }

    [System.Serializable]
    public struct UserProfile
    {
        public string name;
        public string surname;
        public string username;
        public Texture2D profileImg;
    }
}
