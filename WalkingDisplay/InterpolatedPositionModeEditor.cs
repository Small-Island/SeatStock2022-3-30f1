using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(InterpolatedPositionMode))]
public class InterpolatedPositionModeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        InterpolatedPositionMode interpolatedPositionMode = target as InterpolatedPositionMode;

        // EditorGUIUtility.labelWidth = 200;
        // base.OnInspectorGUI();

        // if (GUILayout.Button("CSV Load, Init")) {
        //     walkingDisplayMain.init();
        // }

        if (GUILayout.Button("Start"))
        {
            interpolatedPositionMode.StartInterpolatedPositionMode();
        }

        if (GUILayout.Button("Stop"))
        {
            interpolatedPositionMode.StopInterpolatedPositionMode();
        }

        // if (GUILayout.Button("Walk Stop"))
        // {
        //     walkingDisplayMain.video.Pause();
        //     walkingDisplayMain.WalkStop();
        // }

        // if (GUILayout.Button("Exit")) {
        //     walkingDisplayMain.video.Exit();
        // }
        EditorGUIUtility.labelWidth = 200;
        base.OnInspectorGUI();
    }
}