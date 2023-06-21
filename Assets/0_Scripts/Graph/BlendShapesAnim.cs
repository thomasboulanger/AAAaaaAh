using UnityEngine;

public class BlendShapesAnim : MonoBehaviour
{
    [SerializeField] private RTPCMeterInspiExpi rtpcScript;
    [SerializeField] private float powerBide = 100f;
    [SerializeField] private float powerGlotte = 100f;
    [SerializeField] private float powerSide = 100f;
    [SerializeField] private float lerpSide = 100f;
    [SerializeField] private float noisePowerSide = 100f;
    [SerializeField] private float timeToFadeInOut = 0.5f;
    [SerializeField] private bool screams;
    [Range(0,1)]
    [SerializeField] private float transitionthreshold = 0.5f;
    [SerializeField] private float lerpedintensityVal = 0.5f;
    [SerializeField] private float lerpSpeed = 2f;
    [SerializeField] private AnimationCurve fadeOut;
    [SerializeField] private float redValLerp = 2f;
    [SerializeField] private SkinnedMeshRenderer skinnedMesh;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private float mouthLerpSpeed = 50f;
    [SerializeField] private float mouthSpeed = 700f;

    [SerializeField] private bool canScream;


    private float _blendShape0;
    private float _blendShape1;
    private float _blendShape2;
    private float _redVal;
    private float _fadeOutTime = 0f;
    private float _fadeInTime = 0f;
    private bool _firstPass = true;
    private float _lastNoise = 100f;
    private float _mouthSK;
    private float _mouthSKTarget;



    void Update()
    {
        float deltaTime = Time.deltaTime;

        if (_mouthSKTarget>0)
        {
            _mouthSKTarget -= deltaTime * mouthSpeed;
            _mouthSK = Mathf.Lerp(_mouthSK, _mouthSKTarget, deltaTime * mouthLerpSpeed);
            skinnedMesh.SetBlendShapeWeight(3, _mouthSK);
            if (!(_mouthSKTarget>0))
            {
                skinnedMesh.SetBlendShapeWeight(3, 0);
            }
        }



        lerpedintensityVal = Mathf.Lerp(lerpedintensityVal, rtpcScript.lerpedValueScream, deltaTime * lerpSpeed);
        screams = lerpedintensityVal > transitionthreshold;


        _redVal = Mathf.Lerp(_redVal, rtpcScript.lerpedValueScream, deltaTime * redValLerp);

        animator.SetFloat("ScreamVal",_redVal);

        skinnedMesh.sharedMaterial.SetFloat("_RedSwitch", _redVal);

        if (screams)
        {
            if (_fadeInTime<1f)
            {
                _fadeInTime += deltaTime * timeToFadeInOut;
            }

            _firstPass = true;
            _fadeOutTime = 0;

            skinnedMesh.SetBlendShapeWeight(1, rtpcScript.lerpedValueScream * powerBide * _fadeInTime);
            skinnedMesh.SetBlendShapeWeight(0, rtpcScript.lerpedValueScream * powerGlotte * _fadeInTime);
            _lastNoise = Mathf.Lerp(_lastNoise, (Random.Range(-1f, 1f) * noisePowerSide), lerpSide * deltaTime);
            skinnedMesh.SetBlendShapeWeight(2, rtpcScript.lerpedValueScream * powerSide * _lastNoise * _fadeInTime);

        }else if (_fadeOutTime < 1f)
        {
            if (_firstPass)
            {
                _blendShape0 = skinnedMesh.GetBlendShapeWeight(0);
                _blendShape1 = skinnedMesh.GetBlendShapeWeight(1);
                _blendShape2 = skinnedMesh.GetBlendShapeWeight(2);
                _firstPass = false;
                _fadeInTime = 0f;
            }
            _fadeOutTime += deltaTime * timeToFadeInOut;

            skinnedMesh.SetBlendShapeWeight(0, fadeOut.Evaluate(_fadeOutTime) * _blendShape0);
            skinnedMesh.SetBlendShapeWeight(1, fadeOut.Evaluate(_fadeOutTime) * _blendShape1);
            skinnedMesh.SetBlendShapeWeight(2, fadeOut.Evaluate(_fadeOutTime) * _blendShape2);
        }
    }

    public void RespiCheck(int expiInspi)
    {
        if (screams) return;

        if (expiInspi == 0)
        {
            audioManager.ExpirationSound(gameObject);
        }
        else
        {
            audioManager.InspirationSound(gameObject);
        }
    }

    public void OpenMouth()
    {
        _mouthSKTarget = 100;
    }

    public void CanScream(bool status)
    {
        rtpcScript.RawAmplitudeScream = 0;
        rtpcScript.enabled = status;
    }
}
