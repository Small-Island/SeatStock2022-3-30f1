using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoControl : MonoBehaviour
{
    public UnityEngine.Video.VideoPlayer videoPlayer;
    public UnityEngine.AudioSource audioSource;

    [UnityEngine.SerializeField, UnityEngine.Range(0f, 180f)] public float startPoint = 0f;
    // public UnityEngine.GameObject gameObject;
    // Start is called before the first frame update
    void Start()
    {
        // this.defaultPosition = this.gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // public UnityEngine.Vector3 defaultPosition;

    public void Play() {
        // this.videoPlayer.Stop();
        this.videoPlayer.time = this.startPoint;
        this.videoPlayer.Play();
        this.audioSource.Play();
        // this.gameObject.transform.position = new UnityEngine.Vector3(0, 0, 0);
    }

    // public void Exit() {
    //     this.videoPlayer.Stop();
    //     this.videoPlayer.Play();
    //     this.gameObject.transform.position = this.defaultPosition;
    //     this.MuteOn();
    // }

    public void Pause() {
        this.videoPlayer.Pause();
        this.audioSource.Pause();
    }

    public void Stop() {
        this.videoPlayer.Stop();
        this.audioSource.Stop();
    }

    // public void MuteOn() {
    //     this.videoPlayer.SetDirectAudioMute(0, true);
    // }
    // public void MuteOff() {
    //     this.videoPlayer.SetDirectAudioMute(0, false);
    // }
}
