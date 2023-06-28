using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HommingTruelle : MonoBehaviour
{
    [SerializeField] private GameEvent onTruelleHitJoystickSound;

    [SerializeField] private int matIndex = 1;
    [SerializeField] private Renderer meshRenderer;
    [SerializeField] private float selfDestructTimer = 2;
    [SerializeField] private float selfDestructSecurityTimer = 5;
    [SerializeField] private float animationDuration = 2;
    [SerializeField] private float force = 175;
    [SerializeField] private Transform truelleMesh;
    [SerializeField] private AnimationCurve scaleCurve;

    private Vector3 _destination;
    private bool _homming;
    private Color _playerColor;
    private Rigidbody _rb;
    private Material _truelleMat;
    private float _time;
    private float _truelleMeshScale;
    private bool animating = true;
    private bool sens = true;
    private bool _isStuckInUI;
    private float _timerBeforeDestroy;

    public void Init(Vector3 targetDestination) => _destination = targetDestination;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _truelleMeshScale = truelleMesh.localScale.x;
        truelleMesh.localScale = Vector3.zero;
        StartCoroutine(SecurityTimer());

        meshRenderer.materials[matIndex] = new Material(meshRenderer.materials[matIndex]);
        _truelleMat = meshRenderer.materials[matIndex];
        _truelleMat.SetColor("_BaseColor", _playerColor);

        _rb.AddForce((_destination - transform.position) * force);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Cursor"))
        {
            if (!GameManager.TrowelBouncing) StopTruelle();
            onTruelleHitJoystickSound.Raise(this, null, null, null);
            JoystickManager joystickComp = collision.gameObject.GetComponent<JoystickManager>();
            joystickComp.HitByTruelle(this);
        }
        else if (collision.transform.CompareTag("UIInteractable"))
        {
            if (!GameManager.TrowelBouncing) StopTruelle();
            onTruelleHitJoystickSound.Raise(this, null, null, null);
            collision.transform.GetComponent<UIButtonInfo>().ChangePanelButton();
        }
    }

    public void StopTruelle()
    {
        _isStuckInUI = true;
        _timerBeforeDestroy = 3.5f;
        GetComponent<Collider>().enabled = false;
        _rb.isKinematic = true;
    }

    public void InitiateDiscontrol()
    {
        _rb.drag = 0;
        _rb.angularDrag = 0;
        _rb.useGravity = true;

        StartCoroutine(Timer());
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        if (_isStuckInUI)
        {
            _timerBeforeDestroy -= deltaTime;
            if (!(_timerBeforeDestroy < 0)) return;
            _rb.isKinematic = false;
            _rb.useGravity = true;
            Destroy(gameObject, 2);
            return;
        }

        if (!animating) return;

        switch (_time)
        {
            case < 1 when sens:
                _time += deltaTime * animationDuration;
                truelleMesh.localScale =
                    Vector3.LerpUnclamped(Vector3.zero, Vector3.one * _truelleMeshScale, scaleCurve.Evaluate(_time));
                break;
            case < 1 when !sens:
                _time += deltaTime * animationDuration;
                truelleMesh.localScale =
                    Vector3.LerpUnclamped(Vector3.one * _truelleMeshScale, Vector3.zero, scaleCurve.Evaluate(_time));
                break;
            default:
            {
                animating = false;
                if (!sens)
                {
                    Destroy(gameObject);
                }

                break;
            }
        }
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(selfDestructTimer);
        animating = true;
        _time = 0;
        sens = false;
    }

    IEnumerator SecurityTimer()
    {
        yield return new WaitForSeconds(selfDestructSecurityTimer);
        Destroy(gameObject);
    }
}