using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

    public GameObject infoText;
    public GameObject projectSelect;

    void Awake() {
        infoText.SetActive(false);
    }

    public void ShowInfoText() {
        if (!infoText.activeSelf) {
            infoText.SetActive(true);
        }
    }

    public void HideInfoText() {
        if (infoText.activeSelf) {
            infoText.SetActive(false);
        }
    }

    public void ShowProjectSelect() {
        if (!projectSelect.activeSelf) {
            projectSelect.SetActive(true);
        }
    }

    public void HideProjectSelect() {
        if (projectSelect.activeSelf) {
            projectSelect.SetActive(false);
        }
    }

    public void ShowInfoBox(GameObject infoBox) {
        Instantiate(infoBox, GameObject.Find("Canvas").transform);
    }

}
