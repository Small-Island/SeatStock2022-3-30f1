using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(WalkingDisplayMain))]
public class WalkingDisplayMainEditor : Editor
{
    public override void OnInspectorGUI()
    {
        WalkingDisplayMain walkingDisplayMain = target as WalkingDisplayMain;

        // EditorGUIUtility.labelWidth = 200;
        // base.OnInspectorGUI();

        // if (GUILayout.Button("CSV Load, Init")) {
        //     walkingDisplayMain.init();
        // }

        if (GUILayout.Button("Walk Straight"))
        {
            walkingDisplayMain.video.Play();
            walkingDisplayMain.WalkStraight();
            // walkingDisplayMain.DelaySample();
            // UnityEngine.Debug.Log("hello");
        }

        if (GUILayout.Button("Walk Stop"))
        {
            walkingDisplayMain.video.Pause();
            walkingDisplayMain.WalkStop();
        }

        if (GUILayout.Button("Exit")) {
            walkingDisplayMain.video.Exit();
        }
        EditorGUIUtility.labelWidth = 200;
        base.OnInspectorGUI();
    }
}