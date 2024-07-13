using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trekking : UnityEngine.MonoBehaviour {
    // public Video video;
    // public UnityEngine.AudioSource audioLeftSource;
    // public UnityEngine.AudioSource audioRightSource;
    [UnityEngine.SerializeField, ReadOnly] public Status status;
    [UnityEngine.SerializeField, ReadOnly] public CoolingStatus coolingStatus;
    [ReadOnly] public double clockTime = 0;
    [System.Serializable] public class Length {
        // Unit mm
        [UnityEngine.SerializeField, Range(0, 30)] public double lift = 1;
        [UnityEngine.SerializeField, Range(0, 68)] public double pedal = 1;
        [UnityEngine.SerializeField, Range(0, 100)] public double legForward = 1;
        [UnityEngine.SerializeField, Range(0, 100)] public double legBackward = 1;
        [UnityEngine.SerializeField, Range(0, 100)] public double stockExtendTopPoint = 1;
        [UnityEngine.SerializeField, Range(0, 100)] public double stockExtendPokePoint = 1;
        [UnityEngine.SerializeField, Range(0, 200)] public double stockSlideForward = 1;
        [UnityEngine.SerializeField, Range(0, 200)] public double stockSlideBackward = 1;
    }

    [UnityEngine.SerializeField, UnityEngine.Header("Unit (s)"), UnityEngine.Range(2f, 10f)] public float period = 5;

    public Length length;

    [System.Serializable] public class TimeSchedule {
        private Epos4Node epos4Node;
        public double period;
        public bool activate;
        public bool useStiffness;
        [UnityEngine.SerializeField, Range(1, 10)] public int motion1 = 1;
        [UnityEngine.SerializeField, Range(0, 10)] public int wait1 = 0;
        [UnityEngine.SerializeField, Range(1, 10)] public int motion2 = 1;
        [UnityEngine.SerializeField, Range(0, 10)] public int wait2 = 0;
        [UnityEngine.SerializeField, Range(1, 10)] public int motion3 = 1;
        [UnityEngine.SerializeField, Range(0, 10)] public int wait3 = 0;
        [ReadOnly] public double position1;
        [ReadOnly] public double position2;
        [ReadOnly] public double position3;
        [ReadOnly] public double motionCount = 0;
        public double motion1Duration() {
            if (motionCount == 2) {
                return (double)(this.motion1)/(double)(this.wait1 + this.motion1 + this.wait2 + this.motion2)*this.period;
            }
            else if (motionCount == 3) {
                return (double)(this.motion1)/(double)(this.wait1 + this.motion1 + this.wait2 + this.motion2 + this.wait3 + this.motion3)*this.period;
            }
            return (double)(this.motion1)/(double)(this.wait1 + this.motion1 + this.wait2 + this.motion2)*this.period;
        }
        public double wait1Duration() {
            if (motionCount == 2) {
                return (double)(this.wait1)/(double)(this.wait1 + this.motion1 + this.wait2 + this.motion2)*this.period;
            }
            else if (motionCount == 3) {
                return (double)(this.wait1)/(double)(this.wait1 + this.motion1 + this.wait2 + this.motion2 + this.wait3 + this.motion3)*this.period;
            }
            return (double)(this.wait1)/(double)(this.wait1 + this.motion1 + this.wait2 + this.motion2)*this.period;
        }
        public double motion2Duration() {
            if (motionCount == 2) {
                return (double)(this.motion2)/(double)(this.wait1 + this.motion1 + this.wait2 + this.motion2)*this.period;
            }
            else if (motionCount == 3) {
                return (double)(this.motion2)/(double)(this.wait1 + this.motion1 + this.wait2 + this.motion2 + this.wait3 + this.motion3)*this.period;
            }
            return (double)(this.motion2)/(double)(this.wait1 + this.motion1 + this.wait2 + this.motion2)*this.period;
        }
        public double wait2Duration() {
            if (motionCount == 2) {
                return (double)(this.wait2)/(double)(this.wait1 + this.motion1 + this.wait2 + this.motion2)*this.period;
            }
            else if (motionCount == 3) {
                return (double)(this.wait2)/(double)(this.wait1 + this.motion1 + this.wait2 + this.motion2 + this.wait3 + this.motion3)*this.period;
            }
            return (double)(this.wait2)/(double)(this.wait1 + this.motion1 + this.wait2 + this.motion2)*this.period;
        }
        public double motion3Duration() {
            return (double)(this.motion3)/(double)(this.wait1 + this.motion1 + this.wait2 + this.motion2)*this.period;
        }
        public double wait3Duration() {
            return (double)(this.wait3)/(double)(this.wait1 + this.motion1 + this.wait2 + this.motion2)*this.period;
        }
        [ReadOnly] public int motion1Index = 0;
        [ReadOnly] public int motion2Index = 0;
        [ReadOnly] public int motion3Index = 0;
        [UnityEngine.SerializeField, UnityEngine.Header("歩行周期の割合(%)だけ遅延"), UnityEngine.Range(0f, 100f)] public int waitRate = 0;

        public void init(Epos4Node arg_epos4Node, double arg_period, double arg_position1, double arg_position2) {
            this.epos4Node = arg_epos4Node;
            this.motion1Index = 0;
            this.motion2Index = 0;
            this.motion3Index = 0;
            this.period = arg_period;
            this.position1 = arg_position1;
            this.position2 = arg_position2;
            this.motionCount = 2;
        }

        public void init(Epos4Node arg_epos4Node, double arg_period, double arg_position1, double arg_position2, double arg_position3) {
            this.epos4Node = arg_epos4Node;
            this.motion1Index = 0;
            this.motion2Index = 0;
            this.motion3Index = 0;
            this.period = arg_period;
            this.position1 = arg_position1;
            this.position2 = arg_position2;
            this.position3 = arg_position3;
            this.motionCount = 3;
        }

        public void timerCallback(double arg_clockTime, double arg_stiffness) {
            if (
                arg_clockTime
                > this.motion1Index * this.period
                    + (double)this.waitRate/100.0*this.period
            ) {
                this.motion1Index++;
                this.epos4Node.SetPositionProfileInTime(
                    this.position1,
                    this.motion1Duration(),
                    5, 1
                );
                this.epos4Node.MoveToPosition(this.activate);
            }

            if (
                arg_clockTime
                > this.motion2Index * this.period
                    + this.motion1Duration()
                    + this.wait1Duration()
                    + (double)this.waitRate/100.0*this.period
            ) {
                this.motion2Index++;
                if (this.useStiffness) {
                    this.epos4Node.SetPositionProfileInTime(
                        this.position2,
                        this.motion2Duration(),
                        1, 1 + arg_stiffness
                    );
                }
                else {
                    this.epos4Node.SetPositionProfileInTime(
                        this.position2,
                        this.motion2Duration(),
                        5, 1
                    );
                }
                this.epos4Node.MoveToPosition(this.activate);
            }

            if (this.motionCount == 3) {
                if (
                    arg_clockTime
                    > this.motion3Index * this.period
                        + this.motion1Duration()
                        + this.wait1Duration()
                        + this.motion2Duration()
                        + this.wait2Duration()
                        + (double)this.waitRate/100.0*this.period
                ) {
                    this.motion3Index++;
                    this.epos4Node.SetPositionProfileInTime(
                        this.position3,
                        this.period*this.motion3Duration(),
                        5, 1
                    );
                    this.epos4Node.MoveToPosition(this.activate);
                }
            }
        }   
    }
    [UnityEngine.Header("動作時間比")]
    public TimeSchedule lifter;
    public TimeSchedule leftSlider;
    public TimeSchedule rightSlider;
    public TimeSchedule stockLeftExtend;
    public TimeSchedule stockRightExtend;
    public TimeSchedule stockLeftSlider;
    public TimeSchedule stockRightSlider;

    public bool activateLeftTilt = false;
    public bool activateRightTilt = false;

    public void timerCallback(object source, System.Timers.ElapsedEventArgs e) {
        this.clockTime += 0.005;
        // Lifter
        if (this.status == Status.stop) return;
        this.lifter.timerCallback(this.clockTime, this.stiffness);

        // LegSlider
        if (this.status == Status.stop) return;
        this.leftSlider.timerCallback(this.clockTime, this.stiffness);

        if (this.status == Status.stop) return;
        this.rightSlider.timerCallback(this.clockTime, this.stiffness);

        // Stock Left

        // Extend
        if (this.status == Status.stop) return;
        this.stockLeftExtend.timerCallback(this.clockTime, this.stiffness);
        // Slider
         if (this.status == Status.stop) return;
        this.stockLeftSlider.timerCallback(this.clockTime, this.stiffness);
        
        // Stock Right

        // Extend
         if (this.status == Status.stop) return;
        this.stockRightExtend.timerCallback(this.clockTime, this.stiffness);
        // Slider
        if (this.status == Status.stop) return;
        this.stockRightSlider.timerCallback(this.clockTime, this.stiffness);
    }


    [UnityEngine.SerializeField, Range(100, 10000)] public double stiffness = 100;

    [UnityEngine.SerializeField] private Epos4Main epos4Main;
    public enum Status {
        stop, walking
    }

    private void Start() {
    }


    [UnityEngine.Header("Stock Tilt Conf")]
    // [UnityEngine.SerializeField] public UnityEngine.AddressableAssets.AssetReference csvFileTilt;
    // public string portName = "COM3";    
    // public int baudRate = 9600;
    // private System.IO.Ports.SerialPort client;
    public ESP32Main esp32Main;
    private float degreePerPulse = 0.0072f; //[degrees/pulse]
    public string sendText;
    [UnityEngine.SerializeField, UnityEngine.Header("Unit (deg), Absolute, Backward Positive, Forward Negative"), UnityEngine.Range(0, 10)] public float tiltBackward = 0;
    [UnityEngine.SerializeField, UnityEngine.Range(-30, 0)] public float tiltForward = 0;
    [UnityEngine.SerializeField, UnityEngine.Header("Tilt Backward Time Ratio"), UnityEngine.Range(1f, 5f)] public float tiltBackwardTimeRatio = 1;
    [UnityEngine.SerializeField, UnityEngine.Header("Tilt Forward  Time Ratio"), UnityEngine.Range(1f, 5f)] public float tiltForwardTimeRatio = 1;
    public bool doubleStock = false;
    public double startClockTimeLeftTilt = 0;
    public double startClockTimeRightTilt = 0;
    public double leftTiltDriveTimeBackward = 0;
    public double leftTiltDriveTimeForward = 0;
    public double rightTiltDriveTimeBackward = 0;
    public double rightTiltDriveTimeForward = 0;
    public double leftTiltDelayTimeBackward = 0;
    public double leftTiltDelayTimeForward = 0;
    public double rightTiltDelayTimeBackward = 0;
    public double rightTiltDelayTimeForward = 0;
    //出力パルス（送信）
    private int[] targetPulseUp1 = new int[6] { 0, 0, 0, 0, 0, 0 };//上昇／前進時の目標パルス（左ペダル、左スライダ、右ペダル、右スライダ）[pulse]
    private int[] targetPulseDown1 = new int[6] { 0, 0, 0, 0, 0, 0 };//下降／後退時の目標パルス（左ペダル、左スライダ、右ペダル、右スライダ）[pulse]
    //駆動時間（送信）
    private int[] driveTimeUp1 = new int[6] { 0, 0, 0, 0, 0, 0 };//上昇／前進時の駆動時間（左ペダル、左スライダ、右ペダル、右スライダ）[ms]
    private int[] driveTimeDown1 = new int[6] {0, 0, 0, 0, 0, 0 };//下降／後退時の駆動時間（左ペダル、左スライダ、右ペダル、右スライダ）[ms]
    //待機時間（送信）
    private int[] delayTimeUp1 = new int[6] { 0, 0, 0, 0, 0, 0 };//上昇／前進始めモータ停止時間（左ペダル、左スライダ、右ペダル、右スライダ）[ms]
    private int[] delayTimeDown1 = new int[6] { 0, 0, 0, 0, 0, 0 };//下降／後退始めモータ停止時間（左ペダル、左スライダ、右ペダル、右スライダ）[ms]
    private int[] delayTimeFirst = new int[6] { 0, 0, 0, 0, 0, 0 };//一歩目モータ停止時間（左ペダル、左スライダ、右ペダル、右スライダ）[ms]
    private int seatRotationPulse;

    public float ExperienceTime = 0;

    private void targetCalculate()//振幅値（mm）→出力パルス変換
    {
        //目標パルスを整数型で格納
        if (this.activateLeftTilt) {
            this.targetPulseUp1[0] = (int)(-this.tiltBackward / this.degreePerPulse);
            this.targetPulseDown1[0] = (int)(-this.tiltForward / this.degreePerPulse);
        }
        else {
            this.targetPulseUp1[0] = 0;
            this.targetPulseDown1[0] = 0;
        }
        this.targetPulseUp1[1] = 0;
        this.targetPulseDown1[1] = 0;
        if (this.activateRightTilt) {
            this.targetPulseUp1[2] = (int)(this.tiltBackward / this.degreePerPulse);
            this.targetPulseDown1[2] = (int)(this.tiltForward / this.degreePerPulse);
        }
        else {
            this.targetPulseUp1[2] = 0;
            this.targetPulseDown1[2] = 0;
        }
        this.targetPulseUp1[3] = 0;
        this.targetPulseDown1[3] = 0;
        this.targetPulseUp1[4] = 0;
        this.targetPulseDown1[4] = 0;
        this.targetPulseUp1[5] = 0;
        this.targetPulseDown1[5] = 0;
        this.seatRotationPulse = 0;
        if (this.activateLeftTilt) {
            this.driveTimeUp1[0] = (int)(this.leftTiltDriveTimeBackward * 1000f);
            this.driveTimeDown1[0] = (int)(this.leftTiltDriveTimeForward * 1000f);
            this.delayTimeUp1[0] = (int)(this.leftTiltDelayTimeBackward * 1000f);
            this.delayTimeDown1[0] = (int)(this.leftTiltDelayTimeForward * 1000f);
        }
        else {
            this.driveTimeUp1[0] = 0;
            this.driveTimeDown1[0] = 0;
        }
        if (this.activateRightTilt) {
            this.driveTimeUp1[2] = (int)(this.rightTiltDriveTimeBackward * 1000f);
            this.driveTimeDown1[2] = (int)(this.rightTiltDriveTimeForward * 1000f);
            this.delayTimeUp1[2] = (int)(this.rightTiltDelayTimeBackward * 1000f);
            this.delayTimeDown1[2] = (int)(this.rightTiltDelayTimeForward * 1000f);
        }
        else {
            this.driveTimeUp1[2] = 0;
            this.driveTimeDown1[2] = 0;
        }
        
        this.delayTimeFirst[0] = (int)(startClockTimeLeftTilt * 1000.0);
        this.delayTimeFirst[2] = (int)(startClockTimeRightTilt * 1000.0);
    }

    private System.Threading.Thread th = null;
    private System.Timers.Timer walkStraightTimer;
    private System.Timers.Timer coolingTimer;
    public enum CoolingStatus {
        Readied, NowCooling
    }
    // [UnityEngine.SerializeField, ReadOnly] public CoolingStatus coolingStatus;
    private System.Timers.Timer trekkingTimer;

    public void WalkStraight() {
        if (this.status == Status.walking) return;
        if (this.coolingStatus == CoolingStatus.NowCooling) return;
        UnityEngine.Debug.Log("WalkStraight");
        this.status = Status.walking;
        this.epos4Main.AllNodeDefinePosition();
        this.epos4Main.AllNodeActivateProfilePositionMode();
        if (this.walkStraightTimer != null) {
            this.walkStraightTimer.Stop();
            this.walkStraightTimer.Dispose();
        }
        if (this.coolingStatus == CoolingStatus.NowCooling) return;
        
        this.th = new System.Threading.Thread(new System.Threading.ThreadStart(this.getActualPositionAsync));
        this.th.Start();

        this.targetCalculate();//目標値計算
        //送信するデータを文字列でまとめる
        this.sendText = "start" + ",";
        for (int i = 0; i < 6; i++) {
            this.sendText += this.targetPulseUp1[i].ToString() + "," + this.targetPulseDown1[i].ToString() + ",";
            this.sendText += this.driveTimeUp1[i].ToString() + "," + this.driveTimeDown1[i].ToString() + ",";
            this.sendText += this.delayTimeUp1[i].ToString() + "," + this.delayTimeDown1[i].ToString() + ",";
            this.sendText += this.delayTimeFirst[i].ToString() + ",";
        }
        this.sendText += this.seatRotationPulse.ToString() + ",";
        this.sendText += "/";//終わりの目印
        this.esp32Main.SendText(this.sendText);

        this.clockTime = 0;
        this.lifter.init(this.epos4Main.lifter, this.period/2, this.length.lift, 0);
        this.leftSlider.init(this.epos4Main.leftSlider, this.period, this.length.legForward, -this.length.legBackward);
        this.rightSlider.init(this.epos4Main.rightSlider, this.period, this.length.legForward, -this.length.legBackward);
        // this.stockLeftExtend.init(this.epos4Main.stockLeftExtend, this.period, this.length.stockExtendTopPoint, this.length.stockExtendPokePoint, 0);
        // this.stockRightExtend.init(this.epos4Main.stockRightExtend, this.period, this.length.stockExtendTopPoint, this.length.stockExtendPokePoint, 0);
        this.stockLeftExtend.init(this.epos4Main.stockLeftExtend, this.period, this.length.stockExtendTopPoint, 0);
        this.stockRightExtend.init(this.epos4Main.stockRightExtend, this.period, this.length.stockExtendTopPoint, 0);
        this.stockLeftSlider.init(this.epos4Main.stockLeftSlider, this.period, this.length.stockSlideForward, -this.length.stockSlideBackward);
        this.stockRightSlider.init(this.epos4Main.stockRightSlider, this.period, this.length.stockSlideForward, -this.length.stockSlideBackward);
        this.trekkingTimer = new System.Timers.Timer(5);
        this.trekkingTimer.AutoReset = true;
        this.trekkingTimer.Elapsed += this.timerCallback;
        this.trekkingTimer.Start();
    }

    public void WalkStop() {
        this.trekkingTimer?.Stop();
        this.trekkingTimer?.Dispose();
        this.status = Status.stop;
        UnityEngine.Debug.Log("WalkStop");
        this.epos4Main.AllNodeMoveToHome();
        if (this.coolingStatus == CoolingStatus.Readied) {
            this.sendText = "stop" + "," + "/";
            this.esp32Main.SendText(this.sendText);
            UnityEngine.Debug.Log("walkmain: " + this.sendText);

            this.coolingStatus = CoolingStatus.NowCooling;
            if (this.coolingTimer != null) {
                this.coolingTimer.Stop();
                this.coolingTimer.Dispose();
            }
            this.coolingTimer = new System.Timers.Timer(2*this.period*1000f);
            this.coolingTimer.AutoReset = false;
            this.coolingTimer.Elapsed += (sender, e) => {
                this.coolingStatus = CoolingStatus.Readied;
            };
            this.coolingTimer.Start();
        }
        this.pauseFlag = true;
    }

    private void getActualPositionAsync() {
        int N = 10000;
        float[,] data = new float[N,18];
        int i = 0;
        while (!this.Destroied && i < N && this.status == Status.walking) {
            data[i,0] = this.epos4Main.lifter.actualPosition / 10f; // Unit 10cm
            data[i,1] = this.epos4Main.leftPedal.actualPosition / 100f;
            data[i,2] = this.epos4Main.leftSlider.actualPosition / 100f;
            data[i,3] = this.epos4Main.rightPedal.actualPosition / 100f;
            data[i,4] = this.epos4Main.rightSlider.actualPosition / 100f;
            data[i,5] = this.epos4Main.stockLeftExtend.actualPosition / 100f;
            data[i,6] = this.epos4Main.stockLeftSlider.actualPosition / 100f;
            data[i,7] = this.epos4Main.stockRightExtend.actualPosition / 100f;
            data[i,8] = this.epos4Main.stockRightSlider.actualPosition / 100f;

            data[i,9] = this.epos4Main.lifter.current;
            data[i,10] = this.epos4Main.leftPedal.current;
            data[i,11] = this.epos4Main.leftSlider.current;
            data[i,12] = this.epos4Main.rightPedal.current;
            data[i,13] = this.epos4Main.rightSlider.current;
            data[i,14] = this.epos4Main.stockLeftExtend.current;
            data[i,15] = this.epos4Main.stockLeftSlider.current;
            data[i,16] = this.epos4Main.stockRightExtend.current;
            data[i,17] = this.epos4Main.stockRightSlider.current;

            i++;

            System.Threading.Thread.Sleep(10);
        }

        N = i;

        System.IO.StreamWriter sw; // これがキモらしい
        System.IO.FileInfo fi;
        　　// Aplication.dataPath で プロジェクトファイルがある絶対パスが取り込める
        System.DateTime dt = System.DateTime.Now;
        string result = dt.ToString("yyyyMMddHHmmss");
        fi = new System.IO.FileInfo(UnityEngine.Application.dataPath + "/Scripts/log/current" + result + ".csv");
        sw = fi.AppendText();
        sw.WriteLine("time (s), lifter (1cm), left pedal pos (10cm), left slider pos (10cm), right pedal pos (10cm), right slider pos (10cm), stock left extend pos (10cm), stock left slider pos (10cm), stock right extend pos (10cm), stock right slider pos (10cm), lifter current (A), left pedal current (A), left slider current (A), right pedal current (A), right slider current (A), stock left extend current (A), stock left slider current (A), stock right extend current (A), stock right slider current (A)");
        for (i = 0; i < N; i++)
        {
            float time = i*0.01f;
            string a = time.ToString() + ",";
            for (int j = 0; j < 18; j++) {
                a += data[i,j].ToString() + ",";
            }
            sw.WriteLine(a);
        }
        sw.Flush();
        sw.Close();
        return;
    }
    
    [UnityEngine.SerializeField, ReadOnly] private UnityEngine.Vector2 thumbStickR;
    [UnityEngine.SerializeField, ReadOnly] private UnityEngine.Vector2 thumbStickL;


    private bool pauseFlag = false;
    private bool audioLeftFlag = false;
    private bool audioRightFlag = false;

    private void Update() {
        // this.thumbStickR = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
        // this.thumbStickL = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
        // if (System.Math.Abs(this.thumbStickR.y) > 0.5 && System.Math.Abs(this.thumbStickL.y) > 0.5) {
        // }
        // else if (this.thumbStickR.y > 0.5 || this.thumbStickL.y > 0.5) {
        //     // this.WalkStraight();
        // }
        // else if (this.thumbStickR.y < -0.5 || this.thumbStickL.y < -0.5) {
        //     // this.WalkStop();
        // }

        // if (this.video != null) {
        //     if (this.status == Status.stop & this.video.videoPlayer.isPlaying & this.pauseFlag) {
        //         this.video?.Pause();
        //         this.pauseFlag = false;
        //     }
        // }
        // if (this.audioLeftFlag) {
        //     if (this.audioLeftSource != null) {
        //         this.audioLeftSource.Play();
        //     }
        //     this.audioLeftFlag = false;
        // }
        // if (this.audioRightFlag) {
        //     if (this.audioRightSource != null) {
        //         this.audioRightSource.Play();
        //     }
        //     this.audioRightFlag = false;
        // }
    }

    private bool Destroied = false;

    private void OnDestroy() {
        this.WalkStop();
        this.Destroied = true;
    }
}