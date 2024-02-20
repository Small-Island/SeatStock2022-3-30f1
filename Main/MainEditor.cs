using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Main))]
public class MainEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Main main = target as Main;

        EditorGUI.BeginDisabledGroup(main.get_button_play_disabled());
        if (GUILayout.Button("play"))
        {
            main.button_play();
        }
        EditorGUI.EndDisabledGroup();
        if (GUILayout.Button("pause"))
        {
            main.button_pause();
        }
        if (GUILayout.Button("stop"))
        {
            main.button_stop();
        }
    }
}