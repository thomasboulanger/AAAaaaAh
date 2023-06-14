using UnityEngine;

public class TestingPoubelle : MonoBehaviour
{
    public GameObject test;
    public PoubelleVisualManager poubelleRef;

    void Update()
    {
        if (Input.GetKey(KeyCode.B))
        {
            GameObject testInstance = Instantiate(test, transform.position, transform.rotation);
        
            poubelleRef.InitializeFruitThenMoveIt(testInstance.transform, false);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            poubelleRef.EjectFruits();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            poubelleRef.EjectFruits();
        }
    }
}
