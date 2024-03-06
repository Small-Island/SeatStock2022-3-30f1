using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingDisplayMain : UnityEngine.MonoBehaviour {
    [UnityEngine.SerializeField, Range(0.3f, 1.4f)] public float period = 1.4f;
    [UnityEngine.SerializeField, Range(1, 10)] public int forwardRate = 4;
    [UnityEngine.SerializeField, Range(1, 10)] public int backwardRate = 6;
    // [UnityEngine.SerializeField] private UnityEngine.AddressableAssets.AssetReference csvFile;

    [UnityEngine.SerializeField] private Activate activate;

    [System.Serializable]
    public class Activate {
        // Unit mm
        [UnityEngine.SerializeField] public bool lifter      = false;
        [UnityEngine.SerializeField] public bool leftPedal   = false;
        [UnityEngine.SerializeField] public bool leftSlider  = false;
        [UnityEngine.SerializeField] public bool rightPedal  = false;
        [UnityEngine.SerializeField] public bool rightSlider = false;
    }

    [UnityEngine.Header("Unit mm")]
    [UnityEngine.SerializeField] public Amptitude amptitude;

    [System.Serializable]
    public class Amptitude {
        // Unit mm
        [UnityEngine.SerializeField, Range(0, 10)] public double lift = 1;
        [UnityEngine.SerializeField, Range(0, 50)] public double pedal = 1;
        [UnityEngine.SerializeField, Range(0, 100)] public double leftSlider = 1;
        [UnityEngine.SerializeField, Range(0, 100)] public double rightSlider = 1;
    }

    [UnityEngine.SerializeField] private Epos4Main epos4Main;
    public enum Status {
        stop, walking
    }
    [UnityEngine.SerializeField, ReadOnly] public Status status;

    private float halfPeriod = 0;
    private float quaterPeriod = 0;
    private float forwardPeriod = 0;
    private float backwardPeriod = 0;

    private UnityEngine.WaitForSeconds waitForQuaterPeriod;
    
    class LegCoroutines {
        public static UnityEngine.Coroutine lifter      = null;
        public static UnityEngine.Coroutine leftPedal   = null;
        public static UnityEngine.Coroutine leftSlider  = null;
        public static UnityEngine.Coroutine rightPedal  = null;
        public static UnityEngine.Coroutine rightSlider = null;

        public static void stop(WalkingDisplayMain arg_walkMain) {
            if (LegCoroutines.lifter != null) {
                arg_walkMain.StopCoroutine(LegCoroutines.lifter);
                LegCoroutines.lifter = null;
            }
            if (LegCoroutines.leftPedal != null) {
                arg_walkMain.StopCoroutine(LegCoroutines.leftPedal);
                LegCoroutines.leftPedal = null;
            }
            if (LegCoroutines.leftSlider != null) {
                arg_walkMain.StopCoroutine(LegCoroutines.leftSlider);
                LegCoroutines.leftSlider = null;
            }
            if (LegCoroutines.rightPedal != null) {
                arg_walkMain.StopCoroutine(LegCoroutines.rightPedal);
                LegCoroutines.rightPedal = null;
            }
            if (LegCoroutines.rightSlider != null) {
                arg_walkMain.StopCoroutine(LegCoroutines.rightSlider);
                LegCoroutines.rightSlider = null;
            }
            return;
        }
    }

    class LegThreads {
        public static System.Threading.Thread lifter      = null;
        public static System.Threading.Thread leftPedal   = null;
        public static System.Threading.Thread leftSlider  = null;
        public static System.Threading.Thread rightPedal  = null;
        public static System.Threading.Thread rightSlider = null;

        public static void start(WalkingDisplayMain arg_walkMain) {
            if (arg_walkMain.activate.lifter) {
                // LegThreads.lifter = new System.Threading.Thread(new System.Threading.ThreadStart(arg_walkMain.WalkStraightLifterThread));
                // LegThreads.lifter.Start();

                // arg_walkMain.WalkStraightLifterAsync();
            }
            if (arg_walkMain.activate.leftPedal) {
                // LegThreads.leftPedal = new System.Threading.Thread(new System.Threading.ThreadStart(arg_walkMain.WalkStraightLeftPedalThread));
                // LegThreads.leftPedal.Start();

                // arg_walkMain.WalkStraightLeftPedalAsync();
            }
            if (arg_walkMain.activate.leftSlider) {
                // LegThreads.leftSlider = new System.Threading.Thread(new System.Threading.ThreadStart(arg_walkMain.WalkStraightLeftSliderThread));
                // LegThreads.leftSlider.Start();

                // arg_walkMain.WalkStraightLeftSliderAsync();
            }
            if (arg_walkMain.activate.rightPedal) {
                // LegThreads.rightPedal = new System.Threading.Thread(new System.Threading.ThreadStart(arg_walkMain.WalkStraightRightPedalThread));
                // LegThreads.rightPedal.Start();

                // arg_walkMain.WalkStraightRightPedalAsync();
            }
            if (arg_walkMain.activate.rightSlider) {
                // LegThreads.rightSlider = new System.Threading.Thread(new System.Threading.ThreadStart(arg_walkMain.WalkStraightRightSliderThread));
                // LegThreads.rightSlider.Start();

                // arg_walkMain.WalkStraightRightSliderAsync();
            }
            return;
        }
        public static void stop() {
            if (LegThreads.lifter != null) {
                LegThreads.lifter.Abort();
                LegThreads.lifter = null;
            }
            if (LegThreads.leftPedal != null) {
                LegThreads.leftPedal.Abort();
                LegThreads.leftPedal = null;
            }
            if (LegThreads.leftSlider != null) {
                LegThreads.leftSlider.Abort();
                LegThreads.leftSlider = null;
            }
            if (LegThreads.rightPedal != null) {
                LegThreads.rightPedal.Abort();
                LegThreads.rightPedal = null;
            }
            if (LegThreads.rightSlider != null) {
                LegThreads.rightSlider.Abort();
                LegThreads.rightSlider = null;
            }
            return;
        }
    }

    private void Start() {
    }

    private void setPeriod() {
        this.quaterPeriod   = this.period/4.0f;
        this.halfPeriod     = this.period/2.0f;
        this.forwardPeriod  = (float)this.forwardRate/(float)(this.forwardRate + this.backwardRate)*this.period;
        this.backwardPeriod = (float)this.backwardRate/(float)(this.forwardRate + this.backwardRate)*this.period;
        return;
    }

    List<System.Threading.Tasks.Task> arrayTask;

    public void WalkStraight(float incdec_time)
    {
        this.walkstop = false;
        this.period = this.period + incdec_time;
        this.epos4Main.AllNodeDefinePosition();
        this.setPeriod();
        // LegThreads.start(this);
        if (this.activate.lifter) {
            System.Threading.Tasks.Task.Run(this.WalkStraightLifterAsync);
        }
        if (this.activate.leftPedal) {
            System.Threading.Tasks.Task.Run(this.WalkStraightLeftPedalAsync);
        }
        if (this.activate.leftSlider) {
            System.Threading.Tasks.Task.Run(this.WalkStraightLeftSliderAsync);
        }
        if (this.activate.rightPedal) {
            System.Threading.Tasks.Task.Run(this.WalkStraightRightPedalAsync);
        }
        if (this.activate.rightSlider) {
            System.Threading.Tasks.Task.Run(this.WalkStraightRightSliderAsync);
        }
    }

    private void WalkStraightLifterThread() {
        while (true) {
            this.status = Status.walking;
            this.setPeriod();
            this.epos4Main.lifter.MoveToPositionInTime(-this.amptitude.lift, this.forwardPeriod*0.5);
            System.Threading.Thread.Sleep((int)(1000*this.forwardPeriod*0.5));
            this.epos4Main.lifter.MoveToPositionInTime(0, this.forwardPeriod*0.5);
            System.Threading.Thread.Sleep((int)(1000*this.forwardPeriod*0.5));
            System.Threading.Thread.Sleep((int)(1000*System.Math.Abs(this.halfPeriod - this.forwardPeriod)));
        }
    }

    private async void WalkStraightLifterAsync() {
        while (!this.walkstop) {
            this.status = Status.walking;
            this.setPeriod();
            this.epos4Main.lifter.MoveToPositionInTime(-this.amptitude.lift, this.forwardPeriod*0.5f);
            await System.Threading.Tasks.Task.Delay((int)(1000*this.forwardPeriod*0.5f));
            if (this.walkstop) return;
            this.epos4Main.lifter.MoveToPositionInTime(0, this.forwardPeriod*0.5f);
            // await System.Threading.Tasks.Task.Delay((int)(1000*this.forwardPeriod*0.5f));
            await System.Threading.Tasks.Task.Delay((int)(1000*System.Math.Abs(this.halfPeriod - this.forwardPeriod*0.5)));
        }
        return;
    }

    private void WalkStraightLeftPedalThread() {
        while (true) {
            this.status = Status.walking;
            this.setPeriod();
            this.epos4Main.leftPedal.MoveToPositionInTime(this.amptitude.pedal, this.forwardPeriod*0.5);
            // epos4Main.lifter.MoveToPositionInTime(-this.amptitude.lift, this.forwardPeriod*0.5);
            System.Threading.Thread.Sleep((int)(1000*this.forwardPeriod*0.5));
            this.epos4Main.leftPedal.MoveToPositionInTime(0, this.forwardPeriod*0.5);
            // epos4Main.lifter.MoveToPositionInTime(0, this.forwardPeriod*0.5);
            System.Threading.Thread.Sleep((int)(1000*this.forwardPeriod*0.5));
            System.Threading.Thread.Sleep((int)(1000*this.backwardPeriod));

            // epos4Main.leftPedal.MoveToPositionInTime(this.amptitude.pedal, this.quaterPeriod);
            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
            // epos4Main.leftPedal.MoveToPositionInTime(0, this.quaterPeriod);
            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
        }
    }

     private async void WalkStraightLeftPedalAsync() {
        while (!this.walkstop) {
            this.status = Status.walking;
            this.setPeriod();
            this.epos4Main.leftPedal.MoveToPositionInTime(this.amptitude.pedal, this.forwardPeriod*0.5f);
            // if (this.activate.lifter)
            // epos4Main.lifter.MoveToPositionInTime(-this.amptitude.lift, this.forwardPeriod*0.5);
            await System.Threading.Tasks.Task.Delay((int)(1000f*this.forwardPeriod*0.5f));
            if (this.walkstop) return;
            this.epos4Main.leftPedal.MoveToPositionInTime(0, this.forwardPeriod*0.5);
            // if (this.activate.lifter)
            // epos4Main.lifter.MoveToPositionInTime(0, this.forwardPeriod*0.5);
            await System.Threading.Tasks.Task.Delay((int)(1000f*this.forwardPeriod*0.5f + 1000f*this.backwardPeriod));

            // epos4Main.leftPedal.MoveToPositionInTime(this.amptitude.pedal, this.quaterPeriod);
            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
            // epos4Main.leftPedal.MoveToPositionInTime(0, this.quaterPeriod);
            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
        }
        return;
    }

    private void WalkStraightLeftSliderThread() {
        while (true) {
            this.status = Status.walking;
            this.setPeriod();
            this.epos4Main.leftSlider.MoveToPositionInTime(-this.amptitude.leftSlider, this.forwardPeriod);
            System.Threading.Thread.Sleep((int)(1000*this.forwardPeriod));
            this.epos4Main.leftSlider.MoveToPositionInTime(this.amptitude.leftSlider, this.backwardPeriod);
            System.Threading.Thread.Sleep((int)(1000*this.backwardPeriod));
            // epos4Main.leftSlider.MoveToPositionInTime(-this.amptitude.slider, this.halfPeriod);
            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
            // epos4Main.leftSlider.MoveToPositionInTime(this.amptitude.slider, this.halfPeriod);
            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
        }
    }

    private async void WalkStraightLeftSliderAsync() {
        while (!this.walkstop) {
            this.status = Status.walking;
            this.setPeriod();
            this.epos4Main.leftSlider.MoveToPositionInTime(-this.amptitude.leftSlider, this.forwardPeriod);
            await System.Threading.Tasks.Task.Delay((int)(1000f*this.forwardPeriod));
            if (this.walkstop) return;
            this.epos4Main.leftSlider.MoveToPositionInTime(this.amptitude.leftSlider, this.backwardPeriod);
            await System.Threading.Tasks.Task.Delay((int)(1000f*this.backwardPeriod));
            // epos4Main.leftSlider.MoveToPositionInTime(-this.amptitude.slider, this.halfPeriod);
            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
            // epos4Main.leftSlider.MoveToPositionInTime(this.amptitude.slider, this.halfPeriod);
            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
        }
        return;
    }

    private void WalkStraightRightPedalThread() {
        this.setPeriod();
        System.Threading.Thread.Sleep((int)(1000*this.halfPeriod));
        while (true) {
            this.status = Status.walking;
            this.setPeriod();
            this.epos4Main.rightPedal.MoveToPositionInTime(this.amptitude.pedal, this.forwardPeriod*0.5);
            // epos4Main.lifter.MoveToPositionInTime(-this.amptitude.lift, this.forwardPeriod*0.5);
            System.Threading.Thread.Sleep((int)(1000*this.forwardPeriod*0.5));
            this.epos4Main.rightPedal.MoveToPositionInTime(0, this.forwardPeriod*0.5);
            // epos4Main.lifter.MoveToPositionInTime(0, this.forwardPeriod*0.5);
            System.Threading.Thread.Sleep((int)(1000*this.forwardPeriod*0.5));
            System.Threading.Thread.Sleep((int)(1000*this.backwardPeriod));

            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
            // epos4Main.rightPedal.MoveToPositionInTime(this.amptitude.pedal, this.quaterPeriod);
            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
            // epos4Main.rightPedal.MoveToPositionInTime(0, this.quaterPeriod);
            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
        }
    }

    private async void WalkStraightRightPedalAsync() {
        this.setPeriod();
        await System.Threading.Tasks.Task.Delay((int)(1062f*this.halfPeriod));
        while (!this.walkstop) {
            this.status = Status.walking;
            this.setPeriod();
            this.epos4Main.rightPedal.MoveToPositionInTime(this.amptitude.pedal, this.forwardPeriod*0.5f);
            // if (this.activate.lifter)
            // epos4Main.lifter.MoveToPositionInTime(-this.amptitude.lift, this.forwardPeriod*0.5f);
            await System.Threading.Tasks.Task.Delay((int)(1000f*this.forwardPeriod*0.5f));
            if (this.walkstop) return;
            this.epos4Main.rightPedal.MoveToPositionInTime(0, this.forwardPeriod*0.5f);
            // if (this.activate.lifter)
            // epos4Main.lifter.MoveToPositionInTime(0, this.forwardPeriod*0.5f);
            await System.Threading.Tasks.Task.Delay((int)(1000f*this.forwardPeriod*0.5f + 1000f*this.backwardPeriod));

            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
            // epos4Main.rightPedal.MoveToPositionInTime(this.amptitude.pedal, this.quaterPeriod);
            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
            // epos4Main.rightPedal.MoveToPositionInTime(0, this.quaterPeriod);
            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
        }
        return;
    }

    private void WalkStraightRightSliderThread() {
        this.setPeriod();
        System.Threading.Thread.Sleep((int)(1000*this.halfPeriod));
        while (true) {
            this.status = Status.walking;
            this.setPeriod();
            this.epos4Main.rightSlider.MoveToPositionInTime(-this.amptitude.rightSlider, this.forwardPeriod);
            System.Threading.Thread.Sleep((int)(1000*this.forwardPeriod));
            this.epos4Main.rightSlider.MoveToPositionInTime(this.amptitude.rightSlider, this.backwardPeriod);
            System.Threading.Thread.Sleep((int)(1000*this.backwardPeriod));
            // OVRInput.SetControllerVibration(1, 1, OVRInput.Controller.RTouch);
            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
            // epos4Main.rightSlider.MoveToPositionInTime(-this.amptitude.slider, this.halfPeriod);
            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
            // epos4Main.rightSlider.MoveToPositionInTime(this.amptitude.slider, this.halfPeriod);
        }
    }

    private async void WalkStraightRightSliderAsync() {
        this.setPeriod();
        await System.Threading.Tasks.Task.Delay((int)(1062f*this.halfPeriod));
        while (!this.walkstop) {
            this.status = Status.walking;
            this.setPeriod();
            this.epos4Main.rightSlider.MoveToPositionInTime(-this.amptitude.rightSlider, this.forwardPeriod);
            await System.Threading.Tasks.Task.Delay((int)(1000f*this.forwardPeriod));
            if (this.walkstop) return;
            this.epos4Main.rightSlider.MoveToPositionInTime(this.amptitude.rightSlider, this.backwardPeriod);
            await System.Threading.Tasks.Task.Delay((int)(1000f*this.backwardPeriod));
            // OVRInput.SetControllerVibration(1, 1, OVRInput.Controller.RTouch);
            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
            // epos4Main.rightSlider.MoveToPositionInTime(-this.amptitude.slider, this.halfPeriod);
            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
            // System.Threading.Thread.Sleep((int)(1000*this.quaterPeriod));
            // epos4Main.rightSlider.MoveToPositionInTime(this.amptitude.slider, this.halfPeriod);
        }
        return;
    }

    public void WalkStop()
    {
        // UnityEngine.Debug.Log("Walk stop");
        // LegCoroutines.stop(this);
        // LegThreads.stop();
        this.status = Status.stop;
        this.walkstop = true;
        this.epos4Main.AllNodeMoveToHome();
    }

    public async void WalkRestart() {
        this.WalkStop();
        await System.Threading.Tasks.Task.Delay((int)(1000*1.5*this.period));
        this.WalkStraight(0);
    }

    private bool walkstop = false;
    private void OnDestroy()
    {
        this.walkstop = true;
        this.WalkStop();
    }

    public async void DelaySample()
    {
        for (int i = 0; i < 5; i++)
        {
            if (this.walkstop) {
                return;
            }
            UnityEngine.Debug.Log(i);
            await System.Threading.Tasks.Task.Delay(1000);
        }
    }

    // public void sample() {
    //     DelaySample
    //     UnityEngine.Debug.Log("hello");
    // }
}