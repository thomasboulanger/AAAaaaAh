using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Transform body;
    public float speed = 10;
    private Vector3 _path;
    public float radius = 3;
    private float _randomAngle;
    private float _randomOffset;
    public Rigidbody bodyRB;
    public float force = 5;
    // Start is called before the first frame update
    void Start()
    {
        _randomAngle = Random.Range(0f, 359f);
        _randomOffset = Random.Range(0f, radius);
        _path = body.position - transform.position + new Vector3(_randomOffset * Mathf.Cos(_randomAngle), _randomOffset * Mathf.Sin(_randomAngle),0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += _path / 100 * Random.Range(speed/2,speed) * Time.deltaTime;

        if (Vector3.Magnitude(transform.position - body.position) > 20)
        {
            Destroy(transform.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Bouclier")
        {
            Destroy(transform.gameObject);
        }
        else if (other.transform.tag == "Player")
        {
            Debug.Log("poutprout");
            bodyRB.AddForceAtPosition(force * Vector3.Normalize(_path), transform.position);
            Destroy(transform.gameObject);

        }

    }
}
