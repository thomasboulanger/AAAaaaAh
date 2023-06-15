using UnityEngine;
using System.Collections.Generic;

public class PoubelleCollider : MonoBehaviour
{
    private PoubelleVisualManager parent;

    private void Awake() => parent = transform.parent.GetComponent<PoubelleVisualManager>();
    

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.CompareTag("Fruit")) return;
        FruitSelector fruit = other.transform.GetComponentInParent<FruitSelector>();

        fruit.CanBeStrored(this, true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.transform.CompareTag("Fruit")) return;
        FruitSelector fruit = other.transform.GetComponentInParent<FruitSelector>();

        fruit.CanBeStrored(this, false);
    }

    public void Store(FruitSelector fruit)
    {
        fruit.CanBeStrored(this, false);

        if (fruit.animating)
        {
            fruit.animating = false;
            return;
        }
        if (fruit.IsFruitGrabbed()) fruit.ReleaseFruit(false);
        fruit.animating = true;

        parent.InitializeFruitThenMoveIt(fruit.transform, false);
    }
}