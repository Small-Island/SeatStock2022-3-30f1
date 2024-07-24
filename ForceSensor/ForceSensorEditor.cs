using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(ForceSensor))]
public class ForceSensorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ForceSensor forceSensor = target as ForceSensor;

        // EditorGUIUtility.labelWidth = 200;
        // base.OnInspectorGUI();

        // if (GUILayout.Button("CSV Load, Init")) {
        //     walkingDisplayMain.init();
        // }

        if (GUILayout.Button("Info"))
        {
            forceSensor.info();
        }

        EditorGUIUtility.labelWidth = 200;
        base.OnInspectorGUI();
    }
}