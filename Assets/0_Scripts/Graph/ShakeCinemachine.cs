using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ShakeCinemachine : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCam;
    private CinemachineBasicMultiChannelPerlin perlinnoise;
    [SerializeField] private Transform _monsterPosTransform;
    [SerializeField] private float added;
    [SerializeField] private float scalePower = 2f;

    [SerializeField] private RTPCMeterInspiExpi rtpcScript;


    private Vector3 _monsterPos;


    private float distanceMax;

    void Start()
    {
        perlinnoise = virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        perlinnoise.m_AmplitudeGain = 0f;

        _monsterPos = _monsterPosTransform.position;

        distanceMax = Vector3.Distance(transform.position, _monsterPos);
    }

    void Update()
    {
        float powerByDistance = Mathf.Clamp( ((distanceMax / Vector3.Distance(transform.position, _monsterPos)) + added) *scalePower, 0, 99999) * rtpcScript.RawAmplitudeScream;
        perlinnoise.m_AmplitudeGain = powerByDistance ;
        Debug.Log(powerByDistance);

    }
}
