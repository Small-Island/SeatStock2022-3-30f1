public class ForceSensor : UnityEngine.MonoBehaviour {
    public string portName = "COM3";    
    private System.IO.Ports.SerialPort client;
    public string recvstr;
    public int len = 0;
    public ushort[] data;
    private int samplingTime = 20;
    private long clockTimeMs = 0;
    [UnityEngine.SerializeField, ReadOnly] private double clockTimeSec = 0;
    public double c = 0;
    public double F = 0;

    public UnityEngine.UI.Image[] image;
    private System.Timers.Timer timer;

    void Start() {
        this.client = new System.IO.Ports.SerialPort();
        this.client.PortName = this.portName;
        this.client.BaudRate = 921600;
        this.client.DataBits = 8;
        this.client.Parity = System.IO.Ports.Parity.None;
        this.client.StopBits = System.IO.Ports.StopBits.One;
        this.client.Handshake = System.IO.Ports.Handshake.None;
        this.client.ReadTimeout = 10;
        this.client.Open();
        // this.client.Write(new char[] {'R'}, 0, 1);//送信
        this.client.Write("R");//送信
        this.timer = new System.Timers.Timer(this.samplingTime);
        this.timer.AutoReset = true;
        this.timer.Elapsed += this.timerCallback;
        this.timer.Start();
        this.clockTimeMs = 0;
        this.image[0].color = new UnityEngine.Color32(0, 100, 0, 255);
    }

    public void WriteRead() {
        this.data = new ushort[6];
        if (this.client != null)
        {
            char[] buffer = new char[27];
            // this.client.Write(new char[] {'R'});//送信
            this.client.Write("R");//送信
            this.client.Read(buffer, 0, 27);//受信
            this.recvstr = new string(buffer);
            this.len = this.recvstr.Length;
            this.data[0] = (ushort)(System.Convert.ToUInt16(recvstr[1 .. 5], 16));
            this.data[1] = (ushort)(System.Convert.ToUInt16(recvstr[5 .. 9], 16));
            this.data[2] = (ushort)(System.Convert.ToUInt16(recvstr[9 .. 13], 16));
            this.data[3] = (ushort)(System.Convert.ToUInt16(recvstr[13 .. 17], 16));
            this.data[4] = (ushort)(System.Convert.ToUInt16(recvstr[17 .. 21], 16));
            this.data[5] = (ushort)(System.Convert.ToUInt16(recvstr[21 .. 25], 16));
            // - 6553 - 655
            this.F = ((double)data[2] - 8192)/32.0;
            this.c = (double)(-this.F + 40);
            // this.image[0].color = new UnityEngine.Color32(0, 100, 0, 255);
            // this.data[0] = System.BitConverter.ToUInt16(new byte[] {buffer[0], buffer[1], buffer[2], buffer[3]}, 0);
            // UnityEngine.Debug.Log(buffer[0]);
        }
    }

    private void Update() {
        this.image[0].color = new UnityEngine.Color32(0, 255, 0, (byte)this.c);
    }

    public void timerCallback(object source, System.Timers.ElapsedEventArgs e) {
        this.WriteRead();
        this.clockTimeMs += this.samplingTime;
        this.clockTimeSec = (double)this.clockTimeMs/1000.0;
    }

    private void OnDestroy() {
        this.timer?.Stop();
        this.timer?.Dispose();
        this.client.Close();
    }
}
