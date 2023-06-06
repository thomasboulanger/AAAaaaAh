using UnityEngine;

public class RTPCMeterCriRespi : MonoBehaviour
{
    [HideInInspector] public AK.Wwise.RTPC listenScreamRtpcAmplitudeValue;
    [HideInInspector] public AK.Wwise.RTPC listenRespiRtpcAmplitudeValue;
    [HideInInspector] public float lerpedValue;

    [SerializeField] private float mySize2;
    [SerializeField] private float lerpedValueRespi;
    [SerializeField] private float RawAmplitude;
    [SerializeField] private float RawAmplituderespi;
    [SerializeField] private GameObject _sphereRespi;
    [SerializeField] [Range(0f, 100f)] private float SmoothAmplitudeResponse = 0f;

    private float _duration = 100;
    private float _end = 1;
    
    void Update()
    {
        if (SmoothAmplitudeResponse < _duration)
        {
            //scream part

            // will contain the value of the RTPC parameter
            float t = SmoothAmplitudeResponse / _duration;
            RawAmplitude = (listenScreamRtpcAmplitudeValue.GetValue(gameObject) + 48f) / 48f * mySize2 * mySize2;

            t = t * t * (3f - 2f * t);

            lerpedValue = Mathf.Lerp(RawAmplitude, lerpedValue, t);

            // which will scale by the value of the RTPC parameter
            transform.localScale = new Vector3(lerpedValue, lerpedValue, lerpedValue);

            // respi part

            RawAmplituderespi = (listenRespiRtpcAmplitudeValue.GetValue(gameObject) + 48f) / 48f * mySize2 * mySize2;

            lerpedValueRespi = Mathf.Lerp(RawAmplituderespi, lerpedValue, t);
            _sphereRespi.transform.localScale = new Vector3(lerpedValueRespi, lerpedValueRespi, lerpedValueRespi);


            //V2 : Inspiration, expiration et cris s�par�s
        }
        else
        {
            lerpedValue = _end;
            lerpedValueRespi = _end;
        }
    }
}