using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(WalkingDisplayMain))]
public class WalkingDisplayMainEditor : Editor
{
    public override void OnInspectorGUI()
    {
        WalkingDisplayMain walkingDisplayMain = target as WalkingDisplayMain;

        EditorGUIUtility.labelWidth = 200;
        base.OnInspectorGUI();

        // if (GUILayout.Button("CSV Load, Init")) {
        //     walkingDisplayMain.init();
        // }

        if (GUILayout.Button("Walk Straight"))
        {
            walkingDisplayMain.WalkStraight(0);
            // walkingDisplayMain.DelaySample();
            // UnityEngine.Debug.Log("hello");
        }

        if (GUILayout.Button("Walk Stop"))
        {
            walkingDisplayMain.WalkStop();
        }
    }
}