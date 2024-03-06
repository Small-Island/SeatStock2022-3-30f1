using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Epos4Main))]
public class Epos4MainEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Epos4Main epos4Main = target as Epos4Main;

        EditorGUIUtility.labelWidth = 200;
        base.OnInspectorGUI();
        
        EditorGUIUtility.labelWidth = 120;

        int maxCurrent = 20;
        int maxActualPosition = 8000;
        int maxPosition = 8000;
        int maxVel = 4545;
        int maxAcceleration = 20000;

        if (GUILayout.Button("All Node Clear Error")) {
            epos4Main.clearError();
        }

        EditorGUILayout.LabelField("Lifter");
        EditorGUILayout.Slider("Current (A)", epos4Main.lifter.current, -maxCurrent, maxCurrent);
        EditorGUILayout.Slider("Actual Pos (inch)", epos4Main.lifter.actualPosition, -maxActualPosition, maxActualPosition);
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
        if (GUILayout.Button("Activate PPM")) {
            epos4Main.lifter.ActivateProfilePositionMode();
        }
        epos4Main.lifter.profile.absolute     = EditorGUILayout.Toggle ("Absolute", epos4Main.lifter.profile.absolute);
        epos4Main.lifter.profile.position     = (int)EditorGUILayout.Slider("Position (inch)", epos4Main.lifter.profile.position, -maxPosition, maxPosition);
        epos4Main.lifter.profile.velocity     = (int)EditorGUILayout.Slider("Velocity (rpm)", epos4Main.lifter.profile.velocity, 0, maxVel);
        epos4Main.lifter.profile.acceleration = (int)EditorGUILayout.Slider("Acceleration (rpm/s)", epos4Main.lifter.profile.acceleration, 0, maxAcceleration);
        epos4Main.lifter.profile.deceleration = (int)EditorGUILayout.Slider("Deceleration (rpm/s)", epos4Main.lifter.profile.deceleration, 0, maxAcceleration);
        if (GUILayout.Button("Move as Profile")) {
            epos4Main.lifter.MoveToPosition();
        }
        EditorGUILayout.Space(20);

        using (new EditorGUILayout.HorizontalScope()) {
            using (new EditorGUILayout.VerticalScope()) {
                EditorGUILayout.LabelField("Left Pedal");
                EditorGUILayout.Slider("Current (A)", epos4Main.leftPedal.current, -maxCurrent, maxCurrent);
                EditorGUILayout.Slider("Actual Pos (inch)", epos4Main.leftPedal.actualPosition, -maxActualPosition, maxActualPosition);
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
                if (GUILayout.Button("Activate PPM")) {
                    epos4Main.leftPedal.ActivateProfilePositionMode();
                }
                epos4Main.leftPedal.profile.absolute     = EditorGUILayout.Toggle ("Absolute", epos4Main.leftPedal.profile.absolute);
                epos4Main.leftPedal.profile.position     = (int)EditorGUILayout.Slider("Position (inch)", epos4Main.leftPedal.profile.position, -maxPosition, maxPosition);
                epos4Main.leftPedal.profile.velocity     = (int)EditorGUILayout.Slider("Velocity (rpm)", epos4Main.leftPedal.profile.velocity, 0, maxVel);
                epos4Main.leftPedal.profile.acceleration = (int)EditorGUILayout.Slider("Acceleration (rpm/s)", epos4Main.leftPedal.profile.acceleration, 0, maxAcceleration);
                epos4Main.leftPedal.profile.deceleration = (int)EditorGUILayout.Slider("Deceleration (rpm/s)", epos4Main.leftPedal.profile.deceleration, 0, maxAcceleration);
                if (GUILayout.Button("Move as Profile")) {
                    epos4Main.leftPedal.MoveToPosition();
                }
                EditorGUILayout.Space(20);

                EditorGUILayout.LabelField("Left Slider");
                EditorGUILayout.Slider("Current (A)", epos4Main.leftSlider.current, -maxCurrent, maxCurrent);
                EditorGUILayout.Slider("Actual Pos (inch)", epos4Main.leftSlider.actualPosition, -maxActualPosition, maxActualPosition);
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
                if (GUILayout.Button("Activate PPM")) {
                    epos4Main.leftSlider.ActivateProfilePositionMode();
                }
                epos4Main.leftSlider.profile.absolute     = EditorGUILayout.Toggle ("Absolute", epos4Main.leftSlider.profile.absolute);
                epos4Main.leftSlider.profile.position     = (int)EditorGUILayout.Slider("Position (inch)", epos4Main.leftSlider.profile.position, -maxPosition, maxPosition);
                epos4Main.leftSlider.profile.velocity     = (int)EditorGUILayout.Slider("Velocity (rpm)", epos4Main.leftSlider.profile.velocity, 0, maxVel);
                epos4Main.leftSlider.profile.acceleration = (int)EditorGUILayout.Slider("Acceleration (rpm/s)", epos4Main.leftSlider.profile.acceleration, 0, maxAcceleration);
                epos4Main.leftSlider.profile.deceleration = (int)EditorGUILayout.Slider("Deceleration (rpm/s)", epos4Main.leftSlider.profile.deceleration, 0, maxAcceleration);
                if (GUILayout.Button("Move as Profile")) {
                    epos4Main.leftSlider.MoveToPosition();
                }
            }

            using (new EditorGUILayout.VerticalScope()) {
                EditorGUILayout.LabelField("Right Pedal");
                EditorGUILayout.Slider("Current (A)", epos4Main.rightPedal.current, -maxCurrent, maxCurrent);
                EditorGUILayout.Slider("Actual Pos (inch)", epos4Main.rightPedal.actualPosition, -maxActualPosition, maxActualPosition);
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
                if (GUILayout.Button("Activate PPM")) {
                    epos4Main.rightPedal.ActivateProfilePositionMode();
                }
                epos4Main.rightPedal.profile.absolute     = EditorGUILayout.Toggle ("Absolute", epos4Main.rightPedal.profile.absolute);
                epos4Main.rightPedal.profile.position     = (int)EditorGUILayout.Slider("Position (inch)", epos4Main.rightPedal.profile.position, -maxPosition, maxPosition);
                epos4Main.rightPedal.profile.velocity     = (int)EditorGUILayout.Slider("Velocity (rpm)", epos4Main.rightPedal.profile.velocity, 0, maxVel);
                epos4Main.rightPedal.profile.acceleration = (int)EditorGUILayout.Slider("Acceleration (rpm/s)", epos4Main.rightPedal.profile.acceleration, 0, maxAcceleration);
                epos4Main.rightPedal.profile.deceleration = (int)EditorGUILayout.Slider("Deceleration (rpm/s)", epos4Main.rightPedal.profile.deceleration, 0, maxAcceleration);
                if (GUILayout.Button("Move as Profile")) {
                    epos4Main.rightPedal.MoveToPosition();
                }
                EditorGUILayout.Space(20);

                EditorGUILayout.LabelField("Right Slider");
                EditorGUILayout.Slider("Current (A)", epos4Main.rightSlider.current, -maxCurrent, maxCurrent);
                EditorGUILayout.Slider("Actual Pos (inch)", epos4Main.rightSlider.actualPosition, -maxActualPosition, maxActualPosition);
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
                if (GUILayout.Button("Activate PPM")) {
                    epos4Main.rightSlider.ActivateProfilePositionMode();
                }
                epos4Main.rightSlider.profile.absolute     = EditorGUILayout.Toggle ("Absolute", epos4Main.rightSlider.profile.absolute);
                epos4Main.rightSlider.profile.position     = (int)EditorGUILayout.Slider("Position (inch)", epos4Main.rightSlider.profile.position, -maxPosition, maxPosition);
                epos4Main.rightSlider.profile.velocity     = (int)EditorGUILayout.Slider("Velocity (rpm)", epos4Main.rightSlider.profile.velocity, 0, maxVel);
                epos4Main.rightSlider.profile.acceleration = (int)EditorGUILayout.Slider("Acceleration (rpm/s)", epos4Main.rightSlider.profile.acceleration, 0, maxAcceleration);
                epos4Main.rightSlider.profile.deceleration = (int)EditorGUILayout.Slider("Deceleration (rpm/s)", epos4Main.rightSlider.profile.deceleration, 0, maxAcceleration);
                if (GUILayout.Button("Move as Profile")) {
                    epos4Main.rightSlider.MoveToPosition();
                }
            }
        }
    }
}