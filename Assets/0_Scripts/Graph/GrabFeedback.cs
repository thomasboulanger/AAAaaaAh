using System;
using UnityEngine;

public class GrabFeedback : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer baseMesh;
    [SerializeField] private SkinnedMeshRenderer outlineMesh;
    [SerializeField] private AnimationCurve grabCurve;

    private float[] _grabStates = new float[4];
    private float[] _lastGrabStates = new float[4];

    public void UpdateGrabValue(Component sender, object data1, object playerID, object inputID)
    {
        if (playerID is not int) return;
        if (inputID is not int) return;
        if (data1 is not float) return;

        _grabStates[(int) inputID] = (float) data1;

        if (Math.Abs(_lastGrabStates[(int) inputID] - _grabStates[(int) inputID]) < .01f) return;

        float grabValue = grabCurve.Evaluate(_grabStates[(int) inputID]);
        baseMesh.SetBlendShapeWeight((int) inputID, grabValue * 100);
        outlineMesh.SetBlendShapeWeight((int) inputID, grabValue * 100);
        _lastGrabStates[(int) inputID] = _grabStates[(int) inputID];
    }
}