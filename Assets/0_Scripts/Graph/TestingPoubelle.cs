using UnityEngine;

public class TestingPoubelle : MonoBehaviour
{
    public FruitSelector test;
    public PoubelleVisualManager poubelleRef;
    public PoubelleCollider poubelleCRef;

    void Update()
    {
        bool trigered = false;
        if (Input.GetKey(KeyCode.B))
        {
            FruitSelector testInstance = Instantiate(test, transform.position, transform.rotation);
            testInstance.FakePossessesion(poubelleCRef);
            poubelleRef.InitializeFruitThenMoveIt(testInstance.transform, false);
            trigered = true;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            poubelleRef.EjectFruits();
            trigered = true;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            poubelleRef.PrepareCinematic();
            trigered = true;
        }
        // if (Input.GetKeyDown(KeyCode.J))
        // {
        //     poubelleRef.EjectFruitAtEndLevel();
        //     trigered = true;
        // }

        if (trigered) Debug.LogWarning("paf");//pour pas oublier que c'est la
    }
}
