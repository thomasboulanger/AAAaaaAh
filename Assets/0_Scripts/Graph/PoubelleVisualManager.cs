using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class PoubelleVisualManager : MonoBehaviour
{
    [Header("Valeurs a set")]
    [SerializeField] private Transform topPoint;
    [SerializeField] private Transform dansPoubelle;
    [SerializeField] private Transform couvercle;
    [SerializeField] private Transform corp;

    [Header("CinematiqueFin")]
    [SerializeField] private Transform midlePoint;
    [SerializeField] private Transform insideMonster;

    [Header("Valeurs de tweak")]

    [SerializeField] private float fruitSpeed = 2f;
    [SerializeField] private float couvercleSpeed = 2f;
    [SerializeField] private float randomizeTrajectoryPower = 0.2f;
    [SerializeField] private float randomizeTrajectoryPowerDropping = 1f;

    [Header("Courbe d'anims")]

    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private AnimationCurve speedCurveDropping;
    [SerializeField] private AnimationCurve bloupCurve;
    [SerializeField] private AnimationCurve offsetCurve;//avec top point
    [SerializeField] private AnimationCurve destinationCurve;
    [SerializeField] private AnimationCurve couvercleAnim;

    [Header("purement debug")]

    [SerializeField] private float timerPoubelle;
    [SerializeField] private List<Transform> fruits = new List<Transform>();
    [SerializeField] private List<float> fruitTimers = new List<float>();
    [SerializeField] private List<Vector3> basePos = new List<Vector3>();
    [SerializeField] private List<bool> hasBouped = new List<bool>();
    [SerializeField] private List<Transform> storedFruits = new List<Transform>();
    [SerializeField] private List<Vector3> midlePosesOffsets = new List<Vector3>();

    private Animator _animator;
    private Quaternion _openedRotation;
    private Quaternion _closedRotation;
    private Quaternion _lastRotation;
    private bool _ejectFruits;
    private bool _finished;
    private int ejectedFruits;
    private bool _ejectSingleFruit;
    private Vector3 fruitPos;

    void Start()
    {
        _openedRotation = couvercle.localRotation;
        couvercle.localEulerAngles = new Vector3(180, couvercle.localEulerAngles.y, couvercle.localEulerAngles.z);
        _closedRotation = couvercle.localRotation;
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        //init des valeurs utilisées
        List<Transform> teRemoveFromLists = new List<Transform>();
        float dt = Time.deltaTime;

        Vector3 endPos;
        Vector3 midlePos;

        if (_ejectFruits && !_ejectSingleFruit)
        {
            endPos = insideMonster.position;
            midlePos = midlePoint.position;
        }
        else
        {
            endPos = dansPoubelle.position;
            midlePos = topPoint.position;
        }

        if (fruits.Count > 0 || _ejectFruits)
        {
            couvercle.localRotation = Quaternion.Lerp(couvercle.localRotation, _openedRotation, dt * couvercleSpeed);//ouvrir le couvercle
        }
        else
        {
            if (timerPoubelle < 1f)
            {
                timerPoubelle += dt * couvercleSpeed;
                couvercle.localRotation = Quaternion.LerpUnclamped(_lastRotation, fruits.Count == 0 ? _closedRotation : _openedRotation, couvercleAnim.Evaluate(timerPoubelle)); //fermer le couvercle avec timer et courbe
            }
        }

        for (int i = 0; i < fruits.Count; i++)
        {
            if (fruitTimers[i] >= 1f)
            {
                _lastRotation = couvercle.localRotation;//setup fermeture couvercle
                timerPoubelle = 0;
                //--------------------------------------------------------
                if (!_ejectSingleFruit)
                {
                    fruits[i].gameObject.SetActive(false); // on laisse le fruit activé si il a été drop----------------------------------
                }

                //Destroy(fruits[i].gameObject);
                //--------------------------------------------------------
                teRemoveFromLists.Add(fruits[i]);// on a joute le fruit pour la supression des listes
                continue;
            }

            Vector3 midlePosOffseted = midlePos + midlePosesOffsets[i] * (_ejectFruits ? randomizeTrajectoryPowerDropping : randomizeTrajectoryPower);

            fruitTimers[i] += dt * fruitSpeed;
            fruits[i].transform.position = Vector3.Lerp(basePos[i], Vector3.Lerp(midlePosOffseted, _ejectSingleFruit ? fruitPos: endPos, destinationCurve.Evaluate(fruitTimers[i])), (_ejectFruits? speedCurveDropping : speedCurve).Evaluate(fruitTimers[i]));//deplacement fruit

            if (fruitTimers[i] > (!_ejectFruits ? 0.8f : 0.2f) && !hasBouped[i])
            {
                hasBouped[i] = true;
                _animator.SetTrigger("boup");//fruit rentre/sort de la poubelle : animation
            }
        }

        foreach (Transform item in teRemoveFromLists)
        {
            int index = fruits.IndexOf(item);
            fruits.RemoveAt(index); fruitTimers.RemoveAt(index); basePos.RemoveAt(index); hasBouped.RemoveAt(index); midlePosesOffsets.RemoveAt(index); //on suprime tt les fruits détruits cette frame

            if (_finished && fruits.Count == 0)
            {
                _finished = false;
                _ejectFruits = false;

                if (_ejectSingleFruit)
                {
                    _ejectSingleFruit = false;
                    storedFruits.RemoveAt(0);
                }
                else
                {
                    storedFruits.Clear();
                }
                continue;
            }
            if (_ejectFruits) continue;
            storedFruits.Add(item);//on l'ajoute au fruits sauves
        }
    }

    public void InitializeMovement(Transform fruit, bool ejecting)// il faut désactiver le fruits avant de l'evoyer ici
    {
        fruits.Add(fruit); fruitTimers.Add(0); basePos.Add(fruit.position); hasBouped.Add(false); //ajout du fruit dans les listes
        midlePosesOffsets.Add(RandomizedVector());//offset randoms

        if (!ejecting) fruitPos = fruit.position;
    }

    public void EjectFruits(bool allfruits)
    {
        if (storedFruits.Count == 0) return;
        _ejectFruits = true;
        if (allfruits)
        {
            StartCoroutine(RandomDelayedFruits());
        }
        else
        {
            _ejectSingleFruit = true;
            storedFruits[0].gameObject.SetActive(true);
            InitializeMovement(storedFruits[0], true);
            _finished = true;
        }

    }

    IEnumerator RandomDelayedFruits()
    {
        float timer = 1f;
        for (ejectedFruits = 0; ejectedFruits < storedFruits.Count; ejectedFruits++)
        {
            storedFruits[ejectedFruits].gameObject.SetActive(true);
            InitializeMovement(storedFruits[ejectedFruits], true);
            if (ejectedFruits == storedFruits.Count - 1) continue;
            yield return new WaitForSeconds(timer / Mathf.Clamp(ejectedFruits / 2, 1, 999999));
        }

        _finished = true;
    }

    Vector3 RandomizedVector()
    {
        return (new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized);
    }
}
