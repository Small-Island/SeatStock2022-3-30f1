// using UnityEngine;
// using UnityEditor;
[UnityEditor.CustomEditor(typeof(VideoControl))]
public class VideoControlEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        VideoControl videoControl = target as VideoControl;

        // EditorGUIUtility.labelWidth = 200;
        // base.OnInspectorGUI();

        // if (GUILayout.Button("CSV Load, Init")) {
        //     walkingDisplayMain.init();
        // }

        if (UnityEngine.GUILayout.Button("Play"))
        {
            videoControl.Play();
        }

        if (UnityEngine.GUILayout.Button("Stop"))
        {
            videoControl.Stop();
        }

        if (UnityEngine.GUILayout.Button("Pause")) {
            videoControl.Pause();
        }
        UnityEditor.EditorGUIUtility.labelWidth = 200;
        base.OnInspectorGUI();
    }
}