using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoubelleVisualManager : MonoBehaviour
{
    [SerializeField] private Transform topPoint;
    [SerializeField] private Transform dansPoubelle;
    [SerializeField] private Transform couvercle;
    [SerializeField] private Transform corp;
    [SerializeField] private List<Transform> fruits = new List<Transform>();
    [SerializeField] private List<float> fruitTimers = new List<float>();
    [SerializeField] private List<Vector3> basePos = new List<Vector3>();

    [SerializeField] private float speed = 2f;

    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private AnimationCurve bloupCurve;
    [SerializeField] private AnimationCurve offsetCurve;//avec top point

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        List<Transform> toRemove = new List<Transform>();
        float dt = Time.deltaTime;

        Vector3 dansPoubelleStored = dansPoubelle.position;
        Vector3 topPointStored = topPoint.position;

        for (int i = 0; i < fruits.Count; i++)
        {
            if (fruitTimers[i] >= 1f)
            {
                Destroy(fruits[i].gameObject);
                toRemove.Add(fruits[i]);
                Debug.Log("fruitremoved");
                continue;
            }

            fruitTimers[i] += dt * speed;

            fruits[i].transform.position = Vector3.Lerp(basePos[i], new Vector3(dansPoubelleStored.x, Mathf.Lerp(basePos[i].y, topPointStored.y, offsetCurve.Evaluate(fruitTimers[i])), dansPoubelleStored.z), speedCurve.Evaluate(fruitTimers[i]));
        }

        foreach (Transform item in toRemove)
        {
            int index = fruits.IndexOf(item);

            fruits.RemoveAt(index);fruitTimers.RemoveAt(index);basePos.RemoveAt(index);
        }

    }

    public void InitializeMovement(Transform fruit)// il faut désactiver le fruits vanat de l'evoyer ici
    {
        fruits.Add(fruit);fruitTimers.Add(0);basePos.Add(fruit.position);
    }
}
