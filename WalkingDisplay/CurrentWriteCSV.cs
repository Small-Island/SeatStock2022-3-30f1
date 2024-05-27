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
        float[,] data = new float[N,18];
        int i = 0;
        while (!this.Destroied && i < N) {
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

    private void OnDestroy()
    {
        this.Destroied = true;
        // this.sw.Flush();
        // this.sw.Close();
        // this.th.Abort();
    }
}
