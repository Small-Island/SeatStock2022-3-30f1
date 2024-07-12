using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Trekking))]
public class TrekkingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Trekking trekking = target as Trekking;

        // EditorGUIUtility.labelWidth = 200;
        // base.OnInspectorGUI();

        // if (GUILayout.Button("CSV Load, Init")) {
        //     trekking.init();
        // }

        if (GUILayout.Button("Walk Straight"))
        {
            // trekking.video.Play();
            trekking.WalkStraight();
            // trekking.DelaySample();
            // UnityEngine.Debug.Log("hello");
        }

        if (GUILayout.Button("Walk Stop"))
        {
            // trekking.video.Pause();
            trekking.WalkStop();
        }

        // if (GUILayout.Button("Exit")) {
        //     trekking.video.Exit();
        // }
        EditorGUIUtility.labelWidth = 200;
        base.OnInspectorGUI();
    }
}