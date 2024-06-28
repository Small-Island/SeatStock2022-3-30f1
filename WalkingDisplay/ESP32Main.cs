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

    void Start() {
        this.client = new System.IO.Ports.SerialPort(portName, baudRate, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
        this.client.Open();
    }

    public void SendText(string arg_sendText) {
        this.sendText = arg_sendText;
        byte[] sendByte = System.Text.Encoding.ASCII.GetBytes(this.sendText);//送信する文字列をbyteに変換
        if (this.client != null)
        {
            this.client.Write(sendByte, 0, sendByte.Length);//送信
        }
        UnityEngine.Debug.Log(this.sendText);
    }
}
