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
        [UnityEngine.SerializeField] public bool stockLeftTilt    = false;
        [UnityEngine.SerializeField] public bool stockRightTilt   = false;
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
        this.tiltCSVLoad();
        this.client = new System.IO.Ports.SerialPort(portName, baudRate, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
        this.client.Open();
    }

    public MotorTrajectory lifter, leftPedal, leftSlider, rightPedal, rightSlider, stockLeftExtend, stockLeftSlider, stockRightExtend, stockRightSlider;

    [System.Serializable]
    public class Trajectory {
        [UnityEngine.SerializeField, UnityEngine.Header("指令時刻 (s)")] public double clockTime;
        [UnityEngine.SerializeField, UnityEngine.Header("動作時間 (s)")] public double deltaTime;
        [UnityEngine.SerializeField, UnityEngine.Header("目標位置 (mm)")] public double position;
        [UnityEngine.SerializeField, UnityEngine.Header("硬度使用")] public double useStiffness;
        public Trajectory(double arg_clockTime, double arg_deltaTime, double arg_position, double arg_useStiffness) {
            this.clockTime = arg_clockTime;
            this.deltaTime = arg_deltaTime;
            this.position  = arg_position;
            this.useStiffness = arg_useStiffness;
        }
    }

    [System.Serializable]
    public class MotorTrajectory {
        [UnityEngine.SerializeField] public UnityEngine.AddressableAssets.AssetReference csvFile;
        [UnityEngine.SerializeField] public List<Trajectory> trajectories;
        
        private string name = "";
        private int index = 0;
        private System.Timers.Timer[] timers;
        private Epos4Node epos4Node;
        private bool activate = false;
        private bool zeroClockStart = false;
        private WalkingDisplayMain walkingDisplayMain;

        public void init(Epos4Node arg_epos4Node, WalkingDisplayMain arg_walkingDisplayMain, string arg_name) {
            this.epos4Node = arg_epos4Node;
            this.walkingDisplayMain = arg_walkingDisplayMain;
            this.name = arg_name;
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
            if (this.walkingDisplayMain.status == Status.stop) {
                this.stop();
                return;
            }
            if (this.index + 1 < this.trajectories.Count) this.timers[this.index + 1].Start();
            if (this.name == "stockSlider") {
                this.epos4Node.SetPositionProfileInTime(this.trajectories[this.index].position, this.trajectories[this.index].deltaTime, 5, 1);
            }
            else {
                this.epos4Node.SetPositionProfileInTime(this.trajectories[this.index].position, this.trajectories[this.index].deltaTime, 1, 1 + this.trajectories[this.index].useStiffness*this.walkingDisplayMain.stiffness);
            }
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
                        this.trajectories.Add(new Trajectory(System.Convert.ToDouble(splitedLine[0]), System.Convert.ToDouble(splitedLine[1]), System.Convert.ToDouble(splitedLine[2]), System.Convert.ToDouble(splitedLine[3])));
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
                    if (this.name == "stockSlider") {
                        this.epos4Node.SetPositionProfileInTime(this.trajectories[0].position, this.trajectories[0].deltaTime, 5, 1);
                    }
                    else {
                        this.epos4Node.SetPositionProfileInTime(this.trajectories[0].position, this.trajectories[0].deltaTime, 1, 1 + this.trajectories[0].useStiffness*this.walkingDisplayMain.stiffness);
                    }
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

    [UnityEngine.Header("Stock Tilt Conf")]
    [UnityEngine.SerializeField] public UnityEngine.AddressableAssets.AssetReference csvFileTilt;
    public string portName = "COM3";    
    public int baudRate = 9600;
    private System.IO.Ports.SerialPort client;
    private float degreePerPulse = 0.0072f; //[degrees/pulse]
    public string sendText;
    [UnityEngine.SerializeField, UnityEngine.Header("Unit (deg), Absolute, Backward Positive, Forward Negative"), UnityEngine.Range(0, 10)] public float tiltBackward = 0;
    [UnityEngine.SerializeField, UnityEngine.Range(-30, 0)] public float tiltForward = 0;
    [UnityEngine.SerializeField, UnityEngine.Header("Unit (s)"), UnityEngine.Range(2f, 10f)] public float period = 5;
    [UnityEngine.SerializeField, UnityEngine.Header("Tilt Backward Time Ratio"), UnityEngine.Range(1f, 5f)] public float tiltBackwardTimeRatio = 1;
    [UnityEngine.SerializeField, UnityEngine.Header("Tilt Forward  Time Ratio"), UnityEngine.Range(1f, 5f)] public float tiltForwardTimeRatio = 1;
    public bool doubleStock = false;
    public double startClockTimeLeftTilt = 0;
    public double startClockTimeRightTilt = 0;
    public double leftTiltDriveTimeBackward = 0;
    public double leftTiltDriveTimeForward = 0;
    public double rightTiltDriveTimeBackward = 0;
    public double rightTiltDelayTimeForward = 0;
    public double leftTiltDelayTimeBackward = 0;
    public double leftTiltDelayTimeForward = 0;
    public double rightTiltDelayTimeBackward = 0;
    public double rightTiltDelayTimeForward = 0;
    //出力パルス（送信）
    private int[] targetPulseUp1 = new int[6] { 0, 0, 0, 0, 0, 0 };//上昇／前進時の目標パルス（左ペダル、左スライダ、右ペダル、右スライダ）[pulse]
    private int[] targetPulseDown1 = new int[6] { 0, 0, 0, 0, 0, 0 };//下降／後退時の目標パルス（左ペダル、左スライダ、右ペダル、右スライダ）[pulse]
    //駆動時間（送信）
    private int[] driveTimeUp1 = new int[6] { 5000, 0, 5000, 0, 0, 0 };//上昇／前進時の駆動時間（左ペダル、左スライダ、右ペダル、右スライダ）[ms]
    private int[] driveTimeDown1 = new int[6] {5000, 0, 5000, 0, 0, 0 };//下降／後退時の駆動時間（左ペダル、左スライダ、右ペダル、右スライダ）[ms]
    //待機時間（送信）
    private int[] delayTimeUp1 = new int[6] { 0, 0, 0, 0, 0, 0 };//上昇／前進始めモータ停止時間（左ペダル、左スライダ、右ペダル、右スライダ）[ms]
    private int[] delayTimeDown1 = new int[6] { 0, 0, 0, 0, 0, 0 };//下降／後退始めモータ停止時間（左ペダル、左スライダ、右ペダル、右スライダ）[ms]
    private int[] delayTimeFirst = new int[6] { 2500, 0, 0, 0, 0, 0 };//一歩目モータ停止時間（左ペダル、左スライダ、右ペダル、右スライダ）[ms]
    private int seatRotationPulse;

    private void tiltCSVLoad() {
        UnityEngine.TextAsset dataFile = new UnityEngine.TextAsset(); //テキストファイルの保持
        dataFile = null;
        string str = "";
        //Assetのロード
        UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<UnityEngine.TextAsset>(this.csvFileTilt).Completed += op => {
            //ロードに成功
            str = op.Result.text;
            System.IO.StringReader reader = new System.IO.StringReader(str);
            int count = 0;
            while (reader.Peek() != -1) // reader.Peaekが-1になるまで
            {
                string line = reader.ReadLine(); // 一行ずつ読み込み
                string[] splitedLine = line.Split(','); // , で分割
                if (count > 0) {
                    this.period = System.Convert.ToSingle(splitedLine[0]);
                    this.tiltBackward = System.Convert.ToSingle(splitedLine[1]);
                    this.tiltForward = System.Convert.ToSingle(splitedLine[2]);
                    this.tiltBackwardTimeRatio = System.Convert.ToSingle(splitedLine[3]);
                    this.tiltForwardTimeRatio = System.Convert.ToSingle(splitedLine[4]);
                    this.startClockTimeLeftTilt = System.Convert.ToDouble(splitedLine[5]);
                    this.startClockTimeRightTilt = System.Convert.ToDouble(splitedLine[6]);
                    this.leftTiltDriveTimeBackward = System.Convert.ToDouble(splitedLine[7]);
                    this.leftTiltDriveTimeForward = System.Convert.ToDouble(splitedLine[8]);
                    this.rightTiltDriveTimeBackward = System.Convert.ToDouble(splitedLine[9]);
                    this.rightTiltDriveTimeForward = System.Convert.ToDouble(splitedLine[10]);
                    this.leftTiltDelayTimeBackward = System.Convert.ToDouble(splitedLine[11]);
                    this.leftTiltDelayTimeForward = System.Convert.ToDouble(splitedLine[12]);
                    this.rightTiltDelayTimeBackward = System.Convert.ToDouble(splitedLine[13]);
                    this.rightTiltDelayTimeForward = System.Convert.ToDouble(splitedLine[14]);
                }
                count++;
            }
        };
    }
    private void targetCalculate()//振幅値（mm）→出力パルス変換
    {
        //目標パルスを整数型で格納
        if (this.activate.stockLeftTilt) {
            this.targetPulseUp1[0] = (int)(-this.tiltBackward / this.degreePerPulse);
            this.targetPulseDown1[0] = (int)(-this.tiltForward / this.degreePerPulse);
        }
        else {
            this.targetPulseUp1[0] = 0;
            this.targetPulseDown1[0] = 0;
        }
        this.targetPulseUp1[1] = 0;
        this.targetPulseDown1[1] = 0;
        if (this.activate.stockRightTilt) {
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
        if (this.activate.stockLeftTilt) {
            this.driveTimeUp1[0] = (int)(this.leftTiltDriveTimeBackward * 1000f);
            this.driveTimeDown1[0] = (int)(this.leftTiltDriveTimeForward * 1000f);
            this.delayTimeUp1[0] = (int)(this.leftTiltDelayTimeBackward * 1000f);
            this.delayTimeDown1[0] = (int)(this.leftTiltDelayTimeForward * 1000f);
        }
        else {
            this.driveTimeUp1[0] = 0;
            this.driveTimeDown1[0] = 0;
        }
        if (this.activate.stockRightTilt) {
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
    [UnityEngine.SerializeField, ReadOnly] public CoolingStatus coolingStatus;

    public void WalkStraight() {
        if (this.status == Status.walking) return;
        if (this.coolingStatus == CoolingStatus.NowCooling) return;
        UnityEngine.Debug.Log("WalkStraight");
        this.status = Status.walking;
        this.epos4Main.AllNodeDefinePosition();
        this.lifter.init(this.epos4Main.lifter, this, "seat");
        this.leftPedal.init(this.epos4Main.leftPedal, this, "seat");
        this.leftSlider.init(this.epos4Main.leftSlider, this, "seat");
        this.rightPedal.init(this.epos4Main.rightPedal, this, "seat");
        this.rightSlider.init(this.epos4Main.rightSlider, this, "seat");
        this.stockLeftExtend.init(this.epos4Main.stockLeftExtend, this, "seat");
        this.stockLeftSlider.init(this.epos4Main.stockLeftSlider, this, "stockSlider");
        this.stockRightExtend.init(this.epos4Main.stockRightExtend, this, "stockExtend");
        this.stockRightSlider.init(this.epos4Main.stockRightSlider, this, "stockSlider");
        this.epos4Main.AllNodeActivateProfilePositionMode();
        if (this.walkStraightTimer != null) {
            this.walkStraightTimer.Stop();
            this.walkStraightTimer.Dispose();
        }
        this.walkStraightTimer = new System.Timers.Timer(100);
        this.walkStraightTimer.AutoReset = false;
        this.walkStraightTimer.Elapsed += (sender, e) => {
            if (this.coolingStatus == CoolingStatus.NowCooling) return;
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
            this.walkStraightTimer.Stop();
            this.walkStraightTimer.Dispose();

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
            byte[] sendByte = System.Text.Encoding.ASCII.GetBytes(sendText);//送信する文字列をbyteに変換
            if (this.client != null)
            {
                this.client.Write(sendByte, 0, sendByte.Length);//送信
            }
            UnityEngine.Debug.Log(this.sendText);
        };
        this.walkStraightTimer.Start();
    }

    public void WalkStop() {
        if (this.status == Status.stop) return;
        this.status = Status.stop;
        UnityEngine.Debug.Log("WalkStop");
        // this.epos4Main.AllNodeMoveStop();    
        this.epos4Main.AllNodeMoveToHome();

        if (this.coolingStatus == CoolingStatus.Readied) {
            this.sendText = "stop" + "," + "/";
            byte[] sendByte = System.Text.Encoding.ASCII.GetBytes(sendText);//送信する文字列をbyteに変換
            if (client != null)
            {
                this.client.Write(sendByte, 0, sendByte.Length);//送信
            }
            UnityEngine.Debug.Log(sendText);

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

    private void Update() {
        this.thumbStickR = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
        this.thumbStickL = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
        if (System.Math.Abs(this.thumbStickR.y) > 0.5 && System.Math.Abs(this.thumbStickL.y) > 0.5) {
        }
        else if (this.thumbStickR.y > 0.5 || this.thumbStickL.y > 0.5) {
            this.WalkStraight();
        }
        else if (this.thumbStickR.y < -0.5 || this.thumbStickL.y < -0.5) {
            this.WalkStop();
        }
    }

    private bool Destroied = false;

    private void OnDestroy() {
        this.WalkStop();
        this.Destroied = true;
    }
}