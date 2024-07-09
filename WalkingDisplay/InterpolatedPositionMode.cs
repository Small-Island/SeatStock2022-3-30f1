using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterpolatedPositionMode : UnityEngine.MonoBehaviour {
    [UnityEngine.SerializeField] private Epos4Main epos4Main;

    private void Start() {

    }

    public void StartInterpolatedPositionMode() {
        this.status = Status.upping;
        this.epos4Main.AllNodeDefinePosition();
        this.epos4Main.stockLeftExtend.ActivateInterpolatedPositionMode();
        this.epos4Main.stockLeftExtend.ClearIpmBuffer();
        for (double i = 0; i < 5; i = i + 0.01) {
            this.epos4Main.stockLeftExtend.AddPvtValueToIpmBuffer(i, 60, 10);
        }
    }

    public double position = 0; // Unit mm
    public double LimitPosition = 10; // Unit mm
    public enum Status {
        stop, upping, dowing
    }
    [ReadOnly] public Status status = Status.stop;

    public void StopInterpolatedPositionMode() {
        if (this.status == Status.stop) return;
        this.status = Status.stop;
        this.epos4Main.stockLeftExtend.StopIpmTrajectory();
        UnityEngine.Debug.Log("WalkStop");
        this.epos4Main.AllNodeActivateProfilePositionMode();
        this.epos4Main.AllNodeMoveToHome();
    }

    private void OnDestroy() {
        this.StopInterpolatedPositionMode();
    }
}