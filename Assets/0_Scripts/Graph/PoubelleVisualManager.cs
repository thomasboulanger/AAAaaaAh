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

    [Header("Valeurs de tweak")]

    [SerializeField] private float fruitSpeed = 2f;
    [SerializeField] private float couvercleSpeed = 2f;

    [Header("Courbe d'anims")]

    [SerializeField] private AnimationCurve speedCurve;
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

    private Animator _animator;
    private Quaternion _openedRotation;
    private Quaternion _closedRotation;
    private Quaternion lastRotation;

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
        //init des valeurs utilisées
        List<Transform> teRemoveFromLists = new List<Transform>();
        float dt = Time.deltaTime;

        Vector3 dansPoubelleStored = dansPoubelle.position;
        Vector3 topPointStored = topPoint.position;

        if (fruits.Count > 0)
        {
            couvercle.localRotation = Quaternion.Lerp(couvercle.localRotation, _openedRotation, dt * couvercleSpeed);//ouvrir le couvercle
        }
        else
        {
            if (timerPoubelle < 1f)
            {
                timerPoubelle += dt * couvercleSpeed;
                couvercle.localRotation = Quaternion.LerpUnclamped(lastRotation, fruits.Count == 0 ? _closedRotation : _openedRotation, couvercleAnim.Evaluate(timerPoubelle)); //fermer le couvercle avec timer et courbe
            }
        }

        for (int i = 0; i < fruits.Count; i++)
        {
            if (fruitTimers[i] >= 1f)
            {
                lastRotation = couvercle.localRotation;//setup fermeture couvercle
                timerPoubelle = 0;
                //--------------------------------------------------------
                Destroy(fruits[i].gameObject);
                //--------------------------------------------------------
                teRemoveFromLists.Add(fruits[i]);// on a joute le fruit pour la supression des listes
                continue;
            }

            fruitTimers[i] += dt * fruitSpeed;
            fruits[i].transform.position = Vector3.Lerp(basePos[i], Vector3.Lerp(topPointStored, dansPoubelleStored, destinationCurve.Evaluate(fruitTimers[i])), speedCurve.Evaluate(fruitTimers[i]));//deplacement fruit
            if (fruitTimers[i] > 0.8f && !hasBouped[i])
            {
                hasBouped[i] = true;
                _animator.SetTrigger("boup");//fruit rentre dans la poubelle : animation
            }
        }

        foreach (Transform item in teRemoveFromLists)
        {
            int index = fruits.IndexOf(item);
            fruits.RemoveAt(index); fruitTimers.RemoveAt(index); basePos.RemoveAt(index); hasBouped.RemoveAt(index); //on suprime tt les fruits détruits cette frame
        }
    }

    public void InitializeMovement(Transform fruit)// il faut désactiver le fruits avant de l'evoyer ici
    {
        fruits.Add(fruit); fruitTimers.Add(0); basePos.Add(fruit.position); hasBouped.Add(false); //ajout du fruit dans les listes
    }
}
