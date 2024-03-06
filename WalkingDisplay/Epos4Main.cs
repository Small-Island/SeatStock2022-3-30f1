// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.IO;
// using System.Text;
// using UnityEngine.AddressableAssets;

public class Epos4Main : UnityEngine.MonoBehaviour {
    [UnityEngine.SerializeField]
    public Epos4Node lifter, leftPedal, leftSlider, rightPedal, rightSlider;

    // private UnityEngine.Coroutine coroutineActualPosition = null;

    // private UnityEngine.WaitForSeconds waitForSeconds;

    private System.Threading.Thread th = null;

    private bool Destroied = false;

    void Start() {
        EposCmd.Net.DeviceManager connector = null;
        try {
            connector = new EposCmd.Net.DeviceManager("EPOS4", "MAXON SERIAL V2", "USB", "USB0");
        }
        catch (EposCmd.Net.DeviceException) {
        }
        this.lifter      = new Epos4Node(connector, 1, "Lifter",       2);
        this.lifter.MotorInit();
        this.leftPedal   = new Epos4Node(connector, 2, "Left Pedal",   6);
        this.leftPedal.MotorInit();
        this.leftSlider  = new Epos4Node(connector, 3, "Left Slider",  12);
        this.leftSlider.MotorInit();
        this.rightPedal  = new Epos4Node(connector, 4, "Right Pedal",  6);
        this.rightPedal.MotorInit();
        this.rightSlider = new Epos4Node(connector, 5, "Right Slider", 12);
        this.rightSlider.MotorInit();
        // this.waitForSeconds = new UnityEngine.WaitForSeconds(0.1f);
        // this.coroutineActualPosition = StartCoroutine(this.getActualPositionAsync());
        // this.th = new System.Threading.Thread(new System.Threading.ThreadStart(this.getActualPositionAsync));
        System.Threading.Tasks.Task.Run(this.getActualPositionAsync);
        // this.th.Start();
    }

    public void clearError() {
        EposCmd.Net.DeviceManager connector = null;
        try {
            connector = new EposCmd.Net.DeviceManager("EPOS4", "MAXON SERIAL V2", "USB", "USB0");
        }
        catch (EposCmd.Net.DeviceException) {
        }
        this.lifter      = new Epos4Node(connector, 1, "Lifter",       2);
        this.lifter.MotorInit();
        this.leftPedal   = new Epos4Node(connector, 2, "Left Pedal",   6);
        this.leftPedal.MotorInit();
        this.leftSlider  = new Epos4Node(connector, 3, "Left Slider",  12);
        this.leftSlider.MotorInit();
        this.rightPedal  = new Epos4Node(connector, 4, "Right Pedal",  6);
        this.rightPedal.MotorInit();
        this.rightSlider = new Epos4Node(connector, 5, "Right Slider", 12);
        this.rightSlider.MotorInit();
    }

    // void Update() {
    // }

    private void getActualPositionAsync() {
        while (!this.Destroied) {
            this.lifter.actualPosition      = this.lifter.getPositionIs();
            this.leftPedal.actualPosition   = this.leftPedal.getPositionIs();
            this.leftSlider.actualPosition  = this.leftSlider.getPositionIs();
            this.rightPedal.actualPosition  = this.rightPedal.getPositionIs();
            this.rightSlider.actualPosition = this.rightSlider.getPositionIs();

            this.lifter.current      = this.lifter.getCurrentIs();
            this.leftPedal.current   = this.leftPedal.getCurrentIs();
            this.leftSlider.current  = this.leftSlider.getCurrentIs();
            this.rightPedal.current  = this.rightPedal.getCurrentIs();
            this.rightSlider.current = this.rightSlider.getCurrentIs();
            System.Threading.Thread.Sleep(10);
        }
        return;
    }

    public void AllNodeMoveToHome()
    {
        this.lifter.MoveToHome();
        this.leftPedal.MoveToHome();
        this.leftSlider.MoveToHome();
        this.rightPedal.MoveToHome();
        this.rightSlider.MoveToHome();
    }

    public void AllNodeDefinePosition()
    {
        this.lifter.definePosition();
        this.leftPedal.definePosition();
        this.leftSlider.definePosition();
        this.rightPedal.definePosition();
        this.rightSlider.definePosition();
    }

    private void OnDestroy()
    {
        this.Destroied = true;
        this.th.Abort();
    }
}
