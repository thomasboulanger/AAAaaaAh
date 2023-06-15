using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Animator))]
public class PoubelleVisualManager : MonoBehaviour
{
    [Header("Valeurs a set")] 
    [SerializeField] private Transform topPoint;
    [SerializeField] private Transform dansPoubelle;
    [SerializeField] private Transform couvercle;
    [SerializeField] private Transform ejectPos;
    [Header("CinematiqueFin")] 
    [SerializeField] private Transform midlePoint;
    [SerializeField] private Transform insideMonster;
    [SerializeField] Animator paparingCinematiqueAnimator;
    [SerializeField] BlendShapesAnim monstreFin;
    [SerializeField] Transform poubelleFinalPos;
    [Header("Valeurs de tweak")] 
    [SerializeField] private float fruitSpeed = 2f;
    [SerializeField] private float couvercleSpeed = 2f;
    [SerializeField] private float randomizeTrajectoryPower = 0.2f;
    [SerializeField] private float randomizeTrajectoryPowerDropping = 1f;
    [Header("Courbe d'anims")]
    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private AnimationCurve speedCurveDropping;
    [SerializeField] private AnimationCurve destinationCurve;
    [SerializeField] private AnimationCurve couvercleAnim;
    [Header("for debug purpose only")]
    [SerializeField] private float timerPoubelle;
    [SerializeField] private List<Transform> fruits = new();
    [SerializeField] private List<float> fruitTimers = new();
    [SerializeField] private List<Vector3> basePos = new();
    [SerializeField] private List<bool> hasBouped = new();
    [SerializeField] private List<bool> hasBoupedMonster = new();
    [SerializeField] private List<Transform> storedFruits = new();
    [FormerlySerializedAs("midlePosesOffsets")] //dont mind that...
    [SerializeField] private List<Vector3> middlePosOffsets = new();

    [Header("for debug purpose only")]
    [SerializeField] private float gaugeSize = 100;
    [SerializeField] private float gaugeIncrementByHit = 50;
    [SerializeField] private float gaugeDecrementOverTime = 10;
    [SerializeField] private float currentGaugeLevel;




    private Animator _animator;
    private Quaternion _openedRotation;
    private Quaternion _closedRotation;
    private Quaternion _lastRotation;
    private bool _ejectFruits;
    private bool _finished;
    private int _storedFruitActualIndex;
    private bool _ejectSingleFruit;

    void Start() => Init();

    private void Init()
    {
        _openedRotation = couvercle.localRotation;
        couvercle.localEulerAngles = new Vector3(180, couvercle.localEulerAngles.y, couvercle.localEulerAngles.z);
        _closedRotation = couvercle.localRotation;
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        //initialization
        List<Transform> toRemoveFromLists = new List<Transform>();
        float deltaTime = Time.deltaTime;
        Vector3 endPos = _ejectFruits ? insideMonster.position : dansPoubelle.position;
        Vector3 middlePos = _ejectFruits ? midlePoint.position : topPoint.position;

        currentGaugeLevel -= deltaTime * gaugeDecrementOverTime;
        if (currentGaugeLevel < 0) currentGaugeLevel = 0;

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
            fruits[i].transform.position = Vector3.Lerp (_ejectFruits ? transform.position:basePos[i],Vector3.Lerp( middlePosOffseted,_ejectSingleFruit ? ejectPos.position : endPos,destinationCurve.Evaluate(fruitTimers[i])), (_ejectFruits ? speedCurveDropping : speedCurve).Evaluate(fruitTimers[i]));

            if((fruitTimers[i] > 0.4f) && !hasBoupedMonster[i])
            {
                monstreFin.OpenMouth();
                hasBoupedMonster[i] = true;
            }

            if (!(fruitTimers[i] > (!_ejectFruits ? 0.8f : 0.2f)) || hasBouped[i]) continue;

            //fruit in/out of the garbage can lid -> animation
            hasBouped[i] = true;
            _animator.SetTrigger("boup");
        }

