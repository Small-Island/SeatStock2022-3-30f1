using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(StockDragVibro))]
public class StockDragVibroEditor : Editor
{
    public override void OnInspectorGUI()
    {
        StockDragVibro stockDragVibro = target as StockDragVibro;
        if (GUILayout.Button("Walk Straight")) {
            stockDragVibro.WalkStraight();
            
        }

        if (GUILayout.Button("Walk Stop")) {
            stockDragVibro.WalkStop();
        }
        EditorGUIUtility.labelWidth = 200;
        base.OnInspectorGUI();
    }
}