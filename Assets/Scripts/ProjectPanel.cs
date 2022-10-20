using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProjectPanel : MonoBehaviour {

    public Scenes sceneScript;

    public void Awake() {
        sceneScript = GameObject.FindGameObjectWithTag("sceneManager").GetComponent<Scenes>();
    }

    public void HideInfoBox() {
        Destroy(GameObject.FindGameObjectWithTag("infobox"));
    }

    public void LoadPTP() {
        sceneScript.ToPTP();
    }

    public void LoadOIF() {
        sceneScript.ToOIF();
    }

    public void LoadTH() {
        sceneScript.ToTH();
    }

}
