using UnityEngine;

public class AddForceRBGraph : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _force = 100f;
    [SerializeField] private float _maxDistance = 3f;

    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _rb.AddForce((_target.position - _rb.position) * _force);

        if (Vector3.Distance(_target.position, _rb.position) > _maxDistance) //eviter le cringe
        {
            _rb.position = _target.position;
        }
    }
}