// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.IO;
// using System.Text;
// using UnityEngine.AddressableAssets;

[System.Serializable]
public class Epos4Node {
    private EposCmd.Net.Device device;
    private DeviceOperation deviceOperation;
    private int motorId;
    private EposCmd.Net.DeviceManager connector;
    [UnityEngine.HideInInspector] public float targetTime = 2;
    [UnityEngine.HideInInspector] public float targetPosMilli = 0;

    [UnityEngine.HideInInspector] public bool oldUpForwardRepeatButton = false;
    [UnityEngine.HideInInspector] public bool oldDownBackwardRepeatButton = false;

    private int incrementPerRotation = 2000;

    private string name = "";

    public enum ConnectionStatus {
        success = 1,
        failed = 0
    }

    public enum WhichMode {
        Position,
        Velocity,
        Homing
    }

    public ConnectionStatus cs;
    public WhichMode whichMode;

    [UnityEngine.HideInInspector] public string status = ""; 

    [UnityEngine.SerializeField] public double milliPerARotation = 0;

    [UnityEngine.HideInInspector] public Profile profile;
    [UnityEngine.HideInInspector] public int actualPosition = 0;
    [UnityEngine.HideInInspector] public float current = 0;

    public Epos4Node(EposCmd.Net.DeviceManager arg_connector, int arg_idx, string arg_name, double arg_milliPerARotation) {
        this.name = arg_name;
        this.motorId = arg_idx;
        this.connector = arg_connector;
        this.milliPerARotation = arg_milliPerARotation;
    }

    public void MotorInit()
    {
        // EposCmd.Net.DeviceManager connector = null;
        // try {
        //     connector = new EposCmd.Net.DeviceManager("EPOS4", "MAXON SERIAL V2", "USB", "USB0");
        // }
        // catch (EposCmd.Net.DeviceException) {
        //     this.status = "Connection failed";
        //     return;
        // }
        this.status = "";
        try {
            this.device = this.connector.CreateDevice((ushort)this.motorId);
        }
        catch (EposCmd.Net.DeviceException) {
            this.status = "Connection failed";
            this.cs = ConnectionStatus.failed;
            return;
        }
        catch (System.Exception) {
            this.status = "Connection failed";
            this.cs = ConnectionStatus.failed;
            return;
        }
        this.deviceOperation = new DeviceOperation(this.device);
        try {
            this.deviceOperation.ClearFaultAndSetEnableState();
        }
        catch (EposCmd.Net.DeviceException) {
            this.status = "Connection failed";
            this.cs = ConnectionStatus.failed;
            return;
        }
        this.ActivateProfilePositionMode();
        this.cs = ConnectionStatus.success;
        return;
    }

    public void ActivateProfilePositionMode() {
        if (this.cs == ConnectionStatus.failed) return;
        try {
            this.deviceOperation.ActivateProfilePositionMode();
        }
        catch (System.Exception e) {
            this.status = e.ToString();
        }
        this.whichMode = WhichMode.Position;
        return;
    }

    public int getPositionIs() {
        if (this.cs == ConnectionStatus.failed) return 0;
        int value = 0;
        try {
            value = this.deviceOperation.GetPositionIs();
        }
        catch (System.Exception) {
            // this.status = e.ToString();
        }
        return value;
    }

    public int getCurrentIs() {
        if (this.cs == ConnectionStatus.failed) return 0;
        int value = 0;
        try {
            value = this.deviceOperation.GetCurrentIs();
        }
        catch (System.Exception) {
            // this.status = e.ToString();
        }
        return value;
    }

    public void ActivateHomingMode() {
        if (this.cs == ConnectionStatus.failed) return;
        try {
            this.deviceOperation.ActivateHomingMode();
        }
        catch (System.Exception e)
        {
            this.status = e.ToString();
        }
        this.whichMode = WhichMode.Homing;
    }

    public void definePosition() {
        if (this.cs == ConnectionStatus.failed) return;
        try {
            this.deviceOperation.DefinePosition(0);
        }
        catch (System.Exception e)
        {
            this.status = e.ToString();
        }
    }

    private double old_arg_pos_in = 0;

