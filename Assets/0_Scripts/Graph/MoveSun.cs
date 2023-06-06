using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSun : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private Vector3 direction = Vector3.forward;

    private Light _sunLight;

    // Update is called once per frame
    private void Start()
    {
        _sunLight = GetComponent<Light>();
    }
    //Doto tache animation lampe
    void Update()
    {
        transform.position += direction.normalized * speed * Time.deltaTime;
    }

    public void DayNight(bool night)
    {
        Debug.Log(night);
    }
}
