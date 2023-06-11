using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float power = 1f;
    [SerializeField] private Vector3[] _mediumNoise = new Vector3[10];
    [SerializeField] private Vector3[] _largeNoise = new Vector3[10];
    [SerializeField] private Vector3[] _expertise = new Vector3[10];
    [SerializeField] private RTPCMeterInspiExpi rtpcScript;
    [SerializeField] private bool useSound;
    [SerializeField] private float soundMultiplier = 20f;
    [SerializeField] private Vector3 offset = Vector3.zero;
    [Range(0, 2)] [SerializeField] private int noiseSelector;
    private int _i;

    private Vector3 _basePos;

    void Start()
    {
        _mediumNoise = RandomArray(-2f, 2f, 10);
        _largeNoise = RandomArray(-10f, 10f, 10);
        _expertise = RandomArray(-200f, 200f, 10);
        _basePos = transform.localPosition;
    }

    void Update()
    {
        if (useSound)
        {
            transform.localPosition = Vector3.Lerp(_basePos, _basePos + ChooseArray(noiseSelector)[_i] * rtpcScript.RawAmplitudeScream * soundMultiplier, Time.deltaTime * speed);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(_basePos, _basePos + ChooseArray(noiseSelector)[_i] * power, Time.deltaTime * speed);
        }


        _i++;
        if (_i > 9) _i = 0;
    }

    Vector3[] RandomArray(float min, float max, int n)
    {
        List<Vector3> tempArray = new List<Vector3>();
        for (int i = 0; i < n; i++)
        {
            tempArray.Add(new Vector3(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max)));
        }
        return tempArray.ToArray();
    }

    Vector3[] ChooseArray(int i)
    {
        return i switch
        {
            0 => _mediumNoise,
            1 => _largeNoise,
            2 => _expertise,
            _ => _mediumNoise
        };
    }
}