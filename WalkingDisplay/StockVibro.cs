using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockVibro : UnityEngine.MonoBehaviour {
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
        [UnityEngine.SerializeField, UnityEngine.Header("目標位置 (mm)")] public double position;
        public Trajectory(double arg_clockTime, double arg_position) {
            this.clockTime = arg_clockTime;
            this.position  = arg_position;
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
        private StockVibro stockVibro;

        public void init(Epos4Node arg_epos4Node, StockVibro arg_stockVibro, string arg_name) {
            this.epos4Node = arg_epos4Node;
            this.stockVibro = arg_stockVibro;
            this.name = arg_name;
            this.index = 0;
            this.timers = new System.Timers.Timer[this.trajectories.Count];
            for (int i = 0; i < this.trajectories.Count - 1; i++) {        
                this.timers[i] = new System.Timers.Timer((this.trajectories[i+1].clockTime - this.trajectories[i].clockTime)*1000.0);
                this.timers[i].AutoReset = false; // 一回のみ．繰り返し無し．
                this.timers[i].Elapsed += this.timerCallback;
            }
        }
        
        private void timerCallback(object source, System.Timers.ElapsedEventArgs e) {
            if (this.stockVibro.status == Status.stop) {
                this.stop();
                return;
            }
            this.timers[this.index].Stop();
            this.timers[this.index].Dispose();
            this.index++;
            if (this.index + 1 > this.trajectories.Count - 1) {
                return;
            }
            this.timers[this.index].Start();
            this.epos4Node.MoveToPositionInTimeWithLinear(
                this.trajectories[this.index+1].position,
                this.trajectories[this.index+1].clockTime - this.trajectories[this.index].clockTime,
                this.activate
            );
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
                            this.trajectories.Add(new Trajectory(System.Convert.ToDouble(splitedLine[0]), System.Convert.ToDouble(splitedLine[1])));
                        }
                        count++;
                    }
                };
            }
            catch (UnityEngine.AddressableAssets.InvalidKeyException) {

            }
        }

        public void start(bool arg_activate) {
            this.index = 0;
            this.activate = arg_activate;
            this.timers[this.index].Start();
            this.epos4Node.MoveToPositionInTimeWithLinear(
                this.trajectories[this.index+1].clockTime - this.trajectories[this.index].clockTime,
                this.trajectories[this.index+1].position,
                this.activate
            );
        }

        public void stop() {
            for (int i = 0; i < this.trajectories.Count - 1; i++) {
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