using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(LowerLimb6MotorSerial))]
public class LowerLimb6MotorSerialEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LowerLimb6MotorSerial lowerLimb6MotorSerial = target as LowerLimb6MotorSerial;

        EditorGUIUtility.labelWidth = 200;
        base.OnInspectorGUI();

        // if (GUILayout.Button("CSV Load, Init")) {
        //     walkingDisplayMain.init();
        // }

        if (GUILayout.Button("Walk Straight"))
        {
            lowerLimb6MotorSerial.WalkStraight();
            // walkingDisplayMain.DelaySample();
            // UnityEngine.Debug.Log("hello");
        }

        if (GUILayout.Button("Walk Stop"))
        {
            lowerLimb6MotorSerial.WalkStop();
        }
    }
}