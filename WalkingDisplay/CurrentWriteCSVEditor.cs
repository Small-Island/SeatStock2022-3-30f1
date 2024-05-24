using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(CurrentWriteCSV))]
public class CurrentWriteCSVEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CurrentWriteCSV currentWriteCSV = target as CurrentWriteCSV;

        base.OnInspectorGUI();

        if (GUILayout.Button("Write Start"))
        {
            currentWriteCSV.writeStart();
            // walkingDisplayMain.DelaySample();
            // UnityEngine.Debug.Log("hello");
        }
    }
}