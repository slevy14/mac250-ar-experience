using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MuteScript : MonoBehaviour {
    
    //set in inspector
    public MainController mainControllerScript;
    public Sprite muteImage;
    public Sprite unmuteImage;

    //set by main controller
    public GameObject currentVideo;
    public string curVidName;

    //other
    public bool muted = true;

    void Awake() {
        gameObject.SetActive(false);
        // this.initialize();
    }

    public void toggle() {
        if (muted) {
            gameObject.GetComponent<Image>().sprite = unmuteImage;
            muted = false;
            findVideo(curVidName);
            currentVideo.GetComponent<VideoPlayer>().SetDirectAudioMute(0, false);
            currentVideo.GetComponent<VideoPlayer>().Play();
        } else {
            gameObject.GetComponent<Image>().sprite = muteImage;
            muted = true;
            findVideo(curVidName);
            currentVideo.GetComponent<VideoPlayer>().SetDirectAudioMute(0, true);
            currentVideo.GetComponent<VideoPlayer>().Pause();
        }
    }

    public void initialize() {
        this.gameObject.GetComponent<Image>().sprite = muteImage;
    }

    public void muteAll() {
        if (currentVideo != null) {
            currentVideo.GetComponent<VideoPlayer>().SetDirectAudioMute(0, true);
            currentVideo.GetComponent<VideoPlayer>().Pause();
        }
    }

    public void setCurrentVideo(string name) {
        curVidName = name;
    }

    public void findVideo(string title) {
        switch (title) {
            case "Clueless (JSC)":
                currentVideo = GameObject.FindWithTag("clueless1");
                break;
            case "Star Trek":
                currentVideo = GameObject.FindWithTag("startrek");
                break;
            case "Pat And Mike":
                currentVideo = GameObject.FindWithTag("patandmike");
                break;
            case "Glee":
                currentVideo = GameObject.FindWithTag("glee");
                break;
            case "Clueless (Quad)":
                currentVideo = GameObject.FindWithTag("clueless2");
                break;
            case "90210":
                currentVideo = GameObject.FindWithTag("90210");
                break;
            case "The Holiday":
                currentVideo = GameObject.FindWithTag("holiday");
                break;
            case "Don't Be a Menace":
                currentVideo = GameObject.FindWithTag("menace");
                break;
            case "Jurassic Park":
                currentVideo = GameObject.FindWithTag("jurassic");
                break;
            case "The West Wing":
                currentVideo = GameObject.FindWithTag("westwing");
                break;
        }
    }
}
