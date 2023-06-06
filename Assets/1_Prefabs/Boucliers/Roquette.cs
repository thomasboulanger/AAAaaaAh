using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roquette : MonoBehaviour
{
    public Transform body;
    public float speed = 100;
    private Vector3 _path;
    public float radius = 1;
    private float _randomAngle;
    private float _randomOffset;
    public Rigidbody bodyRB;
    public float force = 5;
    public Transform[] bouclierList = new Transform[4];
    public float blockRadius = 0.2f;
    public float delay = 3f;
    public float timer = 0f;
    private bool _pathSet = false;
    public GameObject cibleRoquette;
    private GameObject _cible;
    private Vector3 _initialPosition;
    private Vector3 _initialBodyPosition;
    // Start is called before the first frame update
    void Start()
    {
        _randomAngle = Random.Range(0f, 359f);
        _randomOffset = Random.Range(0f, radius);
        _path = body.position - transform.position + _randomOffset * new Vector3(Mathf.Cos(_randomAngle), Mathf.Sin(_randomAngle), 0);
        _cible = Instantiate(cibleRoquette, transform.position, Quaternion.identity);
        _cible.transform.LookAt(body);
        _initialBodyPosition = body.position;
        _initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > delay)
        {
            _pathSet = true;
            Destroy(_cible);

        }
        else
        {
            _cible.transform.LookAt(body);
            _cible.transform.position = _initialPosition + body.position - _initialBodyPosition;
            transform.position = _initialPosition + body.position - _initialBodyPosition;
        }


        if (_pathSet == true)
        {
            transform.position += _path / 100 * Random.Range(speed / 2, speed) * Time.deltaTime;

            if (Vector3.Magnitude(transform.position - body.position) > 20)
            {
                Destroy(gameObject);
            }
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
