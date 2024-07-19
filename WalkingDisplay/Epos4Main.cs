// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.IO;
// using System.Text;
// using UnityEngine.AddressableAssets;

public class Epos4Main : UnityEngine.MonoBehaviour {
    [UnityEngine.SerializeField]
    public Epos4Node lifter, leftPedal, leftSlider, rightPedal, rightSlider, stockLeftExtend, stockLeftSlider, stockRightExtend, stockRightSlider;

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
            // this.connector.Baudrate = 1000000;
        }
        catch (EposCmd.Net.DeviceException) {
        }
        this.lifter      = new Epos4Node(connector, 8,  5, -1, 0.2, 1.1);
        this.lifter.MotorInit();
        this.leftPedal   = new Epos4Node(connector, 1,  6,  1, 0.2, 1.1);
        this.leftPedal.MotorInit();
        this.leftSlider  = new Epos4Node(connector, 2, 12, -1, 0.2, 1.1);
        this.leftSlider.MotorInit();
        this.rightPedal  = new Epos4Node(connector, 3,  6,  1, 0.2, 1.1);
        this.rightPedal.MotorInit();
        this.rightSlider = new Epos4Node(connector, 4, 12, -1, 0.2, 1.1);
        this.rightSlider.MotorInit();

        this.stockLeftExtend   = new Epos4Node(this.connector, 9, 20,  -1, 0.2, 1.1);
        this.stockLeftExtend.MotorInit();
        this.stockLeftSlider  =  new Epos4Node(this.connector, 7, 20, -1, 0.1, 1.1);
        this.stockLeftSlider.MotorInit();
        this.stockRightExtend  = new Epos4Node(this.connector, 6, 20, -1, 0.2, 1.1);
        this.stockRightExtend.MotorInit();
        this.stockRightSlider =  new Epos4Node(this.connector, 5, 20, -1, 0.1, 1.1);
        this.stockRightSlider.MotorInit();
    }

    // void Update() {
    // }

    private void getActualPositionAsync() {
        while (!this.Destroied) {
            // this.lifter.actualPosition           = this.lifter.getPositionMM();
            // this.leftPedal.actualPosition        = this.leftPedal.getPositionMM();
            // this.leftSlider.actualPosition       = this.leftSlider.getPositionMM();
            // this.rightPedal.actualPosition       = this.rightPedal.getPositionMM();
            // this.rightSlider.actualPosition      = this.rightSlider.getPositionMM();
            // this.stockLeftExtend.actualPosition  = this.stockLeftExtend.getPositionMM();
            // this.stockLeftSlider.actualPosition  = this.stockLeftSlider.getPositionMM();
            // this.stockRightExtend.actualPosition = this.stockRightExtend.getPositionMM();
            // this.stockRightSlider.actualPosition = this.stockRightSlider.getPositionMM();

            // this.lifter.current           = this.lifter.getCurrentA();
            // this.leftPedal.current        = this.leftPedal.getCurrentA();
            // this.leftSlider.current       = this.leftSlider.getCurrentA();
            // this.rightPedal.current       = this.rightPedal.getCurrentA();
            // this.rightSlider.current      = this.rightSlider.getCurrentA();
            // this.stockLeftExtend.current  = this.stockLeftExtend.getCurrentA();
            // this.stockLeftSlider.current  = this.stockLeftSlider.getCurrentA();
            // this.stockRightExtend.current = this.stockRightExtend.getCurrentA();
            // this.stockRightSlider.current = this.stockRightSlider.getCurrentA();

            this.lifter.actualVelocity           = this.lifter.getVelocityIs();
            this.leftPedal.actualVelocity        = this.leftPedal.getVelocityIs();
            this.leftSlider.actualVelocity       = this.leftSlider.getVelocityIs();
            this.rightPedal.actualVelocity       = this.rightPedal.getVelocityIs();
            this.rightSlider.actualVelocity      = this.rightSlider.getVelocityIs();
            this.stockLeftExtend.actualVelocity  = this.stockLeftExtend.getVelocityIs();
            this.stockLeftSlider.actualVelocity  = this.stockLeftSlider.getVelocityIs();
            this.stockRightExtend.actualVelocity = this.stockRightExtend.getVelocityIs();
            this.stockRightSlider.actualVelocity = this.stockRightSlider.getVelocityIs();

            // this.lifter.powerSupplyVoltage = this.lifter.GetPowerSupplyVoltage();

            // this.lifter.getError();
            System.Threading.Thread.Sleep(1000);
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
        this.stockLeftExtend.ActivateProfilePositionMode();
        this.stockLeftSlider.ActivateProfilePositionMode();
        this.stockRightExtend.ActivateProfilePositionMode();
        this.stockRightSlider.ActivateProfilePositionMode();
    }

    public void AllNodeMoveStop()
    {
        this.lifter.MoveStop();
        this.leftPedal.MoveStop();
        this.leftSlider.MoveStop();
        this.rightPedal.MoveStop();
        this.rightSlider.MoveStop();
        this.stockLeftExtend.MoveStop();
        this.stockLeftSlider.MoveStop();
        this.stockRightExtend.MoveStop();
        this.stockRightSlider.MoveStop();
    }

    public void AllNodeMoveToHome()
    {
        this.AllNodeActivateProfilePositionMode();
        this.lifter.MoveToHome();
        this.leftPedal.MoveToHome();
        this.leftSlider.MoveToHome();
        this.rightPedal.MoveToHome();
        this.rightSlider.MoveToHome();
        this.stockLeftExtend.MoveToHome();
        this.stockLeftSlider.MoveToHome();
        this.stockRightExtend.MoveToHome();
        this.stockRightSlider.MoveToHome();
    }

    public void AllNodeDefinePosition()
    {
        this.lifter.definePosition();
        this.leftPedal.definePosition();
        this.leftSlider.definePosition();
        this.rightPedal.definePosition();
        this.rightSlider.definePosition();
        this.stockLeftExtend.definePosition();
        this.stockLeftSlider.definePosition();
        this.stockRightExtend.definePosition();
        this.stockRightSlider.definePosition();
    }

    private void OnDestroy()
    {
        this.Destroied = true;
        this.th.Abort();
    }
}
