// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.IO;
// using System.Text;
// using UnityEngine.AddressableAssets;

public class ESP32Main : UnityEngine.MonoBehaviour {
    public string portName = "COM3";    
    public int baudRate = 9600;
    private System.IO.Ports.SerialPort client;
    public string sendText;
    [UnityEngine.TextArea] public string log;

    void Start() {
        this.client = new System.IO.Ports.SerialPort(portName, baudRate, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
        this.client.ReadTimeout = 500;
        this.client.WriteTimeout = 500;

        try {
            this.client.Open();
        }
        catch (System.IO.IOException e) {
            UnityEngine.Debug.Log(e.ToString());
            this.log = e.ToString() + "\n" + this.log;
        }
    }

    public void SendText(string arg_sendText) {
        this.sendText = arg_sendText;
        byte[] sendByte = System.Text.Encoding.ASCII.GetBytes(this.sendText);//送信する文字列をbyteに変換
        if (this.client != null)
        {
            try {
                this.client.Write(sendByte, 0, sendByte.Length);//送信
            }
            catch (System.InvalidOperationException e) {
                UnityEngine.Debug.Log(e.ToString());
                this.log = e.ToString() + "\n" + this.log;
            }
        }
        // UnityEngine.Debug.Log(this.sendText);
    }

    public void Write() {
        byte[] sendByte = System.Text.Encoding.ASCII.GetBytes(this.sendText + "\n");//送信する文字列をbyteに変換
        if (this.client != null)
        {
            try {
                this.client.Write(sendByte, 0, sendByte.Length);//送信
            }
            catch (System.InvalidOperationException e) {
                UnityEngine.Debug.Log(e.ToString());
                this.log = e.ToString() + "\n" + this.log;
            }
        }
        // UnityEngine.Debug.Log(this.sendText);
    }
}
