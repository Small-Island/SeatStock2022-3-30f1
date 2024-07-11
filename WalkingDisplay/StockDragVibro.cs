using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockDragVibro : UnityEngine.MonoBehaviour {
    [UnityEngine.SerializeField] public Activate activate;

    [System.Serializable]
    public class Activate {
        // Unit mm
        [UnityEngine.SerializeField] public bool stockLeftExtend  = false;
    }

    [UnityEngine.SerializeField] private Epos4Main epos4Main;
    public enum Status {
        stop, walking
    }
    
    [UnityEngine.SerializeField, ReadOnly] public Status status;
    private void Start() {
        this.stockLeftExtend.AddressableLoad();
        this.stopCSVLoad();
    }

    public MotorTrajectory stockLeftExtend;

    [System.Serializable]
    public class Trajectory {
        [UnityEngine.SerializeField, UnityEngine.Header("指令時刻 (s)")] public double clockTime;
        [UnityEngine.SerializeField, UnityEngine.Header("動作時間 (s)")] public double duration;
        [UnityEngine.SerializeField, UnityEngine.Header("目標位置 (mm)")] public double position;
        [UnityEngine.SerializeField, UnityEngine.Header("速度制御")] public bool useVelocityMode;
        public Trajectory(double arg_clockTime, double arg_duration, double arg_position, bool arg_useVelocityMode) {
            this.clockTime = arg_clockTime;
            this.duration = arg_duration;
            this.position  = arg_position;
            this.useVelocityMode = arg_useVelocityMode;
        }
    }

    [System.Serializable]
    public class MotorTrajectory {
        [UnityEngine.SerializeField] public UnityEngine.AddressableAssets.AssetReference csvFile;
        [UnityEngine.SerializeField] public bool useCsvFile;
        [UnityEngine.SerializeField] public List<Trajectory> trajectories;
        
        private string name = "";
        private int index = 0;
        private System.Timers.Timer[] timers;
        private Epos4Node epos4Node;
        private bool activate = false;
        private StockDragVibro stockDragVibro;

        public void init(Epos4Node arg_epos4Node, StockDragVibro arg_stockDragVibro, string arg_name) {
            this.epos4Node = arg_epos4Node;
            this.stockDragVibro = arg_stockDragVibro;
            this.name = arg_name;
            this.timers = new System.Timers.Timer[this.trajectories.Count];
            if (this.trajectories[0].clockTime == 0) {

            }
            else {
                this.timers[0] = new System.Timers.Timer((this.trajectories[0].clockTime)*1000.0);
                this.timers[0].AutoReset = false; // 一回のみ．繰り返し無し．
                this.timers[0].Elapsed += this.timerCallback;
            }
            for (int i = 1; i < this.trajectories.Count; i++) {        
                this.timers[i] = new System.Timers.Timer((this.trajectories[i].clockTime - this.trajectories[i-1].clockTime)*1000.0);
                this.timers[i].AutoReset = false; // 一回のみ．繰り返し無し．
                this.timers[i].Elapsed += this.timerCallback;
            }
        }
        
        private void timerCallback(object source, System.Timers.ElapsedEventArgs e) {
            if (this.stockDragVibro.status == Status.stop) {
                this.stop();
                return;
            }
            this.timers[this.index].Stop();
            this.timers[this.index].Dispose();
            if (this.index >= this.trajectories.Count - 1) {
                return;
            }
            this.timers[this.index+1].Start();
            if (this.trajectories[this.index].useVelocityMode) {
                // if (!this.trajectories[this.index-1].useVelocityMode) {
                //     this.epos4Node.ActivateVelocityMode();
                // }
                // this.epos4Node.SetVelocityMust(
                //     (this.trajectories[this.index+1].position - this.trajectories[this.index].position)/this.trajectories[this.index].duration
                // );
                this.epos4Node.MoveToPositionInTimeWithLinear(
                    this.trajectories[this.index].position,
                    this.trajectories[this.index].duration,
                    this.activate
                );
            }
            else {
                // if (this.index - 1 >= 0) {
                //     if (this.trajectories[this.index-1].useVelocityMode) {
                //         this.epos4Node.SetVelocityMust(0);
                //         this.epos4Node.ActivateProfilePositionMode();
                //     }
                // }
                this.epos4Node.SetPositionProfileInTime(this.trajectories[this.index].position, this.trajectories[this.index].duration, 1, 1);
                this.epos4Node.MoveToPosition(this.activate);
            }
            this.index++;
        }

        public void AddressableLoad() {
            if (!this.useCsvFile) {
                return;
            }
            UnityEngine.TextAsset dataFile = new UnityEngine.TextAsset(); //テキストファイルの保持
            dataFile = null;
            string str = "";
            //Assetのロード
            try {
                UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<UnityEngine.TextAsset>(this.csvFile).Completed += op => {
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
                            this.trajectories.Add(
                                new Trajectory(
                                    System.Convert.ToDouble(splitedLine[0]),
                                    System.Convert.ToDouble(splitedLine[1]),
                                    System.Convert.ToDouble(splitedLine[2]),
                                    System.Convert.ToBoolean(System.Convert.ToDouble(splitedLine[3]))
                                )
                            );
                        }
                        count++;
                    }
                };
            }
            catch (UnityEngine.AddressableAssets.InvalidKeyException) {

            }
        }

        public void start(bool arg_activate) {
            this.activate = arg_activate;
            if (this.trajectories[0].clockTime == 0) {
                this.timers[1].Start();
                if (this.trajectories[0].useVelocityMode) {
                    // this.epos4Node.MoveToPositionInTimeWithLinear(
                    //     this.trajectories[0].position,
                    //     this.trajectories[0].duration,
                    //     this.activate
                    // );
                    // this.epos4Node.ActivateProfileVelocityMode();
                    // this.epos4Node.VelocityCmd(
                    //     (this.trajectories[this.index].position - this.trajectories[this.index-1].position)/this.trajectories[this.index].duration
                    // );
                    // this.epos4Node.ActivateVelocityMode();
                    // this.epos4Node.SetPositionMust(
                    //     (this.trajectories[1].position - this.trajectories[this.index-1].position)/this.trajectories[this.index].duration
                    // );
                }
                else {
                    // this.epos4Node.ActivateProfilePositionMode();
                    this.epos4Node.SetPositionProfileInTime(this.trajectories[0].position, this.trajectories[0].duration, 1, 1);
                    this.epos4Node.MoveToPosition(this.activate);
                }
                this.index = 1;
            }
            else {
                this.timers[0].Start();
                this.index = 0;
            }
        }

        public void stop() {
            for (int i = 1; i < this.trajectories.Count - 1; i++) {
                this.timers[i].Stop();
                this.timers[i].Dispose();
            }
        }

        public Trajectory getTrajectory() {
            return this.trajectories[this.index];
        }
    }

    public UnityEngine.AddressableAssets.AssetReference csvFileStop;

    public float ExperienceTime = 0;

    private void stopCSVLoad() {
        UnityEngine.TextAsset dataFile = new UnityEngine.TextAsset(); //テキストファイルの保持
        dataFile = null;
        string str = "";
        //Assetのロード
        UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<UnityEngine.TextAsset>(this.csvFileStop).Completed += op => {
            //ロードに成功
            str = op.Result.text;
            System.IO.StringReader reader = new System.IO.StringReader(str);
            int count = 0;
            while (reader.Peek() != -1) // reader.Peaekが-1になるまで
            {
                string line = reader.ReadLine(); // 一行ずつ読み込み
                string[] splitedLine = line.Split(','); // , で分割
                if (count > 0) {
                    this.ExperienceTime = System.Convert.ToSingle(splitedLine[0])*1000f;
                }
                count++;
            }
        };
    }

    public System.Timers.Timer walkStopTimer;

    public void WalkStraight() {
        if (this.status == Status.walking) return;
        UnityEngine.Debug.Log("WalkStraight");
        this.status = Status.walking;
        this.stockLeftExtend.init(this.epos4Main.stockLeftExtend, this, "StockLeftExtend");
        this.epos4Main.AllNodeActivateProfilePositionMode();
        this.stockLeftExtend.start(this.activate.stockLeftExtend);
        this.walkStopTimer = new System.Timers.Timer(this.ExperienceTime);
        this.walkStopTimer.AutoReset = false;
        this.walkStopTimer.Elapsed += (sender, e) => {
            this.WalkStop();
        };
        this.walkStopTimer.Start();
    }

    public void WalkStop() {
        if (this.status == Status.stop) return;
        this.status = Status.stop;
        UnityEngine.Debug.Log("WalkStop");
        this.stockLeftExtend.stop();
        this.walkStopTimer?.Stop();
        this.walkStopTimer?.Dispose();
        this.epos4Main.AllNodeMoveToHome();
    }
    
    [UnityEngine.SerializeField, ReadOnly] private UnityEngine.Vector2 thumbStickR;
    [UnityEngine.SerializeField, ReadOnly] private UnityEngine.Vector2 thumbStickL;


    private bool pauseFlag = false;
    private bool audioLeftFlag = false;
    private bool audioRightFlag = false;

    private void Update() {
    }

    private bool Destroied = false;

    private void OnDestroy() {
        this.WalkStop();
        this.Destroied = true;
        this.walkStopTimer?.Stop();
        this.walkStopTimer?.Dispose();
    }
}