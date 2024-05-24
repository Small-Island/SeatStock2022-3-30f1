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
    }

    public void writeStart() {
        this.th.Start();
    }

    private void getActualPositionAsync() {
        int N = 1000;
        float[,] data = new float[N,10];
        int i = 0;
        while (!this.Destroied && i < N) {
            data[i,0] = this.epos4Main.lifter.actualPosition / 100f; // Unit 10cm
            data[i,1] = this.epos4Main.leftPedal.actualPosition / 100f;
            data[i,2] = this.epos4Main.leftSlider.actualPosition / 100f;
            data[i,3] = this.epos4Main.rightPedal.actualPosition / 100f;
            data[i,4] = this.epos4Main.rightSlider.actualPosition / 100f;

            data[i,5] = this.epos4Main.lifter.current;
            data[i,6] = this.epos4Main.leftPedal.current;
            data[i,7] = this.epos4Main.leftSlider.current;
            data[i,8] = this.epos4Main.rightPedal.current;
            data[i,9] = this.epos4Main.rightSlider.current;

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
        sw.WriteLine("count, Stick Right Slider (10cm), left pedal pos (cm), left slider pos (cm), right pedal pos (cm), right slider pos (cm), current (A), left pedal current (A), left slider current (A), right pedal current (A), right slider current (A)");
        for (i = 0; i < N; i++)
        {
            float time = i*0.01f;
            string a = time.ToString() + ",";
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
        // this.sw.Flush();
        // this.sw.Close();
        // this.th.Abort();
    }
}
