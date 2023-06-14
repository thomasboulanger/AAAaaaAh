using UnityEngine;

public class PoubelleCollider : MonoBehaviour
{
    private PoubelleVisualManager parent;

    private void Awake() => parent = transform.parent.GetComponent<PoubelleVisualManager>();
    

    private void OnTriggerEnter(Collider other)
    {

        if (!other.transform.CompareTag("Fruit")) return;
        FruitSelector fruit = other.transform.GetComponentInParent<FruitSelector>();
        if (fruit.animating)
        {
            fruit.animating = false;
            return;
        }
        if (fruit.IsFruitGrabbed()) fruit.ReleaseFruit();
        fruit.animating = true;

        parent.InitializeFruitThenMoveIt(fruit.transform,false);
    }
}