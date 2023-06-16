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
    [SerializeField] private float force = 5;
    [SerializeField] private Transform[] bouclierList = new Transform[4];
    [SerializeField] private float forceDoremin = 100f;
    [SerializeField] private float forceMegaplex = 300f;
    [SerializeField] private float linearSpeedAdaptator=  2f;
    [SerializeField] private float oscillationSpeedAdaptator = 2f;

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

    public float lerpSpeed = 0.2f;
    public float lerpSpeed2 = 10f;
    public float zRotPower = 5f;

    public float mainLerpPower = 5f;


    private Vector3 _previousPosition;

    //Audio : Wwise RTPC & variable distances
    [SerializeField] private AK.Wwise.RTPC RTPCdistancePlayerMouche;
    private float distancePlayerMouche = 0f;

    private uint _enventID;

    void Start()
    {
        _trashTankRef = GameObject.FindGameObjectWithTag("TrashTank").GetComponent<PoubelleVisualManager>();

        _enventID = AkSoundEngine.PostEvent("Play_mouche_fly_loop", gameObject);
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
        

        Vector3 pos = transform.position; // limiter l'impact perf
        Vector3 bodypos = body.position;
        //Debug.Log(_case1 + "   " + _case2 + "   " + _case3 + "   " + _case4 + "   ");
        //Debug.Log(_oscillationSpeedToTarget);
        //Debug.Log(Mathf.Sign(_a));
        //Debug.Log(_oscillationSpeedToTarget);

        _path = bodypos - _initialPosition;

        //------------------------------------------------AUDIO-------------------------
        distancePlayerMouche = Vector3.Distance(bodypos, transform.position);
        //Debug.Log(distancePlayerMouche);
        RTPCdistancePlayerMouche.SetValue(gameObject, distancePlayerMouche);

        //---------------------
        _p = bodypos + Vector3.Project(pos - bodypos, -_path);
        _a = _initialA * (Vector3.Distance(bodypos, _p) / _initialLength);
        _previousA = _initialPreviousA * (Vector3.Distance(bodypos, _p) / _initialLength);
        //Debug.Log(Vector3.Distance(bodypos, _p) / _initialLength);
        _limitVector = _a * Vector3.Normalize(new Vector3(_path.y, -_path.x, 0));


        if (_oscillationTimer > _oscillationTimerLimit)
        {
            _oscillationSpeedAddition = (int)Mathf.Pow(-1f, (float)Random.Range(1, 3));

            _oscillationSpeedIncrement = Random.Range(oscillationSpeedIncrement.x, oscillationSpeedIncrement.y);
            _oscillationTimer = 0;
            _oscillationTimerLimit = Random.Range(oscillationChangeTimer.x, oscillationChangeTimer.y);
        }

        if (_linearTimer > _linearTimerLimit)
        {
            _linearSpeedAddition = (int)Mathf.Pow(-1f, (float)Random.Range(1, 3));

            _linearSpeedIncrement = Random.Range(linearSpeedIncrement.x, linearSpeedIncrement.y);
            _linearTimer = 0;
            _linearTimerLimit = Random.Range(linearSpeedChangeTimer.x, linearSpeedChangeTimer.y);
        }

        if (_oscillationSpeedToTarget < oscillationSpeedLimit.x * (1 + Vector3.Distance(pos, bodypos) / destroyDistance * oscillationSpeedAdaptator))
        {
            _oscillationSpeedToTarget = oscillationSpeedLimit.x * (1 + Vector3.Distance(pos, bodypos) / destroyDistance * oscillationSpeedAdaptator);
        }

        if (_oscillationSpeedToTarget > oscillationSpeedLimit.y * (1 + Vector3.Distance(pos, bodypos) / destroyDistance * oscillationSpeedAdaptator))
        {
            _oscillationSpeedToTarget = oscillationSpeedLimit.y * (1 + Vector3.Distance(pos, bodypos) / destroyDistance * oscillationSpeedAdaptator);
        }

        if (_linearSpeedToTarget < linearSpeedLimit.x * (1 + Vector3.Distance(pos, bodypos) / destroyDistance * linearSpeedAdaptator))
        {
            _linearSpeedToTarget = linearSpeedLimit.x * (1 + Vector3.Distance(pos, bodypos) / destroyDistance * linearSpeedAdaptator);
        }

        if (_linearSpeedToTarget > linearSpeedLimit.y * (1 + Vector3.Distance(pos, bodypos) / destroyDistance * linearSpeedAdaptator))
        {
            _linearSpeedToTarget = linearSpeedLimit.y * (1 + Vector3.Distance(pos, bodypos) / destroyDistance * linearSpeedAdaptator);
        }

        float dt = Time.deltaTime; //modif maros pour calmer l'esprit du chef prog

        _linearTimer += dt;
        _linearSpeedToTarget += _linearSpeedAddition * _linearSpeedIncrement * dt;

        pos += _linearSpeedToTarget * Vector3.Normalize(_path) * dt; //opti

        if (_a > 0 && _a > _previousA && Vector3.Dot(pos - _p, _limitVector) > 0 &&
            Vector3.Magnitude(pos - _p) / Vector3.Magnitude(_limitVector) > 1)
        {
            _case1 = true;
        }
        else
        {
            _case1 = false;
        }

        if (_a > 0 && _a < _previousA && Vector3.Dot(pos - _p, _limitVector) > 0 &&
            Vector3.Magnitude(pos - _p) / Vector3.Magnitude(_limitVector) < 1)
        {
            _case2 = true;
        }
        else
        {
            _case2 = false;
        }

        if (_a < 0 && _a < _previousA && Vector3.Dot(pos - _p, _limitVector) > 0 &&
            Vector3.Magnitude(pos - _p) / Vector3.Magnitude(_limitVector) > 1)
        {
            _case3 = true;
        }
        else
        {
            _case3 = false;
        }

        if (_a < 0 && _a > _previousA && Vector3.Dot(pos - _p, _limitVector) > 0 &&
            Vector3.Magnitude(pos - _p) / Vector3.Magnitude(_limitVector) < 1)
        {
            _case4 = true;
        }
        else
        {
            _case4 = false;
        }

        if (_case1 || _case2 || _case3 || _case4)
        {
            _previousA = _a;
            _initialPreviousA = _a;
            //_a = Random.Range(
            //    -coneEndRadius - coneBeginingRadius * Vector3.Distance(bodypos, _p) /
            //    (coneBeginingRadius / coneEndRadius),
            //    coneEndRadius + coneBeginingRadius * Vector3.Distance(bodypos, _p) /
            //    (coneBeginingRadius / coneEndRadius));
            _a = Random.Range(-coneEndRadius - (coneBeginingRadius - coneEndRadius) * Vector3.Distance(bodypos, _p) / Vector3.Distance(bodypos, _initialPosition), coneEndRadius + (coneBeginingRadius-coneEndRadius) * Vector3.Distance(bodypos, _p) /Vector3.Distance(bodypos, _initialPosition));
            if (_a < 0.05f && _a > 0)
                _a = 0.05f;
            if (_a > -0.05f && _a < 0)
                _a = -0.05f;
            _initialA = _a;
            _initialLength = Vector3.Distance(bodypos, _p);
            if (_a > _previousA)
            {
                _sign = 1;
            }
            else
            {
                _sign = -1;
            }
        }


        _oscillationTimer += dt;
        _oscillationSpeedToTarget += _oscillationSpeedAddition * _oscillationSpeedIncrement * dt;

        pos += _sign * _oscillationSpeedToTarget * Vector3.Normalize(new Vector3(_path.y, -_path.x, 0)) * dt;

        //_angle = Vector3.SignedAngle(Vector3.right, _oscillationSpeedToTarget * Vector3.Normalize(_limitVector) + _speedToTarget * Vector3.Normalize(_path), Vector3.forward);

        //float _angle = Vector3.SignedAngle(Vector3.right, transform.position - _previousPosition, Vector3.forward); //du coup plus besoin-----------------------------------------------

        //transform.eulerAngles = new Vector3(0, 0, _angle);
        //transform.rotation = Quaternion.LerpUnclamped( transform.rotation, Quaternion.Euler( new Vector3(0, 0, _angle)), lerpSpeed * dt);// modif maros pour l'angle--------------------




        Vector3 cachedLocalRot = fly.transform.localEulerAngles;
        fly.transform.localRotation = Quaternion.LerpUnclamped(fly.transform.localRotation,
            Quaternion.Euler(new Vector3((_previousPosition.y - pos.y) * zRotPower, cachedLocalRot.y,
                cachedLocalRot.z)), lerpSpeed2 * dt); //-----
        transform.rotation = Quaternion.LerpUnclamped(transform.rotation,
            Quaternion.Euler(new Vector3(0, pos.x > _previousPosition.x ? 0 : 180, 0)), lerpSpeed * dt);
        _previousPosition = pos;

        if (Vector3.Magnitude(pos - bodypos) > destroyDistance)
        {
            VoiceCleanUp();

            Destroy(gameObject);
        }

        transform.position = Vector3.Lerp(transform.position, pos, dt * mainLerpPower);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (!other.transform.CompareTag("Bouclier") && !other.transform.CompareTag("Player")) return;
        Debug.Log("doubidoubidou");

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
        Debug.Log("doubidoubidou");
        Destroy(gameObject);
    }

    private void VoiceCleanUp() =>
        AkSoundEngine.StopPlayingID(_enventID, 200, AkCurveInterpolation.AkCurveInterpolation_Constant);
    private void OnDrawGizmos()
    {
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