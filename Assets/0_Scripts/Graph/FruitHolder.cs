using System.Collections.Generic;
using UnityEngine;

public class FruitHolder : MonoBehaviour
{
    [SerializeField] private List<FruitSelector> _fruitList = new List<FruitSelector>();

    private FruitSelector _fruitSelectorTemp;

    [SerializeField] private float _pullingForce = 5f;
    [SerializeField] private Vector3 _axisAffectedByForce = new Vector3(1, 0, 1);

    [SerializeField] private float _xFroceThreshold = 5f;


    private void FixedUpdate()
    {
        List<FruitSelector> fruitsTeDelete = new List<FruitSelector>();
        foreach (FruitSelector fruit in _fruitList)
        {
            if (fruit == null) return;
            Rigidbody fruitRb = fruit.rb;
            Vector3 force = Vector3.zero;

            if (fruit == null)
            {
                fruitsTeDelete.Add(fruit);
                continue;
            }

            if (fruitRb.position.x < transform.position.x - _xFroceThreshold ||
                 fruitRb.position.x > transform.position.x + _xFroceThreshold)
            {
                force = (transform.position - fruitRb.position) * _pullingForce * Time.deltaTime;
            }

            fruitRb.AddForce(new Vector3(force.x * _axisAffectedByForce.x, force.y * _axisAffectedByForce.y,
                force.z * _axisAffectedByForce.z));
        }

        foreach (FruitSelector fruit in fruitsTeDelete) //eviter de laisser les fruits detruits par cascade de fruit poluer l'array
        {
            _fruitList.Remove(fruit);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GetFruit(other)) return;
        _fruitList.Add(_fruitSelectorTemp);
    }


    private void OnTriggerExit(Collider other)
    {
        if (GetFruit(other)) return;
        _fruitList.Remove(_fruitSelectorTemp);
    }

    bool GetFruit(Collider other)
    {
        _fruitSelectorTemp = null;
        other.transform.parent.TryGetComponent<FruitSelector>(out _fruitSelectorTemp);
        if (_fruitSelectorTemp == null)
        {
            other.TryGetComponent<FruitSelector>(out _fruitSelectorTemp);
        }

        return _fruitSelectorTemp == null;
    }
}