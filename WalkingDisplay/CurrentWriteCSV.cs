// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.IO;
// using System.Text;
// using UnityEngine.AddressableAssets;

public class CurrentWriteCSV : UnityEngine.MonoBehaviour {
    [UnityEngine.SerializeField]
    public Epos4Main epos4Main;

    private System.Threading.Thread th = null;
    private bool Destroied = false;

    void Start() {
        this.th = new System.Threading.Thread(new System.Threading.ThreadStart(this.getActualPositionAsync));
        this.th.Start();
    }

    private void getActualPositionAsync() {
        int N = 1000;
        float[,] data = new float[N,10];
        System.Threading.Thread.Sleep(5000);
        int i = 0;
        while (!this.Destroied && i < N) {
            float lifter_actualPosition      = this.epos4Main.lifter.actualPosition;
            float leftPedal_actualPosition   = this.epos4Main.leftPedal.actualPosition;
            float leftSlider_actualPosition  = this.epos4Main.leftSlider.actualPosition;
            float rightPedal_actualPosition  = this.epos4Main.rightPedal.actualPosition;
            float rightSlider_actualPosition = this.epos4Main.rightSlider.actualPosition;

            float lifter_current      = this.epos4Main.lifter.current;
            float leftPedal_current   = this.epos4Main.leftPedal.current;
            float leftSlider_current  = this.epos4Main.leftSlider.current;
            float rightPedal_current  = this.epos4Main.rightPedal.current;
            float rightSlider_current = this.epos4Main.rightSlider.current;

            data[i,0] = lifter_actualPosition * 2f / 2000f;
            data[i,1] = leftPedal_actualPosition * 6f / 2000f;
            data[i,2] = leftSlider_actualPosition * 12f / 2000f;
            data[i,3] = rightPedal_actualPosition * 6f / 2000f;
            data[i,4] = rightSlider_actualPosition * 12f / 2000f;

            data[i,5] = lifter_current / 1000f;
            data[i,6] = leftPedal_current / 1000f;
            data[i,7] = leftSlider_current / 1000f;
            data[i,8] = rightPedal_current / 1000f;
            data[i,9] = rightSlider_current / 1000f;

            i++;

            System.Threading.Thread.Sleep(10);
        }

        System.IO.StreamWriter sw; // これがキモらしい
        System.IO.FileInfo fi;
        　　// Aplication.dataPath で プロジェクトファイルがある絶対パスが取り込める
        System.DateTime dt = System.DateTime.Now;
        string result = dt.ToString("yyyyMMddHHmmss");
        fi = new System.IO.FileInfo(UnityEngine.Application.dataPath + "/Scripts/log/current" + result + ".csv");
        sw = fi.AppendText();
        sw.WriteLine("count, lifter pos (mm), left pedal pos (mm), left slider pos (mm), right pedal pos (mm), right slider pos (mm), lifter current (A), left pedal current (A), left slider current (A), right pedal current (A), right slider current (A)");
        for (i = 0; i < N; i++)
        {
            string a = i.ToString() + ",";
            for (int j = 0; j < 10; j++) {
                a += data[i,j].ToString() + ",";
            }
            sw.WriteLine(a);
        }
        sw.Flush();
        sw.Close();
        return;
    }

    private void OnDestroy()
    {
        this.Destroied = true;
        // this.th.Abort();
    }
}
