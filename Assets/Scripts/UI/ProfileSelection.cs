using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileSelection : MonoBehaviour {

    public Text nameText;
    public Text jobText;
    public Text companyText;

    public Image profileImage;

    public GameObject buttonClicked;

    public void UpdateProfileInfo(ServerDataRequest.UserProfile profile)
    {
        nameText.text = profile.surname + " " + profile.name;
        jobText.text = profile.username;
        companyText.text = "VR SQUAD";

        profileImage.sprite = Sprite.Create(profile.profileImg, new Rect(0f,0f, profile.profileImg.width, profile.profileImg.height), Vector2.zero);
    }
}
