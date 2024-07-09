using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterpolatedPositionMode : UnityEngine.MonoBehaviour {
    [UnityEngine.SerializeField] private Epos4Main epos4Main;

    private void Start() {

    }

    public void StartInterpolatedPositionMode() {
        this.status = Status.upping;
        // this.epos4Main.AllNodeDefinePosition();
        // this.epos4Main.stockLeftSlider.ActivateInterpolatedPositionMode();
        // this.epos4Main.stockRightExtend.ActivateInterpolatedPositionMode();
        // this.epos4Main.stockLeftExtend.StopIpmTrajectory();
        // this.epos4Main.stockLeftExtend.ActivateInterpolatedPositionMode();
        this.epos4Main.stockLeftExtend.SetOperationMode(EposCmd.Net.EOperationMode.OmdCyclicSynchronousPositionMode);
        // this.epos4Main.stockLeftExtend.ClearIpmBuffer();
        // for (int i = 0; i < 50; i++) {
        //     this.epos4Main.stockLeftExtend.AddPvtValueToIpmBuffer((double)i/10, 60, (byte)i);
        // }
        // this.epos4Main.stockLeftExtend.StartIpmTrajectory();
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