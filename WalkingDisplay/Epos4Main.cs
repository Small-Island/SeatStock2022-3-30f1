// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.IO;
// using System.Text;
// using UnityEngine.AddressableAssets;

public class Epos4Main : UnityEngine.MonoBehaviour {
    [UnityEngine.SerializeField]
    public Epos4Node
        lifter,
        leftSlider, stockLeftSlider, leftPedal, leftPedalYaw, stockLeftExtend,
        rightSlider, stockRightSlider, rightPedal, rightPedalYaw, stockRightExtend;

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
        this.lifter      = new Epos4Node(connector, 1,  5, -1, 0.2, 1.1);
        this.lifter.MotorInit();
        this.lifter.SetEnableState();
        this.lifter.ActivateProfilePositionMode();

        this.leftSlider  = new Epos4Node(connector, 2, 20, -1, 0.2, 1.1);
        this.leftSlider.MotorInit();
        this.leftSlider.SetEnableState();
        this.leftSlider.ActivateProfilePositionMode();
        this.stockLeftSlider  =  new Epos4Node(this.connector, 3, 20, -1, 0.1, 1.1);
        this.stockLeftSlider.MotorInit();
        this.stockLeftSlider.SetEnableState();
        this.stockLeftSlider.ActivateProfilePositionMode();
        this.leftPedal   = new Epos4Node(connector, 4,  5,  1, 0.2, 1.1);
        this.leftPedal.MotorInit();
        this.leftPedal.SetEnableState();
        this.leftPedal.ActivateProfilePositionMode();
        this.leftPedalYaw   = new Epos4Node(connector, 5,  360,  1, 0.2, 1.1);
        this.leftPedalYaw.MotorInit();
        this.leftPedalYaw.SetEnableState();
        this.leftPedalYaw.ActivateProfilePositionMode();
        this.stockLeftExtend   = new Epos4Node(this.connector, 6, 20,  -1, 0.2, 1.1);
        this.stockLeftExtend.MotorInit();
        this.stockLeftExtend.SetEnableState();
        this.stockLeftExtend.ActivateProfilePositionMode();

        this.rightSlider = new Epos4Node(connector, 7, 20, -1, 0.2, 1.1);
        this.rightSlider.MotorInit();
        this.rightSlider.SetEnableState();
        this.rightSlider.ActivateProfilePositionMode();
        this.stockRightSlider =  new Epos4Node(this.connector, 8, 20, -1, 0.1, 1.1);
        this.stockRightSlider.MotorInit();
        this.stockRightSlider.SetEnableState();
        this.stockRightSlider.ActivateProfilePositionMode();
        this.rightPedal  = new Epos4Node(connector, 9,  5,  1, 0.2, 1.1);
        this.rightPedal.MotorInit();
        this.rightPedal.SetEnableState();
        this.rightPedal.ActivateProfilePositionMode();
        this.rightPedalYaw  = new Epos4Node(connector, 10, 360,  1, 0.2, 1.1);
        this.rightPedalYaw.MotorInit();
        this.rightPedalYaw.SetEnableState();
        this.rightPedalYaw.ActivateProfilePositionMode();
        this.stockRightExtend  = new Epos4Node(this.connector, 11, 20, -1, 0.2, 1.1);
        this.stockRightExtend.MotorInit();
        this.stockRightExtend.SetEnableState();
        this.stockRightExtend.ActivateProfilePositionMode();
    }

    // void Update() {
    // }

    private void getActualPositionAsync() {
        while (!this.Destroied) {
            this.lifter.actualPosition           = this.lifter.getPositionMM();
            this.leftPedalYaw.actualPosition     = this.leftPedalYaw.getPositionMM();
            this.leftPedal.actualPosition        = this.leftPedal.getPositionMM();
            this.leftSlider.actualPosition       = this.leftSlider.getPositionMM();
            this.rightPedalYaw.actualPosition    = this.rightPedalYaw.getPositionMM();
            this.rightPedal.actualPosition       = this.rightPedal.getPositionMM();
            this.rightSlider.actualPosition      = this.rightSlider.getPositionMM();
            this.stockLeftExtend.actualPosition  = this.stockLeftExtend.getPositionMM();
            this.stockLeftSlider.actualPosition  = this.stockLeftSlider.getPositionMM();
            this.stockRightExtend.actualPosition = this.stockRightExtend.getPositionMM();
            this.stockRightSlider.actualPosition = this.stockRightSlider.getPositionMM();

            this.lifter.current           = this.lifter.getCurrentA();
            this.leftPedalYaw.current     = this.leftPedalYaw.getCurrentA();
            this.leftPedal.current        = this.leftPedal.getCurrentA();
            this.leftSlider.current       = this.leftSlider.getCurrentA();
            this.rightPedalYaw.current    = this.rightPedalYaw.getCurrentA();
            this.rightPedal.current       = this.rightPedal.getCurrentA();
            this.rightSlider.current      = this.rightSlider.getCurrentA();
            this.stockLeftExtend.current  = this.stockLeftExtend.getCurrentA();
            this.stockLeftSlider.current  = this.stockLeftSlider.getCurrentA();
            this.stockRightExtend.current = this.stockRightExtend.getCurrentA();
            this.stockRightSlider.current = this.stockRightSlider.getCurrentA();

            this.lifter.actualVelocity           = this.lifter.getVelocityIs();
            this.leftPedalYaw.actualVelocity     = this.leftPedalYaw.getVelocityIs();
            this.leftPedal.actualVelocity        = this.leftPedal.getVelocityIs();
            this.leftSlider.actualVelocity       = this.leftSlider.getVelocityIs();
            this.rightPedalYaw.actualVelocity       = this.rightPedalYaw.getVelocityIs();
            this.rightPedal.actualVelocity       = this.rightPedal.getVelocityIs();
            this.rightSlider.actualVelocity      = this.rightSlider.getVelocityIs();
            this.stockLeftExtend.actualVelocity  = this.stockLeftExtend.getVelocityIs();
            this.stockLeftSlider.actualVelocity  = this.stockLeftSlider.getVelocityIs();
            this.stockRightExtend.actualVelocity = this.stockRightExtend.getVelocityIs();
            this.stockRightSlider.actualVelocity = this.stockRightSlider.getVelocityIs();

            // this.lifter.powerSupplyVoltage = this.lifter.GetPowerSupplyVoltage();

            // this.lifter.getError();
            System.Threading.Thread.Sleep(100);
        }
        return;
    }

    public void AllNodeActivateProfilePositionMode()
    {
        this.lifter.ActivateProfilePositionMode();
        this.leftPedalYaw.ActivateProfilePositionMode();
        this.leftPedal.ActivateProfilePositionMode();
        this.leftSlider.ActivateProfilePositionMode();
        this.rightPedalYaw.ActivateProfilePositionMode();
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
        this.leftPedalYaw.MoveToHome();
        this.leftPedal.MoveToHome();
        this.leftSlider.MoveToHome();
        this.rightPedalYaw.MoveToHome();
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
