using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Video;

 
public class VideoPlayerScript : MonoBehaviour {
    private VideoPlayer vidPlayer;
 
    private void Start() {
        vidPlayer = GetComponent<VideoPlayer>();
        vidPlayer.SetDirectAudioMute(0, true);
        // vidPlayer.Play();
    }
}
