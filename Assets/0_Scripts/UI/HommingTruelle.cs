using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HommingTruelle : MonoBehaviour
{
    [SerializeField] private int matindex = 1;
    [SerializeField] private float rotationRandom = 50f;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private float selfDestructTimer = 2;
    [SerializeField] private float selfDestructSecurityTimer = 5;
    [SerializeField] private float aimationDuration = 2;
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

        meshRenderer.materials[matindex] = new Material(meshRenderer.materials[matindex]);
        _truelleMat = meshRenderer.materials[matindex];
        _truelleMat.SetColor("_BaseColor", _playerColor);

        transform.rotation = Quaternion.Euler(new Vector3(
            Random.Range(-rotationRandom, rotationRandom) + transform.rotation.eulerAngles.x,
            Random.Range(-rotationRandom, rotationRandom) + transform.rotation.eulerAngles.y,
            Random.Range(-rotationRandom, rotationRandom) + transform.rotation.eulerAngles.z));
        
        _rb.AddForce((_destination - _rb.transform.position) * 175);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Cursor"))
        {
            JoystickManager joystickComp = collision.gameObject.GetComponent<JoystickManager>();
            joystickComp.HitByTruelle(this);
        }
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
        if (!animating) return;
        if (_time < 1 && sens)
        {
            _time += Time.deltaTime * aimationDuration;

            truelleMesh.localScale =
                Vector3.LerpUnclamped(Vector3.zero, Vector3.one * _truelleMeshScale, scaleCurve.Evaluate(_time));
        }
        else if (_time < 1 && !sens)
        {
            _time += Time.deltaTime * aimationDuration;

            truelleMesh.localScale =
                Vector3.LerpUnclamped(Vector3.one * _truelleMeshScale, Vector3.zero, scaleCurve.Evaluate(_time));
        }
        else
        {
            animating = false;
            if (!sens)
            {
                Destroy(gameObject);
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