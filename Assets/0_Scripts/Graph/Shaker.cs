using System.Collections.Generic;
using UnityEngine;

public class Shaker : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float power = 1f;
    [SerializeField] private float powerSetting = 1f;
    [SerializeField] private float powerBuildings = 0f;
    [SerializeField] private float powerBuildingsSetting = 1f;
    [SerializeField] private Vector3[] _mediumNoise = new Vector3[10];
    [SerializeField] private Vector3[] _largeNoise = new Vector3[10];
    [SerializeField] private Vector3[] _expertise = new Vector3[10];
    [SerializeField] private RTPCMeterInspiExpi rtpcScript;
    [SerializeField] private bool useSound;
    [SerializeField] private float soundMultiplier = 20f;
    [SerializeField] private float soundMultiplierBuildings = 20f;

    [SerializeField] private Vector3 offset = Vector3.zero;
    [Range(0, 2)] [SerializeField] private int noiseSelector;

    [SerializeField] private List<Transform> buildingsToMove = new List<Transform>();
    private List<Vector3> basePoseBuildings = new List<Vector3>();

    [SerializeField] private bool scaleOnDistance;
    [SerializeField] private float scalePower = 1f;
    [SerializeField] private float added = 1f;


    [SerializeField] private AudioManager _am;
    private int _i;

    private Vector3 _basePos;
    private Vector3 _monsterPos;

    private float distanceMax;

    private bool wasBuildingMoving;


    void Start()
    {
        _monsterPos = GameObject.FindObjectOfType<BlendShapesAnim>().transform.position;
        distanceMax = Vector3.Distance(transform.position, _monsterPos);

        foreach (Transform item in buildingsToMove)
        {
            basePoseBuildings.Add(item.transform.localPosition);
        }

        _mediumNoise = RandomArray(-2f, 2f, 10);
        _largeNoise = RandomArray(-10f, 10f, 10);
        _expertise = RandomArray(-200f, 200f, 10);
        _basePos = transform.localPosition;
    }

    void Update()
    {

        float dt = Time.deltaTime;
        Debug.Log(_am.listenMusicRtpc.GetValue(gameObject));

        if (useSound)
        {
            float powerByDistance = ((distanceMax/Vector3.Distance(transform.position, _monsterPos))+ added) * scalePower;

            transform.localPosition = Vector3.Lerp(_basePos, _basePos + ChooseArray(noiseSelector)[_i] * rtpcScript.RawAmplitudeScream * soundMultiplier* powerByDistance * power, dt * speed);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(_basePos, _basePos + ChooseArray(noiseSelector)[_i] * power * powerSetting, dt * speed);
        }

        if (powerBuildings>0)
        {
            for (int i = 0; i < buildingsToMove.Count; i++)
            {
                buildingsToMove[i].localPosition = Vector3.Lerp(basePoseBuildings[i], basePoseBuildings[i] + ChooseArray(noiseSelector)[Random.Range(0, 10)] * powerBuildings* powerBuildingsSetting, dt * speed);
                wasBuildingMoving = true;
            }
        }
        else if (wasBuildingMoving)
        {
            wasBuildingMoving = false;
            for (int i = 0; i < buildingsToMove.Count; i++)
            {
                buildingsToMove[i].localPosition = basePoseBuildings[i];
                wasBuildingMoving = true;
            }
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