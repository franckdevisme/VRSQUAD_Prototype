using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardInput : MonoBehaviour {

    public InputField[] inputFields;
    private int currentSelectedField;

    public GameObject keyboardParent;

    private bool isMaj = true;

    private void Awake()
    {
        currentSelectedField = 0;
        SwitchUpperLowerCase();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SwitchUpperLowerCase();
        }
    }

    public void AddCharacter(string character)
    {
        string chara;

        if (isMaj)
           chara = character.ToUpper();
        else
            chara = character.ToLower();

        inputFields[currentSelectedField].text += chara;
    }

    public void RemoveLastCharacter()
    {
        if(inputFields[currentSelectedField].text.Length > 0)
            inputFields[currentSelectedField].text = inputFields[currentSelectedField].text.Substring(0, inputFields[currentSelectedField].text.Length - 1);
    }

    public void ResetField()
    {
        inputFields[currentSelectedField].text = "";
    }

    public void SelectInputField(int i)
    {
        currentSelectedField = i;
    }

    public void SwitchUpperLowerCase()
    {
        foreach (Transform child in keyboardParent.transform)
        {
            // Si le clavier est en majuscules, passer en minuscules
            if(isMaj)
            {
                if (child.GetComponent<Button>() != null)
                {
                    Text buttonText = child.GetChild(0).GetComponent<Text>();

                    if(buttonText.text.Length == 1)
                       buttonText.text = buttonText.text.ToLower();
                }
            }
            // Si le clavier est en minuscules, passer en majuscules
            else
            {
                if (child.GetComponent<Button>() != null)
                {
                    Text buttonText = child.GetChild(0).GetComponent<Text>();

                    if (buttonText.text.Length == 1)
                        buttonText.text = buttonText.text.ToUpper();
                }
            }        
        }
        isMaj = !isMaj;
    }

    public void Confirm()
    {
        // Lancer la formation en outrepassant la requête serveur
        if(inputFields[0].text == "admin" && inputFields[1].text == "admin")
        {
            gameObject.SetActive(false);
        }
        else
        {
            GetComponent<ServerDataRequest>().LoginValidate();
        }
    }
}
