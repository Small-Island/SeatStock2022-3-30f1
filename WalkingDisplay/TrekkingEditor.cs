using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Trekking))]
public class TrekkingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Trekking trekking = target as Trekking;

        // EditorGUIUtility.labelWidth = 200;
        // base.OnInspectorGUI();

        // if (GUILayout.Button("CSV Load, Init")) {
        //     trekking.init();
        // }

        if (GUILayout.Button("Walk Straight"))
        {
            // trekking.video.Play();
            trekking.WalkStraight();
            // trekking.DelaySample();
            // UnityEngine.Debug.Log("hello");
        }

        if (GUILayout.Button("Walk Stop"))
        {
            // trekking.video.Pause();
            trekking.WalkStop();
        }

        trekking.scaledLength.lift = trekking.length.lift * trekking.scale;
        trekking.scaledLength.pedal = trekking.length.pedal;
        trekking.scaledLength.pedalYaw = trekking.length.pedalYaw;
        trekking.scaledLength.legForward = trekking.length.legForward;
        trekking.scaledLength.legBackward = trekking.length.legBackward;
        trekking.scaledLength.stockExtendTopPoint = trekking.length.stockExtendTopPoint * trekking.scale;
        trekking.scaledLength.stockExtendPokePoint = trekking.length.stockExtendPokePoint * trekking.scale;
        trekking.scaledLength.stockSlideForward = trekking.length.stockSlideForward * trekking.scale;
        trekking.scaledLength.stockSlideBackward = trekking.length.stockSlideBackward * trekking.scale;
        trekking.tiltBackwardScaled = trekking.tiltBackward * trekking.scale;
        trekking.tiltForwardScaled = trekking.tiltForward * trekking.scale;

        // if (GUILayout.Button("Exit")) {
        //     trekking.video.Exit();
        // }
        EditorGUIUtility.labelWidth = 200;
        base.OnInspectorGUI();
    }
}