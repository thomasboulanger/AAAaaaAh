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
    [HideInInspector] public bool isRecycling;

    [SerializeField] private GameEvent onTruelleHitUIEvent;
    [SerializeField] private float force = 250;

    private Vector3 _destination;
    private Rigidbody _rb;
    private bool _isStuckInUI;
    private float _timerBeforeDestroy = 1.5f;

    public void Init(Vector3 targetDestination) => _destination = targetDestination;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        
        Ray ray = Camera.main.ScreenPointToRay(_destination);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;
            Vector3 hitPosition = hitObject.transform.position;
            
            if(hitObject.CompareTag("UIInteractable"))
                _rb.AddForce((hitPosition - transform.position) * force);
            else _rb.AddForce((_destination - transform.position) * force);
            
        }
        else _rb.AddForce((_destination - transform.position) * force);
    }

    private void FixedUpdate()
    {
        if (_isStuckInUI)
        {
            if (GameManager.UICanvaState == GameManager.UIStateEnum.RebindInputs || isRecycling)
            {
                _timerBeforeDestroy -= Time.fixedDeltaTime;
                if (!(_timerBeforeDestroy < 0)) return;
                _rb.isKinematic = false;
                _rb.useGravity = true;
                Destroy(gameObject, 2);
                return;
            }
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