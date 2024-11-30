using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(ESP32Main))]
public class ESP32MainEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ESP32Main esp32Main = target as ESP32Main;

        // EditorGUIUtility.labelWidth = 200;
        // base.OnInspectorGUI();

        if (GUILayout.Button("Write"))
        {
            esp32Main.Write();
        }

        EditorGUIUtility.labelWidth = 200;
        base.OnInspectorGUI();
    }
}