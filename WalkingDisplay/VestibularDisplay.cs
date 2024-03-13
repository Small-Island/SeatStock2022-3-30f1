using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VestibularDisplay : UnityEngine.MonoBehaviour {
    public CameraWave cameraWave;

    [UnityEngine.SerializeField, Range(0.3f, 4f)] public float period = 1.4f;
    
    [UnityEngine.Header("Unit mm")]
    [UnityEngine.SerializeField] public Amptitude amptitude;

    [System.Serializable]
    public class Amptitude {
        // Unit mm
        [UnityEngine.SerializeField, Range(0, 50)] public double lift = 1;
    }

    [UnityEngine.SerializeField] private Epos4Main epos4Main;
    public enum Status {
        stop, waving
    }
    [UnityEngine.SerializeField, ReadOnly] public Status status;

    private bool waveStopped = false;

    private void Start() {
    }

    List<System.Threading.Tasks.Task> arrayTask;

    public void WaveStart()
    {
        this.waveStopped = false;
        this.epos4Main.AllNodeDefinePosition();
        System.Threading.Tasks.Task.Run(this.WalkStraightLifterAsync);
    }

    private async void WalkStraightLifterAsync() {
        while (!this.waveStopped) {
            this.status = Status.waving;
            this.epos4Main.lifter.MoveToPositionInTime(-this.amptitude.lift, this.period);
            cameraWave.height += cameraWave.amptitude;
            await System.Threading.Tasks.Task.Delay((int)(1000*this.period));
            if (this.waveStopped) return;
            this.epos4Main.lifter.MoveToPositionInTime(0, this.period);
            cameraWave.height -= cameraWave.amptitude;
            // await System.Threading.Tasks.Task.Delay((int)(1000*this.forwardPeriod*0.5f));
            await System.Threading.Tasks.Task.Delay((int)(1000*this.period));
        }
        return;
    }

    public void WaveStop()
    {
        // UnityEngine.Debug.Log("Walk stop");
        // LegCoroutines.stop(this);
        // LegThreads.stop();
        this.status = Status.stop;
        this.waveStopped = true;
        this.epos4Main.AllNodeMoveToHome();
    }

    public async void WaveRestart() {
        this.WaveStop();
        await System.Threading.Tasks.Task.Delay((int)(1000*1.5*this.period));
        this.WaveStart();
    }

    private void OnDestroy()
    {
        this.waveStopped = true;
        this.WaveStop();
    }
}