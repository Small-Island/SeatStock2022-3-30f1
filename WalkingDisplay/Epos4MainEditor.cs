using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Epos4Main))]
public class Epos4MainEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Epos4Main epos4Main = target as Epos4Main;
        
        EditorGUIUtility.labelWidth = 125;

        int maxCurrent = 20;
        int maxActualPosition = 500;
        int maxPosition = 1000;
        int maxVel = 4545;
        int maxAcceleration = 100000;

        if (GUILayout.Button("All Node Clear Error")) {
            epos4Main.clearError();
        }

        EditorGUILayout.LabelField("Seat");
        EditorGUILayout.LabelField("Lifter");
        EditorGUILayout.Slider("Current (A)", epos4Main.lifter.current, -maxCurrent, maxCurrent);
        EditorGUILayout.Slider("Actual Pos (mm)", epos4Main.lifter.actualPosition, -maxActualPosition, maxActualPosition);
        EditorGUILayout.Slider("Actual Velocity (rpm)", epos4Main.lifter.actualVelocity, -maxVel, maxVel);
        if (GUILayout.Button("Set Home")) {
            epos4Main.lifter.definePosition();
        }
        EditorGUILayout.TextArea(
            epos4Main.lifter.status,
            GUILayout.Width(EditorGUIUtility.currentViewWidth/2 - 30)
        );
        if (GUILayout.Button("Clear Error")) {
            epos4Main.lifter.MotorInit();
        }
        if (GUILayout.Button("Show Error")) {
            epos4Main.lifter.getError();
        }
        // if (GUILayout.Button("Activate Profile Position mode")) {
        //     epos4Main.lifter.ActivateProfilePositionMode();
        // }
        // if (GUILayout.Button("Activate Cyclic Synchronous Position mode")) {
        //     epos4Main.lifter.ActivatePositionMode();
        // }
        epos4Main.lifter.profile.absolute     = EditorGUILayout.Toggle ("Absolute", epos4Main.lifter.profile.absolute);
        epos4Main.lifter.profile.position     = EditorGUILayout.Slider("Position (mm)", epos4Main.lifter.profile.position, -maxPosition, maxPosition);
        epos4Main.lifter.profile.velocity     = (int)EditorGUILayout.Slider("Velocity (rpm)", epos4Main.lifter.profile.velocity, 0, maxVel);
        epos4Main.lifter.profile.acceleration = (int)EditorGUILayout.Slider("Acceleration (rpm/s)", epos4Main.lifter.profile.acceleration, 0, maxAcceleration);
        epos4Main.lifter.profile.deceleration = (int)EditorGUILayout.Slider("Deceleration (rpm/s)", epos4Main.lifter.profile.deceleration, 0, maxAcceleration);
        // EditorGUILayout.Toggle("Absolute", epos4Main.lifter.profile.absolute);
        // EditorGUILayout.Slider("Position (mm)", epos4Main.lifter.profile.position, -maxPosition, maxPosition);
        // EditorGUILayout.Slider("Velocity (rpm)", epos4Main.lifter.profile.velocity, 0, maxVel);
        // EditorGUILayout.Slider("Acceleration (rpm/s)", epos4Main.lifter.profile.acceleration, 0, maxAcceleration);
        // EditorGUILayout.Slider("Deceleration (rpm/s)", epos4Main.lifter.profile.deceleration, 0, maxAcceleration);
        if (GUILayout.Button("Move to Position")) {
            epos4Main.lifter.ActivateProfilePositionMode();
            epos4Main.lifter.SetPositionProfile();
            epos4Main.lifter.MoveToPosition(true);
        }
        // if (GUILayout.Button("Set Position Must")) {
        //     epos4Main.lifter.SetPositionMust();
        // }
        // epos4Main.lifter.targetTime  = (int)EditorGUILayout.Slider("target Time (sec)", epos4Main.lifter.targetTime, 1, 10);
        // epos4Main.lifter.targetPosMilli  = (int)EditorGUILayout.Slider("target Position (mm)", epos4Main.lifter.targetPosMilli, -200, 200);
        bool uprb = GUILayout.RepeatButton("Up    as Velocity Mode");
        bool downrb = GUILayout.RepeatButton("Down as Velocity Mode");
        if (EditorApplication.isPlaying) {
            if (uprb || epos4Main.lifter.oldUpForwardRepeatButton) {
                epos4Main.lifter.ActivateProfileVelocityMode();
                epos4Main.lifter.MoveWithVelocity(1);
            }
            else if (downrb || epos4Main.lifter.oldDownBackwardRepeatButton) {
                epos4Main.lifter.ActivateProfileVelocityMode();
                epos4Main.lifter.MoveWithVelocity(-1);
            }
            else {
                if (epos4Main.lifter.whichMode == Epos4Node.WhichMode.Velocity) {
                    epos4Main.lifter.QuickStop();
                    epos4Main.lifter.ActivateProfilePositionMode();
                }
            }
            epos4Main.lifter.oldUpForwardRepeatButton = uprb;
            epos4Main.lifter.oldDownBackwardRepeatButton = downrb;
        }
        if (GUILayout.RepeatButton("Quick Stop")) {
            epos4Main.lifter.QuickStop();
        }
        EditorGUILayout.Space(20);

        using (new EditorGUILayout.HorizontalScope()) {
            using (new EditorGUILayout.VerticalScope()) {
                EditorGUILayout.LabelField("Left Pedal");
                EditorGUILayout.Slider("Current (A)", epos4Main.leftPedal.current, -maxCurrent, maxCurrent);
                EditorGUILayout.Slider("Actual Pos (mm)", epos4Main.leftPedal.actualPosition, -maxActualPosition, maxActualPosition);
                EditorGUILayout.Slider("Actual Velocity (rpm)", epos4Main.leftPedal.actualVelocity, -maxVel, maxVel);
                if (GUILayout.Button("Set Home")) {
                    epos4Main.leftPedal.definePosition();
                }
                EditorGUILayout.TextArea(
                    epos4Main.leftPedal.status,
                    GUILayout.Width(EditorGUIUtility.currentViewWidth/2 - 30)
                );
                if (GUILayout.Button("Clear Error")) {
                    epos4Main.leftPedal.MotorInit();
                }
                // if (GUILayout.Button("Activate PPM")) {
                //     epos4Main.leftPedal.ActivateProfilePositionMode();
                // }
                epos4Main.leftPedal.profile.absolute     = EditorGUILayout.Toggle ("Absolute", epos4Main.leftPedal.profile.absolute);
                epos4Main.leftPedal.profile.position     = EditorGUILayout.Slider("Position (mm)", epos4Main.leftPedal.profile.position, -maxPosition, maxPosition);
                epos4Main.leftPedal.profile.velocity     = (int)EditorGUILayout.Slider("Velocity (rpm)", epos4Main.leftPedal.profile.velocity, 0, maxVel);
                epos4Main.leftPedal.profile.acceleration = (int)EditorGUILayout.Slider("Acceleration (rpm/s)", epos4Main.leftPedal.profile.acceleration, 0, maxAcceleration);
                epos4Main.leftPedal.profile.deceleration = (int)EditorGUILayout.Slider("Deceleration (rpm/s)", epos4Main.leftPedal.profile.deceleration, 0, maxAcceleration);
                // EditorGUILayout.Toggle("Absolute", epos4Main.leftPedal.profile.absolute);
                // EditorGUILayout.Slider("Position (mm)", epos4Main.leftPedal.profile.position, -maxPosition, maxPosition);
                // EditorGUILayout.Slider("Velocity (rpm)", epos4Main.leftPedal.profile.velocity, 0, maxVel);
                // EditorGUILayout.Slider("Acceleration (rpm/s)", epos4Main.leftPedal.profile.acceleration, 0, maxAcceleration);
                // EditorGUILayout.Slider("Deceleration (rpm/s)", epos4Main.leftPedal.profile.deceleration, 0, maxAcceleration);
                if (GUILayout.Button("Move to Position")) {
                    epos4Main.leftPedal.ActivateProfilePositionMode();
                    epos4Main.leftPedal.SetPositionProfile();
                    epos4Main.leftPedal.MoveToPosition(true);
                }
                uprb = GUILayout.RepeatButton("Up    as Velocity Mode");
                downrb = GUILayout.RepeatButton("Down as Velocity Mode");
                if (EditorApplication.isPlaying) {
                    if (uprb || epos4Main.leftPedal.oldUpForwardRepeatButton) {
                        epos4Main.leftPedal.ActivateProfileVelocityMode();
                        epos4Main.leftPedal.MoveWithVelocity(1);
                    }
                    else if (downrb || epos4Main.leftPedal.oldDownBackwardRepeatButton) {
                        epos4Main.leftPedal.ActivateProfileVelocityMode();
                        epos4Main.leftPedal.MoveWithVelocity(-1);
                    }
                    else {
                        if (epos4Main.leftPedal.whichMode == Epos4Node.WhichMode.Velocity) {
                            epos4Main.leftPedal.QuickStop();
                            epos4Main.leftPedal.ActivateProfilePositionMode();
                        }
                    }
                    epos4Main.leftPedal.oldUpForwardRepeatButton = uprb;
                    epos4Main.leftPedal.oldDownBackwardRepeatButton = downrb;
                }
                if (GUILayout.RepeatButton("Quick Stop")) {
                    epos4Main.leftPedal.QuickStop();
                }
                EditorGUILayout.Space(20);

                EditorGUILayout.LabelField("Left Slider");
                EditorGUILayout.Slider("Current (A)", epos4Main.leftSlider.current, -maxCurrent, maxCurrent);
                EditorGUILayout.Slider("Actual Pos (mm)", epos4Main.leftSlider.actualPosition, -maxActualPosition, maxActualPosition);
                EditorGUILayout.Slider("Actual Velocity (rpm)", epos4Main.leftSlider.actualVelocity, -maxVel, maxVel);
                if (GUILayout.Button("Set Home")) {
                    epos4Main.leftSlider.definePosition();
                }
                EditorGUILayout.TextArea(
                    epos4Main.leftSlider.status,
                    GUILayout.Width(EditorGUIUtility.currentViewWidth/2 - 30)
                );
                if (GUILayout.Button("Clear Error")) {
                    epos4Main.leftSlider.MotorInit();
                }
                // if (GUILayout.Button("Activate PPM")) {
                //     epos4Main.leftSlider.ActivateProfilePositionMode();
                // }
                epos4Main.leftSlider.profile.absolute     = EditorGUILayout.Toggle ("Absolute", epos4Main.leftSlider.profile.absolute);
                epos4Main.leftSlider.profile.position     = EditorGUILayout.Slider("Position (mm)", epos4Main.leftSlider.profile.position, -maxPosition, maxPosition);
                epos4Main.leftSlider.profile.velocity     = (int)EditorGUILayout.Slider("Velocity (rpm)", epos4Main.leftSlider.profile.velocity, 0, maxVel);
                epos4Main.leftSlider.profile.acceleration = (int)EditorGUILayout.Slider("Acceleration (rpm/s)", epos4Main.leftSlider.profile.acceleration, 0, maxAcceleration);
                epos4Main.leftSlider.profile.deceleration = (int)EditorGUILayout.Slider("Deceleration (rpm/s)", epos4Main.leftSlider.profile.deceleration, 0, maxAcceleration);
                // EditorGUILayout.Toggle ("Absolute", epos4Main.leftSlider.profile.absolute);
                // EditorGUILayout.Slider("Position (mm)", epos4Main.leftSlider.profile.position, -maxPosition, maxPosition);
                // EditorGUILayout.Slider("Velocity (rpm)", epos4Main.leftSlider.profile.velocity, 0, maxVel);
                // EditorGUILayout.Slider("Acceleration (rpm/s)", epos4Main.leftSlider.profile.acceleration, 0, maxAcceleration);
                // EditorGUILayout.Slider("Deceleration (rpm/s)", epos4Main.leftSlider.profile.deceleration, 0, maxAcceleration);
                if (GUILayout.Button("Move to Position")) {
                    epos4Main.leftSlider.ActivateProfilePositionMode();
                    epos4Main.leftSlider.SetPositionProfile();
                    epos4Main.leftSlider.MoveToPosition(true);
                }
                uprb = GUILayout.RepeatButton("Forward as Velocity Mode");
                downrb = GUILayout.RepeatButton("Backward as Velocity Mode");
                if (EditorApplication.isPlaying) {
                    if (uprb || epos4Main.leftSlider.oldUpForwardRepeatButton) {
                        epos4Main.leftSlider.ActivateProfileVelocityMode();
                        epos4Main.leftSlider.MoveWithVelocity(1);
                    }
                    else if (downrb || epos4Main.leftSlider.oldDownBackwardRepeatButton) {
                        epos4Main.leftSlider.ActivateProfileVelocityMode();
                        epos4Main.leftSlider.MoveWithVelocity(-1);
                    }
                    else {
                        if (epos4Main.leftSlider.whichMode == Epos4Node.WhichMode.Velocity) {
                            epos4Main.leftSlider.QuickStop();
                            epos4Main.leftSlider.ActivateProfilePositionMode();
                        }
                    }
                    epos4Main.leftSlider.oldUpForwardRepeatButton = uprb;
                    epos4Main.leftSlider.oldDownBackwardRepeatButton = downrb;
                }
                if (GUILayout.RepeatButton("Quick Stop")) {
                    epos4Main.leftSlider.QuickStop();
                }
            }

            using (new EditorGUILayout.VerticalScope()) {
                EditorGUILayout.LabelField("Right Pedal");
                EditorGUILayout.Slider("Current (A)", epos4Main.rightPedal.current, -maxCurrent, maxCurrent);
                EditorGUILayout.Slider("Actual Pos (mm)", epos4Main.rightPedal.actualPosition, -maxActualPosition, maxActualPosition);
                EditorGUILayout.Slider("Actual Velocity (rpm)", epos4Main.rightPedal.actualVelocity, -maxVel, maxVel);
                if (GUILayout.Button("Set Home")) {
                    epos4Main.rightPedal.definePosition();
                }
                EditorGUILayout.TextArea(
                    epos4Main.rightPedal.status,
                    GUILayout.Width(EditorGUIUtility.currentViewWidth/2 - 30)
                );
                if (GUILayout.Button("Clear Error")) {
                    epos4Main.rightPedal.MotorInit();
                }
                // if (GUILayout.Button("Activate PPM")) {
                //     epos4Main.rightPedal.ActivateProfilePositionMode();
                // }
                epos4Main.rightPedal.profile.absolute     = EditorGUILayout.Toggle ("Absolute", epos4Main.rightPedal.profile.absolute);
                epos4Main.rightPedal.profile.position     = EditorGUILayout.Slider("Position (mm)", epos4Main.rightPedal.profile.position, -maxPosition, maxPosition);
                epos4Main.rightPedal.profile.velocity     = (int)EditorGUILayout.Slider("Velocity (rpm)", epos4Main.rightPedal.profile.velocity, 0, maxVel);
                epos4Main.rightPedal.profile.acceleration = (int)EditorGUILayout.Slider("Acceleration (rpm/s)", epos4Main.rightPedal.profile.acceleration, 0, maxAcceleration);
                epos4Main.rightPedal.profile.deceleration = (int)EditorGUILayout.Slider("Deceleration (rpm/s)", epos4Main.rightPedal.profile.deceleration, 0, maxAcceleration);
                // EditorGUILayout.Toggle("Absolute", epos4Main.rightPedal.profile.absolute);
                // EditorGUILayout.Slider("Position (mm)", epos4Main.rightPedal.profile.position, -maxPosition, maxPosition);
                // EditorGUILayout.Slider("Velocity (rpm)", epos4Main.rightPedal.profile.velocity, 0, maxVel);
                // EditorGUILayout.Slider("Acceleration (rpm/s)", epos4Main.rightPedal.profile.acceleration, 0, maxAcceleration);
                // EditorGUILayout.Slider("Deceleration (rpm/s)", epos4Main.rightPedal.profile.deceleration, 0, maxAcceleration);
                if (GUILayout.Button("Move to Position")) {
                    epos4Main.rightPedal.ActivateProfilePositionMode();
                    epos4Main.rightPedal.SetPositionProfile();
                    epos4Main.rightPedal.MoveToPosition(true);
                }
                uprb = GUILayout.RepeatButton("Up    as Velocity Mode");
                downrb = GUILayout.RepeatButton("Down as Velocity Mode");
                if (EditorApplication.isPlaying) {
                    if (uprb || epos4Main.rightPedal.oldUpForwardRepeatButton) {
                        epos4Main.rightPedal.ActivateProfileVelocityMode();
                        epos4Main.rightPedal.MoveWithVelocity(1);
                    }
                    else if (downrb || epos4Main.rightPedal.oldDownBackwardRepeatButton) {
                        epos4Main.rightPedal.ActivateProfileVelocityMode();
                        epos4Main.rightPedal.MoveWithVelocity(-1);
                    }
                    else {
                        if (epos4Main.rightPedal.whichMode == Epos4Node.WhichMode.Velocity) {
                            epos4Main.rightPedal.QuickStop();
                            epos4Main.rightPedal.ActivateProfilePositionMode();
                        }
                    }
                    epos4Main.rightPedal.oldUpForwardRepeatButton = uprb;
                    epos4Main.rightPedal.oldDownBackwardRepeatButton = downrb;
                }
                if (GUILayout.RepeatButton("Quick Stop")) {
                    epos4Main.rightPedal.QuickStop();
                }
                EditorGUILayout.Space(20);

                EditorGUILayout.LabelField("Right Slider");
                EditorGUILayout.Slider("Current (A)", epos4Main.rightSlider.current, -maxCurrent, maxCurrent);
                EditorGUILayout.Slider("Actual Pos (mm)", epos4Main.rightSlider.actualPosition, -maxActualPosition, maxActualPosition);
                EditorGUILayout.Slider("Actual Velocity (rpm)", epos4Main.rightSlider.actualVelocity, -maxVel, maxVel);
                if (GUILayout.Button("Set Home")) {
                    epos4Main.rightSlider.definePosition();
                }
                EditorGUILayout.TextArea(
                    epos4Main.rightSlider.status,
                    GUILayout.Width(EditorGUIUtility.currentViewWidth/2 - 30)
                );
                if (GUILayout.Button("Clear Error")) {
                    epos4Main.rightSlider.MotorInit();
                }
                // if (GUILayout.Button("Activate PPM")) {
                //     epos4Main.rightSlider.ActivateProfilePositionMode();
                // }
                epos4Main.rightSlider.profile.absolute     = EditorGUILayout.Toggle ("Absolute", epos4Main.rightSlider.profile.absolute);
                epos4Main.rightSlider.profile.position     = EditorGUILayout.Slider("Position (mm)", epos4Main.rightSlider.profile.position, -maxPosition, maxPosition);
                epos4Main.rightSlider.profile.velocity     = (int)EditorGUILayout.Slider("Velocity (rpm)", epos4Main.rightSlider.profile.velocity, 0, maxVel);
                epos4Main.rightSlider.profile.acceleration = (int)EditorGUILayout.Slider("Acceleration (rpm/s)", epos4Main.rightSlider.profile.acceleration, 0, maxAcceleration);
                epos4Main.rightSlider.profile.deceleration = (int)EditorGUILayout.Slider("Deceleration (rpm/s)", epos4Main.rightSlider.profile.deceleration, 0, maxAcceleration);
                // EditorGUILayout.Toggle ("Absolute", epos4Main.rightSlider.profile.absolute);
                // EditorGUILayout.Slider("Position (mm)", epos4Main.rightSlider.profile.position, -maxPosition, maxPosition);
                // EditorGUILayout.Slider("Velocity (rpm)", epos4Main.rightSlider.profile.velocity, 0, maxVel);
                // EditorGUILayout.Slider("Acceleration (rpm/s)", epos4Main.rightSlider.profile.acceleration, 0, maxAcceleration);
                // EditorGUILayout.Slider("Deceleration (rpm/s)", epos4Main.rightSlider.profile.deceleration, 0, maxAcceleration);
                if (GUILayout.Button("Move to Position")) {
                    epos4Main.rightSlider.ActivateProfilePositionMode();
                    epos4Main.rightSlider.SetPositionProfile();
                    epos4Main.rightSlider.MoveToPosition(true);
                }
                uprb = GUILayout.RepeatButton("Foward    as Velocity Mode");
                downrb = GUILayout.RepeatButton("Backward as Velocity Mode");
                if (EditorApplication.isPlaying) {
                    if (uprb || epos4Main.rightSlider.oldUpForwardRepeatButton) {
                        epos4Main.rightSlider.ActivateProfileVelocityMode();
                        epos4Main.rightSlider.MoveWithVelocity(1);
                    }
                    else if (downrb || epos4Main.rightSlider.oldDownBackwardRepeatButton) {
                        epos4Main.rightSlider.ActivateProfileVelocityMode();
                        epos4Main.rightSlider.MoveWithVelocity(-1);
                    }
                    else {
                        if (epos4Main.rightSlider.whichMode == Epos4Node.WhichMode.Velocity) {
                            epos4Main.rightSlider.QuickStop();
                            epos4Main.rightSlider.ActivateProfilePositionMode();
                        }
                    }
                    epos4Main.rightSlider.oldUpForwardRepeatButton = uprb;
                    epos4Main.rightSlider.oldDownBackwardRepeatButton = downrb;
                }
                if (GUILayout.RepeatButton("Quick Stop")) {
                    epos4Main.rightSlider.QuickStop();
                }
            }
        }

        using (new EditorGUILayout.HorizontalScope()) {
            using (new EditorGUILayout.VerticalScope()) {
                EditorGUILayout.LabelField("Stock Left Extend");
                EditorGUILayout.Slider("Current (A)", epos4Main.stockLeftExtend.current, -maxCurrent, maxCurrent);
                EditorGUILayout.Slider("Actual Pos (mm)", epos4Main.stockLeftExtend.actualPosition, -maxActualPosition, maxActualPosition);
                EditorGUILayout.Slider("Actual Velocity (rpm)", epos4Main.stockLeftExtend.actualVelocity, -maxVel, maxVel);
                if (GUILayout.Button("Set Home")) {
                    epos4Main.stockLeftExtend.definePosition();
                }
                EditorGUILayout.TextArea(
                    epos4Main.stockLeftExtend.status,
                    GUILayout.Width(EditorGUIUtility.currentViewWidth/2 - 30)
                );
                if (GUILayout.Button("Clear Error")) {
                    epos4Main.stockLeftExtend.MotorInit();
                }
                // if (GUILayout.Button("Activate PPM")) {
                //     epos4Main.leftPedal.ActivateProfilePositionMode();
                // }
                epos4Main.stockLeftExtend.profile.absolute     = EditorGUILayout.Toggle ("Absolute", epos4Main.stockLeftExtend.profile.absolute);
                epos4Main.stockLeftExtend.profile.position     = EditorGUILayout.Slider("Position (mm)", epos4Main.stockLeftExtend.profile.position, -maxPosition, maxPosition);
                epos4Main.stockLeftExtend.profile.velocity     = (int)EditorGUILayout.Slider("Velocity (rpm)", epos4Main.stockLeftExtend.profile.velocity, 0, maxVel);
                epos4Main.stockLeftExtend.profile.acceleration = (int)EditorGUILayout.Slider("Acceleration (rpm/s)", epos4Main.stockLeftExtend.profile.acceleration, 0, maxAcceleration);
                epos4Main.stockLeftExtend.profile.deceleration = (int)EditorGUILayout.Slider("Deceleration (rpm/s)", epos4Main.stockLeftExtend.profile.deceleration, 0, maxAcceleration);
                // EditorGUILayout.Toggle ("Absolute", epos4Main.stockLeftExtend.profile.absolute);
                // EditorGUILayout.Slider("Position (mm)", epos4Main.stockLeftExtend.profile.position, -maxPosition, maxPosition);
                // EditorGUILayout.Slider("Velocity (rpm)", epos4Main.stockLeftExtend.profile.velocity, 0, maxVel);
                // EditorGUILayout.Slider("Acceleration (rpm/s)", epos4Main.stockLeftExtend.profile.acceleration, 0, maxAcceleration);
                // EditorGUILayout.Slider("Deceleration (rpm/s)", epos4Main.stockLeftExtend.profile.deceleration, 0, maxAcceleration);
                if (GUILayout.Button("Move to Position")) {
                    epos4Main.stockLeftExtend.ActivateProfilePositionMode();
                    epos4Main.stockLeftExtend.SetPositionProfile();
                    epos4Main.stockLeftExtend.MoveToPosition(true);
                }
                uprb = GUILayout.RepeatButton("Up    as Velocity Mode");
                downrb = GUILayout.RepeatButton("Down as Velocity Mode");
                if (EditorApplication.isPlaying) {
                    if (uprb || epos4Main.stockLeftExtend.oldUpForwardRepeatButton) {
                        epos4Main.stockLeftExtend.ActivateProfileVelocityMode();
                        epos4Main.stockLeftExtend.MoveWithVelocity(1);
                    }
                    else if (downrb || epos4Main.stockLeftExtend.oldDownBackwardRepeatButton) {
                        epos4Main.stockLeftExtend.ActivateProfileVelocityMode();
                        epos4Main.stockLeftExtend.MoveWithVelocity(-1);
                    }
                    else {
                        if (epos4Main.stockLeftExtend.whichMode == Epos4Node.WhichMode.Velocity) {
                            epos4Main.stockLeftExtend.QuickStop();
                            epos4Main.stockLeftExtend.ActivateProfilePositionMode();
                        }
                    }
                    epos4Main.stockLeftExtend.oldUpForwardRepeatButton = uprb;
                    epos4Main.stockLeftExtend.oldDownBackwardRepeatButton = downrb;
                }
                if (GUILayout.RepeatButton("Quick Stop")) {
                    epos4Main.leftPedal.QuickStop();
                }
                EditorGUILayout.Space(20);

                EditorGUILayout.LabelField("Stock Left Slider");
                EditorGUILayout.Slider("Current (A)", epos4Main.stockLeftSlider.current, -maxCurrent, maxCurrent);
                EditorGUILayout.Slider("Actual Pos (mm)", epos4Main.stockLeftSlider.actualPosition, -maxActualPosition, maxActualPosition);
                EditorGUILayout.Slider("Actual Velocity (rpm)", epos4Main.stockLeftSlider.actualVelocity, -maxVel, maxVel);
                if (GUILayout.Button("Set Home")) {
                    epos4Main.stockLeftSlider.definePosition();
                }
                EditorGUILayout.TextArea(
                    epos4Main.stockLeftSlider.status,
                    GUILayout.Width(EditorGUIUtility.currentViewWidth/2 - 30)
                );
                if (GUILayout.Button("Clear Error")) {
                    epos4Main.stockLeftSlider.MotorInit();
                }
                // if (GUILayout.Button("Activate PPM")) {
                //     epos4Main.stockLeftSlider.ActivateProfilePositionMode();
                // }
                epos4Main.stockLeftSlider.profile.absolute     = EditorGUILayout.Toggle ("Absolute", epos4Main.stockLeftSlider.profile.absolute);
                epos4Main.stockLeftSlider.profile.position     = EditorGUILayout.Slider("Position (mm)", epos4Main.stockLeftSlider.profile.position, -maxPosition, maxPosition);
                epos4Main.stockLeftSlider.profile.velocity     = (int)EditorGUILayout.Slider("Velocity (rpm)", epos4Main.stockLeftSlider.profile.velocity, 0, maxVel);
                epos4Main.stockLeftSlider.profile.acceleration = (int)EditorGUILayout.Slider("Acceleration (rpm/s)", epos4Main.stockLeftSlider.profile.acceleration, 0, maxAcceleration);
                epos4Main.stockLeftSlider.profile.deceleration = (int)EditorGUILayout.Slider("Deceleration (rpm/s)", epos4Main.stockLeftSlider.profile.deceleration, 0, maxAcceleration);
                // EditorGUILayout.Toggle ("Absolute", epos4Main.stockLeftSlider.profile.absolute);
                // EditorGUILayout.Slider("Position (mm)", epos4Main.stockLeftSlider.profile.position, -maxPosition, maxPosition);
                // EditorGUILayout.Slider("Velocity (rpm)", epos4Main.stockLeftSlider.profile.velocity, 0, maxVel);
                // EditorGUILayout.Slider("Acceleration (rpm/s)", epos4Main.stockLeftSlider.profile.acceleration, 0, maxAcceleration);
                // EditorGUILayout.Slider("Deceleration (rpm/s)", epos4Main.stockLeftSlider.profile.deceleration, 0, maxAcceleration);
                if (GUILayout.Button("Move to Position")) {
                    epos4Main.stockLeftSlider.ActivateProfilePositionMode();
                    epos4Main.stockLeftSlider.SetPositionProfile();
                    epos4Main.stockLeftSlider.MoveToPosition(true);
                }
                uprb = GUILayout.RepeatButton("Forward as Velocity Mode");
                downrb = GUILayout.RepeatButton("Backward as Velocity Mode");
                if (EditorApplication.isPlaying) {
                    if (uprb || epos4Main.stockLeftSlider.oldUpForwardRepeatButton) {
                        epos4Main.stockLeftSlider.ActivateProfileVelocityMode();
                        epos4Main.stockLeftSlider.MoveWithVelocity(1);
                    }
                    else if (downrb || epos4Main.stockLeftSlider.oldDownBackwardRepeatButton) {
                        epos4Main.stockLeftSlider.ActivateProfileVelocityMode();
                        epos4Main.stockLeftSlider.MoveWithVelocity(-1);
                    }
                    else {
                        if (epos4Main.stockLeftSlider.whichMode == Epos4Node.WhichMode.Velocity) {
                            epos4Main.stockLeftSlider.QuickStop();
                            epos4Main.stockLeftSlider.ActivateProfilePositionMode();
                        }
                    }
                    epos4Main.stockLeftSlider.oldUpForwardRepeatButton = uprb;
                    epos4Main.stockLeftSlider.oldDownBackwardRepeatButton = downrb;
                }
                if (GUILayout.RepeatButton("Quick Stop")) {
                    epos4Main.stockLeftSlider.QuickStop();
                }
            }

            using (new EditorGUILayout.VerticalScope()) {
                EditorGUILayout.LabelField("Stock Right Extend");
                EditorGUILayout.Slider("Current (A)", epos4Main.stockRightExtend.current, -maxCurrent, maxCurrent);
                EditorGUILayout.Slider("Actual Pos (mm)", epos4Main.stockRightExtend.actualPosition, -maxActualPosition, maxActualPosition);
                EditorGUILayout.Slider("Actual Velocity (rpm)", epos4Main.stockRightExtend.actualVelocity, -maxVel, maxVel);
                if (GUILayout.Button("Set Home")) {
                    epos4Main.stockRightExtend.definePosition();
                }
                EditorGUILayout.TextArea(
                    epos4Main.stockRightExtend.status,
                    GUILayout.Width(EditorGUIUtility.currentViewWidth/2 - 30)
                );
                if (GUILayout.Button("Clear Error")) {
                    epos4Main.stockRightExtend.MotorInit();
                }
                // if (GUILayout.Button("Activate PPM")) {
                //     epos4Main.stockRightExtend.ActivateProfilePositionMode();
                // }
                epos4Main.stockRightExtend.profile.absolute     = EditorGUILayout.Toggle ("Absolute", epos4Main.stockRightExtend.profile.absolute);
                epos4Main.stockRightExtend.profile.position     = EditorGUILayout.Slider("Position (mm)", epos4Main.stockRightExtend.profile.position, -maxPosition, maxPosition);
                epos4Main.stockRightExtend.profile.velocity     = (int)EditorGUILayout.Slider("Velocity (rpm)", epos4Main.stockRightExtend.profile.velocity, 0, maxVel);
                epos4Main.stockRightExtend.profile.acceleration = (int)EditorGUILayout.Slider("Acceleration (rpm/s)", epos4Main.stockRightExtend.profile.acceleration, 0, maxAcceleration);
                epos4Main.stockRightExtend.profile.deceleration = (int)EditorGUILayout.Slider("Deceleration (rpm/s)", epos4Main.stockRightExtend.profile.deceleration, 0, maxAcceleration);
                // EditorGUILayout.Toggle ("Absolute", epos4Main.stockRightExtend.profile.absolute);
                // EditorGUILayout.Slider("Position (mm)", epos4Main.stockRightExtend.profile.position, -maxPosition, maxPosition);
                // EditorGUILayout.Slider("Velocity (rpm)", epos4Main.stockRightExtend.profile.velocity, 0, maxVel);
                // EditorGUILayout.Slider("Acceleration (rpm/s)", epos4Main.stockRightExtend.profile.acceleration, 0, maxAcceleration);
                // EditorGUILayout.Slider("Deceleration (rpm/s)", epos4Main.stockRightExtend.profile.deceleration, 0, maxAcceleration);
                if (GUILayout.Button("Move to Position")) {
                    epos4Main.stockRightExtend.ActivateProfilePositionMode();
                    epos4Main.stockRightExtend.SetPositionProfile();
                    epos4Main.stockRightExtend.MoveToPosition(true);
                }
                uprb = GUILayout.RepeatButton("Up    as Velocity Mode");
                downrb = GUILayout.RepeatButton("Down as Velocity Mode");
                if (EditorApplication.isPlaying) {
                    if (uprb || epos4Main.stockRightExtend.oldUpForwardRepeatButton) {
                        epos4Main.stockRightExtend.ActivateProfileVelocityMode();
                        epos4Main.stockRightExtend.MoveWithVelocity(1);
                    }
                    else if (downrb || epos4Main.stockRightExtend.oldDownBackwardRepeatButton) {
                        epos4Main.stockRightExtend.ActivateProfileVelocityMode();
                        epos4Main.stockRightExtend.MoveWithVelocity(-1);
                    }
                    else {
                        if (epos4Main.stockRightExtend.whichMode == Epos4Node.WhichMode.Velocity) {
                            epos4Main.stockRightExtend.QuickStop();
                            epos4Main.stockRightExtend.ActivateProfilePositionMode();
                        }
                    }
                    epos4Main.stockRightExtend.oldUpForwardRepeatButton = uprb;
                    epos4Main.stockRightExtend.oldDownBackwardRepeatButton = downrb;
                }
                if (GUILayout.RepeatButton("Quick Stop")) {
                    epos4Main.stockRightExtend.QuickStop();
                }
                EditorGUILayout.Space(20);

                EditorGUILayout.LabelField("Stock Right Slider");
                EditorGUILayout.Slider("Current (A)", epos4Main.stockRightSlider.current, -maxCurrent, maxCurrent);
                EditorGUILayout.Slider("Actual Pos (mm)", epos4Main.stockRightSlider.actualPosition, -maxActualPosition, maxActualPosition);
                EditorGUILayout.Slider("Actual Velocity (rpm)", epos4Main.stockRightSlider.actualVelocity, -maxVel, maxVel);
                if (GUILayout.Button("Set Home")) {
                    epos4Main.stockRightSlider.definePosition();
                }
                EditorGUILayout.TextArea(
                    epos4Main.stockRightSlider.status,
                    GUILayout.Width(EditorGUIUtility.currentViewWidth/2 - 30)
                );
                if (GUILayout.Button("Clear Error")) {
                    epos4Main.stockRightSlider.MotorInit();
                }
                // if (GUILayout.Button("Activate PPM")) {
                //     epos4Main.stockRightSlider.ActivateProfilePositionMode();
                // }
                epos4Main.stockRightSlider.profile.absolute     = EditorGUILayout.Toggle ("Absolute", epos4Main.stockRightSlider.profile.absolute);
                epos4Main.stockRightSlider.profile.position     = EditorGUILayout.Slider("Position (mm)", epos4Main.stockRightSlider.profile.position, -maxPosition, maxPosition);
                epos4Main.stockRightSlider.profile.velocity     = (int)EditorGUILayout.Slider("Velocity (rpm)", epos4Main.stockRightSlider.profile.velocity, 0, maxVel);
                epos4Main.stockRightSlider.profile.acceleration = (int)EditorGUILayout.Slider("Acceleration (rpm/s)", epos4Main.stockRightSlider.profile.acceleration, 0, maxAcceleration);
                epos4Main.stockRightSlider.profile.deceleration = (int)EditorGUILayout.Slider("Deceleration (rpm/s)", epos4Main.stockRightSlider.profile.deceleration, 0, maxAcceleration);
                // EditorGUILayout.Toggle ("Absolute", epos4Main.stockRightSlider.profile.absolute);
                // EditorGUILayout.Slider("Position (mm)", epos4Main.stockRightSlider.profile.position, -maxPosition, maxPosition);
                // EditorGUILayout.Slider("Velocity (rpm)", epos4Main.stockRightSlider.profile.velocity, 0, maxVel);
                // EditorGUILayout.Slider("Acceleration (rpm/s)", epos4Main.stockRightSlider.profile.acceleration, 0, maxAcceleration);
                // EditorGUILayout.Slider("Deceleration (rpm/s)", epos4Main.stockRightSlider.profile.deceleration, 0, maxAcceleration);
                if (GUILayout.Button("Move to Position")) {
                    epos4Main.stockRightSlider.SetPositionProfile();
                    epos4Main.stockRightSlider.MoveToPosition(true);
                }
                uprb = GUILayout.RepeatButton("Foward    as Velocity Mode");
                downrb = GUILayout.RepeatButton("Backward as Velocity Mode");
                if (EditorApplication.isPlaying) {
                    if (uprb || epos4Main.stockRightSlider.oldUpForwardRepeatButton) {
                        epos4Main.stockRightSlider.ActivateProfileVelocityMode();
                        epos4Main.stockRightSlider.MoveWithVelocity(1);
                    }
                    else if (downrb || epos4Main.stockRightSlider.oldDownBackwardRepeatButton) {
                        epos4Main.stockRightSlider.ActivateProfileVelocityMode();
                        epos4Main.stockRightSlider.MoveWithVelocity(-1);
                    }
                    else {
                        if (epos4Main.stockRightSlider.whichMode == Epos4Node.WhichMode.Velocity) {
                            epos4Main.stockRightSlider.QuickStop();
                            epos4Main.stockRightSlider.ActivateProfilePositionMode();
                        }
                    }
                    epos4Main.stockRightSlider.oldUpForwardRepeatButton = uprb;
                    epos4Main.stockRightSlider.oldDownBackwardRepeatButton = downrb;
                }
                if (GUILayout.RepeatButton("Quick Stop")) {
                    epos4Main.stockRightSlider.QuickStop();
                }
            }
        }

        EditorGUIUtility.labelWidth = 200;
        base.OnInspectorGUI();
    }
}