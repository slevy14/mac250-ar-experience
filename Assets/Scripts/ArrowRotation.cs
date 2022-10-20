using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;

public class ArrowRotation : MonoBehaviour {
    //set in inspector
    public MainController mainScript;
    public Sprite arrowSprite;
    public Sprite checkSprite;

    //not set in inspector
    private bool inRange = false;
    private GameObject distanceTextObject;

    float curBearing = 0;

    void Awake() {
        Input.gyro.enabled = true;
        Input.compass.enabled = true;
    }

    void Update() {
        inRange = mainScript.InRange();
        if (inRange) {
            if (transform.rotation != Quaternion.Euler(0,0,0)) {
                transform.rotation = Quaternion.Euler(0,0,0);
            }
            // if in range change arrow to check
            if (gameObject.GetComponent<Image>().sprite != checkSprite) {
                gameObject.GetComponent<Image>().sprite = checkSprite;
            }
        } else {
            if (gameObject.GetComponent<Image>().sprite != arrowSprite) {
                gameObject.GetComponent<Image>().sprite = arrowSprite;
            }


            // transform.rotation = Quaternion.Euler(0,0,0);
            // transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0,0,0), 1f);

            // gameObject.Image.sourceImage = whichever image (check mark)

            curBearing = (float)mainScript.GetBearing();
            //smooth the rotation as it continually updates
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, ((Input.compass.trueHeading + curBearing) % 360)), 0.9f);
        }
    }

    public float GetCurBearing() {
        return curBearing;
    }
}
