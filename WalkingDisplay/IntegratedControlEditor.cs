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
        integratedControl.scaledLength.pedal = integratedControl.length.pedal * integratedControl.scale;
        integratedControl.scaledLength.pedalYaw = integratedControl.length.pedalYaw * integratedControl.scale;
        integratedControl.scaledLength.legForward = integratedControl.length.legForward * integratedControl.scale;
        integratedControl.scaledLength.legBackward = integratedControl.length.legBackward * integratedControl.scale;
        integratedControl.scaledLength.stockExtendTopPoint = integratedControl.length.stockExtendTopPoint * integratedControl.scale;
        integratedControl.scaledLength.stockExtendPokePoint = integratedControl.length.stockExtendPokePoint * integratedControl.scale;
        integratedControl.scaledLength.stockSlideForward = integratedControl.length.stockSlideForward * integratedControl.scale;
        integratedControl.scaledLength.stockSlideBackward = integratedControl.length.stockSlideBackward * integratedControl.scale;
        integratedControl.scaledLength.windHigh = integratedControl.length.windHigh * integratedControl.scale;
        integratedControl.scaledLength.windLow = integratedControl.length.windLow * integratedControl.scale;


        integratedControl.lifter.position1 = integratedControl.scaledLength.lift;
        integratedControl.lifter.position2 = 0;

        integratedControl.leftPedalYaw.position1 = -integratedControl.scaledLength.pedalYaw;
        integratedControl.leftPedalYaw.position2 = integratedControl.scaledLength.pedalYaw;

        integratedControl.rightPedalYaw.position1 = -integratedControl.scaledLength.pedalYaw;
        integratedControl.rightPedalYaw.position2 = integratedControl.scaledLength.pedalYaw;

        integratedControl.leftPedal.position1 = integratedControl.scaledLength.pedal;
        integratedControl.leftPedal.position2 = 0;

        integratedControl.rightPedal.position1 = integratedControl.scaledLength.pedal;
        integratedControl.rightPedal.position2 = 0;

        integratedControl.leftSlider.position1 = integratedControl.scaledLength.legForward;
        integratedControl.leftSlider.position2 = -integratedControl.scaledLength.legBackward;

        integratedControl.rightSlider.position1 = integratedControl.scaledLength.legForward;
        integratedControl.rightSlider.position2 = -integratedControl.scaledLength.legBackward;

        integratedControl.stockLeftExtend.position1 = integratedControl.scaledLength.stockExtendTopPoint;
        integratedControl.stockLeftExtend.position2 = integratedControl.scaledLength.stockExtendPokePoint;
        integratedControl.stockLeftExtend.position3 = 0;

        integratedControl.stockRightExtend.position1 = integratedControl.scaledLength.stockExtendTopPoint;
        integratedControl.stockRightExtend.position2 = integratedControl.scaledLength.stockExtendPokePoint;
        integratedControl.stockRightExtend.position3 = 0;


        integratedControl.stockLeftSlider.position1 = integratedControl.scaledLength.stockSlideForward;
        integratedControl.stockLeftSlider.position2 = -integratedControl.scaledLength.stockSlideBackward;

        integratedControl.stockRightSlider.position1 = integratedControl.scaledLength.stockSlideForward;
        integratedControl.stockRightSlider.position2 = -integratedControl.scaledLength.stockSlideBackward;

        integratedControl.windLeft.position1 = integratedControl.length.windHigh;
        integratedControl.windLeft.position2 = integratedControl.length.windLow;

        integratedControl.windRight.position1 = integratedControl.length.windHigh;
        integratedControl.windRight.position2 = integratedControl.length.windLow;


        // if (GUILayout.Button("Exit")) {
        //     integratedControl.video.Exit();
        // }
        EditorGUIUtility.labelWidth = 200;
        base.OnInspectorGUI();
    }
}