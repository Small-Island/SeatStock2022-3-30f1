using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(IntegratedControl))]
public class IntegratedControlEditor : Editor
{
    public override void OnInspectorGUI()
    {
        IntegratedControl integratedControl = target as IntegratedControl;

        // EditorGUIUtility.labelWidth = 200;
        // base.OnInspectorGUI();

        // if (GUILayout.Button("CSV Load, Init")) {
        //     integratedControl.init();
        // }

        if (GUILayout.Button("Walk Straight"))
        {
            // integratedControl.video.Play();
            integratedControl.WalkStraight();
            // integratedControl.DelaySample();
            // UnityEngine.Debug.Log("hello");
        }

        if (GUILayout.Button("Walk Stop"))
        {
            // integratedControl.video.Pause();
            integratedControl.WalkStop();
        }

        integratedControl.scaledLength.lift = integratedControl.length.lift * integratedControl.scale;
        integratedControl.scaledLength.pedal = integratedControl.length.pedal;
        integratedControl.scaledLength.pedalYaw = integratedControl.length.pedalYaw;
        integratedControl.scaledLength.legForward = integratedControl.length.legForward;
        integratedControl.scaledLength.legBackward = integratedControl.length.legBackward;
        integratedControl.scaledLength.stockExtendTopPoint = integratedControl.length.stockExtendTopPoint * integratedControl.scale;
        integratedControl.scaledLength.stockExtendPokePoint = integratedControl.length.stockExtendPokePoint * integratedControl.scale;
        integratedControl.scaledLength.stockSlideForward = integratedControl.length.stockSlideForward * integratedControl.scale;
        integratedControl.scaledLength.stockSlideBackward = integratedControl.length.stockSlideBackward * integratedControl.scale;
        integratedControl.tiltBackwardScaled = integratedControl.tiltBackward * integratedControl.scale;
        integratedControl.tiltForwardScaled = integratedControl.tiltForward * integratedControl.scale;

        // if (GUILayout.Button("Exit")) {
        //     integratedControl.video.Exit();
        // }
        EditorGUIUtility.labelWidth = 200;
        base.OnInspectorGUI();
    }
}