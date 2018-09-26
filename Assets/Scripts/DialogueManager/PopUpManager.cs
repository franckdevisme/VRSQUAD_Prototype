using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpManager : MonoBehaviour {

    public GameObject popUpParent;
    public Text popUpText;
    public GameObject popUpButton;
    
	// Use this for initialization
	void Start () {

      //  string newName = this?.myName;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangePopUp(string newtext, float duration)
    {
        PopUpButtonVisibility(true);
        popUpText.text = newtext;
        StartCoroutine(HidePopUPAfterDelay(duration));
    }

    public void TogglePopUp()
    {
        popUpParent.SetActive(!popUpParent.activeSelf);
    }

    public void PopUpButtonVisibility(bool b)
    {
        popUpButton.SetActive(b);
    }

    IEnumerator HidePopUPAfterDelay(float duration)
    {
        yield return new WaitForSeconds(duration);
        popUpButton.SetActive(false);
        popUpParent.SetActive(false);
    }
       
}
