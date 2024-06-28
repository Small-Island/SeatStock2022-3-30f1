using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Video : MonoBehaviour
{
    public UnityEngine.Video.VideoPlayer videoPlayer;
    public UnityEngine.GameObject gameObject;
    // Start is called before the first frame update
    void Start()
    {
        this.defaultPosition = this.gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public UnityEngine.Vector3 defaultPosition;

    public void Play() {
        this.videoPlayer.Stop();
        this.videoPlayer.Play();
        this.gameObject.transform.position = new UnityEngine.Vector3(0, 0, 0);
    }

    public void Exit() {
        this.videoPlayer.Stop();
        this.videoPlayer.Play();
        this.gameObject.transform.position = this.defaultPosition;
        this.MuteOn();
    }

    public void Pause() {
        this.videoPlayer.Pause();
    }

    public void MuteOn() {
        this.videoPlayer.SetDirectAudioMute(0, true);
    }
    public void MuteOff() {
        this.videoPlayer.SetDirectAudioMute(0, false);
    }
}
