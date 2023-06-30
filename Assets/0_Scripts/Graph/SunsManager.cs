using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class SunsManager : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private Vector3 direction = Vector3.forward;

    [SerializeField] private Light sunForest;

    [SerializeField]private float sunForestIntensity = 130000f;

    [SerializeField] private AnimationCurve sunTransitionCurve;
    [SerializeField] private AnimationCurve sunTransitionOutCurve;
    [SerializeField] private float sunTransitionSpeed = 2f;


    private bool currentStatus;
    private bool transition;

    private float t;

    //Doto tache animation lampe
    void Update()
    {
        transform.position += direction.normalized * speed * Time.deltaTime;
        if (transition && t <= 1)
        {
            AnimationCurve forestCurve = currentStatus ? sunTransitionCurve : sunTransitionOutCurve;
            t += Time.deltaTime * sunTransitionSpeed;

            float forestVal = forestCurve.Evaluate(t);

            sunForest.intensity = forestVal * sunForestIntensity;

            //Debug.Log("intensity "+ sunForest.intensity +" "+forestVal);
        }
        else if (transition)
        {
            t = 0;
            transition = false;
        }
    }

    public void SwitchStatus(Component sender, object status, object data2, object data3)
    {
        if (status is not bool) return;

        if (currentStatus != (bool)status)
        {
            currentStatus = (bool)status;
            transition = true;
            t = 0;
        }
    }
}
