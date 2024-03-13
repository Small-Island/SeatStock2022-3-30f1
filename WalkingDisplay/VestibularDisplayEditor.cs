using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(VestibularDisplay))]
public class VestibularDisplayEditor : Editor
{
    public override void OnInspectorGUI()
    {
        VestibularDisplay vestibularDisplay = target as VestibularDisplay;

        base.OnInspectorGUI();

        if (GUILayout.Button("Wave Start"))
        {
            vestibularDisplay.WaveStart();
        }

        if (GUILayout.Button("Wave Stop"))
        {
            vestibularDisplay.WaveStop();
        }
    }
}