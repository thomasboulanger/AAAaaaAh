using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MoucheAMerde : MonoBehaviour
{
    public Transform body;
    public float speed = 100;
    private Vector3 _path;
    public float radius = 1;
    public Rigidbody bodyRB;
    public float force = 5;
    public Transform[] bouclierList = new Transform[4];
    public float blockRadius = 0.2f;
    public float delay = 3f;
    public float timer = 0f;


    private Vector3 _initialPosition;
    private float _a;
    private float _randomSpeed;
    private int _sign = 1;
    private Vector3 _p;
    private Vector3 _limitVector;
    // Start is called before the first frame update
    void Start()
    {
        _randomSpeed = Random.Range(10f,20f);
        _a = Random.Range(0f,1f);
        _initialPosition = new Vector3(transform.position.x, transform.position.y, -1) ;
    }

    // Update is called once per frame
    void Update()
    {
        _path = body.position - _initialPosition;
        transform.position += _randomSpeed * Vector3.Normalize(_path) * Time.deltaTime;
        _p = body.position + Vector3.Project(transform.position - body.position, _initialPosition - body.position);
        _limitVector = _p + _a * _sign * Vector3.Normalize(new Vector3( -_path.y, _path.x, 0));
        if (Vector3.Magnitude(_p - transform.position) / Vector3.Magnitude(_limitVector) > 1)
        {
            _randomSpeed = Random.Range(10f, 20f);
            _a = Random.Range(0f, 1f);
            _sign *= -1;
        }
        else
        {
            transform.position += _randomSpeed * Vector3.Normalize(_limitVector) * Time.deltaTime;
        }
        if (Vector3.Magnitude(transform.position - body.position) > 20)
        {
            Destroy(transform.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Bouclier")
        {
            for (int i = 0; i < bouclierList.Length; i++)
            {
                if (bouclierList[i] != other.transform && Vector3.Distance(bouclierList[i].position, other.transform.position) < blockRadius)
                {
                    Destroy(gameObject);
                }
            }

        }
        else if (other.transform.tag == "Player")
        {
            Debug.Log("poutprout");
            bodyRB.AddForceAtPosition(force * Vector3.Normalize(_path), transform.position);
            Destroy(gameObject);

        }

    }


}
