using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoutteParticule : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer mr;

    [SerializeField] private int gouttesNumber = 1;
    [SerializeField] private float dropSpeed = 1f;
    [SerializeField] private float randomPower = 0.2f;
    [SerializeField] private float randomQuaternionPower = 50f;
    [SerializeField] private AnimationCurve dropSpeedCurve;
    [SerializeField] private AnimationCurve fallingSKCurve;
    [SerializeField] private AnimationCurve toutchCruve;
    [SerializeField] private AnimationCurve yScaleCurve;
    [SerializeField] private AnimationCurve xzScaleCurve;
    [SerializeField] private AnimationCurve shapeKeyAnimationCruve;
    [SerializeField] private Vector2 minMaxWait = new Vector2(0,1);

    Vector3 baseScale = new Vector3(100f,100f,100f);

    private List<SkinnedMeshRenderer> gouttes = new List<SkinnedMeshRenderer>();
    private List<Vector3> targets = new List<Vector3>();
    private List<Vector3> dropPoses = new List<Vector3>();
    private List<int> shapeIndexes = new List<int>();
    private List<float> temp = new List<float>();
    private List<float> tempToWait = new List<float>();
    private List<Quaternion> baseRot = new List<Quaternion>();
    private List<Quaternion> targetRot = new List<Quaternion>();
    private List<float> distance = new List<float>();




    // Start is called before the first frame update
    void Start()
    {
        Vector3 baseDropPos = transform.position;

        for (int i = 0; i < gouttesNumber; i++)
        {
            Quaternion randomRot = RandomQuaternion();
            baseRot.Add(randomRot);
            targetRot.Add(Quaternion.Euler(new Vector3(-90,0,Random.Range(0f,360f))));
            SkinnedMeshRenderer tempMr = Instantiate(mr, baseDropPos, randomRot);
            gouttes.Add(tempMr);
            temp.Add(Random.Range(0f, 1f));
            SelectNewTarget(i, true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.deltaTime;
        Vector3 pos = transform.position;

        for (int i = 0; i < gouttesNumber; i++)
        {

            if (temp[i] <= 1)
            {
                if (temp[i] < 0.1f)
                {
                    dropPoses[i] = pos;
                }

                gouttes[i].transform.position = Vector3.LerpUnclamped(dropPoses[i], targets[i], dropSpeedCurve.Evaluate(temp[i]));
                gouttes[i].SetBlendShapeWeight(shapeIndexes[i], Mathf.LerpUnclamped(0, 100, shapeKeyAnimationCruve.Evaluate(temp[i])));
                gouttes[i].SetBlendShapeWeight(0, Mathf.LerpUnclamped(0, 100, fallingSKCurve.Evaluate(temp[i])));
                gouttes[i].SetBlendShapeWeight(1, Mathf.LerpUnclamped(0, 100, toutchCruve.Evaluate(temp[i])));
                gouttes[i].transform.localScale = new Vector3(Mathf.LerpUnclamped(baseScale.x, 0, xzScaleCurve.Evaluate(temp[i])), Mathf.LerpUnclamped(baseScale.y, 0, xzScaleCurve.Evaluate(temp[i])), Mathf.LerpUnclamped(baseScale.z, 0, yScaleCurve.Evaluate(temp[i])));

                gouttes[i].transform.rotation = Quaternion.Lerp(baseRot[i], targetRot[i], Mathf.Clamp(temp[i] * 2.8f,0,1));
                /*
                if (temp[i]==0)
                {
                    gouttes[i].transform.LookAt(targets[i], Vector3.up);
                    gouttes[i].transform.localRotation = Quaternion.Euler(new Vector3(gouttes[i].transform.localRotation.eulerAngles.x+180, gouttes[i].transform.localRotation.eulerAngles.y, gouttes[i].transform.localRotation.eulerAngles.z));
                }*/

                temp[i] += (deltaTime * dropSpeed)/distance[i];
            }
            else if (tempToWait[i]<=0)
            {
                SelectNewTarget(i, false);
                temp[i] = 0;
            }
            else
            {
                tempToWait[i] -= deltaTime;
            }
        }

    }

    void SelectNewTarget(int index, bool add)
    {
        RaycastHit hit;
        Vector3 dropPos = transform.position;

        if (Physics.Raycast(dropPos, Vector3.down + RandomVector3(), out hit))
        {
            if (add)
            {
                targets.Add(hit.point);
                dropPoses.Add(dropPos);
                shapeIndexes.Add(Random.Range(2, 5));
                tempToWait.Add(Random.Range(minMaxWait.x, minMaxWait.y));

                distance.Add(Vector3.Distance(dropPos, hit.point));
            }
            else
            {
                dropPoses[index] = dropPos;
                targets[index] = hit.point;
                gouttes[index].SetBlendShapeWeight(shapeIndexes[index], 0);
                shapeIndexes[index] = Random.Range(2, 5);
                tempToWait[index] = (Random.Range(minMaxWait.x, minMaxWait.y));

                baseRot[index] = RandomQuaternion();
                targetRot[index] = Quaternion.Euler(new Vector3(-90, 0, Random.Range(0f, 360f)));

                distance[index] = Vector3.Distance(dropPos, hit.point);
            }
        }
    }

    Vector3 RandomVector3()
    {
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * randomPower;
    }

    Quaternion RandomQuaternion()
    {
        return Quaternion.Euler(transform.rotation.eulerAngles + RandomVector3() * randomQuaternionPower);
    }

    
}
