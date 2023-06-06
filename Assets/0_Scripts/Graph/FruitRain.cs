using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitRain : MonoBehaviour
{
    [SerializeField] private GameObject _fruitPrefab;
    [SerializeField] private Vector3 _spawnSquare;
    private Vector3 _otherPos;
    [SerializeField] private float _time = 1f;
    [SerializeField] private float _timeToDestroy = 10f;
    private float _t;

    [SerializeField] private float _debugSpereRadius = 0.2f;

    private void Start()
    {
        _otherPos = _spawnSquare + transform.position;
    }

    private void Update()
    {
        if (_fruitPrefab == null) return;

        _otherPos = _spawnSquare + transform.position;//si on bouge le vector en runtime

        _t += Time.deltaTime;
        if (_t > _time)
        {
            _t = 0;
            SpawnFruit();
        }
    }

    private void SpawnFruit()
    {
        GameObject tempFruit = Instantiate(_fruitPrefab);
        tempFruit.transform.position = new Vector3(Random.Range(transform.position.x, _otherPos.x), Random.Range(transform.position.y, _otherPos.y), Random.Range(transform.position.z, _otherPos.z));
        tempFruit.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360)));
        Destroy(tempFruit, _timeToDestroy);
    }


    private void OnDrawGizmos()
    {
        Vector3 spawnPos = _spawnSquare + transform.position;
        Gizmos.DrawSphere(spawnPos, _debugSpereRadius);
        Gizmos.DrawLine(transform.position, spawnPos);
        Gizmos.DrawLine(transform.position, new Vector3(_spawnSquare.x, 0, 0) + transform.position);
        Gizmos.DrawLine(transform.position, new Vector3(0, 0, _spawnSquare.z) + transform.position);
        Gizmos.DrawLine(spawnPos, new Vector3(_spawnSquare.x, 0, 0) + transform.position);
        Gizmos.DrawLine(spawnPos, new Vector3(0, 0, _spawnSquare.z) + transform.position);

        Gizmos.DrawLine(transform.position, new Vector3(0, _spawnSquare.y, 0) + transform.position);
        Gizmos.DrawLine(spawnPos, new Vector3(0, _spawnSquare.y, 0) + transform.position);

        

    }
}
