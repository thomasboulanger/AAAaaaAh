using System;
using UnityEngine;

public class RTPCMeterInspiExpi : MonoBehaviour
{
    [HideInInspector] public AK.Wwise.RTPC listenInspiRtpcAmplitudeValue;
    [HideInInspector] public AK.Wwise.RTPC listenExpiRtpcAmplitudeValue;

    public GameObject sphereRespi;

    public float myComponentSize = 1f;

    //Valeur a prendre pour l'inspiration
    public float lerpedValueInspi;

    //Valeur a prendre pour l'expiration
    public float lerpedValueExpi;

    //Valeur a prendre pour le cri
    public float lerpedValueScream;

    //Valeurs pour calculs d'experts tavu
    private float duration = 100;
    public float RawAmplitudeInspi;
    public float RawAmplitudeExpi;
    public float RawAmplitudeScream;

    [SerializeField] [Range(0f, 100f)] private float SmoothAmplitudeResponse;
    [SerializeField] private AudioManager audioManager;


    private float _end = 1;

    private void Start()
    {
        if (audioManager == null) audioManager = FindObjectOfType<AudioManager>();
    }

    void Update()
    {
        if (SmoothAmplitudeResponse < duration)
        {
            // will contain the value of the RTPC parameter
            float t = SmoothAmplitudeResponse / duration;
            RawAmplitudeInspi = (listenInspiRtpcAmplitudeValue.GetValue(gameObject) + 48f) / 48f * myComponentSize *
                                myComponentSize;

            t = t * t * (3f - 2f * t); //calcul de temps d'interpolation (groservo)

            lerpedValueInspi = Mathf.Lerp(RawAmplitudeInspi, lerpedValueInspi, t);

            // Expi part

            RawAmplitudeExpi = (listenExpiRtpcAmplitudeValue.GetValue(gameObject) + 48f) / 48f * myComponentSize *
                               myComponentSize;

            lerpedValueExpi = Mathf.Lerp(RawAmplitudeExpi, lerpedValueExpi, t);

            //Scream amplitude part
            RawAmplitudeScream = (audioManager.GetScreamRTPCValue(gameObject) + 48f) / 48f * myComponentSize * myComponentSize;

            lerpedValueScream = Mathf.Lerp(RawAmplitudeScream, lerpedValueScream, t);
        }
        else
        {
            // pour avoir une valeur acceptable en cas de temps d'int�gration sup�rieur � 1 ce qui exploserait la plan�te par le biais d'un paradoxe calculatoire
            lerpedValueInspi = _end;
            lerpedValueExpi = _end;
        }
    }
}