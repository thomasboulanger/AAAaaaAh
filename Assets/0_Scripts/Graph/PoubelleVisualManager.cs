using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Animator))]
public class PoubelleVisualManager : MonoBehaviour
{
    [Header("Valeurs a set")] [SerializeField]
    private Transform topPoint;

    [SerializeField] private Transform dansPoubelle;
    [SerializeField] private Transform couvercle;

    [Header("CinematiqueFin")] [SerializeField]
    private Transform midlePoint;

    [SerializeField] private Transform insideMonster;

    [Header("Valeurs de tweak")] [SerializeField]
    private float fruitSpeed = 2f;

    [SerializeField] private float couvercleSpeed = 2f;
    [SerializeField] private float randomizeTrajectoryPower = 0.2f;
    [SerializeField] private float randomizeTrajectoryPowerDropping = 1f;

    [Header("Courbe d'anims")] [SerializeField]
    private AnimationCurve speedCurve;

    [SerializeField] private AnimationCurve speedCurveDropping;
    [SerializeField] private AnimationCurve bloupCurve;
    [SerializeField] private AnimationCurve offsetCurve; //avec top point
    [SerializeField] private AnimationCurve destinationCurve;
    [SerializeField] private AnimationCurve couvercleAnim;

    [Header("for debug purpose only")] [SerializeField]
    private float timerPoubelle;

    [SerializeField] private List<Transform> fruits = new();
    [SerializeField] private List<float> fruitTimers = new();
    [SerializeField] private List<Vector3> basePos = new();
    [SerializeField] private List<bool> hasBouped = new();
    [SerializeField] private List<Transform> storedFruits = new();

    [FormerlySerializedAs("midlePosesOffsets")] //dont mind that...
    [SerializeField]
    private List<Vector3> middlePosOffsets = new();

    private Animator _animator;
    private Quaternion _openedRotation;
    private Quaternion _closedRotation;
    private Quaternion _lastRotation;
    private bool _ejectFruits;
    private bool _finished;
    private int index;

    void Start() => Init();

    private void Init()
    {
        _openedRotation = couvercle.localRotation;
        couvercle.localEulerAngles = new Vector3(180, couvercle.localEulerAngles.y, couvercle.localEulerAngles.z);
        _closedRotation = Quaternion.Euler(180, couvercle.localEulerAngles.y, couvercle.localEulerAngles.z);
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        //initialization
        List<Transform> toRemoveFromLists = new List<Transform>();
        float deltaTime = Time.deltaTime;
        Vector3 endPos = _ejectFruits ? insideMonster.position : dansPoubelle.position;
        Vector3 middlePos = _ejectFruits ? midlePoint.position : topPoint.position;

        //open the garbage can lid
        if (fruits.Count > 0 || _ejectFruits)
            couvercle.localRotation =
                Quaternion.Lerp(couvercle.localRotation, _openedRotation, deltaTime * couvercleSpeed);
        else
        {
            //close the garbage can lid with timer and animation curve
            if (timerPoubelle < 1f)
            {
                timerPoubelle += deltaTime * couvercleSpeed;
                couvercle.localRotation = Quaternion.LerpUnclamped(_lastRotation,
                    fruits.Count == 0 ? _closedRotation : _openedRotation, couvercleAnim.Evaluate(timerPoubelle));
            }
        }

        for (int i = 0; i < fruits.Count; i++)
        {
            if (fruitTimers[i] >= 1f)
            {
                //setup to close garbage can lid
                _lastRotation = couvercle.localRotation;
                timerPoubelle = 0;

                //deactivate current fruit
                fruits[i].gameObject.SetActive(false);

                //adding current fruit for suppression from lists
                toRemoveFromLists.Add(fruits[i]);
                continue;
            }

            Vector3 middlePosOffseted = middlePos + middlePosOffsets[i] *
                (_ejectFruits ? randomizeTrajectoryPowerDropping : randomizeTrajectoryPower);

            fruitTimers[i] += deltaTime * fruitSpeed;

            //fruit movement
            fruits[i].transform.position = Vector3.Lerp
            (
                basePos[i],
                Vector3.Lerp
                (
                    middlePosOffseted,
                    endPos,
                    destinationCurve.Evaluate(fruitTimers[i])
                ),
                (_ejectFruits ? speedCurveDropping : speedCurve).Evaluate(fruitTimers[i])
            );

            if (!(fruitTimers[i] > (!_ejectFruits ? 0.8f : 0.2f)) || hasBouped[i]) continue;

            //fruit in/out of the garbage can lid -> animation
            hasBouped[i] = true;
            _animator.SetTrigger("boup");
        }

        for (int i = 0; i < toRemoveFromLists.Count; i++)
        {
            //clear our lists of all fruits that have been deleted this frame
            fruits.RemoveAt(i);
            fruitTimers.RemoveAt(i);
            basePos.RemoveAt(i);
            hasBouped.RemoveAt(i);
            middlePosOffsets.RemoveAt(i);

            if (_finished && fruits.Count == 0)
            {
                _finished = false;
                _ejectFruits = false;
                storedFruits.Clear();
                continue;
            }

            if (_ejectFruits) continue;
            storedFruits.Add(toRemoveFromLists[i]);
        }
    }

    //deactivate the fruit before sending it here
    public void InitializeMovement(Transform fruit)
    {
        fruits.Add(fruit);
        fruitTimers.Add(0);
        basePos.Add(fruit.position);

        //fruit added to lists
        hasBouped.Add(false);

        //get a random offset
        middlePosOffsets.Add
        (
            new Vector3
            (
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)
            ).normalized
        );
    }

    public void EjectFruits()
    {
        if (storedFruits?.Any() != true) return;
        _ejectFruits = true;
        StartCoroutine(RandomDelayedFruits());
    }

    IEnumerator RandomDelayedFruits()
    {
        const float timer = 1f;
        for (int i = 0; i < storedFruits.Count; i++)
        {
            storedFruits[i].gameObject.SetActive(true);
            InitializeMovement(storedFruits[i]);
            if (i == storedFruits.Count - 1) continue;
            yield return new WaitForSeconds(timer / Mathf.Clamp(i / 2, 1, 999999));
        }

        _finished = true;
    }
}