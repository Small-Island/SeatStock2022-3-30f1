using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingDisplayMain : UnityEngine.MonoBehaviour {
    // [UnityEngine.SerializeField, UnityEngine.Header("歩行周期 (s)"), Range(2f, 10f)] public float period = 10f;
    // [UnityEngine.HideInInspector, UnityEngine.SerializeField, Range(1, 10)] public int forwardRate = 5;
    // [UnityEngine.HideInInspector, UnityEngine.SerializeField, Range(1, 10)] public int backwardRate = 5;
    // [UnityEngine.SerializeField] private UnityEngine.AddressableAssets.AssetReference csvFile;

    [UnityEngine.SerializeField] public Activate activate;

    [System.Serializable]
    public class Activate {
        // Unit mm
        [UnityEngine.SerializeField] public bool lifter           = false;
        [UnityEngine.SerializeField] public bool leftPedal        = false;
        [UnityEngine.SerializeField] public bool leftSlider       = false;
        [UnityEngine.SerializeField] public bool rightPedal       = false;
        [UnityEngine.SerializeField] public bool rightSlider      = false;
        [UnityEngine.SerializeField] public bool stockLeftExtend  = false;
        [UnityEngine.SerializeField] public bool stockLeftSlider  = false;
        [UnityEngine.SerializeField] public bool stockRightExtend = false;
        [UnityEngine.SerializeField] public bool stockRightSlider = false;
    }
    [UnityEngine.Header("Unit mm")]
    private Length length;
    // [UnityEngine.SerializeField, Range(0.3f, 10f)] public float drop = 3;

    [System.Serializable]
    public class Length {
        // Unit mm
        [UnityEngine.SerializeField, Range(0, 30)] public double lift = 1;
        [UnityEngine.SerializeField, Range(0, 68)] public double pedal = 1;
        [UnityEngine.SerializeField, Range(0, 190)] public double legSlider = 1;
        [UnityEngine.SerializeField, Range(0, 100)] public double stockExtend = 1;
        [UnityEngine.SerializeField, Range(0, 500)] public double stockSlider = 1;
    }

    [UnityEngine.SerializeField, Range(0.1f, 20)] public double stiffness = 1;

    [UnityEngine.SerializeField] private Epos4Main epos4Main;
    public enum Status {
        stop, walking
    }
    [UnityEngine.SerializeField, ReadOnly] public Status status;

    private void Start() {
        // this.trajectories = new trajectories()
    }

    private bool walkstop = false;
    private System.Timers.Timer timer_walk, timer_walkstop;
    private System.Timers.Timer timer_clock;
    // [UnityEngine.Header("時刻 (s)")] public double clockTime;
    // [UnityEngine.Header("体験時間 (s)")] public double LimitClockTime = 30;
    private System.Timers.Timer timer_seat, timer_leftPedal, timer_leftSlider, timer_rightPedal, timer_rightSlider, timer_stockLeftExtend, timer_stockLeftSlider, timer_stockRightExtend, timer_stockRightSlider;

    public MotorTrajectory lifter, leftPedal, leftSlider, rightPedal, rightSlider, stockLeftExtend, stockLeftSlider, stockRightExtend, stockRightSlider;

    [System.Serializable]
    public class Trajectory {
        [UnityEngine.SerializeField, UnityEngine.Header("指令時刻 (s)")] public double clockTime;
        [UnityEngine.SerializeField, UnityEngine.Header("動作時間 (s)")] public double deltaTime;
        [UnityEngine.SerializeField, UnityEngine.Header("目標位置 (mm)")] public double position;
        public Trajectory(double arg_clockTime, double arg_deltaTime, double arg_position) {
            this.clockTime = arg_clockTime;
            this.deltaTime = arg_deltaTime;
            this.position = arg_position;
        }
    }

    [System.Serializable]
    public class MotorTrajectory {
        [UnityEngine.SerializeField] public UnityEngine.AddressableAssets.AssetReference csvFile;
        [UnityEngine.SerializeField] public List<Trajectory> trajectories;
        [UnityEngine.SerializeField] public int index = 0;

        private System.Timers.Timer[] timers;
        private Epos4Node epos4Node;
        private bool activate = false;
        private bool zeroClockStart = false;
        private WalkingDisplayMain walkingDisplayMain;

        public void init(Epos4Node arg_epos4Node, WalkingDisplayMain arg_walkingDisplayMain) {
            this.epos4Node = arg_epos4Node;
            this.walkingDisplayMain = arg_walkingDisplayMain;
            this.zeroClockStart = false;
            this.index = 0;
            this.timers = new System.Timers.Timer[this.trajectories.Count];
            for (int i = 0; i < this.trajectories.Count; i++) {
                if (i == 0 && this.trajectories[i].clockTime < 0.001) {
                    this.zeroClockStart = true;
                    continue;
                }

                if (i == 0) {
                    this.timers[i] = new System.Timers.Timer(this.trajectories[i].clockTime*1000.0);
                }
                else {
                    this.timers[i] = new System.Timers.Timer((this.trajectories[i].clockTime - this.trajectories[i-1].clockTime)*1000.0);
                }
                this.timers[i].AutoReset = false; // 一回のみ．繰り返し無し．
                this.timers[i].Elapsed += this.timerCallback;
            }
        }
        
        private void timerCallback(object source, System.Timers.ElapsedEventArgs e) {
            if (this.walkingDisplayMain.walkstop) {
                UnityEngine.Debug.Log("stop timercallback");
                return;
            }
            this.epos4Node.SetPositionProfileInTime(this.trajectories[this.index].position, this.trajectories[this.index].deltaTime, 1, 1);
            this.epos4Node.MoveToPosition(this.activate);
            this.index++;
            if (this.index == this.trajectories.Count) return;
            this.timers[this.index].Start();
        }

        public void AddressableLoad() {
            UnityEngine.TextAsset dataFile = new UnityEngine.TextAsset(); //テキストファイルの保持
            dataFile = null;
            string str = "";
            //Assetのロード
            UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<UnityEngine.TextAsset>(csvFile).Completed += op => {
                //ロードに成功
                this.trajectories = new List<Trajectory>();
                str = op.Result.text;
                System.IO.StringReader reader = new System.IO.StringReader(str);
                int count = 0;
                while (reader.Peek() != -1) // reader.Peaekが-1になるまで
                {
                    string line = reader.ReadLine(); // 一行ずつ読み込み
                    string[] splitedLine = line.Split(','); // , で分割
                    if (count > 0) {
                        this.trajectories.Add(new Trajectory(System.Convert.ToDouble(splitedLine[0]), System.Convert.ToDouble(splitedLine[1]), System.Convert.ToDouble(splitedLine[2])));
                    }
                    count++;
                }
            };
        }

        public void start(bool arg_activate) {
            this.index = 0;
            this.activate = arg_activate;
            if (this.timers.Length > 0) {
                if (this.zeroClockStart) {
                    this.epos4Node.SetPositionProfileInTime(this.trajectories[0].position, this.trajectories[0].deltaTime, 1, 1);
                    this.epos4Node.MoveToPosition(this.activate);
                    this.index++;   
                }
                if (this.index < this.timers.Length - 1) {
                    this.timers[this.index].Start();
                }
            }
        }

        public void stop() {
            for (int i = 0; i < 0; i++) {
                if (i > 0 || !this.zeroClockStart) {
                    this.timers[i].Stop();
                }
            }
        }

        public Trajectory getTrajectory() {
            return this.trajectories[this.index];
        }
    }

    public void init() {
        this.lifter.AddressableLoad();
        this.leftPedal.AddressableLoad();
        this.stockLeftSlider.AddressableLoad();
    }

    public void WalkStraight(float incdec_time)
    {
        if (this.status == Status.walking) return;
        this.status = Status.walking;
        this.walkstop = false;
        this.epos4Main.AllNodeDefinePosition();
        // this.stockLeftExtend.init(this.epos4Main.stockLeftExtend, this.activate.stockLeftExtend, this);
        this.lifter.init(this.epos4Main.lifter, this);
        this.leftPedal.init(this.epos4Main.leftPedal, this);
        this.stockLeftSlider.init(this.epos4Main.stockLeftSlider, this);
        this.lifter.start(this.activate.lifter);
        this.leftPedal.start(this.activate.leftPedal);
        this.stockLeftSlider.start(this.activate.stockLeftSlider);
        // this.stockLeftExtend.start();
        // this.clockTime = 0;
        // this.lifter.index = 0; this.leftPedal.index = 0; this.rightPedal.index = 0; this.rightSlider.index = 0; this.stockLeftExtend.index = 0; this.stockLeftSlider.index = 0; this.stockRightExtend.index = 0; this.stockRightSlider.index = 0;
        // this.trajectories = new Trajectory[] {seat, leftPedal, leftSlider, rightPedal, rightSlider, stockLeftExtend, stockLeftSlider, stockRightExtend, stockRightSlider};

        // this.stockLeftSlider.timer = new System.Timers.Timer(this.stockLeftSlider.getTrajectory().deltaTime);
        // this.stockLeftSlider.timer.Elapsed += (sender, e) => {
        //     this.epos4Main.stockLeftSlider.SetPositionProfileInTime(this.stockLeftSlider.getTrajectory().position, this.stockLeftSlider, this.getTrajectory().deltaTime);
        //     this.epos4Main.stockLeftSlider.MoveToPosition(this.activate.stockLeftSlider);
        //     this.stockLeftSlider.index++;
        //     this.stockLeftSlider.timer.Stop();
        // };
        // this.stockLeftSlider.timer.Start();

        // this.timer_clock = new System.Timers.Timer(10);
        // this.timer_clock.Elapsed += (sender, e) => {
        //     this.clockTime += 0.010;
        //     if (this.walkstop) {
        //         this.timer_clock.Stop();
        //         return;
        //     }
        //     if (this.clockTime > this.LimitClockTime && this.walkstop == false) {
        //         this.WalkStop();
        //         return;
        //     }

        //     if (this.seat.trajectory.Length > 0) {
        //         if (this.seat.index < this.seat.trajectory.Length - 1) {
        //             if (this.seat.getTrajectory().clockTime < this.clockTime) {
        //                 this.epos4Main.lifter.SetPositionProfileInTime(this.seat.getTrajectory().position, this.seat.getTrajectory().deltaTime, 1, 1);
        //                 this.epos4Main.lifter.MoveToPosition(this.activate.lifter);
        //                 this.seat.index++;
        //             }
        //         }
        //     }
        //     if (this.leftPedal.trajectory.Length > 0) {
        //         if (this.seat.index < this.leftPedal.trajectory.Length - 1) {
        //             if (leftPedal.getTrajectory().clockTime < this.clockTime) {
        //                 this.epos4Main.leftPedal.SetPositionProfileInTime(leftPedal.getTrajectory().position, leftPedal.getTrajectory().deltaTime, 1, 1);
        //                 this.epos4Main.leftPedal.MoveToPosition(this.activate.leftPedal);
        //                 this.leftPedal.index++;
        //             }
        //         }
        //     }
        //     if (leftSlider.getTrajectory().clockTime < this.clockTime) {
        //         this.epos4Main.leftSlider.SetPositionProfileInTime(leftSlider.getTrajectory().position, leftSlider.getTrajectory().deltaTime, 1, 1);
        //         this.epos4Main.leftSlider.MoveToPosition(this.activate.leftSlider);
        //         this.leftSlider.index++;
        //     }
        //     if (rightPedal.getTrajectory().clockTime < this.clockTime) {
        //         this.epos4Main.rightPedal.SetPositionProfileInTime(rightPedal.getTrajectory().position, rightPedal.getTrajectory().deltaTime, 1, 1);
        //         this.epos4Main.rightPedal.MoveToPosition(this.activate.rightPedal);
        //         this.rightPedal.index++;
        //     }
        //     if (rightSlider.getTrajectory().clockTime < this.clockTime) {
        //         this.epos4Main.rightSlider.SetPositionProfileInTime(rightSlider.getTrajectory().position, rightSlider.getTrajectory().deltaTime, 1, 1);
        //         this.epos4Main.rightSlider.MoveToPosition(this.activate.rightSlider);
        //         this.rightSlider.index++;
        //     }
        //     if (stockLeftExtend.getTrajectory().clockTime < this.clockTime) {
        //         this.epos4Main.stockLeftExtend.SetPositionProfileInTime(stockLeftExtend.getTrajectory().position, stockLeftExtend.getTrajectory().deltaTime, 1, 1);
        //         this.epos4Main.stockLeftExtend.MoveToPosition(this.activate.stockLeftExtend);
        //         this.stockLeftExtend.index++;
        //     }
        //     if (this.stockLeftSlider.getTrajectory().clockTime < this.clockTime) {
        //         this.epos4Main.stockLeftSlider.SetPositionProfileInTime(this.stockLeftSlider.getTrajectory().position, this.stockLeftSlider.getTrajectory().deltaTime, 1, 1);
        //         this.epos4Main.stockLeftSlider.MoveToPosition(this.activate.stockLeftSlider);
        //         this.stockLeftSlider.index++;
        //     }
        //     if (this.stockRightExtend.getTrajectory().clockTime < this.clockTime) {
        //         this.epos4Main.stockRightExtend.SetPositionProfileInTime(this.stockRightExtend.getTrajectory().position, this.stockRightExtend.getTrajectory().deltaTime, 1, 1);
        //         this.epos4Main.stockRightExtend.MoveToPosition(this.activate.stockRightExtend);
        //         this.stockRightExtend.index++;
        //     }
        //     if (this.stockRightSlider.getTrajectory().clockTime < this.clockTime) {
        //         this.epos4Main.stockRightSlider.SetPositionProfileInTime(stockRightSlider.getTrajectory().position, stockRightSlider.getTrajectory().deltaTime, 1, 1);
        //         this.epos4Main.stockRightSlider.MoveToPosition(this.activate.stockRightSlider);
        //         this.stockRightSlider.index++;
        //     }
        // };

        // this.timer_clock.Start();

        // UnityEngine.Debug.Log("clockTime: " + stockLeftSlider.getTrajectory().clockTime);

        // this.timer_walk = new System.Timers.Timer(0.25f*this.period*1000f);
        // this.timer_walk.Elapsed += (sender, e) => {
        //     this.time += 0.25f*this.period;
        //     if (this.time > 30 && this.walkstop == false) {
        //         this.WalkStop();
        //     }
        //     if (this.walkstop) {
        //         this.timer_walk.Stop();
        //     }
        //     else if (this.phase == 1) {
        //         this.phase++;
        //         this.status = Status.walking;
        //         // スライダ後退 3/4*period (秒), 左踵下降 1/4*period (秒)
        //         this.epos4Main.leftPedal.SetPositionProfileInTime(this.length.pedal, this.period*0.75f, 1, 1);
        //         this.epos4Main.leftPedal.MoveToPosition(this.activate.leftPedal);
        //         this.epos4Main.leftSlider.SetPositionProfileInTime(-this.length.legSlider*0.5, this.period*0.75f, 1, 1);
        //         this.epos4Main.leftSlider.MoveToPosition(this.activate.leftSlider);
        //         this.epos4Main.stockRightSlider.SetPositionProfileInTime(-this.length.stockSlider*0.5, this.period*0.75f, 1, 1);
        //         this.epos4Main.stockRightSlider.MoveToPosition(this.activate.stockRightSlider);
        //         this.epos4Main.stockRightExtend.SetPositionProfileInTime(0, this.period*0.75f, 1, 1);
        //         this.epos4Main.stockRightExtend.MoveToPosition(this.activate.stockRightExtend);
        //     }
        //     else if (this.phase == 2) {
        //         this.phase++;
        //         this.status = Status.walking;
        //         this.epos4Main.rightPedal.SetPositionProfileInTime(0, this.period*0.25f, 4, 1);
        //         this.epos4Main.rightPedal.MoveToPosition(this.activate.rightPedal);
        //         this.epos4Main.rightSlider.SetPositionProfileInTime(this.length.legSlider*0.5, this.period*0.25f, 4, 1);
        //         this.epos4Main.rightSlider.MoveToPosition(this.activate.rightSlider);
        //         this.epos4Main.stockLeftSlider.SetPositionProfileInTime(this.length.stockSlider*0.5, this.period*0.25f, 4, 1);
        //         this.epos4Main.stockLeftSlider.MoveToPosition(this.activate.stockLeftSlider);
        //         this.epos4Main.stockLeftExtend.SetPositionProfileInTime(this.length.stockExtend, this.period*0.25f*0.4f, 1, 1);
        //         this.epos4Main.stockLeftExtend.MoveToPosition(this.activate.stockLeftExtend);
        //         System.Threading.Thread.Sleep((int)(1000f*this.period*0.25f*0.4f));
        //         this.epos4Main.stockLeftExtend.SetPositionProfileInTime(this.length.stockExtend*0.5f, this.period*0.25f*0.3f, 1, this.stiffness);
        //         this.epos4Main.stockLeftExtend.MoveToPosition(this.activate.stockLeftExtend);

        //         //次のphaseをあらかじめ SetPositionProfileInTime
        //         // this.epos4Main.lifter.SetPositionProfileInTime(0, this.period*0.25f);
        //     }
        //     else if (this.phase == 3) {
        //         this.phase++;
        //         this.status = Status.walking;
        //         this.epos4Main.rightPedal.SetPositionProfileInTime(this.length.pedal, this.period*0.75f, 4, 1);
        //         this.epos4Main.rightPedal.MoveToPosition(this.activate.rightPedal);
        //         this.epos4Main.rightSlider.SetPositionProfileInTime(-this.length.legSlider*0.5, this.period*0.75f, 1, 1);
        //         this.epos4Main.rightSlider.MoveToPosition(this.activate.rightSlider);
        //         this.epos4Main.stockLeftSlider.SetPositionProfileInTime(-this.length.stockSlider*0.5, this.period*0.75f, 1, 1);
        //         this.epos4Main.stockLeftSlider.MoveToPosition(this.activate.stockLeftSlider);
        //         this.epos4Main.stockLeftExtend.SetPositionProfileInTime(0, this.period*0.75f, 1, 1);
        //         this.epos4Main.stockLeftExtend.MoveToPosition(this.activate.stockLeftExtend);

        //         //次のphaseをあらかじめ SetPositionProfileInTime
        //         // this.epos4Main.lifter.SetPositionProfileInTime(this.length.lift, this.period*0.25f);
        //     }
        //     else if (this.phase == 4) {
        //         this.phase = 1;
        //         this.status = Status.walking;
        //         // スライダ前進 1/4*Period (秒)
        //         this.epos4Main.leftPedal.SetPositionProfileInTime(0, this.period*0.25f, 4, 1);
        //         this.epos4Main.leftPedal.MoveToPosition(this.activate.leftPedal);
        //         this.epos4Main.leftSlider.SetPositionProfileInTime(this.length.legSlider, this.period*0.25f, 4, 1);
        //         this.epos4Main.leftSlider.MoveToPosition(this.activate.leftSlider);
        //         this.epos4Main.stockRightSlider.SetPositionProfileInTime(this.length.stockSlider*0.5, this.period*0.25f, 4, 1);
        //         this.epos4Main.stockRightSlider.MoveToPosition(this.activate.stockRightSlider);
        //         this.epos4Main.stockRightExtend.SetPositionProfileInTime(this.length.stockExtend, this.period*0.25f*0.4f, 1, 1);
        //         this.epos4Main.stockRightExtend.MoveToPosition(this.activate.stockRightExtend);
        //         System.Threading.Thread.Sleep((int)(1000f*this.period*0.25f*0.4f));
        //         this.epos4Main.stockRightExtend.SetPositionProfileInTime(this.length.stockExtend*0.5f, this.period*0.25f*0.3f, 1, this.stiffness);
        //         this.epos4Main.stockRightExtend.MoveToPosition(this.activate.stockRightExtend);
        //     }
        // };

        // // System.Threading.Thread.Sleep((int)(5000));
        // this.phase = 0;
        // this.time = 0;
        // this.timer_walk.Start();
        // this.phase++;
        // this.status = Status.walking;
        // // スライダ前進 1/4*Period (秒)
        // this.epos4Main.leftPedal.SetPositionProfileInTime(0, this.period*0.25f, 4, 1);
        // this.epos4Main.leftPedal.MoveToPosition(this.activate.leftPedal);
        // this.epos4Main.leftSlider.SetPositionProfileInTime(this.length.legSlider*0.5, this.period*0.25f, 4, 1);
        // this.epos4Main.leftSlider.MoveToPosition(this.activate.leftSlider);
        // this.epos4Main.stockRightSlider.SetPositionProfileInTime(this.length.stockSlider*0.5, this.period*0.25f, 4, 1);
        // this.epos4Main.stockRightSlider.MoveToPosition(this.activate.stockRightSlider);
        // this.epos4Main.stockRightExtend.SetPositionProfileInTime(this.length.stockExtend, this.period*0.25f*0.4f, 1, 1);
        // this.epos4Main.stockRightExtend.MoveToPosition(this.activate.stockRightExtend);
        // System.Threading.Thread.Sleep((int)(1000f*this.period*0.25f*0.4f));
        // this.epos4Main.stockRightExtend.SetPositionProfileInTime(this.length.stockExtend*0.5f, this.period*0.25f*0.3f, 1, this.stiffness);
        // this.epos4Main.stockRightExtend.MoveToPosition(this.activate.stockRightExtend);
    }

    public void WalkStop()
    {
        // UnityEngine.Debug.Log("Walk stop");
        // LegCoroutines.stop(this);
        // LegThreads.stop();
        this.status = Status.stop;
        this.walkstop = true;
        this.epos4Main.AllNodeMoveStop();    
        this.epos4Main.AllNodeMoveToHome();
    }

    private void OnDestroy()
    {
        this.walkstop = true;
        this.WalkStop();
    }
}