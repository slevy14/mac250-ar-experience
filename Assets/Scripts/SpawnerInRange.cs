using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;

public class SpawnerInRange : MonoBehaviour {

    //set in inspector
    public float targetLatitude;
    public float targetLongitude;

    public float rangeToCheck;

    public Image backgroundBox;

    public ArrowRotation arrow;

    //not set in inspector
    private float currentLongitude;
    private float currentLatitude;

    private GameObject distanceTextObject;
    private double distance;
    private float rangeAtLocation = 30f;
    private float defaultRange;

    private float bearing;

    private Vector3 targetPosition;
    private Vector3 originalPosition;

    private string locationStr;

    //for raycasts:
    public ARRaycastManager arRaycastManager; //assigned in inspector
    public ARPlaneManager arPlaneManager; // assigned in inspector
    private List<ARRaycastHit> arRaycastHits = new List<ARRaycastHit>();
    public GameObject objectPrefab; //assigned in inspector

    IEnumerator GetCoordinates() {
        //while true so this function keeps running once started.
        while (true) {
            // check if user has location service enabled
            if (!Input.location.isEnabledByUser) {
                yield break;
            }

            // Start service before querying location
            Input.location.Start (1f,.1f);

            // Wait until service initializes
            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
                yield return new WaitForSeconds (1);
                maxWait--;
            }

            // Service didn't initialize in 20 seconds
            if (maxWait < 1) {
                print("Timed out");
                yield break;
            }

            // Connection has failed
            if (Input.location.status == LocationServiceStatus.Failed) {
                print ("Unable to determine device location");
                yield break;
            } else {
                // Access granted and location value could be retrieved
                print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);

                //overwrite current lat and lon everytime
                currentLatitude = Input.location.lastData.latitude;
                currentLongitude = Input.location.lastData.longitude;

                //calculate the distance between where the player needs to be and where the player is
                Calc(targetLatitude, targetLongitude, currentLatitude, currentLongitude);
                //calculate amount to rotate arrow
                AngleFromCoordinate(currentLatitude, currentLongitude, targetLatitude, targetLongitude);

            }
            Input.location.Stop();
        }
    }

    //calculates distance between two sets of coordinates, taking into account the curvature of the earth.
    public void Calc(float lat1, float lon1, float lat2, float lon2) {

        var R = 6371; // Radius of earth in KM
        var dLat = ((lat2 * Mathf.PI) / 180) - ((lat1 * Mathf.PI) / 180);
        var dLon = ((lon2 * Mathf.PI) / 180) - ((lon1 * Mathf.PI) / 180);
        float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
            Mathf.Cos(lat1 * Mathf.PI / 180) * Mathf.Cos(lat2 * Mathf.PI / 180) *
            Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);
        var c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        distance = R * c;
        distance = distance * 3280.84f; // convert to feet
        
        //set the distance text on the canvas
        //distanceTextObject.GetComponent<Text>().text = "Current Lat: " + lat2 + "\nCurrent Lon: " + lon2 + "\nTarget Lat: " + lat1 + "\nTarget Lon: " + lon1 + "\nDistance: " + Math.Round(distance, 2);
        if ((targetLatitude == 0) && (targetLongitude == 0)) {
            distanceTextObject.GetComponent<Text>().text = "Target not set!";
        } else if (! InRange()) {
            distanceTextObject.GetComponent<Text>().text = "Distance from " + locationStr + ": " + Math.Round(distance, 2) + "ft.";
        } else {
            distanceTextObject.GetComponent<Text>().text = "Arrived at " + locationStr + "!";
        }
        //convert distance from double to float
        float distanceFloat = (float)distance;
    }

    // public float ArrowCalc() {
    //     float dy = currentLatitude - targetLatitude;
    //     float dx = Mathf.Cos(Mathf.PI / 180 * targetLatitude) * (currentLongitude - targetLongitude);
    //     float angle = Mathf.Atan2(dy, dx);
    //     return angle;
    // }

    //calculate compass bearing from one location to another
    public void AngleFromCoordinate(float lat1, float lon1, float lat2, float lon2) {
        lat1 *= Mathf.Deg2Rad;
        lat2 *= Mathf.Deg2Rad;
        lon1 *= Mathf.Deg2Rad;
        lon2 *= Mathf.Deg2Rad;

        float dLon = lon2 - lon1;
        float y = Mathf.Sin(dLon) * Mathf.Cos(lat2);
        float x = (Mathf.Cos(lat1) * Mathf.Sin(lat2)) - (Mathf.Sin(lat1) * Mathf.Cos(lat2) * Mathf.Cos(dLon));
        float angle_bearing = Mathf.Atan2(y, x);
        angle_bearing = Mathf.Rad2Deg * angle_bearing;
        angle_bearing = (angle_bearing + 360) % 360;
        angle_bearing = 360 - angle_bearing;
        bearing = angle_bearing;
    }

    public float GetBearing() {
        return bearing;
    }

    void Awake() {
        //disable plane detection (for accuracy)
        arPlaneManager.enabled = false;
        //get distance text reference
        distanceTextObject = GameObject.FindGameObjectWithTag("distanceText");
        //start GetCoordinate() function 
        StartCoroutine("GetCoordinates");
        //initialize target and original position
        targetPosition = transform.position;
        originalPosition = transform.position;
        //set the default range
        defaultRange = rangeToCheck;
    }

    void Update() {
        // placing objects
        if (InRange()) {
            //turn on plane detection if it's off
            if (!arPlaneManager.enabled) {
                arPlaneManager.enabled = true;
            }

            //increase range
            if (rangeToCheck != rangeAtLocation) {
                rangeToCheck = rangeAtLocation;
            }

            //raycast
            if(Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) {
                if (arRaycastManager.Raycast(Input.GetTouch(0).position, arRaycastHits)) {
                    var pose = arRaycastHits[0].pose;
                    CreateObject(pose.position);
                    return;
                }

                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                if (Physics.Raycast(ray, out RaycastHit hit)) {
                    if (hit.collider.tag == "spawnedObject") {
                        DeleteObject(hit.collider.gameObject);
                    }
                }
            }
        } else {
            //turn off plane detection if it's on
            if (arPlaneManager.enabled) {
                arPlaneManager.enabled = false;
            }

            //decrease range
            if (rangeToCheck != defaultRange) {
                rangeToCheck = defaultRange;
            }
        }
    }

    public void SetTargetCoords(float lat, float lon) {
        targetLatitude = lat;
        targetLongitude = lon;
    }

    public void SetLocationString(string location) {
        locationStr = location;
    }

    public void SetObject(GameObject newObject) {
        objectPrefab = newObject;
    }

    public bool InRange() {
        if (distance <= rangeToCheck) {
            return true;
        } else {
            return false;
        }
    }

    private void CreateObject(Vector3 position) {
        Instantiate(objectPrefab, position, Quaternion.identity);
    }

    private void DeleteObject(GameObject spawnedObject) {
        Destroy(spawnedObject);
    }
}