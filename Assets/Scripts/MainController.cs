using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;

public class MainController : MonoBehaviour {

    //set in inspector
    public ARPlaneManager arPlaneManager;
    public ARLocation.LocationData targetLocation;
    public ARLocation.ARLocationProvider locationProvider;
    public Camera mainCamera;
    [SerializeField] private GameObject distanceTextObject;
    [SerializeField] private GameObject muteButton;

    // public GameObject moreInfoButton;
    // public GameObject 

    //not set in inspector
    double curLatitude;
    double curLongitude;
    double targetLatitude = 0;
    double targetLongitude = 0;

    double bearing;
    double distance;

    double rangeAtLocation = 70.0;
    double defaultRange = 20.0;
    double rangeToCheck;

    void Awake() {
        rangeToCheck = defaultRange;
    }

    void Update() {
        locationProvider = ARLocation.ARLocationProvider.Instance;

        curLatitude = locationProvider.CurrentLocation.latitude;
        curLongitude = locationProvider.CurrentLocation.longitude;
        targetLatitude = targetLocation.Location.Latitude;
        targetLongitude =  targetLocation.Location.Longitude;

        Calc(targetLatitude, targetLongitude, curLatitude, curLongitude);
        AngleFromCoordinate(curLatitude, curLongitude, targetLatitude, targetLongitude);
    }

    public void Calc(double lat1, double lon1, double lat2, double lon2) {

        var R = 6371; // Radius of earth in KM
        var dLat = ((lat2 * Mathf.PI) / 180) - ((lat1 * Mathf.PI) / 180);
        var dLon = ((lon2 * Mathf.PI) / 180) - ((lon1 * Mathf.PI) / 180);
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(lat1 * Mathf.PI / 180) * Math.Cos(lat2 * Mathf.PI / 180) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        distance = R * c;
        distance = distance * 3280.84; // convert to feet
        
        //set the distance text on the canvas
        //distanceTextObject.GetComponent<Text>().text = "Current Lat: " + lat2 + "\nCurrent Lon: " + lon2 + "\nTarget Lat: " + lat1 + "\nTarget Lon: " + lon1 + "\nDistance: " + Math.Round(distance, 2);
        if ((targetLatitude == 0) && (targetLongitude == 0)) {
            distanceTextObject.GetComponent<Text>().text = "Target not set!";
        } else if (! InRange()) {
            distanceTextObject.GetComponent<Text>().text = targetLocation.Location.Label + ": " + Math.Round(distance, 1) + " ft.";
        } else {
            distanceTextObject.GetComponent<Text>().text = "Arrived at " + targetLocation.Location.Label + "!";
        }
    }

    public bool InRange() {
        if (distance <= rangeToCheck) {
            if (rangeToCheck != rangeAtLocation) {
                rangeToCheck = rangeAtLocation;
            }
            // if (!moreInfoButton.activeSelf) {
            //     moreInfoButton.SetActive(true);
            // }

            //turn on plane detection if it's off, update positions
            if (!arPlaneManager.enabled) {
                arPlaneManager.enabled = true;

                //increase clipping planes
                mainCamera.farClipPlane = 30;

                ARLocation.ARLocationManager.Instance.ResetARSession((() =>
                {
                    Debug.Log("AR+GPS and AR Session were restarted!");
                }));
            }
            if (muteButton != null && !muteButton.activeSelf) {
                muteButton.SetActive(true);
                muteButton.GetComponent<MuteScript>().initialize();
                muteButton.GetComponent<MuteScript>().setCurrentVideo(targetLocation.Location.Label);
            }
            return true;
        } else {
            if (rangeToCheck != defaultRange) {
                rangeToCheck = defaultRange;
            }
            // if (moreInfoButton.activeSelf) {
            //     moreInfoButton.SetActive(false);
            // }
            if (muteButton != null && muteButton.activeSelf) {
                muteButton.SetActive(false);
                muteButton.GetComponent<MuteScript>().muteAll();
            }

            //turn off plane detection if it's on
            if (arPlaneManager.enabled) {
                arPlaneManager.enabled = false;

                //decrease clipping planes
                mainCamera.farClipPlane = 1;
            }
            return false;
        }
    }

    public void SetOutOfRange() {
        rangeToCheck = defaultRange;
        
        if (muteButton != null && muteButton.activeSelf) {
            muteButton.SetActive(false);
            muteButton.GetComponent<MuteScript>().muteAll();
        }

        if (arPlaneManager.enabled) {
            arPlaneManager.enabled = false;

            //decrease clipping planes
            mainCamera.farClipPlane = 1;
        }
    }

    public void AngleFromCoordinate(double lat1, double lon1, double lat2, double lon2) {
        lat1 *= Mathf.Deg2Rad;
        lat2 *= Mathf.Deg2Rad;
        lon1 *= Mathf.Deg2Rad;
        lon2 *= Mathf.Deg2Rad;

        double dLon = lon2 - lon1;
        double y = Math.Sin(dLon) * Math.Cos(lat2);
        double x = (Math.Cos(lat1) * Math.Sin(lat2)) - (Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon));
        double angle_bearing = Math.Atan2(y, x);
        angle_bearing = Mathf.Rad2Deg * angle_bearing;
        angle_bearing = (angle_bearing + 360) % 360;
        angle_bearing = 360 - angle_bearing;
        bearing = angle_bearing;
    }

    public double GetBearing() {
        return bearing;
    }


}