    public void MoveToPositionInTime(double arg_pos_milli, double arg_sec_time) {
        if (this.cs == ConnectionStatus.failed) return;
        double arg_pos_r = arg_pos_milli / this.milliPerARotation;
        double arg_pos_in = this.incrementPerRotation * arg_pos_r;

        double x_in = arg_pos_in - this.old_arg_pos_in;
        double x_r  = x_in / this.incrementPerRotation;

        this.old_arg_pos_in = arg_pos_in;

        double c = 1.0;

        this.profile.absolute     = true;
        this.profile.position     = (int)arg_pos_in;
        this.profile.velocity     = (int)System.Math.Abs(c * 2.0 * x_r / arg_sec_time * 60.0);
        this.profile.acceleration = (int)System.Math.Abs(c * 4.0 * x_r / arg_sec_time / arg_sec_time * 60.0);
        this.profile.deceleration = (int)System.Math.Abs(c * 4.0 * x_r / arg_sec_time / arg_sec_time * 60.0);

        this.MoveToPosition();
    }

    public void MoveToPosition() {
        if (this.cs == ConnectionStatus.failed) return;
        try {
            this.deviceOperation.SetPositionProfile(
                this.profile.velocity,
                this.profile.acceleration,
                this.profile.deceleration
            );
            this.deviceOperation.MoveToPosition(
                this.profile.position,
                this.profile.absolute,
                false
            );
        }
        catch (System.Exception e) {
            this.status = e.ToString();
        }
    }

    public void MoveToHome() {
        if (this.cs == ConnectionStatus.failed) return;
        this.old_arg_pos_in = 0;
        this.profile.position = 0;
        this.profile.absolute = true;
        try {
            this.deviceOperation.SetPositionProfile(
                this.profile.velocity,
                this.profile.acceleration,
                this.profile.deceleration
            );
            this.deviceOperation.MoveToPosition(
                this.profile.position,
                this.profile.absolute,
                true
            );
        }
        catch (System.Exception e) {
            this.status = e.ToString();
        }
        this.profile.absolute     = false;
        this.profile.position     = 0;
        this.profile.velocity     = 120;
        this.profile.acceleration = 240;
        this.profile.deceleration = 240;
        // this.deviceOperation.SetQuickStopState();
    }

    public void QuickStop() {
        if (this.cs == ConnectionStatus.failed) return;
        try {
           this.deviceOperation.SetQuickStopState();
        }
        catch (System.Exception e) {
            this.status = e.ToString();
        }
    }

    public void ActivateProfileVelocityMode() {
        if (this.cs == ConnectionStatus.failed) return;
        try {
            this.deviceOperation.ActivateProfileVelocityMode();
        }
        catch (System.Exception e) {
            this.status = e.ToString();
        }
        this.whichMode = WhichMode.Velocity;
    }

    public void MoveWithVelocity(int arg_pm) {
        if (this.cs == ConnectionStatus.failed) return;
        try {
            this.deviceOperation.SetVelocityProfile(
                this.profile.acceleration,
                this.profile.deceleration
            );
            this.deviceOperation.MoveWithVelocity(
                arg_pm * this.profile.velocity
            );
        }
        catch (System.Exception e) {
            this.status = e.ToString();
        }
    }

    // Unit inc   2000 inc == 1 rotation == 2 mm
    private void OnDestroy()
    {
        this.deviceOperation.AdvancedDispose();
    }

    private class DeviceOperation {
        private EposCmd.Net.Device device;
        private EposCmd.Net.DeviceCmdSet.Operation.StateMachine sm;
        private EposCmd.Net.DeviceCmdSet.Operation.ProfilePositionMode ppm;
        private EposCmd.Net.DeviceCmdSet.Operation.ProfileVelocityMode pvm;
        private EposCmd.Net.DeviceCmdSet.Operation.MotionInfo mi;
        private EposCmd.Net.DeviceCmdSet.Operation.HomingMode hm;
        public DeviceOperation(EposCmd.Net.Device arg_device) {
            this.device = arg_device;
            this.sm = this.device.Operation.StateMachine;
            this.ppm = this.device.Operation.ProfilePositionMode;
            this.pvm = this.device.Operation.ProfileVelocityMode;
            this.mi = this.device.Operation.MotionInfo;
            this.hm = this.device.Operation.HomingMode;
        }

