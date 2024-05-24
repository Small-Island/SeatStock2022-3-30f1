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

    private EposCmd.Net.DeviceManager connector = null;

    void Start() {
        this.clearError();
        // this.waitForSeconds = new UnityEngine.WaitForSeconds(0.1f);
        // this.coroutineActualPosition = StartCoroutine(this.getActualPositionAsync());
        this.th = new System.Threading.Thread(new System.Threading.ThreadStart(this.getActualPositionAsync));
        this.th.Start();
    }

    public void clearError() {
        try {
            this.connector = new EposCmd.Net.DeviceManager("EPOS4", "MAXON SERIAL V2", "USB", "USB0");
            this.connector.Baudrate = 1000000;
        }
        catch (EposCmd.Net.DeviceException) {
        }
        this.lifter      = new Epos4Node(connector, 1, "Lifter",        20, -1, 0.5, 1);
        this.lifter.MotorInit();
        this.leftPedal   = new Epos4Node(connector, 2, "Left Pedal",    6,  1, 0.2, 1);
        this.leftPedal.MotorInit();
        this.leftSlider  = new Epos4Node(connector, 3, "Left Slider",  12, -1, 0.2, 1);
        this.leftSlider.MotorInit();
        this.rightPedal  = new Epos4Node(connector, 4, "Right Pedal",   6,  1, 0.2, 1);
        this.rightPedal.MotorInit();
        this.rightSlider = new Epos4Node(connector, 5, "Right Slider", 12, -1, 0.2, 1);
        this.rightSlider.MotorInit();
    }

    // void Update() {
    // }

    private void getActualPositionAsync() {
        while (!this.Destroied) {
            this.lifter.actualPosition      = (int)(-(float) this.lifter.getPositionIs()/2000f*this.lifter.milliPerRotation);
            this.leftPedal.actualPosition   = (int)((float) this.leftPedal.getPositionIs()/2000f*this.leftPedal.milliPerRotation);
            this.leftSlider.actualPosition  = (int)(-(float) this.leftSlider.getPositionIs()/2000f*this.leftSlider.milliPerRotation);
            this.rightPedal.actualPosition  = (int)((float) this.rightPedal.getPositionIs()/2000f*this.rightPedal.milliPerRotation);
            this.rightSlider.actualPosition = (int)(-(float) this.rightSlider.getPositionIs()/2000f*this.rightSlider.milliPerRotation);

            this.lifter.current      = this.lifter.getCurrentIs()/1000f;
            this.leftPedal.current   = this.leftPedal.getCurrentIs()/1000f;
            this.leftSlider.current  = this.leftSlider.getCurrentIs()/1000f;
            this.rightPedal.current  = this.rightPedal.getCurrentIs()/1000f;
            this.rightSlider.current = this.rightSlider.getCurrentIs()/1000f;

            // this.lifter.getError();
            System.Threading.Thread.Sleep(20);
        }
        return;
    }

    public void AllNodeActivateProfilePositionMode()
    {
        this.lifter.ActivateProfilePositionMode();
        this.leftPedal.ActivateProfilePositionMode();
        this.leftSlider.ActivateProfilePositionMode();
        this.rightPedal.ActivateProfilePositionMode();
        this.rightSlider.ActivateProfilePositionMode();
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
