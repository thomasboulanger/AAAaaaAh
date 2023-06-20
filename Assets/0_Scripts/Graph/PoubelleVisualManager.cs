using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class PoubelleVisualManager : MonoBehaviour
{
    [Header("Valeurs a set")] [SerializeField]
    private Transform topPoint;

    [SerializeField] private Transform dansPoubelle;
    [SerializeField] private Transform couvercle;
    [SerializeField] private Transform ejectPos;
    [SerializeField] private RectTransform gaucgeRT;
    [SerializeField] private Image gaugeColorSprite;
    [SerializeField] private RectTransform fruitRT;
    [SerializeField] private Color[] gaugeColor = new Color[2];



    [Header("CinematiqueFin")] [SerializeField]
    private Transform midlePoint;

    [SerializeField] private Transform insideMonster;
    [SerializeField] Animator paparingCinematiqueAnimator;
    [SerializeField] BlendShapesAnim monstreFin;
    [SerializeField] Animator playerAnimator;
    [SerializeField] Transform poubelleFinalPos;

    [Header("Valeurs de tweak")] [SerializeField]
    private float fruitSpeed = 2f;

    [SerializeField] private float couvercleSpeed = 2f;
    [SerializeField] private float randomizeTrajectoryPower = 0.2f;
    [SerializeField] private float randomizeTrajectoryPowerDropping = 1f;

    [Header("Courbe d'anims")] [SerializeField]
    private AnimationCurve speedCurve;

    [SerializeField] private AnimationCurve speedCurveDropping;
    [SerializeField] private AnimationCurve destinationCurve;
    [SerializeField] private AnimationCurve couvercleAnim;

    [Header("for debug purpose only")] [SerializeField]
    private float timerPoubelle;

    [SerializeField] private List<Transform> fruits = new();
    [SerializeField] private List<float> fruitTimers = new();
    [SerializeField] private List<Vector3> basePos = new();
    [SerializeField] private List<bool> hasBouped = new();
    [SerializeField] private List<bool> hasBoupedMonster = new();
    [SerializeField] private List<Transform> storedFruits = new();

    [FormerlySerializedAs("midlePosesOffsets")] //dont mind that...
    [SerializeField]
    private List<Vector3> middlePosOffsets = new();

    [Header("for debug purpose only")] [SerializeField]
    private float gaugeSize = 100;

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
    private bool _triggerOnceGrab;

    private float _fruitBaseScale=0.1f;
    private float _fruitScale=1;

    [SerializeField] private Vector2 minMaxGaugepositions = new Vector2(99.9f,-89);
    [SerializeField] private float gaugeSpeed = 8f;

    private Cinemachine.CinemachineVirtualCamera endCam;

    private void Start() => Init();

    public void Init()
    {
        if (fruitRT != null) _fruitBaseScale = fruitRT.localScale.x;
        _openedRotation = couvercle.localRotation;
        couvercle.localEulerAngles = new Vector3(180, couvercle.localEulerAngles.y, couvercle.localEulerAngles.z);
        _closedRotation = couvercle.localRotation;
        _animator = GetComponent<Animator>();

        GameObject camGO = GameObject.Find("/canFin");
        if (camGO != null)
        {
            endCam.TryGetComponent(out endCam);
        }
    }

    void Update()
    {
        //initialization
        List<Transform> toRemoveFromLists = new List<Transform>();
        float deltaTime = Time.deltaTime;

        Vector3 selfPos = dansPoubelle.position;

        Vector3 endPos = _ejectSingleFruit == _ejectFruits
            ? (_ejectSingleFruit ? ejectPos.position : selfPos)
            : insideMonster.position;

        //eject pos faut que ce soit soi poubelle inside soi eject pos soi inside monster si 2 vrai : eject pos, si 2 fau poubelle inside, si dif inside position

        Vector3 middlePos = _ejectSingleFruit == _ejectFruits == false ? midlePoint.position : topPoint.position;

        //Debug.Log(endPos == insideMonster.position);


        currentGaugeLevel -= deltaTime * gaugeDecrementOverTime;
        if (currentGaugeLevel < 0) currentGaugeLevel = 0;

        if (gaucgeRT != null && gaugeColor != null && fruitRT != null)
        {
            gaucgeRT.anchoredPosition = Vector2.Lerp(gaucgeRT.anchoredPosition, new Vector2(gaucgeRT.anchoredPosition.x, Remap(currentGaugeLevel, gaugeSize, 0, minMaxGaugepositions.x, minMaxGaugepositions.y)), deltaTime * gaugeSpeed);
            gaugeColorSprite.color = Color.Lerp(gaugeColor[0], gaugeColor[1], Remap(currentGaugeLevel, gaugeSize, 0, 1, 0));

            _fruitScale = Mathf.Lerp(_fruitScale, _fruitBaseScale, deltaTime * gaugeSpeed/2);
            fruitRT.localScale = new Vector3(_fruitScale, _fruitScale, _fruitScale);
        }
        else Debug.LogWarning("GRO CONNNNN PAS TA PSSIGNE TOUT");// si tu delete la ligne et que t'a des soucis je te ration
        

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

                //adding current fruit for suppression from lists
                toRemoveFromLists.Add(fruits[i]);
                continue;
            }

            Vector3 middlePosOffseted = middlePos + middlePosOffsets[i] *
                (_ejectFruits ? randomizeTrajectoryPowerDropping : randomizeTrajectoryPower);

            fruitTimers[i] += deltaTime * fruitSpeed;

            //fruit movement
            fruits[i].transform.position = Vector3.Lerp(_ejectSingleFruit ? selfPos : basePos[i],
                Vector3.Lerp(middlePosOffseted, _ejectSingleFruit ? ejectPos.position : endPos,
                    destinationCurve.Evaluate(fruitTimers[i])),
                (_ejectFruits ? speedCurveDropping : speedCurve).Evaluate(fruitTimers[i]));

            if ((fruitTimers[i] > 0.8f) && !hasBoupedMonster[i] &&
                GameManager.UICanvaState == GameManager.UIStateEnum.PlayerHaveReachEndOfLevel)
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
            Debug.Log(_ejectFruits + " " + _ejectSingleFruit + " expertiseFruits");
            int index = fruits.IndexOf(item);
            fruits.RemoveAt(index);
            fruitTimers.RemoveAt(index);
            basePos.RemoveAt(index);
            hasBouped.RemoveAt(index);
            middlePosOffsets.RemoveAt(index);
            hasBoupedMonster.RemoveAt(index); //on suprime tt les fruits d�truits cette frame

            //clear our lists of all fruits that have been deleted this frame

            if (_finished && fruits.Count == 0)
            {
                FruitSelector fruit;
                item.TryGetComponent<FruitSelector>(out fruit);
                if (fruit != null)
                {
                    if (_ejectSingleFruit && _ejectFruits)
                    {
                        fruit.transform.position = ejectPos.position;
                    }

                    fruit.animating = false;
                    fruit.ReleaseFruit(true);
                }

                if (_ejectSingleFruit)
                {
                    _ejectSingleFruit = false;
                    storedFruits.RemoveAt(0);
                }
                else storedFruits.Clear();

                _finished = false;
                _ejectFruits = false;

                

                continue;
            }


            if (GameManager.UICanvaState == GameManager.UIStateEnum.PlayerHaveReachEndOfLevel)
                Destroy(item.gameObject); // ond d�tui le fruit pour cine fin
            else item.gameObject.SetActive(false);

            if (_ejectFruits)
            {
                AkSoundEngine.PostEvent("Play_fruit_quit_bag", gameObject);//son ejection de fruit
                GrabFeedback.emotionsInstance.sadPower = 100f;
                continue;
            }

            AkSoundEngine.PostEvent("Play_sfx_ui_put_in_bag_short", gameObject);//son ajout de fruit
            storedFruits.Add(item);
        }
    }

    public void PlayerHitByFly()
    {
        float gaugeLastlevel = currentGaugeLevel;
        currentGaugeLevel += gaugeIncrementByHit;
        if (currentGaugeLevel >= gaugeSize && gaugeLastlevel<gaugeSize*0.6f)
        {
            currentGaugeLevel = gaugeSize * 0.97f;
        }
        if (currentGaugeLevel >= gaugeSize)
        {
            currentGaugeLevel = 0;
            _fruitScale = 3*_fruitBaseScale;
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
        middlePosOffsets.Add(
            new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized);

        if (_ejectFruits && paparingCinematiqueAnimator.gameObject.activeSelf)
        {
            paparingCinematiqueAnimator.SetTrigger("FruitOut");
        }
    }

    //clear the all fruits and reset lists 
    public void EmptyFruitList(Component sender, object unUsed1, object unUsed2, object unUsed3)
    {
        foreach (Transform element in fruits)
            Destroy(element.gameObject);
        
        fruits.Clear();
        fruitTimers.Clear();
        basePos.Clear();
        hasBouped.Clear();
        middlePosOffsets.Clear();
        hasBoupedMonster.Clear();
    }

    public void EjectFruits()
    {
        if (storedFruits.Count == 0) return;

        _ejectFruits = true;
        //Debug.Log(GameManager.UICanvaState);
        //GameManager.UICanvaState = GameManager.UIStateEnum.PlayerHaveReachEndOfLevel; //�tre a la fin
        if (GameManager.UICanvaState != GameManager.UIStateEnum.PlayerHaveReachEndOfLevel)
        {
            _ejectSingleFruit = true;
            Debug.Log("insgleFruit");
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

        playerAnimator.speed = 0;

        monstreFin.CanScream(false);

        EjectFruits();
    }


    public void EjectFruitEndLevelInternalCall()
    {
        
        if (storedFruits.Count > 0 && _storedFruitActualIndex < storedFruits.Count)
        {
            _ejectFruits = true;
            storedFruits[_storedFruitActualIndex].gameObject.SetActive(true);
            storedFruits[_storedFruitActualIndex].transform.position = dansPoubelle.position;

            InitializeFruitThenMoveIt(storedFruits[_storedFruitActualIndex], true);
            _storedFruitActualIndex++;
        }
        else _finished = true;
    }

    public void EjectFruitAtEndLevel(Component sender, object data1, object playerID, object unUsed3)
    {
        if (GameManager.UICanvaState != GameManager.UIStateEnum.PlayerHaveReachEndOfLevel) return;
        bool isPressed = (float) data1 > .9f;
        

        if (isPressed && !_triggerOnceGrab)
        {
            _triggerOnceGrab = true;
            if (storedFruits.Count > 0 && _storedFruitActualIndex < storedFruits.Count)
            {
                _ejectFruits = true;
                storedFruits[_storedFruitActualIndex].gameObject.SetActive(true);
                storedFruits[_storedFruitActualIndex].transform.position = dansPoubelle.position;

                InitializeFruitThenMoveIt(storedFruits[_storedFruitActualIndex], true);
                _storedFruitActualIndex++;
            }
            else _finished = true;
        }
        else _triggerOnceGrab = false;
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

    private float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}

/*
        fruit.animating = false; + appeler ReleaseFruit dans fruit selector
 * fin 
 * 
 * 
 */