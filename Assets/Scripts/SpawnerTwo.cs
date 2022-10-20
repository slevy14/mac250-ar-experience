using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SpawnerTwo : MonoBehaviour {
    public ARRaycastManager arRaycastManager; //assigned in inspector
    private List<ARRaycastHit> arRaycastHits = new List<ARRaycastHit>();
    public GameObject cubePrefab; //assigned in inspector
    void Update() {
        if(Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) {
            if (arRaycastManager.Raycast(Input.GetTouch(0).position, arRaycastHits)) {
                var pose = arRaycastHits[0].pose;
                CreateCube(pose.position);
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                if (hit.collider.tag == "cube") {
                    DeleteCube(hit.collider.gameObject);
                }
            }
        }
    }

    private void CreateCube(Vector3 position) {
        Instantiate(cubePrefab, position, Quaternion.identity);
    }

    private void DeleteCube(GameObject cubeObject) {
        Destroy(cubeObject);
    }
}
