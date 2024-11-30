using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntegratedControl : UnityEngine.MonoBehaviour {
    [UnityEngine.SerializeField, Range(0, 20)] public int delay = 1;
    public VideoControl videoControl;
    public VideoControl[] otherVideo;
    public UnityEngine.AudioSource audioLeftSource;
    public UnityEngine.AudioSource audioRightSource;
    public ESP32Main esp32Wind;
    [UnityEngine.SerializeField, ReadOnly] public Status status;
    [UnityEngine.SerializeField, ReadOnly] public CoolingStatus coolingStatus;
    [ReadOnly] public double clockTime = 0;
    public float ExperienceTime = 0;

    [UnityEngine.SerializeField, Range(0.5f, 1.5f)] public double scale = 1;

    [System.Serializable] public class Length {
        // Unit mm
        [UnityEngine.SerializeField, Range(0, 100)] public double lift = 1;
        [UnityEngine.SerializeField, Range(0, 60)] public double pedal = 1;
        [UnityEngine.SerializeField, Range(0, 30)] public double pedalYaw = 1;
        [UnityEngine.SerializeField, Range(0, 100)] public double legForward = 1;
        [UnityEngine.SerializeField, Range(0, 100)] public double legBackward = 1;
        [UnityEngine.SerializeField, Range(0, 300)] public double stockExtendTopPoint = 1;
        [UnityEngine.SerializeField, Range(0, 300)] public double stockExtendPokePoint = 1;
        [UnityEngine.SerializeField, Range(0, 300)] public double stockSlideForward = 1;
        [UnityEngine.SerializeField, Range(0, 300)] public double stockSlideBackward = 1;
        [UnityEngine.SerializeField, Range(0, 300)] public double windHigh = 1;
        [UnityEngine.SerializeField, Range(0, 300)] public double windLow = 0;
    }

    [UnityEngine.SerializeField, UnityEngine.Header("Unit (s)"), UnityEngine.Range(2f, 10f)] public float period = 5;

    public Length length;
    [UnityEngine.SerializeField] public Length scaledLength;
    [System.Serializable] public class TimeSchedule {
        private Epos4Node epos4Node;
        private ESP32Main esp32Wind;
        private double period;
        public bool activate;
        public bool useStiffness;
        public bool useVibro;
        // public bool yaw = false;
        public bool useRandom;
        public double climbMm = 0;
        public int climbCount = 0;
        public int climbIdx = 0;
        [ReadOnly, UnityEngine.Range(-255, 255)] public double position1;
        [UnityEngine.SerializeField, Range(0, 10)] public int duration1 = 1;
        [UnityEngine.SerializeField, Range(0, 10)] public int wait1 = 0;
        [ReadOnly, UnityEngine.Range(-255, 255)] public double position2;
        [UnityEngine.SerializeField, Range(0, 10)] public int duration2 = 1;
        [UnityEngine.SerializeField, Range(0, 10)] public int wait2 = 0;
        [ReadOnly, UnityEngine.Range(-255, 255)] public double position3;
        [UnityEngine.SerializeField, Range(0, 10)] public int duration3 = 0;
        [UnityEngine.SerializeField, Range(0, 10)] public int wait3 = 0;
        [ReadOnly, UnityEngine.Range(2, 3)] public double motionCount = 0;
        private System.Random random;
        public double motion1Duration() {
            if (this.motionCount == 2) {
                return (double)(this.duration1)/(double)(this.wait1 + this.duration1 + this.wait2 + this.duration2)*this.period;
            }
            else if (this.motionCount == 3) {
                return (double)(this.duration1)/(double)(this.wait1 + this.duration1 + this.wait2 + this.duration2 + this.wait3 + this.duration3)*this.period;
            }
            return (double)(this.duration1)/(double)(this.wait1 + this.duration1 + this.wait2 + this.duration2)*this.period;
        }
        public double wait1Duration() {
            if (this.motionCount == 2) {
                return (double)(this.wait1)/(double)(this.wait1 + this.duration1 + this.wait2 + this.duration2)*this.period;
            }
            else if (this.motionCount == 3) {
                return (double)(this.wait1)/(double)(this.wait1 + this.duration1 + this.wait2 + this.duration2 + this.wait3 + this.duration3)*this.period;
            }
            return (double)(this.wait1)/(double)(this.wait1 + this.duration1 + this.wait2 + this.duration2)*this.period;
        }
        public double motion2Duration() {
            if (this.motionCount == 2) {
                return (double)(this.duration2)/(double)(this.wait1 + this.duration1 + this.wait2 + this.duration2)*this.period;
            }
            else if (this.motionCount == 3) {
                return (double)(this.duration2)/(double)(this.wait1 + this.duration1 + this.wait2 + this.duration2 + this.wait3 + this.duration3)*this.period;
            }
            return (double)(this.duration2)/(double)(this.wait1 + this.duration1 + this.wait2 + this.duration2)*this.period;
        }
        public double wait2Duration() {
            if (this.motionCount == 2) {
                return (double)(this.wait2)/(double)(this.wait1 + this.duration1 + this.wait2 + this.duration2)*this.period;
            }
            else if (this.motionCount == 3) {
                return (double)(this.wait2)/(double)(this.wait1 + this.duration1 + this.wait2 + this.duration2 + this.wait3 + this.duration3)*this.period;
            }
            return (double)(this.wait2)/(double)(this.wait1 + this.duration1 + this.wait2 + this.duration2)*this.period;
        }
        public double motion3Duration() {
            return (double)(this.duration3)/(double)(this.wait1 + this.duration1 + this.wait2 + this.duration2 + this.duration3 + this.wait3)*this.period;
        }
        public double wait3Duration() {
            return (double)(this.wait3)/(double)(this.wait1 + this.duration1 + this.wait2 + this.duration2 + this.duration3 + this.wait3)*this.period;
        }
        [ReadOnly] public int motion1Index = 0;
        [ReadOnly] public int motion2Index = 0;
        [ReadOnly] public int motion3Index = 0;
        [ReadOnly] public int vibroIndex = 0;
        [UnityEngine.SerializeField, UnityEngine.Header("歩行周期の割合(%)だけ遅延"), UnityEngine.Range(0f, 200f)] public int waitRate = 0;

        public void init(Epos4Node arg_epos4Node, double arg_period, double arg_position1, double arg_position2) {
            this.epos4Node = arg_epos4Node;
            this.motion1Index = 0;
            this.motion2Index = 0;
            this.motion3Index = 0;
            this.period = arg_period;
            // this.position1 = arg_position1;
            // this.position2 = arg_position2;
            this.motionCount = 2;
            this.vibroIndex = 0;
            this.climbIdx = 0;
            this.random = new System.Random();
        }

        public void init(Epos4Node arg_epos4Node, double arg_period, double arg_position1, double arg_position2, double arg_position3) {
            this.epos4Node = arg_epos4Node;
            this.motion1Index = 0;
            this.motion2Index = 0;
            this.motion3Index = 0;
            this.period = arg_period;
            // this.position1 = arg_position1;
            // this.position2 = arg_position2;
            // this.position3 = arg_position3;
            if (this.duration3 == 0) {
                this.motionCount = 2;
            }
            else {
                this.motionCount = 3;
            }
            this.vibroIndex = 0;
            this.climbIdx = 0;
            this.random = new System.Random();
        }

        public void initWind(ESP32Main arg_esp32Wind, double arg_period) {
            this.esp32Wind = arg_esp32Wind;
            this.motion1Index = 0;
            this.motion2Index = 0;
            this.motion3Index = 0;
            this.period = arg_period;
            this.motionCount = 2;
        }

        public void timerCallbackWind(double arg_clockTime, string arg_LorR) {
            if (
                arg_clockTime
                > this.motion1Index * this.period
                    + (double)this.waitRate/100.0*this.period
            ) {
                this.motion1Index++;
                this.esp32Wind.SendText(arg_LorR + this.position1 + "e");
            }

            if (
                arg_clockTime
                > this.motion2Index * this.period
                    + this.motion1Duration()
                    + this.wait1Duration()
                    + (double)this.waitRate/100.0*this.period
            ) {
                this.motion2Index++;
                this.esp32Wind.SendText(arg_LorR + this.position2 + "e");
            }
        }


        public void timerCallback(double arg_clockTime, double arg_stiffness, ref bool arg_flag) {
            if (
                arg_clockTime
                > this.motion1Index * this.period
                    + (double)this.waitRate/100.0*this.period
            ) {
                this.motion1Index++;
                this.climbIdx++;
                if (this.climbIdx > this.climbCount) {
                    this.climbIdx = 0;
                }
                double pos = 0;
                if (this.useRandom) {
                    pos = this.position1 * (double)(this.random.Next(1000))/1000.0;
                }
                else {
                    pos = this.position1 + (double)this.climbIdx*this.climbMm;
                }
                this.epos4Node.SetPositionProfileInTime(
                    pos,
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
                double pos = 0;
                if (this.useRandom) {
                    pos = this.position2 * (double)(this.random.Next(1000))/1000.0;
                }
                else {
                    pos = this.position2 + (double)this.climbIdx*this.climbMm;
                }
                if (this.useStiffness) {
                    this.epos4Node.SetPositionProfileInTime(
                        pos,
                        this.motion2Duration(),
                        1, 1 + arg_stiffness
                    );
                }
                else {
                    this.epos4Node.SetPositionProfileInTime(
                        pos,
                        this.motion2Duration(),
                        5, 1
                    );
                }
                this.epos4Node.MoveToPosition(this.activate);
            }

            if (this.useVibro) {
                if (
                    arg_clockTime
                    > this.vibroIndex * this.period
                        + this.motion1Duration()
                        + this.wait1Duration()
                        + this.motion2Duration()
                        + (double)this.waitRate/100.0*this.period
                        - 0.3
                ) {
                    this.vibroIndex++;
                    arg_flag = true;
                }
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
                    double pos = 0;
                    if (this.useRandom) {
                        pos = this.position3 * (double)(this.random.Next(1000))/1000.0;
                    }
                    else {
                        pos = this.position3 + (double)this.climbIdx*this.climbMm;
                    }
                    this.epos4Node.SetPositionProfileInTime(
                        pos,
                        this.motion3Duration(),
                        5, 1
                    );
                    this.epos4Node.MoveToPosition(this.activate);
                }
            }
        }   
    }
    [UnityEngine.Header("動作時間比")]
    public TimeSchedule lifter;
    public TimeSchedule leftPedal;
    public TimeSchedule leftPedalYaw;
    public TimeSchedule leftSlider;
    public TimeSchedule rightPedalYaw;
     public TimeSchedule rightPedal;
    public TimeSchedule rightSlider;
    public TimeSchedule stockLeftExtend;
    public TimeSchedule stockRightExtend;
    public TimeSchedule stockLeftSlider;
    public TimeSchedule stockRightSlider;
    public TimeSchedule windLeft;
    public TimeSchedule windRight;

    public bool activateLeftTilt = false;
    public bool activateRightTilt = false;

    public bool dummyFlag = false;

    public void timerCallback(object source, System.Timers.ElapsedEventArgs e) {
        this.clockTime += 0.01;
        // Lifter
        if (this.status == Status.stop) return;
        this.lifter.timerCallback(this.clockTime, this.stiffness, ref this.dummyFlag);

        if (this.status == Status.stop) return;
        this.leftPedal.timerCallback(this.clockTime, this.stiffness, ref this.audioLeftFlag);

        if (this.status == Status.stop) return;
        this.leftPedalYaw.timerCallback(this.clockTime, this.stiffness, ref this.audioLeftFlag);

        // LegSlider
        if (this.status == Status.stop) return;
        this.leftSlider.timerCallback(this.clockTime, this.stiffness, ref this.dummyFlag);

        if (this.status == Status.stop) return;
        this.rightPedal.timerCallback(this.clockTime, this.stiffness, ref this.audioRightFlag);

        if (this.status == Status.stop) return;
        this.rightPedalYaw.timerCallback(this.clockTime, this.stiffness, ref this.audioLeftFlag);


        if (this.status == Status.stop) return;
        this.rightSlider.timerCallback(this.clockTime, this.stiffness, ref this.dummyFlag);

        // Stock Left

        // Extend
        if (this.status == Status.stop) return;
        this.stockLeftExtend.timerCallback(this.clockTime, this.stiffness, ref this.dummyFlag);
        // Slider
        if (this.status == Status.stop) return;
        this.stockLeftSlider.timerCallback(this.clockTime, this.stiffness, ref this.dummyFlag);
        
        // Stock Right

        // Extend
        if (this.status == Status.stop) return;
        this.stockRightExtend.timerCallback(this.clockTime, this.stiffness, ref this.dummyFlag);
        // Slider
        if (this.status == Status.stop) return;
        this.stockRightSlider.timerCallback(this.clockTime, this.stiffness, ref this.dummyFlag);

        if (this.status == Status.stop) return;
        this.windLeft.timerCallbackWind(this.clockTime, "L");
        if (this.status == Status.stop) return;
        this.windRight.timerCallbackWind(this.clockTime, "R");
    }


    [UnityEngine.SerializeField, Range(100, 10000)] public double stiffness = 100;

    [UnityEngine.SerializeField] private Epos4Main epos4Main;
    public enum Status {
        stop, walking
    }

    private void Start() {
    }


    [UnityEngine.Header("Stock Tilt Conf")]
    public ESP32Main esp32Main;
    private float degreePerPulse = 0.0072f; //[degrees/pulse]
    public string sendText;
    [UnityEngine.Header("Position Unit (deg), Absolute, Backward Positive, Forward Negative")]
    [UnityEngine.SerializeField, UnityEngine.Range(0, 10)] public int wait1 = 0;
    [UnityEngine.SerializeField, UnityEngine.Range(-10, 10)] public int position1 = 2;
    [UnityEngine.SerializeField, UnityEngine.Range(1, 10)] public int duration1 = 1;
     [UnityEngine.SerializeField, UnityEngine.Range(0, 10)] public int wait2 = 0;
    [UnityEngine.SerializeField, UnityEngine.Range(-10, 10)] public int position2 = -7;
    [UnityEngine.SerializeField, UnityEngine.Range(1, 10)] public float duration2 = 1;
    [UnityEngine.SerializeField, UnityEngine.Range(0, 200)] public int waitFirstLeft = 0;
    [UnityEngine.SerializeField, UnityEngine.Range(0, 200)] public int waitFirstRight = 0;
    
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

    private void targetCalculate()//振幅値（mm）→出力パルス変換
    {
        //目標パルスを整数型で格納
        if (this.activateLeftTilt) {
            this.targetPulseUp1[0] = (int)(-this.position1 / this.degreePerPulse);
            this.targetPulseDown1[0] = (int)(-this.position2 / this.degreePerPulse);
        }
        else {
            this.targetPulseUp1[0] = 0;
            this.targetPulseDown1[0] = 0;
        }
        this.targetPulseUp1[1] = 0;
        this.targetPulseDown1[1] = 0;
        if (this.activateRightTilt) {
            this.targetPulseUp1[2] = (int)(this.position1 / this.degreePerPulse);
            this.targetPulseDown1[2] = (int)(this.position2 / this.degreePerPulse);
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
            this.driveTimeUp1[0]   = (int) (this.period * (float)(this.duration1) / (float)(this.wait1 + this.duration1 + this.wait2 + this.duration2) * 1000f);
            this.driveTimeDown1[0] = (int) (this.period * (float)(this.duration2) / (float)(this.wait1 + this.duration1 + this.wait2 + this.duration2) * 1000f);
            this.delayTimeUp1[0]   = (int) (this.period * (float)(this.wait1)     / (float)(this.wait1 + this.duration1 + this.wait2 + this.duration2) * 1000f);
            this.delayTimeDown1[0] = (int) (this.period * (float)(this.wait1)     / (float)(this.wait1 + this.duration1 + this.wait2 + this.duration2) * 1000f);
        }
        else {
            this.driveTimeUp1[0] = 0;
            this.driveTimeDown1[0] = 0;
        }
        if (this.activateRightTilt) {
            this.driveTimeUp1[2]   = (int) (this.period * (float)(this.duration1) / (float)(this.wait1 + this.duration1 + this.wait2 + this.duration2) * 1000f);
            this.driveTimeDown1[2] = (int) (this.period * (float)(this.duration2) / (float)(this.wait1 + this.duration1 + this.wait2 + this.duration2) * 1000f);
            this.delayTimeUp1[2]   = (int) (this.period * (float)(this.wait1)     / (float)(this.wait1 + this.duration1 + this.wait2 + this.duration2) * 1000f);
            this.delayTimeDown1[2] = (int) (this.period * (float)(this.wait1)     / (float)(this.wait1 + this.duration1 + this.wait2 + this.duration2) * 1000f);
        }
        else {
            this.driveTimeUp1[2] = 0;
            this.driveTimeDown1[2] = 0;
        }
        
        this.delayTimeFirst[0] = (int)((float) this.period * this.waitFirstLeft/100f * 1000f);
        this.delayTimeFirst[2] = (int)((float) this.period * this.waitFirstRight/100f * 1000f);

        // //目標パルスを整数型で格納
        // if (this.activateLeftTilt) {
        //     this.targetPulseUp1[0] = (int)(-this.tiltBackwardScaled / this.degreePerPulse);
        //     this.targetPulseDown1[0] = (int)(-this.tiltForwardScaled / this.degreePerPulse);
        // }
        // else {
        //     this.targetPulseUp1[0] = 0;
        //     this.targetPulseDown1[0] = 0;
        // }
        // this.targetPulseUp1[1] = 0;
        // this.targetPulseDown1[1] = 0;
        // if (this.activateRightTilt) {
        //     this.targetPulseUp1[2] = (int)(this.tiltBackwardScaled / this.degreePerPulse);
        //     this.targetPulseDown1[2] = (int)(this.tiltForwardScaled / this.degreePerPulse);
        // }
        // else {
        //     this.targetPulseUp1[2] = 0;
        //     this.targetPulseDown1[2] = 0;
        // }
        // this.targetPulseUp1[3] = 0;
        // this.targetPulseDown1[3] = 0;
        // this.targetPulseUp1[4] = 0;
        // this.targetPulseDown1[4] = 0;
        // this.targetPulseUp1[5] = 0;
        // this.targetPulseDown1[5] = 0;
        // this.seatRotationPulse = 0;
        // if (this.activateLeftTilt) {
        //     this.leftTiltDriveTimeBackward = this.period * (this.tiltBackwardTimeRatio) / (this.tiltBackwardTimeRatio + this.tiltForwardDelayRatio + this.tiltForwardTimeRatio);
        //     this.driveTimeUp1[0] = (int)(this.leftTiltDriveTimeBackward * 1000f);
        //     this.leftTiltDriveTimeForward = this.period * (this.tiltForwardTimeRatio) / (this.tiltBackwardTimeRatio + this.tiltForwardDelayRatio + this.tiltForwardTimeRatio);
        //     this.driveTimeDown1[0] = (int)(this.leftTiltDriveTimeForward * 1000f);
        //     this.delayTimeUp1[0] = (int)(this.leftTiltDelayTimeBackward * 1000f);
        //     this.leftTiltDelayTimeForward = this.period * (this.tiltForwardDelayRatio) / (this.tiltBackwardTimeRatio + this.tiltForwardDelayRatio + this.tiltForwardTimeRatio);
        //     this.delayTimeDown1[0] = (int)(this.leftTiltDelayTimeForward * 1000f);
        // }
        // else {
        //     this.driveTimeUp1[0] = 0;
        //     this.driveTimeDown1[0] = 0;
        // }
        // if (this.activateRightTilt) {
        //     this.rightTiltDriveTimeBackward = this.period * (this.tiltBackwardTimeRatio) / (this.tiltBackwardTimeRatio + this.tiltForwardDelayRatio + this.tiltForwardTimeRatio);
        //     this.driveTimeUp1[2] = (int)(this.rightTiltDriveTimeBackward * 1000f);
        //     this.rightTiltDriveTimeForward = this.period * (this.tiltForwardTimeRatio) / (this.tiltBackwardTimeRatio + this.tiltForwardDelayRatio + this.tiltForwardTimeRatio);
        //     this.driveTimeDown1[2] = (int)(this.rightTiltDriveTimeForward * 1000f);
        //     this.delayTimeUp1[2] = (int)(this.rightTiltDelayTimeBackward * 1000f);
        //     this.rightTiltDelayTimeForward = this.period * (this.tiltForwardDelayRatio) / (this.tiltBackwardTimeRatio + this.tiltForwardDelayRatio + this.tiltForwardTimeRatio);
        //     this.delayTimeDown1[2] = (int)(this.rightTiltDelayTimeForward * 1000f);
        // }
        // else {
        //     this.driveTimeUp1[2] = 0;
        //     this.driveTimeDown1[2] = 0;
        // }
        
        // // this.startClockTimeLeftTilt = this.period*1.0/10.0;
        // this.startClockTimeLeftTilt = this.period*0.0/10.0;
        // this.delayTimeFirst[0] = (int)(startClockTimeLeftTilt * 1000.0);
        // // this.startClockTimeRightTilt = this.period*6.0/10.0;
        // this.startClockTimeRightTilt = this.period*5.0/10.0;
        // this.delayTimeFirst[2] = (int)(startClockTimeRightTilt * 1000.0);
    }

    private System.Threading.Thread th = null;
    private System.Timers.Timer getActualPositionTimer;
    private float[,] data;
    private int idata = 0;
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
        this.videoStartFlag = true;
        // this.video.Stop();
        // this.video.Play();
        // this.epos4Main.AllNodeDefinePosition();
        this.epos4Main.AllNodeActivateProfilePositionMode();
        if (this.walkStraightTimer != null) {
            this.walkStraightTimer.Stop();
            this.walkStraightTimer.Dispose();
        }

        this.walkStraightTimer = new System.Timers.Timer(this.delay*1000);
        this.walkStraightTimer.AutoReset = false;
        this.walkStraightTimer.Elapsed += (sender, e) => {
            if (this.coolingStatus == CoolingStatus.NowCooling) return;
            
            // this.th = new System.Threading.Thread(new System.Threading.ThreadStart(this.getActualPositionAsync));
            // this.th.Start();
            this.data = new float[10000,18];
            this.idata = 0;
            this.getActualPositionTimer = new System.Timers.Timer(100);
            this.getActualPositionTimer.AutoReset = true;
            this.getActualPositionTimer.Elapsed += this.getActualPositionCallback;
            this.getActualPositionTimer.Start();

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
            this.lifter.init(this.epos4Main.lifter, this.period/2, this.scaledLength.lift, 0);
            this.leftPedalYaw.init(this.epos4Main.leftPedalYaw, this.period, -this.scaledLength.pedalYaw, this.scaledLength.pedalYaw);
            this.leftPedal.init(this.epos4Main.leftPedal, this.period, this.scaledLength.pedal, 0);
            this.leftSlider.init(this.epos4Main.leftSlider, this.period, this.scaledLength.legForward, -this.scaledLength.legBackward);
            this.rightPedalYaw.init(this.epos4Main.rightPedalYaw, this.period, -this.scaledLength.pedalYaw, this.scaledLength.pedalYaw);
            this.rightPedal.init(this.epos4Main.rightPedal, this.period, this.scaledLength.pedal, 0);
            this.rightSlider.init(this.epos4Main.rightSlider, this.period, this.scaledLength.legForward, -this.scaledLength.legBackward);
            this.stockLeftExtend.init(this.epos4Main.stockLeftExtend, this.period, this.scaledLength.stockExtendTopPoint, this.scaledLength.stockExtendPokePoint, 0);
            this.stockRightExtend.init(this.epos4Main.stockRightExtend, this.period, this.scaledLength.stockExtendTopPoint, this.scaledLength.stockExtendPokePoint, 0);
            // this.stockLeftExtend.init(this.epos4Main.stockLeftExtend, this.period, this.scaledLength.stockExtendTopPoint, 0);
            // this.stockRightExtend.init(this.epos4Main.stockRightExtend, this.period, this.scaledLength.stockExtendTopPoint, 0);
            this.stockLeftSlider.init(this.epos4Main.stockLeftSlider, this.period, this.scaledLength.stockSlideForward, -this.scaledLength.stockSlideBackward);
            this.stockRightSlider.init(this.epos4Main.stockRightSlider, this.period, this.scaledLength.stockSlideForward, -this.scaledLength.stockSlideBackward);
            this.windLeft.initWind(this.esp32Wind, this.period);
            this.windRight.initWind(this.esp32Wind, this.period);
            this.trekkingTimer = new System.Timers.Timer(10);
            this.trekkingTimer.AutoReset = true;
            this.trekkingTimer.Elapsed += this.timerCallback;
            this.trekkingTimer.Start();
            this.walkStopTimer.Start();
        };
        this.walkStopTimer = new System.Timers.Timer(this.ExperienceTime*1000f);
        this.walkStopTimer.AutoReset = false;
        this.walkStopTimer.Elapsed += (sender, e) => {
            this.WalkStop();
        };
        this.walkStraightTimer.Start();
    }

    private System.Timers.Timer walkStopTimer;

    public void WalkStop() {
        this.trekkingTimer?.Stop();
        this.trekkingTimer?.Dispose();
        this.walkStopTimer?.Stop();
        this.walkStopTimer?.Dispose();
        this.getActualPositionTimer?.Stop();
        this.getActualPositionTimer?.Dispose();
        this.walkStraightTimer?.Stop();
        this.walkStraightTimer?.Dispose();
        this.coolingTimer?.Stop();
        this.coolingTimer?.Dispose();
        UnityEngine.Debug.Log("WalkStop");
        this.epos4Main.AllNodeMoveToHome();
        if (this.status == Status.walking && this.coolingStatus == CoolingStatus.Readied) {
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

            this.esp32Wind.SendText("Se");
        }
        this.pauseFlag = true;
        this.status = Status.stop;
        this.videoStopFlag = true;
    }

    private void getActualPositionCallback(object source, System.Timers.ElapsedEventArgs e) {
        if (this.Destroied || this.idata >= 10000 || this.status == Status.stop) {
            this.getActualPositionTimer?.Stop();
            this.getActualPositionTimer?.Dispose();
            int N = this.idata;
            System.IO.StreamWriter sw; // これがキモらしい
            System.IO.FileInfo fi;
            // Aplication.dataPath で プロジェクトファイルがある絶対パスが取り込める
            System.DateTime dt = System.DateTime.Now;
            string result = dt.ToString("yyyyMMddHHmmss");
            fi = new System.IO.FileInfo(UnityEngine.Application.dataPath + "/Scripts/log/current" + result + ".csv");
            sw = fi.AppendText();
            sw.WriteLine("time (s), lifter (1cm), left pedal pos (1cm), left slider pos (1cm), right pedal pos (1cm), right slider pos (1cm), stock left extend pos (1cm), stock left slider pos (1cm), stock right extend pos (10cm), stock right slider pos (1cm), lifter current (A), left pedal current (A), left slider current (A), right pedal current (A), right slider current (A), stock left extend current (A), stock left slider current (A), stock right extend current (A), stock right slider current (A)");
            for (int i = 0; i < N; i++)
            {
                float time = (float)i*0.10f;
                string a = time.ToString() + ",";
                for (int j = 0; j < 18; j++) {
                    a += this.data[i,j].ToString() + ",";
                }
                sw.WriteLine(a);
            }
            sw.Flush();
            sw.Close();
            return;
        }
        this.data[this.idata,0] = this.epos4Main.lifter.getPositionMMFloat() / 10f; // Unit 10cm
        // data[this.idata,1] = this.epos4Main.leftPedal.getPositionMMFloat() / 10f;
        // data[this.idata,2] = this.epos4Main.leftSlider.getPositionMMFloat() / 10f;
        // data[this.idata,3] = this.epos4Main.rightPedal.getPositionMMFloat() / 10f;
        // data[this.idata,4] = this.epos4Main.rightSlider.getPositionMMFloat() / 10f;
        this.data[this.idata,5] = this.epos4Main.stockLeftExtend.getPositionMMFloat() / 10f;
        this.data[this.idata,6] = this.epos4Main.stockLeftSlider.getPositionMMFloat() / 10f;
        this.data[this.idata,7] = this.epos4Main.stockRightExtend.getPositionMMFloat() / 10f;
        this.data[this.idata,8] = this.epos4Main.stockRightSlider.getPositionMMFloat() / 10f;

        this.data[this.idata,9] = this.epos4Main.lifter.current;
        this.data[this.idata,10] = this.epos4Main.leftPedal.current;
        this.data[this.idata,11] = this.epos4Main.leftSlider.current;
        this.data[this.idata,12] = this.epos4Main.rightPedal.current;
        this.data[this.idata,13] = this.epos4Main.rightSlider.current;
        this.data[this.idata,14] = this.epos4Main.stockLeftExtend.current;
        this.data[this.idata,15] = this.epos4Main.stockLeftSlider.current;
        this.data[this.idata,16] = this.epos4Main.stockRightExtend.current;
        this.data[this.idata,17] = this.epos4Main.stockRightSlider.current;
        this.idata++;
    }

    // private void getActualPositionAsync() {
    //     int N = 10000;
    //     float[,] data = new float[N,18];
    //     int i = 0;
    //     while (!this.Destroied && i < N && this.status == Status.walking) {
    //         data[i,0] = this.epos4Main.lifter.getPositionMMFloat() / 10f; // Unit 10cm
    //         data[i,1] = this.epos4Main.leftPedal.getPositionMMFloat() / 10f;
    //         data[i,2] = this.epos4Main.leftSlider.getPositionMMFloat() / 10f;
    //         data[i,3] = this.epos4Main.rightPedal.getPositionMMFloat() / 10f;
    //         data[i,4] = this.epos4Main.rightSlider.getPositionMMFloat() / 10f;
    //         data[i,5] = this.epos4Main.stockLeftExtend.getPositionMMFloat() / 10f;
    //         data[i,6] = this.epos4Main.stockLeftSlider.getPositionMMFloat() / 10f;
    //         data[i,7] = this.epos4Main.stockRightExtend.getPositionMMFloat() / 10f;
    //         data[i,8] = this.epos4Main.stockRightSlider.getPositionMMFloat() / 10f;

    //         data[i,9] = this.epos4Main.lifter.current;
    //         data[i,10] = this.epos4Main.leftPedal.current;
    //         data[i,11] = this.epos4Main.leftSlider.current;
    //         data[i,12] = this.epos4Main.rightPedal.current;
    //         data[i,13] = this.epos4Main.rightSlider.current;
    //         data[i,14] = this.epos4Main.stockLeftExtend.current;
    //         data[i,15] = this.epos4Main.stockLeftSlider.current;
    //         data[i,16] = this.epos4Main.stockRightExtend.current;
    //         data[i,17] = this.epos4Main.stockRightSlider.current;

    //         i++;

    //         System.Threading.Thread.Sleep(50);
    //     }

    //     N = i;

    //     System.IO.StreamWriter sw; // これがキモらしい
    //     System.IO.FileInfo fi;
    //     　　// Aplication.dataPath で プロジェクトファイルがある絶対パスが取り込める
    //     System.DateTime dt = System.DateTime.Now;
    //     string result = dt.ToString("yyyyMMddHHmmss");
    //     fi = new System.IO.FileInfo(UnityEngine.Application.dataPath + "/Scripts/log/current" + result + ".csv");
    //     sw = fi.AppendText();
    //     sw.WriteLine("time (s), lifter (1cm), left pedal pos (1cm), left slider pos (1cm), right pedal pos (1cm), right slider pos (1cm), stock left extend pos (1cm), stock left slider pos (1cm), stock right extend pos (10cm), stock right slider pos (1cm), lifter current (A), left pedal current (A), left slider current (A), right pedal current (A), right slider current (A), stock left extend current (A), stock left slider current (A), stock right extend current (A), stock right slider current (A)");
    //     for (i = 0; i < N; i++)
    //     {
    //         float time = (float)i*0.05f;
    //         string a = time.ToString() + ",";
    //         for (int j = 0; j < 18; j++) {
    //             a += data[i,j].ToString() + ",";
    //         }
    //         sw.WriteLine(a);
    //     }
    //     sw.Flush();
    //     sw.Close();
    //     return;
    // }
    
    [UnityEngine.SerializeField, ReadOnly] private UnityEngine.Vector2 thumbStickR;
    [UnityEngine.SerializeField, ReadOnly] private UnityEngine.Vector2 thumbStickL;


    private bool pauseFlag = false;
    private bool audioLeftFlag = false;
    private bool audioRightFlag = false;
     private bool videoStartFlag = false;
    private bool videoStopFlag = false;

    private void Update() {
        // this.thumbStickR = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
        // this.thumbStickL = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
        // if (System.Math.Abs(this.thumbStickR.y) > 0.5 && System.Math.Abs(this.thumbStickL.y) > 0.5) {
        // }
        // else if (this.thumbStickR.y > 0.5 || this.thumbStickL.y > 0.5) {
        //     this.WalkStraight();
        // }
        // else if (this.thumbStickR.y < -0.5 || this.thumbStickL.y < -0.5) {
        //     this.WalkStop();
        // }

        // if (this.video != null) {
        //     if (this.status == Status.stop & this.video.videoPlayer.isPlaying & this.pauseFlag) {
        //         this.video?.Pause();
        //         this.pauseFlag = false;
        //     }
        // }

        // this.scaledLength.lift = this.length.lift * this.scale;
        // this.scaledLength.stockExtendTopPoint = this.length.stockExtendTopPoint * this.scale;
        // this.scaledLength.stockExtendPokePoint = this.length.stockExtendPokePoint * this.scale;
        // this.scaledLength.stockSlideForward = this.length.stockSlideForward * this.scale;
        // this.scaledLength.stockSlideBackward = this.length.stockSlideBackward * this.scale;
        // this.tiltBackwardScaled = this.tiltBackward * this.scale;
        // this.tiltForwardScaled = this.tiltForward * this.scale;

        if (this.videoStartFlag) {
            this.videoControl.Stop();
            this.videoControl.Play();
            for (int i = 0; i < this.otherVideo.Length; i++) {
                this.otherVideo[i].Stop();
            }
            this.videoStartFlag = false;
        }

        if (this.videoStopFlag) {
            this.videoControl.Stop();
            this.videoStopFlag = false;
        }

        if (this.audioLeftFlag) {
            if (this.audioLeftSource != null) {
                this.audioLeftSource.Play();
            }
            this.audioLeftFlag = false;
        }
        if (this.audioRightFlag) {
            if (this.audioRightSource != null) {
                this.audioRightSource.Play();
            }
            this.audioRightFlag = false;
        }
    }

    private bool Destroied = false;

    private void OnDestroy() {
        this.WalkStop();
        this.Destroied = true;
    }
}