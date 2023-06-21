using UnityEngine;

public class MMoucheAMerde : MonoBehaviour
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
    [SerializeField] private float linearSpeedAdaptator = 2f;
    [SerializeField] private float oscillationSpeedAdaptator = 2f;
    [SerializeField] private float knockBackOscillationFrequency = 1;
    [SerializeField] private float knockBackRotationSpeed = 1;
    [SerializeField] private Vector2 oscillationKnockBackAmplitude = new(1f, 3f);
    [SerializeField] private Vector2 linearKnockBackForce = new(0.1f, 0.2f);
    [SerializeField] private Vector2 knockBackDepth = new(1f, 10f);
    [SerializeField] private MoucheAnimation moucheAnimation;
    [SerializeField] private float lerpSpeed = 0.2f;
    [SerializeField] private float lerpSpeed2 = 10f;
    [SerializeField] private float zRotPower = 5f;
    [SerializeField] private float mainLerpPower = 5f;

    //Audio : Wwise RTPC & variable distances
    [SerializeField] private AK.Wwise.RTPC RTPCdistancePlayerMouche;
    [SerializeField] private AK.Wwise.Event _playmouche;

    private Vector3 _pos;
    private float _oscillationKnockBackAmplitude;
    private float _linearKnockBackForce;
    private float _knockBackDepth;
    private bool _hitPlayer;
    private bool _hitBouclier;
    private float _alpha;
    private Vector3 _knockBackDirection;
    private PoubelleVisualManager _trashTankRef;
    private Vector3 _path;
    private Vector3 _initialPosition;
    private float _a;
    private float _initialA;
    private float _initialPreviousA;
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
    private Vector3 _previousPosition;
    private float distancePlayerMouche;
    private uint _enventID;
    private bool _eventState;
    private bool _wasAlive = true;
    private bool _isplayingDeathAnim;

    void Start()
    {
        _playmouche.Post(gameObject);
        _trashTankRef = GameObject.FindGameObjectWithTag("TrashTank").GetComponent<PoubelleVisualManager>();

        //_enventID = AkSoundEngine.PostEvent("Play_mouche_fly_loop", gameObject);
        _pos = transform.position;
        _previousA = 0;
        _a = Random.Range(-coneBeginingRadius, coneBeginingRadius);
        _initialA = _a;
        _initialLength = Vector3.Distance(body.position, _pos);
        _initialPosition = _pos;
        _previousPosition = _pos;

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
        _pos = transform.position;
        Vector3 bodypos = body.position;
        _path = bodypos - _initialPosition;

        distancePlayerMouche = Vector3.Distance(bodypos, transform.position);
        RTPCdistancePlayerMouche.SetValue(gameObject, distancePlayerMouche);

        _p = bodypos + Vector3.Project(_pos - bodypos, -_path);
        _a = _initialA * (Vector3.Distance(bodypos, _p) / _initialLength);
        _previousA = _initialPreviousA * (Vector3.Distance(bodypos, _p) / _initialLength);
        _limitVector = _a * Vector3.Normalize(new Vector3(_path.y, -_path.x, 0));


        if (_oscillationTimer > _oscillationTimerLimit)
        {
            _oscillationSpeedAddition = (int) Mathf.Pow(-1f, (float) Random.Range(1, 3));

            _oscillationSpeedIncrement = Random.Range(oscillationSpeedIncrement.x, oscillationSpeedIncrement.y);
            _oscillationTimer = 0;
            _oscillationTimerLimit = Random.Range(oscillationChangeTimer.x, oscillationChangeTimer.y);
        }

        if (_linearTimer > _linearTimerLimit)
        {
            _linearSpeedAddition = (int) Mathf.Pow(-1f, (float) Random.Range(1, 3));

            _linearSpeedIncrement = Random.Range(linearSpeedIncrement.x, linearSpeedIncrement.y);
            _linearTimer = 0;
            _linearTimerLimit = Random.Range(linearSpeedChangeTimer.x, linearSpeedChangeTimer.y);
        }

        if (_oscillationSpeedToTarget < oscillationSpeedLimit.x *
            (1 + Vector3.Distance(_pos, bodypos) / destroyDistance * oscillationSpeedAdaptator))
        {
            _oscillationSpeedToTarget = oscillationSpeedLimit.x *
                                        (1 + Vector3.Distance(_pos, bodypos) / destroyDistance *
                                            oscillationSpeedAdaptator);
        }

        if (_oscillationSpeedToTarget > oscillationSpeedLimit.y *
            (1 + Vector3.Distance(_pos, bodypos) / destroyDistance * oscillationSpeedAdaptator))
        {
            _oscillationSpeedToTarget = oscillationSpeedLimit.y *
                                        (1 + Vector3.Distance(_pos, bodypos) / destroyDistance *
                                            oscillationSpeedAdaptator);
        }

        if (_linearSpeedToTarget < linearSpeedLimit.x *
            (1 + Vector3.Distance(_pos, bodypos) / destroyDistance * linearSpeedAdaptator))
        {
            _linearSpeedToTarget = linearSpeedLimit.x *
                                   (1 + Vector3.Distance(_pos, bodypos) / destroyDistance * linearSpeedAdaptator);
        }

        if (_linearSpeedToTarget > linearSpeedLimit.y *
            (1 + Vector3.Distance(_pos, bodypos) / destroyDistance * linearSpeedAdaptator))
        {
            _linearSpeedToTarget = linearSpeedLimit.y *
                                   (1 + Vector3.Distance(_pos, bodypos) / destroyDistance * linearSpeedAdaptator);
        }

        float deltaTime = Time.deltaTime;
        _linearTimer += deltaTime;
        _linearSpeedToTarget += _linearSpeedAddition * _linearSpeedIncrement * deltaTime;

        _pos += Vector3.Normalize(_path) * (_linearSpeedToTarget * deltaTime);

        if (_a > 0 && _a > _previousA && Vector3.Dot(_pos - _p, _limitVector) > 0 &&
            Vector3.Magnitude(_pos - _p) / Vector3.Magnitude(_limitVector) > 1)
            _case1 = true;
        else _case1 = false;

        if (_a > 0 && _a < _previousA && Vector3.Dot(_pos - _p, _limitVector) > 0 &&
            Vector3.Magnitude(_pos - _p) / Vector3.Magnitude(_limitVector) < 1)
            _case2 = true;
        else _case2 = false;

        if (_a < 0 && _a < _previousA && Vector3.Dot(_pos - _p, _limitVector) > 0 &&
            Vector3.Magnitude(_pos - _p) / Vector3.Magnitude(_limitVector) > 1)
            _case3 = true;
        else _case3 = false;

        if (_a < 0 && _a > _previousA && Vector3.Dot(_pos - _p, _limitVector) > 0 &&
            Vector3.Magnitude(_pos - _p) / Vector3.Magnitude(_limitVector) < 1)
            _case4 = true;
        else _case4 = false;

        if (_case1 || _case2 || _case3 || _case4)
        {
            _previousA = _a;
            _initialPreviousA = _a;
            _a = Random.Range(
                -coneEndRadius - (coneBeginingRadius - coneEndRadius) * Vector3.Distance(bodypos, _p) /
                Vector3.Distance(bodypos, _initialPosition),
                coneEndRadius + (coneBeginingRadius - coneEndRadius) * Vector3.Distance(bodypos, _p) /
                Vector3.Distance(bodypos, _initialPosition));
            if (_a < 0.05f && _a > 0)
                _a = 0.05f;
            if (_a > -0.05f && _a < 0)
                _a = -0.05f;
            _initialA = _a;
            _initialLength = Vector3.Distance(bodypos, _p);
            if (_a > _previousA)
                _sign = 1;
            else _sign = -1;
        }
        
        _oscillationTimer += deltaTime;
        _oscillationSpeedToTarget += _oscillationSpeedAddition * _oscillationSpeedIncrement * deltaTime;

        _pos += Vector3.Normalize(new Vector3(_path.y, -_path.x, 0)) * (_sign * _oscillationSpeedToTarget * deltaTime);

        if (Vector3.Magnitude(_pos - bodypos) > destroyDistance && !_isplayingDeathAnim)
        {
            _isplayingDeathAnim = true;
            DestroyMouche();
        }

        Vector3 cachedLocalRot = fly.transform.localEulerAngles;
        fly.transform.localRotation = Quaternion.LerpUnclamped(fly.transform.localRotation,
            Quaternion.Euler(new Vector3((_previousPosition.y - _pos.y) * zRotPower, cachedLocalRot.y,
                cachedLocalRot.z)), lerpSpeed2 * deltaTime);
        transform.rotation = Quaternion.LerpUnclamped(transform.rotation,
            Quaternion.Euler(new Vector3(0, _pos.x > _previousPosition.x ? 0 : 180, 0)), lerpSpeed * deltaTime);

        if (_hitBouclier == false)
        {
            transform.position = Vector3.Lerp(transform.position, _pos, deltaTime * mainLerpPower);
        }
        else
        {
            if (_wasAlive)
            {
                _wasAlive = false;
                moucheAnimation.LaunchDeathAnimFeedback();
            }

            transform.position += -_knockBackDirection * (_linearKnockBackForce * deltaTime) +
                                  Vector3.Normalize(new Vector3(_knockBackDirection.y, -_knockBackDirection.x, 0)) * (_oscillationKnockBackAmplitude * deltaTime * Mathf.Sin(_alpha * deltaTime)) + Vector3.forward * _knockBackDepth * deltaTime;
            transform.eulerAngles += Vector3.forward * (deltaTime * knockBackRotationSpeed);
            _alpha += knockBackOscillationFrequency * deltaTime; 
        }
        _previousPosition = _pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.CompareTag("Bouclier") && !other.transform.CompareTag("Player")) return;

        AkSoundEngine.PostEvent("Play_mouche_punch", gameObject);


        if (other.transform.CompareTag("Player") && _hitPlayer == false)
        {
            if (bodyRB.drag < 5)
                bodyRB.AddForceAtPosition(forceDoremin * Vector3.Normalize(_path), transform.position);
            else if (bodyRB.drag > 5)
                bodyRB.AddForceAtPosition(forceMegaplex * Vector3.Normalize(_path), transform.position);
            if (_trashTankRef) _trashTankRef.PlayerHitByFly();
            _hitPlayer = true;

            GrabFeedback.emotionsInstance.angryPower = 100f;
        }

        if (!other.transform.CompareTag("Bouclier") || _hitBouclier) return;
        _hitBouclier = true;
        _linearKnockBackForce = Random.Range(linearKnockBackForce.x, linearKnockBackForce.y);
        _oscillationKnockBackAmplitude =
            Random.Range(oscillationKnockBackAmplitude.x, oscillationKnockBackAmplitude.y);
        _knockBackDirection = other.gameObject.transform.position - _pos;
        _knockBackDepth = Random.Range(knockBackDepth.x, knockBackDepth.y);
    }

    void DestroyMouche()
    {
        _playmouche.Stop(gameObject);
        moucheAnimation.Death();
    }


    private void OnDrawGizmos()
    {
        if (body == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_initialPosition, body.position);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_initialPosition + coneBeginingRadius * Vector3.Normalize(new Vector3(_path.y, -_path.x, 0)), body.position + coneEndRadius * Vector3.Normalize(new Vector3(_path.y, -_path.x, 0)));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_initialPosition - coneBeginingRadius * Vector3.Normalize(new Vector3(_path.y, -_path.x, 0)), body.position - coneEndRadius * Vector3.Normalize(new Vector3(_path.y, -_path.x, 0)));

        //Gizmos.DrawWireSphere(_p, 0.5f);
        Gizmos.DrawWireSphere(transform.position, 0.02f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(_p + _previousA * Vector3.Normalize(new Vector3(_path.y, -_path.x, 0)), 0.05f);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(_p, _p + _limitVector);
    }

}