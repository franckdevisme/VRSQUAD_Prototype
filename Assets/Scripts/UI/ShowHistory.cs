using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowHistory : MonoBehaviour {

    public GameObject historyWindow;

    public void ToggleHistoryVisibility()
    {
        historyWindow.SetActive(!historyWindow.activeSelf);
    }
}
