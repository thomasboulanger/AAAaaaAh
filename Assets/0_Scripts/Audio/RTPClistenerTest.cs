using UnityEngine;

public class RTPClistenerTest : MonoBehaviour
{
    [HideInInspector] public AK.Wwise.RTPC listenMusicRtpcAmplitudeValue;

    public float mySize;
    public float lerpedValue;
    private float duration = 100;
    float end = 1;
    public float RawAmplitude;

    [Range(0f, 100f)] 
    [SerializeField] private float SmoothAmplitudeResponse = 0f;

    void Update()
    {
        if (SmoothAmplitudeResponse < duration)
        {
            // will contain the value of the RTPC parameter
            float t = SmoothAmplitudeResponse / duration;
            RawAmplitude = (listenMusicRtpcAmplitudeValue.GetValue(gameObject) + 48f) / 48f * mySize * mySize;

            t = t * t * (3f - 2f * t);

            lerpedValue = Mathf.Lerp(RawAmplitude, lerpedValue, t);

            // which will scale by the value of the RTPC parameter
            transform.localScale = new Vector3(lerpedValue, lerpedValue, lerpedValue);

            //transform.localPosition = new Vector3(value, value, value);
        }
        else lerpedValue = end;
    }
}