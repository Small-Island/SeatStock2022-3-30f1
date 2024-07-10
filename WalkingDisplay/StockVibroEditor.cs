using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(StockVibro))]
public class StockVibroEditor : Editor
{
    public override void OnInspectorGUI()
    {
        StockVibro stockVibro = target as StockVibro;
        if (GUILayout.Button("Walk Straight")) {
            stockVibro.WalkStraight();
            
        }

        if (GUILayout.Button("Walk Stop")) {
            stockVibro.WalkStop();
        }
        EditorGUIUtility.labelWidth = 200;
        base.OnInspectorGUI();
    }
}