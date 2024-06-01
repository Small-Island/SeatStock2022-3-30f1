using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingDisplayMain : UnityEngine.MonoBehaviour {
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
        this.lifter.AddressableLoad();
        this.leftPedal.AddressableLoad();
        this.rightPedal.AddressableLoad();
        this.leftSlider.AddressableLoad();
        this.rightSlider.AddressableLoad();
        this.stockLeftExtend.AddressableLoad();
        this.stockLeftSlider.AddressableLoad();
        this.stockRightExtend.AddressableLoad();
        this.stockRightSlider.AddressableLoad();
    }

    private bool walkstop = false;
    public MotorTrajectory lifter, leftPedal, leftSlider, rightPedal, rightSlider, stockLeftExtend, stockLeftSlider, stockRightExtend, stockRightSlider;

    [System.Serializable]
    public class Trajectory {
        [UnityEngine.SerializeField, UnityEngine.Header("指令時刻 (s)")] public double clockTime;
        [UnityEngine.SerializeField, UnityEngine.Header("動作時間 (s)")] public double deltaTime;
        [UnityEngine.SerializeField, UnityEngine.Header("目標位置 (mm)")] public double position;
        // public Trajectory(double arg_clockTime, double arg_deltaTime, double arg_position, double arg_accel, double arg_decel) {
        //     this.clockTime = arg_clockTime;
        //     this.deltaTime = arg_deltaTime;
        //     this.position  = arg_position;
        //     this.accel     = arg_accel;
        //     this.decel     = arg_decel;
        // }
        public Trajectory(double arg_clockTime, double arg_deltaTime, double arg_position) {
            this.clockTime = arg_clockTime;
            this.deltaTime = arg_deltaTime;
            this.position  = arg_position;
        }
    }

    [System.Serializable]
    public class MotorTrajectory {
        [UnityEngine.SerializeField] public UnityEngine.AddressableAssets.AssetReference csvFile;
        [UnityEngine.SerializeField] public List<Trajectory> trajectories;
        
        private int index = 0;
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
                this.stop();
                return;
            }
            if (this.index + 1 < this.trajectories.Count) this.timers[this.index + 1].Start();
            this.epos4Node.SetPositionProfileInTime(this.trajectories[this.index].position, this.trajectories[this.index].deltaTime, 1, 1);
            this.epos4Node.MoveToPosition(this.activate);
            this.timers[this.index].Stop();
            this.timers[this.index].Dispose();
            this.index++;
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
                    this.timers[i].Dispose();
                }
            }
        }

        public Trajectory getTrajectory() {
            return this.trajectories[this.index];
        }
    }

    private System.Threading.Thread th = null;

    public void WalkStraight(float incdec_time) {
        if (this.status == Status.walking) return;
        this.status = Status.walking;
        this.walkstop = false;
        this.epos4Main.AllNodeDefinePosition();
        this.lifter.init(this.epos4Main.lifter, this);
        this.leftPedal.init(this.epos4Main.leftPedal, this);
        this.leftSlider.init(this.epos4Main.leftSlider, this);
        this.rightPedal.init(this.epos4Main.rightPedal, this);
        this.rightSlider.init(this.epos4Main.rightSlider, this);
        this.stockLeftExtend.init(this.epos4Main.stockLeftExtend, this);
        this.stockLeftSlider.init(this.epos4Main.stockLeftSlider, this);
        this.stockRightExtend.init(this.epos4Main.stockRightExtend, this);
        this.stockRightSlider.init(this.epos4Main.stockRightSlider, this);
        this.lifter.start(this.activate.lifter);
        this.leftPedal.start(this.activate.leftPedal);
        this.leftSlider.start(this.activate.leftSlider);
        this.rightPedal.start(this.activate.rightPedal);
        this.rightSlider.start(this.activate.rightSlider);
        this.stockLeftSlider.start(this.activate.stockLeftSlider);
        this.stockLeftExtend.start(this.activate.stockLeftExtend);
        this.stockRightExtend.start(this.activate.stockRightExtend);
        this.stockRightSlider.start(this.activate.stockRightSlider);
        this.th = new System.Threading.Thread(new System.Threading.ThreadStart(this.getActualPositionAsync));
        this.th.Start();
    }

    public void WalkStop() {
        this.status = Status.stop;
        this.walkstop = true;
        // this.epos4Main.AllNodeMoveStop();    
        this.epos4Main.AllNodeMoveToHome();
    }

    private void getActualPositionAsync() {
        int N = 1000;
        float[,] data = new float[N,18];
        int i = 0;
        while (!this.Destroied && i < N && !this.walkstop) {
            data[i,0] = this.epos4Main.lifter.actualPosition / 100f; // Unit 10cm
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
        sw.WriteLine("time (s), lifter (10cm), left pedal pos (10cm), left slider pos (10cm), right pedal pos (10cm), right slider pos (10cm), stock left extend pos (10cm), stock left slider pos (10cm), stock right extend pos (10cm), stock right slider pos (10cm), lifter current (A), left pedal current (A), left slider current (A), right pedal current (A), right slider current (A), stock left extend current (A), stock left slider current (A), stock right extend current (A), stock right slider current (A)");
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

    private bool Destroied = false;

    private void OnDestroy() {
        this.walkstop = true;
        this.WalkStop();
        this.Destroied = true;
    }
}