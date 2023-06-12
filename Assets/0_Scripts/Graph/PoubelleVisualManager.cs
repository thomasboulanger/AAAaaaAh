using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class PoubelleVisualManager : MonoBehaviour
{
    [SerializeField] private Transform topPoint;
    [SerializeField] private Transform dansPoubelle;
    [SerializeField] private Transform couvercle;
    [SerializeField] private Transform corp;
    [SerializeField] private List<Transform> fruits = new List<Transform>();
    [SerializeField] private List<float> fruitTimers = new List<float>();
    [SerializeField] private List<Vector3> basePos = new List<Vector3>();
    [SerializeField] private List<bool> hasBouped = new List<bool>();

    [SerializeField] private float speed = 2f;
    [SerializeField] private float waitTimePoubelle = 2f;
    [SerializeField] private float couvercleSpeed = 2f;

    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private AnimationCurve bloupCurve;
    [SerializeField] private AnimationCurve offsetCurve;//avec top point
    [SerializeField] private AnimationCurve destinationCurve;
    [SerializeField] private AnimationCurve couvercleAnim;


    private Animator _animator;

    public float timerPoubelle;

    private Quaternion _openedRotation;
    private Quaternion _closedRotation;

    // Start is called before the first frame update
    void Start()
    {
        _openedRotation = couvercle.localRotation;
        couvercle.localEulerAngles = new Vector3(180, couvercle.localEulerAngles.y, couvercle.localEulerAngles.z);
        _closedRotation = couvercle.localRotation;
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        List<Transform> toRemove = new List<Transform>();
        float dt = Time.deltaTime;

        Vector3 dansPoubelleStored = dansPoubelle.position;
        Vector3 topPointStored = topPoint.position;

        bool canClose = timerPoubelle > waitTimePoubelle;


        if (fruits.Count == 0)
        {
            if (!canClose)
            {
                timerPoubelle += dt;
            }
        }

        Quaternion rotaBenne = Quaternion.Lerp(couvercle.localRotation, canClose ? _closedRotation : _openedRotation, dt * couvercleSpeed);
        couvercle.localRotation = rotaBenne;

        for (int i = 0; i < fruits.Count; i++)
        {
            if (fruitTimers[i] >= 1f)
            {
                //--------------------------------------------------------
                Destroy(fruits[i].gameObject);
                //--------------------------------------------------------
                toRemove.Add(fruits[i]);
                continue;
            }

            fruitTimers[i] += dt * speed;


            fruits[i].transform.position = Vector3.Lerp(basePos[i], Vector3.Lerp(topPointStored, dansPoubelleStored, destinationCurve.Evaluate(fruitTimers[i])), speedCurve.Evaluate(fruitTimers[i]));
            if (fruitTimers[i]> 0.8f && !hasBouped[i])
            {
                hasBouped[i] = true;
                _animator.SetTrigger("boup");
            }
        }
        
        foreach (Transform item in toRemove)
        {
            int index = fruits.IndexOf(item);

            fruits.RemoveAt(index); fruitTimers.RemoveAt(index); basePos.RemoveAt(index); hasBouped.RemoveAt(index);
        }
    }

    public void InitializeMovement(Transform fruit)// il faut désactiver le fruits vanat de l'evoyer ici
    {
        fruits.Add(fruit);fruitTimers.Add(0);basePos.Add(fruit.position);hasBouped.Add(false);
        timerPoubelle = 0;
    }
}
