using System;
using UnityEngine;

public class GrabFeedback : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer baseMesh;
    [SerializeField] private SkinnedMeshRenderer outlineMesh;
    [SerializeField] private AnimationCurve grabCurve;

    private float[] _grabStates = new float[4];
    private float[] _lastGrabStates = new float[4];

    private Rigidbody _playerRB;

    [HideInInspector] public float sadPower = 0;
    [HideInInspector] public float angryPower = 0;

    private float _sadSpeed = 1;
    private float _angrySpeed = 3;

    public static GrabFeedback emotionsInstance;

    private void Start()
    {
        emotionsInstance = this;

        transform.parent.TryGetComponent<Rigidbody>(out _playerRB);
    }
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

    private void Update()
    {
        if (_playerRB == null) return;
        // 6 7 sad angry
        float dt = Time.deltaTime;

        sadPower = Mathf.Lerp(sadPower, 0, dt * _sadSpeed);
        angryPower = Mathf.Lerp(angryPower, 0, dt * _angrySpeed);

        baseMesh.SetBlendShapeWeight(6, sadPower);
        baseMesh.SetBlendShapeWeight(7, angryPower);
    }
}