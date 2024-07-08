using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(CyclicSynchronousPositionMode))]
public class CyclicSynchronousPositionModeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CyclicSynchronousPositionMode cyclicSynchronousPositionMode = target as CyclicSynchronousPositionMode;

        // EditorGUIUtility.labelWidth = 200;
        // base.OnInspectorGUI();

        // if (GUILayout.Button("CSV Load, Init")) {
        //     walkingDisplayMain.init();
        // }

        if (GUILayout.Button("Start"))
        {
            cyclicSynchronousPositionMode.StartCyclicSynchronousPositionMode();
        }

        if (GUILayout.Button("Stop"))
        {
            cyclicSynchronousPositionMode.StopCyclicSynchronousPositionMode();
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