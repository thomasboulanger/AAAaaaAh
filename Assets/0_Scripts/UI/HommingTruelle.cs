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
    [SerializeField] private Transform truelleMesh;
    [SerializeField] private AnimationCurve scaleCurve;

    private Vector3 _destination;
    private bool _homming;
    private Color _playerColor;
    private Rigidbody _rb;
    private Material _truelleMat;
    private float _time;
    private float _truelleMeshScale;
    private bool _animating = true;
    private bool _sens = true;

    public void Init(Vector3 truelleTargetPoint, Color actualColor)
    {
        _destination = truelleTargetPoint;
        _playerColor = actualColor;
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _truelleMeshScale = truelleMesh.localScale.x;
        truelleMesh.localScale = Vector3.zero;
        StartCoroutine(SecurityTimer());

        meshRenderer.materials[matIndex] = new Material(meshRenderer.materials[matIndex]);
        _truelleMat = meshRenderer.materials[matIndex];
        _truelleMat.SetColor("_BaseColor", _playerColor);

        transform.rotation = Quaternion.Euler
        (
            Random.Range(-50, 50),
            Random.Range(-50, 50),
            Random.Range(-50, 50)
        );
        _rb.AddForce((_destination - _rb.transform.position) * 175);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Cursor"))
        {
            if (!GameManager.TrowelBouncing) Destroy(gameObject);
            onTruelleHitJoystickSound.Raise(this, null, null, null);
            JoystickManager joystickComp = collision.gameObject.GetComponent<JoystickManager>();
            joystickComp.HitByTruelle(this);
        }

        if (!collision.transform.CompareTag("UIInteractable")) return;
        if (!GameManager.TrowelBouncing) Destroy(gameObject);
        onTruelleHitJoystickSound.Raise(this, null, null, null);
        collision.transform.GetComponent<UIButtonInfo>().ChangePanelButton();
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
        if (!_animating) return;
        switch (_time)
        {
            case < 1 when _sens:
                _time += Time.deltaTime * animationDuration;

                truelleMesh.localScale =
                    Vector3.LerpUnclamped(Vector3.zero, Vector3.one * _truelleMeshScale, scaleCurve.Evaluate(_time));
                break;
            case < 1 when !_sens:
                _time += Time.deltaTime * animationDuration;

                truelleMesh.localScale =
                    Vector3.LerpUnclamped(Vector3.one * _truelleMeshScale, Vector3.zero, scaleCurve.Evaluate(_time));
                break;
            default:
            {
                _animating = false;
                if (!_sens)
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
        _animating = true;
        _time = 0;
        _sens = false;
    }

    IEnumerator SecurityTimer()
    {
        yield return new WaitForSeconds(selfDestructSecurityTimer);
        Destroy(gameObject);
    }
}