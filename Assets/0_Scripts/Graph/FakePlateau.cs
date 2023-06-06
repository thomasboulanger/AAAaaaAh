using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakePlateau : MonoBehaviour
{
    public Transform pos1;
    public Transform pos2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = pos1.position - pos2.position;

        if (pos1.position.x > pos2.position.x) pos = -pos;

        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, Vector3.SignedAngle(Vector3.right, pos, Vector3.forward)));
        transform.position = (pos1.position + pos2.position) / 2;
    }
}
