using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SkinnedMeshRenderer))]
public class MoucheAnimation : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _skinnedMesh;

    [Header("ShapeKey animations curves")]
    [SerializeField] private AnimationCurve wingsFlap;
    [SerializeField] private AnimationCurve idle;
    [SerializeField] private AnimationCurve tilt;
    [Header("ShapeKey animations floats")]
    [SerializeField] private float flapSpeed = 1;
    [Range(0, 1)]
    [SerializeField] private float flapDesynch = 0.5f;
    [SerializeField] private float idle1Speed = 1;
    [SerializeField] private float idle2Speed = 2;
    [SerializeField] private float tiltSpeed = 1;
    [Header("Position animations curves")]
    [SerializeField] private AnimationCurve positionY;
    [SerializeField] private AnimationCurve rotationZ;
    [Header("position animations floats")]
    [SerializeField] private float positionSpeed = 1;
    [SerializeField] private float positionPower = 1;
    [SerializeField] private float rotationSpeed = 1;
    [SerializeField] private float rotationPower = 1;
    [Header("powers of flap1, flap2, idle1, idle2, tilt")]
    [Range(0,200)]
    [SerializeField] private float[] shapeKeyPowers = new float[5] { 100, 100, 100, 100, 100 };

    [SerializeField] private Color[] possibleColors = new Color[5];


    private float[] timers = new float[7];
    private Vector3 localRot;
    private float dt;

    void Start()
    {
        


        localRot = transform.localEulerAngles;

        if (_skinnedMesh == null) _skinnedMesh = GetComponent<SkinnedMeshRenderer>(); // au cas ou c pas assign
        //randomize mat
        _skinnedMesh.material.SetColor("_ORMADodge", possibleColors[Random.Range(0, possibleColors.Length)]);

        //random animation at start
        for (int i = 0; i < timers.Length; i++)
        {
            if (i==1)
            {
                timers[1] = timers[0] + flapDesynch;
                continue;
            }
            timers[i] = Random.value;
        }
    }

    void Update()
    {
        dt = Time.deltaTime;

        _skinnedMesh.SetBlendShapeWeight(0, wingsFlap.Evaluate(timers[0]) * shapeKeyPowers[0]);
        _skinnedMesh.SetBlendShapeWeight(1, wingsFlap.Evaluate(timers[1]) * shapeKeyPowers[1]);
        _skinnedMesh.SetBlendShapeWeight(2, idle.Evaluate(timers[2]) * shapeKeyPowers[2]);
        _skinnedMesh.SetBlendShapeWeight(3, idle.Evaluate(timers[3]) * shapeKeyPowers[3]);
        _skinnedMesh.SetBlendShapeWeight(4, tilt.Evaluate(timers[4]) * shapeKeyPowers[4]);

        transform.localPosition = new Vector3(transform.localPosition.x, positionY.Evaluate(timers[5]) * positionPower, transform.localPosition.z);
        transform.localEulerAngles = new Vector3(localRot.x, localRot.y, rotationZ.Evaluate(timers[6]) * rotationPower);

        IncrementTimers();
    }

    void IncrementTimers()
    {
        for (int i = 0; i < timers.Length; i++)
        {
            if (i == 1)
            {
                timers[i] = timers[0] + flapDesynch;
                continue;
            }
            timers[i] += dt * GetMultiplier(i);
            if (timers[i] >= 1) timers[i] = 0;
        }
    }

    float GetMultiplier(int index)
    {
        switch (index)
        {
            case 0:
            case 1:
                return flapSpeed;
            case 2:
                return idle1Speed;
            case 3:
                return idle2Speed;
            case 4:
                return tiltSpeed;
            case 5:
                return positionSpeed;
            case 6:
                return rotationSpeed;
            default:
                Debug.LogWarning("a bah cringe");
                return 0;
        }
    }
}
