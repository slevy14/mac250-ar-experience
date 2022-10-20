using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoBoxButtons : MonoBehaviour {
    private GameObject map;
    // public SpawnerInRange spawnerScript;
    public MainController mainScript;

    //not in inspector
    private float targetLat = 0;
    private float targetLon = 0;

    private bool latSet = false;
    private bool lonSet = false;

    void Awake() {
        map = GameObject.FindGameObjectWithTag("mapObjects");
        // spawnerScript = GameObject.FindGameObjectWithTag("objectSpawner").GetComponent<SpawnerInRange>();
        mainScript = GameObject.FindGameObjectWithTag("mainController").GetComponent<MainController>();
    }

    public void SetLocationData(ARLocation.LocationData newLocation) {
        mainScript.targetLocation = newLocation;
    }

    public void HideInfoBox() {
        Destroy(GameObject.FindGameObjectWithTag("infobox"));
    }

    public void ForceOutOfRange() {
        mainScript.SetOutOfRange();
    }

    public void HideMapAndInfoBox() {
        if (map.activeSelf) {
            map.SetActive(false);
        }
        HideInfoBox();
    }
}
