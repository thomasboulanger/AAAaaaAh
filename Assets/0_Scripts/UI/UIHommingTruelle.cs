//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// The script handle the the UI vesion of the homming truelle for menus
/// </summary>
public class UIHommingTruelle : MonoBehaviour
{
    [SerializeField] private GameEvent onTruelleHitUIEvent;
    [SerializeField] private float force = 250;

    private Vector3 _destination;
    private Rigidbody _rb;
    private bool _isStuckInUI;
    private float _timerBeforeDestroy = 1.5f;

    public void Init(Vector3 targetDestinantion)
    {
        _destination = targetDestinantion;
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles +
                                              new Vector3(
                                                  Random.Range(-50, 50),
                                                  Random.Range(-50, 50),
                                                  Random.Range(-50, 50)
                                              ));
        _rb.AddForce((_destination - transform.position) * force);
    }

    private void FixedUpdate()
    {
        if (_isStuckInUI)
        {
            if(GameManager.UICanvaState != GameManager.UIStateEnum.RebindInputs) return;
            
            _timerBeforeDestroy -= Time.fixedDeltaTime;
            if (!(_timerBeforeDestroy < 0)) return;
            _rb.isKinematic = false;
            _rb.useGravity = true;
            Destroy(gameObject,2);
            return;
        }
        _timerBeforeDestroy -= Time.fixedDeltaTime;
        if (_timerBeforeDestroy < 0) Destroy(gameObject);
    }

    public void TruelleHitButton()
    {
        _isStuckInUI = true;
        _timerBeforeDestroy = 3.5f;
        onTruelleHitUIEvent.Raise(this, null, null, null);
        GetComponent<Collider>().enabled = false;
        _rb.isKinematic = true;
    }
}