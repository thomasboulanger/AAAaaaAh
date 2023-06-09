using UnityEngine;

public class MoucheAMerde : MonoBehaviour
{
    [HideInInspector] public Transform body;
    [HideInInspector] public Rigidbody bodyRB;

    [SerializeField] private Transform fly;
    [SerializeField] private float coneBeginingRadius = 10;
    [SerializeField] private float coneEndRadius = 1;
    [SerializeField] private float destroyDistance = 15;
    [SerializeField] private Vector2 oscillationChangeTimer = new(0.2f, 0.4f);
    [SerializeField] private Vector2 oscillationSpeedLimit = new(0.1f, 2f);
    [SerializeField] private Vector2 oscillationSpeedIncrement = new(0.5f, 10f);
    [SerializeField] private Vector2 linearSpeedChangeTimer = new(0.2f, 0.4f);
    [SerializeField] private Vector2 linearSpeedLimit = new(0.1f, 2f);
    [SerializeField] private Vector2 linearSpeedIncrement = new(0.5f, 10f);
    [SerializeField] private float forceDoremin = 100f;
    [SerializeField] private float forceMegaplex = 300f;

    private PoubelleVisualManager _trashTankRef;
    private Vector3 _path;
    private Vector3 _initialPosition;
    private float _a;
    private float _initialA;
    private float _initialLength;
    private float _previousA;
    private int _sign = 1;
    private Vector3 _p;
    private Vector3 _limitVector;
    private bool _case1;
    private bool _case2;
    private bool _case3;
    private bool _case4;
    private float _oscillationTimer;
    private float _oscillationTimerLimit;
    private int _oscillationSpeedAddition;
    private float _oscillationSpeedIncrement;
    private float _oscillationSpeedToTarget;
    private float _linearTimer;
    private float _linearTimerLimit;
    private int _linearSpeedAddition;
    private float _linearSpeedIncrement;
    private float _linearSpeedToTarget;
    public float lerpSpeed = 0.2f;
    public float lerpSpeed2 = 10f;
    public float zRotPower = 5f;
    public float mainLerpPower = 5f;
    private Vector3 _previousPosition;

    [SerializeField] private AK.Wwise.RTPC RTPCdistancePlayerMouche;
    private float distancePlayerMouche;

    private uint _enventID;
    private bool _eventState;

    void Start()
    {
        _trashTankRef = GameObject.FindGameObjectWithTag("TrashTank").GetComponent<PoubelleVisualManager>();

        Vector3 pos = transform.position;
        _previousA = 0;
        _a = Random.Range(-coneBeginingRadius, coneBeginingRadius);
        _initialA = _a;
        _initialLength = Vector3.Distance(body.position, pos);
        _initialPosition = pos;
        _previousPosition = pos;

        if (_a > _previousA) _sign = 1;
        else _sign = -1;

        _oscillationTimer = 0;
        _oscillationTimerLimit = 1;
        _oscillationSpeedToTarget = 0.5f;
        _oscillationSpeedAddition = 1;
        _oscillationSpeedIncrement = Random.Range(oscillationSpeedIncrement.x, oscillationSpeedIncrement.y);

        _linearTimer = 0;
        _linearTimerLimit = 1;
        _linearSpeedToTarget = 0.5f;
        _linearSpeedAddition = 1;
        _linearSpeedIncrement = Random.Range(linearSpeedIncrement.x, linearSpeedIncrement.y);
    }

