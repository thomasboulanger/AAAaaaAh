using UnityEngine;

public class TestingPoubelle : MonoBehaviour
{
    public GameObject test;
    public PoubelleVisualManager poubelleRef;

    void Update()
    {
        bool trigered = false;
        if (Input.GetKey(KeyCode.B))
        {
            GameObject testInstance = Instantiate(test, transform.position, transform.rotation);
        
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
            poubelleRef.EjectFruits();
            trigered = true;
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            poubelleRef.EjectFruitAtEndLevel();
            trigered = true;
        }

        if (trigered) Debug.LogWarning("paf");//pour pas oublier que c'est la
    }
}
