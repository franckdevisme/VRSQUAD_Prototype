using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileData : MonoBehaviour {

    public string fullName;
    public string job;
    public string company;

    public Image profileImage;

    public ProfileSelection profileSelec;
/*
    public void ShowProfile()
    {
        profileSelec.UpdateProfileInfo(fullName, job, company, profileImage.sprite, gameObject);
    }
    */
}
