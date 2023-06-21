using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Shaker : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float power = 1f;
    [SerializeField] private float powerSetting = 1f;
    [SerializeField] private float powerBuildings;
    [SerializeField] private float powerBuildingsSetting = 1f;
    [SerializeField] private Vector3[] _mediumNoise = new Vector3[10];
    [SerializeField] private Vector3[] _largeNoise = new Vector3[10];
    [SerializeField] private Vector3[] _expertise = new Vector3[10];
    [SerializeField] private RTPCMeterInspiExpi rtpcScript;
    [SerializeField] private bool useSound;
    [SerializeField] private bool scaleOnDistance;
    [SerializeField] private float soundMultiplier = 20f;
    [Range(0, 2)] [SerializeField] private int noiseSelector;
    [SerializeField] private List<Transform> buildingsToMove = new();
    [SerializeField] private float scalePower = 1f;
    [SerializeField] private float added = 1f;

    [FormerlySerializedAs("_am")] [SerializeField]
    private AudioManager audioManager;

    private int _index;
    private Vector3 _basePos;
    private Vector3 _monsterPos;
    private List<Vector3> _basePoseBuildings = new();
    private float distanceMax;
    private bool wasBuildingMoving;


    void Start()
    {
        _monsterPos = FindObjectOfType<BlendShapesAnim>().transform.position;
        distanceMax = Vector3.Distance(transform.position, _monsterPos);

        foreach (Transform item in buildingsToMove)
        {
            _basePoseBuildings.Add(item.transform.localPosition);
        }

        _mediumNoise = RandomArray(-2f, 2f, 10);
        _largeNoise = RandomArray(-10f, 10f, 10);
        _expertise = RandomArray(-200f, 200f, 10);
        _basePos = transform.localPosition;
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;
        if (useSound)
        {
            float powerByDistance = 1f;
            if (scaleOnDistance)
            {
                powerByDistance = ((distanceMax / Vector3.Distance(transform.position, _monsterPos)) + added) *
                                  scalePower;
            }

            transform.localPosition = Vector3.Lerp(_basePos,
                _basePos + ChooseArray(noiseSelector)[_index] *
                (rtpcScript.RawAmplitudeScream * soundMultiplier * powerByDistance * power), deltaTime * speed);
        }
        else
            transform.localPosition = Vector3.Lerp(_basePos,
                _basePos + ChooseArray(noiseSelector)[_index] * (power * powerSetting), deltaTime * speed);


        if (powerBuildings > 0)
        {
            for (int i = 0; i < buildingsToMove.Count; i++)
            {
                buildingsToMove[i].localPosition = Vector3.Lerp(_basePoseBuildings[i],
                    _basePoseBuildings[i] + ChooseArray(noiseSelector)[Random.Range(0, 10)] *
                    (powerBuildings * powerBuildingsSetting), deltaTime * speed);
                wasBuildingMoving = true;
            }
        }
        else if (wasBuildingMoving)
        {
            wasBuildingMoving = false;
            for (int i = 0; i < buildingsToMove.Count; i++)
            {
                buildingsToMove[i].localPosition = _basePoseBuildings[i];
                wasBuildingMoving = true;
            }
        }

        _index++;
        if (_index > 9) _index = 0;
    }

    Vector3[] RandomArray(float min, float max, int count)
    {
        List<Vector3> tempArray = new List<Vector3>();
        for (int i = 0; i < count; i++)
            tempArray.Add(new Vector3(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max)));
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