    void Update()
    {
        Vector3 pos = transform.position;
        Vector3 bodypos = body.position;
        _path = bodypos - _initialPosition;

        distancePlayerMouche = Vector3.Distance(bodypos, transform.position);
        Debug.Log(distancePlayerMouche);
        RTPCdistancePlayerMouche.SetValue(gameObject, distancePlayerMouche);

        if (distancePlayerMouche < 11 && !_eventState)
        {
            if (!IsEventPlayingOnGameObject()) AkSoundEngine.PostEvent("Play_mouche_fly_loop", gameObject);
            _eventState = true;
        }

        if (distancePlayerMouche > 14 && _eventState)
        {
            if (IsEventPlayingOnGameObject()) VoiceCleanUp();
            _eventState = false;
        }

        _p = bodypos + Vector3.Project(pos - bodypos, -_path);
        _a = _initialA * (Vector3.Distance(bodypos, _p) / _initialLength);
        _limitVector = _a * Vector3.Normalize(new Vector3(_path.y, -_path.x, 0));

        if (_oscillationTimer > _oscillationTimerLimit)
        {
            _oscillationSpeedAddition = (int) Mathf.Pow(-1f, Random.Range(1, 3));

            _oscillationSpeedIncrement = Random.Range(oscillationSpeedIncrement.x, oscillationSpeedIncrement.y);
            _oscillationTimer = 0;
            _oscillationTimerLimit = Random.Range(oscillationChangeTimer.x, oscillationChangeTimer.y);
        }

        if (_linearTimer > _linearTimerLimit)
        {
            _linearSpeedAddition = (int) Mathf.Pow(-1f, Random.Range(1, 3));

            _linearSpeedIncrement = Random.Range(linearSpeedIncrement.x, linearSpeedIncrement.y);
            _linearTimer = 0;
            _linearTimerLimit = Random.Range(linearSpeedChangeTimer.x, linearSpeedChangeTimer.y);
        }

        if (_oscillationSpeedToTarget < oscillationSpeedLimit.x)
            _oscillationSpeedToTarget = oscillationSpeedLimit.x;

        if (_oscillationSpeedToTarget > oscillationSpeedLimit.y)
            _oscillationSpeedToTarget = oscillationSpeedLimit.y;

        if (_linearSpeedToTarget < linearSpeedLimit.x)
            _linearSpeedToTarget = linearSpeedLimit.x;

        if (_linearSpeedToTarget > linearSpeedLimit.y)
            _linearSpeedToTarget = linearSpeedLimit.y;

        float deltaTime = Time.deltaTime;

        _linearTimer += deltaTime;
        _linearSpeedToTarget += _linearSpeedAddition * _linearSpeedIncrement * deltaTime;

        pos += Vector3.Normalize(_path) * (_linearSpeedToTarget * deltaTime);

        if (_a > 0 && _a > _previousA && Vector3.Dot(pos - _p, _limitVector) > 0 &&
            Vector3.Magnitude(pos - _p) / Vector3.Magnitude(_limitVector) > 1)
            _case1 = true;
        else _case1 = false;

        if (_a > 0 && _a < _previousA && Vector3.Dot(pos - _p, _limitVector) > 0 &&
            Vector3.Magnitude(pos - _p) / Vector3.Magnitude(_limitVector) < 1)
            _case2 = true;
        else _case2 = false;

        if (_a < 0 && _a < _previousA && Vector3.Dot(pos - _p, _limitVector) > 0 &&
            Vector3.Magnitude(pos - _p) / Vector3.Magnitude(_limitVector) > 1)
            _case3 = true;
        else _case3 = false;

        if (_a < 0 && _a > _previousA && Vector3.Dot(pos - _p, _limitVector) > 0 &&
            Vector3.Magnitude(pos - _p) / Vector3.Magnitude(_limitVector) < 1)
            _case4 = true;
        else _case4 = false;

        if (_case1 || _case2 || _case3 || _case4)
        {
            _previousA = _a;
            _a = Random.Range(
                -coneEndRadius - coneBeginingRadius * Vector3.Distance(bodypos, _p) /
                (coneBeginingRadius / coneEndRadius),
                coneEndRadius + coneBeginingRadius * Vector3.Distance(bodypos, _p) /
                (coneBeginingRadius / coneEndRadius));
            _initialA = _a;
            _initialLength = Vector3.Distance(bodypos, _p);
            if (_a > _previousA)
                _sign = 1;
            else _sign = -1;
        }


        _oscillationTimer += deltaTime;
        _oscillationSpeedToTarget += _oscillationSpeedAddition * _oscillationSpeedIncrement * deltaTime;

        pos += Vector3.Normalize(new Vector3(_path.y, -_path.x, 0)) * (_sign * _oscillationSpeedToTarget * deltaTime);

        Vector3 cachedLocalRot = fly.transform.localEulerAngles;
        fly.transform.localRotation = Quaternion.LerpUnclamped(fly.transform.localRotation,
            Quaternion.Euler(new Vector3((_previousPosition.y - pos.y) * zRotPower, cachedLocalRot.y,
                cachedLocalRot.z)), lerpSpeed2 * deltaTime);
        transform.rotation = Quaternion.LerpUnclamped(transform.rotation,
            Quaternion.Euler(new Vector3(0, pos.x > _previousPosition.x ? 0 : 180, 0)), lerpSpeed * deltaTime);
        _previousPosition = pos;

        if (Vector3.Magnitude(pos - bodypos) > destroyDistance)
        {
            VoiceCleanUp();
            Destroy(gameObject);
        }

        transform.position = Vector3.Lerp(transform.position, pos, deltaTime * mainLerpPower);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.CompareTag("Bouclier") || !other.transform.CompareTag("Player")) return;

        AkSoundEngine.PostEvent("Play_mouche_punch", gameObject);
        VoiceCleanUp();

        if (other.transform.CompareTag("Player"))
        {
            if (bodyRB.drag < 5)
                bodyRB.AddForceAtPosition(forceDoremin * Vector3.Normalize(_path), transform.position);
            else if (bodyRB.drag > 5)
                bodyRB.AddForceAtPosition(forceMegaplex * Vector3.Normalize(_path), transform.position);
            if (_trashTankRef) _trashTankRef.PlayerHitByFly();
        }

        Destroy(gameObject);
    }

    private void VoiceCleanUp() =>
        AkSoundEngine.StopPlayingID(_enventID, 200, AkCurveInterpolation.AkCurveInterpolation_Constant);

    private bool IsEventPlayingOnGameObject()
    {
        uint[] playingIds = new uint[10];
        GameObject gom = gameObject;
        string eventName = "Play_mouche_fly_loop";
        uint testEventId = AkSoundEngine.GetIDFromString(eventName);

        uint count = (uint) playingIds.Length;
        AKRESULT result = AkSoundEngine.GetPlayingIDsFromGameObject(gom, ref count, playingIds);

        for (int i = 0; i < count; i++)
        {
            uint playingId = playingIds[i];
            uint eventId = AkSoundEngine.GetEventIDFromPlayingID(playingId);

            if (eventId == testEventId)
                return true;
        }

        return false;
    }
}