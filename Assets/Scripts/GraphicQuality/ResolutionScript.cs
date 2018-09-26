using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ResolutionScript : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        XRSettings.eyeTextureResolutionScale = 2.0f;
    }
}