        foreach (Transform item in toRemoveFromLists)
        {
            int index = fruits.IndexOf(item);
            fruits.RemoveAt(index); fruitTimers.RemoveAt(index); basePos.RemoveAt(index); hasBouped.RemoveAt(index); middlePosOffsets.RemoveAt(index);hasBoupedMonster.RemoveAt(index); //on suprime tt les fruits détruits cette frame

            //clear our lists of all fruits that have been deleted this frame

            if (_finished && fruits.Count == 0)
            {
                _finished = false;
                _ejectFruits = false;
                if (_ejectSingleFruit)
                {
                    _ejectSingleFruit = false;
                    storedFruits.RemoveAt(0);
                }
                else storedFruits.Clear();

                FruitSelector fruit;
                item.TryGetComponent<FruitSelector>(out fruit);
                if (fruit != null) fruit.ReleaseFruit();

                continue;
            }

            if (_ejectFruits) continue;
            storedFruits.Add(item);
        }
    }

    public void PlayerHitByFly()
    {
        currentGaugeLevel += gaugeIncrementByHit;
        if (currentGaugeLevel >= gaugeSize)
        {
            currentGaugeLevel = 0;
            EjectFruits();
        }
    }
    
    //deactivate the fruit before sending it here
    public void InitializeFruitThenMoveIt(Transform fruitTransform, bool isFunctionCalledInIntern)
    {
        fruits.Add(fruitTransform);
        fruitTimers.Add(0);
        basePos.Add(fruitTransform.position);

        //fruit added to lists
        hasBouped.Add(false);
        hasBoupedMonster.Add(false);

        //get a random offset
        middlePosOffsets.Add (new Vector3(Random.Range(-1f, 1f),Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized);

        if (_ejectFruits && paparingCinematiqueAnimator.gameObject.activeSelf)
        {

            paparingCinematiqueAnimator.SetTrigger("FruitOut");
        }
    }

    public void EjectFruits()
    {
        if (storedFruits.Count == 0) return;

        _ejectFruits = true;
        GameManager.UICanvaState = GameManager.UIStateEnum.PlayerHaveReachEndOfLevel; //être a la fin
        if (GameManager.UICanvaState != GameManager.UIStateEnum.PlayerHaveReachEndOfLevel)
        {
            _ejectSingleFruit = true;
            storedFruits[0].gameObject.SetActive(true);
            InitializeFruitThenMoveIt(storedFruits[0], true);
            _finished = true;
        }
    }

    public void PrepareCinematic()
    {
        transform.position = poubelleFinalPos.position;
        transform.rotation = poubelleFinalPos.rotation;

        paparingCinematiqueAnimator.gameObject.SetActive(true);
        _storedFruitActualIndex = 0;

        EjectFruits();
    }

    public void EjectFruitAtEndLevel()//fonction call par event manette J
    {
        if (GameManager.UICanvaState != GameManager.UIStateEnum.PlayerHaveReachEndOfLevel) return;

        if (storedFruits.Count > 0 && _storedFruitActualIndex<storedFruits.Count)
        {
            storedFruits[_storedFruitActualIndex].gameObject.SetActive(true);
            InitializeFruitThenMoveIt(storedFruits[_storedFruitActualIndex], true);
            _storedFruitActualIndex++;
        }
        else _finished = true;
        //CALL EVENT FIN DE PARTIE------------------------------------------------------------------------------------------------------------------
    }

    IEnumerator RandomDelayedFruits()
    {
        const float timer = 1f;
        for (int i = 0; i < storedFruits.Count; i++)
        {
            storedFruits[i].gameObject.SetActive(true);
            InitializeFruitThenMoveIt(storedFruits[i], true);
            if (i == storedFruits.Count - 1) continue;
            yield return new WaitForSeconds(timer / Mathf.Clamp(i / 2, 1, 999999));
        }

        _finished = true;
    }
}

/*
        fruit.animating = false; + appeler ReleaseFruit dans fruit selector
 * fin 
 * 
 * 
 */