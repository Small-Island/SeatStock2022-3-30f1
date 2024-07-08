using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclicSynchronousPositionMode : UnityEngine.MonoBehaviour {
    [UnityEngine.SerializeField] private Epos4Main epos4Main;
    private System.Timers.Timer timer;

    private void Start() {

    }

    public void StartCyclicSynchronousPositionMode() {
        this.epos4Main.stockLeftExtend.ActivatePositionMode();
        this.positionMust = 0;
        this.timer = new System.Timers.Timer(1);
        this.timer.AutoReset = true;
        this.timer.Elapsed += this.timerCallback;
        this.status = Status.upping;
        this.timer.Start();
    }

    public double positionMust = 0; // Unit mm
    public double LimitPosition = 10; // Unit mm
    public enum Status {
        stop, upping, dowing
    }
    [ReadOnly] public Status status = Status.stop;

    private void timerCallback(object source, System.Timers.ElapsedEventArgs e) {
        if (this.status == Status.upping) {
            if (this.positionMust < this.LimitPosition) {
                this.epos4Main.stockLeftExtend.SetPositionMust(this.positionMust);
                this.positionMust = this.positionMust + 0.005;
            }
            else {
                this.status = Status.dowing;
            }
        }
        else if (this.status == Status.dowing) {
            if (this.positionMust > 0) {
                this.epos4Main.stockLeftExtend.SetPositionMust(this.positionMust);
                this.positionMust = this.positionMust - 0.005;
            }
            else {
                this.status = Status.upping;
            }
        }
    }

    public void StopCyclicSynchronousPositionMode() {
        this.timer?.Stop();
        this.timer?.Dispose();
        if (this.status == Status.stop) return;
        this.status = Status.stop;
        UnityEngine.Debug.Log("WalkStop");
        this.epos4Main.AllNodeActivateProfilePositionMode();
        this.epos4Main.AllNodeMoveToHome();
    }

    private void OnDestroy() {
        this.StopCyclicSynchronousPositionMode();
    }
}