        public void SetQuickStopState() {
            this.sm.SetQuickStopState();
        }

        public void ClearFaultAndSetEnableState() {
            if (this.sm.GetFaultState()) {
                this.sm.ClearFault();
            }
            this.sm.SetEnableState();
        }

        public void ActivateProfilePositionMode() {
            try {
                this.ppm.ActivateProfilePositionMode();
            }
            catch (System.Exception e) {
                throw e;
            }
        }

        public void ActivateProfileVelocityMode() {
            try {
                this.pvm.ActivateProfileVelocityMode();
            }
            catch (System.Exception e) {
                throw e;
            }
        }

        public void MoveToPosition(int arg_position, bool arg_absolute, bool arg_immediately) {
            try
            {
                // arg_position (inc) == 360/2000 (deg)
                this.ppm.MoveToPosition(arg_position, arg_absolute, arg_immediately);
            }
            catch (System.Exception e) {
                throw e;
            }
            return;
        }

        public void MoveWithVelocity(int arg_target_velocity) {
            try
            {
                // arg_position (inc) == 360/2000 (deg)
                this.pvm.MoveWithVelocity(arg_target_velocity);
            }
            catch (System.Exception e) {
                throw e;
            }
            return;
        }

        public void SetPositionProfile(
            int arg_ProfileVelocity,
            int arg_ProfileAcceleration,
            int arg_ProfileDeceleration
        )
        {
            try {
                this.ppm.SetPositionProfile(
                    (uint)System.Math.Abs(arg_ProfileVelocity),
                    (uint)System.Math.Abs(arg_ProfileAcceleration),
                    (uint)System.Math.Abs(arg_ProfileDeceleration)
                );
            }
            catch (System.Exception e) {
                throw e;
                // UnityEngine.MonoBehaviour.print(e);
            }
        }

        public void SetVelocityProfile(
            int arg_ProfileAcceleration,
            int arg_ProfileDeceleration
        )
        {
            try {
                this.pvm.SetVelocityProfile(
                    (uint)System.Math.Abs(arg_ProfileAcceleration),
                    (uint)System.Math.Abs(arg_ProfileDeceleration)
                );
            }
            catch (System.Exception e) {
                throw e;
                // UnityEngine.MonoBehaviour.print(e);
            }
        }

        public void ActivateHomingMode() {
            try {
                this.hm.ActivateHomingMode();
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        public void DefinePosition(int arg_offsetPosition) {
            try {
                this.hm.DefinePosition(arg_offsetPosition);
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        public int GetPositionIs() {
            int value = 0;
            try {
                value = (int)(this.mi.GetPositionIs());
            }
            catch (System.Exception) {
                // UnityEngine.MonoBehaviour.print(e);
            }
            return value;
        }

        public int GetCurrentIs() {
            int value = 0;
            try {
                value = (int)(this.mi.GetCurrentIs());
            }
            catch (System.Exception) {
                // UnityEngine.MonoBehaviour.print(e);
            }
            return value;
        }

        public void AdvancedDispose() {
            if (this.ppm != null) {
                this.ppm.Advanced.Dispose();
            }
            if (this.pvm != null) {
                this.pvm.Advanced.Dispose();
            }
        }
    }

    [System.Serializable]
    public class Profile {
        public bool absolute = false;

        // [UnityEngine.SerializeField, UnityEngine.Range(-2000, 2000), UnityEngine.Header("Unit inc")]
        public int position = 0;
        // [UnityEngine.SerializeField, UnityEngine.Range(0, 120), UnityEngine.Header("Unit rpm")]
        public int velocity = 120;
        // [UnityEngine.SerializeField, UnityEngine.Range(0, 240), UnityEngine.Header("Unit rpm/s")]
        public int acceleration = 240;
        // [UnityEngine.SerializeField, UnityEngine.Range(0, 240), UnityEngine.Header("Unit rpm/s")]
        public int deceleration = 240;
    }
}