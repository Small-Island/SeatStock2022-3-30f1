using UnityEngine;
// [ExecuteInEditMode]
public enum Controller {
    stop,
    play,
    pause
}

public class Main : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.Vector2 thumbStick;
    [SerializeField, ReadOnly]
    public Controller status = Controller.stop;
    // public Epos4Lift epos4Seat;
    public WalkingDisplayMain walkingDisplayMain;
    public UnityEngine.Video.VideoPlayer videoPlayerLeft;
    public UnityEngine.Video.VideoPlayer videoPlayerRight;
    public UnityEngine.Video.VideoClip[] videoClipLeft;
    public UnityEngine.Video.VideoClip[] videoClipRight;
    public RobotSpeed robotSpeed;
    public enum RobotSpeed {
        slow = 0,
        middle = 1,
        fast = 2
    }
    [SerializeField, UnityEngine.Header("Unit s")]
    private float[] walkDelayTime;

    private bool button_play_disabled = false;
    public bool get_button_play_disabled() {
        return this.button_play_disabled;
    }

    private Coroutine playLoopCoroutine = null;
    public void button_play() {
        // if (this.button_play_disabled) {
        //     return;
        // }
        this.main_play();
        // if (this.playLoopCoroutine != null) {
        //     StopCoroutine(this.playLoopCoroutine);
        // }
        // this.playLoopCoroutine = StartCoroutine(playLoopAsync());
    }
    public void button_pause() {
        this.button_play_disabled = false;
        this.main_pause();
    }
    public void button_stop() {
        this.button_play_disabled = false;
        if (playLoopCoroutine != null) {
            StopCoroutine(playLoopCoroutine);
        }
        this.main_stop();
    }

    private float incdec_time = 0;
    private async void main_play() {
        this.status = Controller.play;
        videoPlayerLeft.clip = this.videoClipLeft[(int)this.robotSpeed];
        videoPlayerRight.clip = this.videoClipRight[(int)this.robotSpeed];
        // if (videoPlayer) {
            videoPlayerLeft.Play();
            videoPlayerRight.Play();
        // }
        // yield return new UnityEngine.WaitForSeconds(1.0f);
        await System.Threading.Tasks.Task.Delay((int)(1000*this.walkDelayTime[(int)this.robotSpeed]));
        // if (walkingDisplayMain) {
            // walkingDisplayMain.WalkStraight(this.incdec_time);
        // }
        // yield return new UnityEngine.WaitForSeconds(0.1f);
        // if (epos4Seat) {
        //     epos4Seat.WalkStraight();
        // }
        // if (lowerLimbMotorSerial) {
            // lowerLimbMotorSerial.WalkStraight();
        // }
    }

    private System.Collections.IEnumerator playLoopAsync() {
        while (true) {
            this.button_play_disabled = true;
            main_stop();
            // float oldHalfTime = lowerLimbMotorSerial.getOldHalfTime();
            // yield return new UnityEngine.WaitForSeconds((float)3*oldHalfTime/1000.0f);

            // yield return this.main_play();
            // yield return new UnityEngine.WaitForSeconds(1.5f);
            this.main_play();
            this.button_play_disabled = false;
            yield return new UnityEngine.WaitForSeconds(5f);
        }
    }

    public void main_pause() {
        this.status = Controller.pause;
        // if (videoPlayer) {
            // videoPlayer.Pause();
            videoPlayerLeft.Pause();
            videoPlayerRight.Pause();
        // }
        if (walkingDisplayMain) {
            walkingDisplayMain.WalkStop();
        }
        // if (epos4Seat) {
        //     epos4Seat.WalkStop();
        // }
        // if (lowerLimbMotorSerial) {
            // lowerLimbMotorSerial.WalkStop();
        // }
    }

    public void main_stop() {
        this.status = Controller.stop;
        // if (videoPlayer) {
            videoPlayerLeft.Stop();
            videoPlayerRight.Stop();
        // }
        if (walkingDisplayMain) {
            walkingDisplayMain.WalkStop();
        }
        // if (epos4Seat) {
        //     epos4Seat.WalkStop();
        // }
        // if (lowerLimbMotorSerial) {
            // lowerLimbMotorSerial.WalkStop();
        // }
    }
    
    void Start() {
        this.status = Controller.stop;
    }

    private bool thumbStickFlag = false;
    private bool abuttonFlag = false;
    private bool bbuttonFlag = false;
    void Update() {
        // this.thumbStick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
        // bool abutton = OVRInput.Get(OVRInput.Button.One);
        // bool bbutton = OVRInput.Get(OVRInput.Button.Two);
        // if (!this.button_play_disabled) {
        //     if (this.thumbStick.y > 0.9 && !thumbStickFlag) {
        //         thumbStickFlag = true;
        //         // lowerLimbMotorSerial.incrementalHallfTime();
        //         this.incdec_time = -0.05f;
        //         this.walkingDisplayMain.period += this.incdec_time;
        //         System.Threading.Tasks.Task.Run(this.walkingDisplayMain.WalkRestart);
        //         // this.walkingDisplayMain.amptitude.slider += 1;
        //         // button_stop();
        //         // button_play();
        //     }
        //     if (this.thumbStick.y < -0.9 && !thumbStickFlag) {
        //         thumbStickFlag = true;
        //         // lowerLimbMotorSerial.decrementalHallfTime();
        //         this.incdec_time = 0.05f;
        //         this.walkingDisplayMain.period += this.incdec_time;
        //         System.Threading.Tasks.Task.Run(this.walkingDisplayMain.WalkRestart);
        //         // this.walkingDisplayMain.amptitude.slider -= 1;
        //         // button_stop();
        //         // button_play();
        //     }
        // }
        // if (System.Math.Abs(this.thumbStick.y) < 0.1 && this.walkingDisplayMain.status == WalkingDisplayMain.Status.walking) {
        //     this.thumbStickFlag = false;
        // }
        // if (abutton & !this.abuttonFlag) {
        //     this.abuttonFlag = true;
        //     this.button_stop();
        // }
        // if (!abutton) {
        //     this.abuttonFlag = false;
        // }
        // if (bbutton & !this.bbuttonFlag) {
        //     this.bbuttonFlag = true;
        //     if (this.status != Controller.play) {
        //         this.button_play();
        //     }
        // }
        // if (!bbutton) {
        //     this.bbuttonFlag = false;
        // }
    }
}