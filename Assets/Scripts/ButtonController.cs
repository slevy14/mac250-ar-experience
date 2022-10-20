using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour {

    //set in inspector
    [SerializeField] private GameObject map;
    [SerializeField] private GameObject workableArea;
    [SerializeField] private GameObject mapButton;
    [SerializeField] private GameObject arrow;
    [SerializeField] private GameObject distanceText;
    [SerializeField] private GameObject distanceBackground;
    public SpawnerInRange spawnerScript;
    public MainController mainScript;

    //not in inspector
    private GameObject closeMapButton;
    private float targetLat = 0;
    private float targetLon = 0;

    private bool latSet = false;
    private bool lonSet = false;

    void Awake() {
        workableArea.SetActive(false);
        closeMapButton = GameObject.Find("CloseMap");
        closeMapButton.SetActive(false);
        HideMainUI();
    }
    
    public void ShowMap() {
        if (!map.activeSelf) {
            map.SetActive(true);
            HideMainUI();
            if (!closeMapButton.activeSelf) {
                closeMapButton.SetActive(true);
            }
        }
    }

    public void HideMap() {
        if (map.activeSelf) {
            map.SetActive(false);
            ShowMainUI();
        }
    }

    public void HideMainUI() {
        mapButton.GetComponent<Renderer>().enabled = false;
        arrow.GetComponent<Renderer>().enabled = false;
        distanceText.GetComponent<Renderer>().enabled = false;
        distanceBackground.GetComponent<Renderer>().enabled = false;
    }

    public void ShowMainUI() {
        mapButton.GetComponent<Renderer>().enabled = true;
        arrow.GetComponent<Renderer>().enabled = true;
        distanceText.GetComponent<Renderer>().enabled = true;
        distanceBackground.GetComponent<Renderer>().enabled = true;
    }

    public void SetLat(float lat) {
        targetLat = lat;
        latSet = true;
    }

    public void SetLon(float lon) {
        targetLon = lon;
        lonSet = true;
    }

    public void SetLocationData(ARLocation.LocationData newLocation) {
        mainScript.targetLocation = newLocation;
    }

    public void SetObjectToSpawn(GameObject toSpawn) {
        spawnerScript.SetObject(toSpawn);
    }

    public void UpdateCoords() {
        if (latSet && lonSet) {
            spawnerScript.SetTargetCoords(targetLat, targetLon);
            latSet = false;
            lonSet = false;
        }
    }

    public void ShowInfoBox(GameObject infoBox) {
        Instantiate(infoBox, GameObject.Find("Canvas").transform);
    }

    public void BackToMainMenu() {
        SceneManager.LoadSceneAsync(0);
    }
}
