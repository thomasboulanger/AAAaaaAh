using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpPosRot : MonoBehaviour
{
    public float speed = 10f;
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * speed);
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * speed);
    }
}
