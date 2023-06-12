using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingPoubelle : MonoBehaviour
{
    public GameObject test;
    public PoubelleVisualManager poubelleRef;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.B))
        {
            GameObject testInstance = Instantiate(test, transform.position, transform.rotation);

            poubelleRef.InitializeMovement(testInstance.transform);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            poubelleRef.EjectFruits();
        }
    }
}
