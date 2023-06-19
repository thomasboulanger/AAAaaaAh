using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SkinnedMeshRenderer))]
public class MoucheAnimation : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer skinnedMesh;

    [Header("ShapeKey animations curves")]
    [SerializeField] private AnimationCurve wingsFlap;
    [SerializeField] private AnimationCurve idle;
    [SerializeField] private AnimationCurve tilt;
    [SerializeField] private AnimationCurve deathBounce;
    [SerializeField] private AnimationCurve deathScaleAnim;


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
    [SerializeField] private float randPositionSpeed = 0.2f;
    [SerializeField] private float randPositionSpeed2 = 0.2f;
    [SerializeField] private float randPositionPower = 0.2f;
    [SerializeField] private float deathSpeed = 0.5f;

    [SerializeField] private float deathScaleSpeed = 0.5f;

    [Header("powers of flap1, flap2, idle1, idle2, tilt")]
    [Range(0, 200)]
    [SerializeField] private float[] shapeKeyPowers = new float[6] { 100, 100, 100, 100, 100, 100 };

    [SerializeField] private Color[] possibleColors = new Color[5];

    private float[] timers = new float[9];
    private Vector3 localRot;
    private float dt;
    private Vector3 noiseVector;
    private Vector3 movementVector;
    private bool _deadFeedback;
    [SerializeField] private bool _dead;

    void Start()
    {

        localRot = transform.localEulerAngles;

        if (skinnedMesh == null) skinnedMesh = GetComponent<SkinnedMeshRenderer>(); // au cas ou c pas assign
        //randomize mat
        skinnedMesh.material.SetColor("_ORMADodge", possibleColors[Random.Range(0, possibleColors.Length)]);

        //random animation at start
        for (int i = 0; i < timers.Length; i++)
        {
            if (i == 1)
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

        if (_deadFeedback) skinnedMesh.SetBlendShapeWeight(5, deathBounce.Evaluate(timers[7]) * shapeKeyPowers[5]);

        if (_dead)
        {
            float scale = deathScaleAnim.Evaluate(timers[8])*100;
            transform.localScale = new Vector3(scale, scale, scale);
        }
        else
        {
            skinnedMesh.SetBlendShapeWeight(0, wingsFlap.Evaluate(timers[0]) * shapeKeyPowers[0]);
            skinnedMesh.SetBlendShapeWeight(1, wingsFlap.Evaluate(timers[1]) * shapeKeyPowers[1]);
            skinnedMesh.SetBlendShapeWeight(2, idle.Evaluate(timers[2]) * shapeKeyPowers[2]);
            skinnedMesh.SetBlendShapeWeight(3, idle.Evaluate(timers[3]) * shapeKeyPowers[3]);
            skinnedMesh.SetBlendShapeWeight(4, tilt.Evaluate(timers[4]) * shapeKeyPowers[4]);
        }



        Vector3 zocilation = new Vector3(0, positionY.Evaluate(timers[5]) * positionPower, 0);
        noiseVector = Vector3.Lerp(noiseVector, RandomDirection(), dt * randPositionSpeed);
        movementVector = Vector3.Lerp(movementVector, (movementVector + noiseVector).normalized, dt * randPositionSpeed2);

        transform.localPosition = zocilation + movementVector * randPositionPower;
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

            if (i == 8)
            {
                if (_dead)
                {
                    timers[i] += dt * GetMultiplier(i);
                    if (timers[i]>=1) Destroy(transform.parent.parent.gameObject);
                }
                else continue;

            }
            else timers[i] += dt * GetMultiplier(i);


            if (i == 7 && _deadFeedback && timers[7] >= 1) _deadFeedback = false;


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
            case 7:
                return deathSpeed;
            case 8:
                return deathScaleSpeed;
            default:
                Debug.LogWarning("a bah cringe");
                return 0;
        }
    }

    Vector3 RandomDirection()
    {
        return new Vector3(TrueRandomValue01(), TrueRandomValue01(), TrueRandomValue01()).normalized;
    }

    float TrueRandomValue01()
    {
        return (Random.value - 0.5f) * 2f;
    }

    public void LaunchDeathAnimFeedback()
    {
        timers[7] = 0; //resetTimerDeathFeedback
        _deadFeedback = true;
    }

    public void Death()
    {
        for (int i = 0; i <= 4; i++)
        {
            skinnedMesh.SetBlendShapeWeight(i, 0);
        }

        _dead = true;
    }
}
