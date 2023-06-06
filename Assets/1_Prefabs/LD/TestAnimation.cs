using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class TestAnimation : MonoBehaviour
{
    [SerializeField] private AnimationCurve _curve;
    [SerializeField] private float _speed = 1;
    [SerializeField] private float _offset = 0;
    [SerializeField] private Vector3 _amplitude;
    private float _timer;
    private Vector3 _initialPosition;
    private Vector3 _position1;
    private Vector3 _position2;
    private bool _sens;
    // Start is called before the first frame update
    void Start()
    {
        _initialPosition = transform.position;
        _position1 = transform.position + _amplitude;
        _position2 = transform.position - _amplitude;
        _timer = _offset;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!_sens) 
        {
            _timer += Time.deltaTime * _speed;
        }
        else
        {
            _timer -= Time.deltaTime * _speed;
        }

        transform.position = Vector3.LerpUnclamped(_position2, _position1, _curve.Evaluate(_timer));
    
        if (_timer > 1 || _timer < 0)
        {
            _sens =  !_sens;
            if (_sens)
            {
                _timer = 1;
                
            }
            else
            {
                _timer = 0;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + _amplitude + Vector3.Project(transform.localScale, _amplitude)/2, 0.2f);
        Gizmos.DrawWireSphere(transform.position - _amplitude - Vector3.Project(transform.localScale, _amplitude)/2, 0.2f);
    }
}
