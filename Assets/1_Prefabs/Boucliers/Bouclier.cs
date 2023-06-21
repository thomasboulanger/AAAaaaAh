using UnityEngine;

public class Bouclier : MonoBehaviour
{
    public Transform[] limbControllerList = new Transform[4];
    private int _j;
    private float _randomX;
    private float _randomXRoquette;
    private float _randomY;
    private float _randomYRoquette;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) _j++;
        if (_j > 3) _j = 0;

        transform.position = limbControllerList[_j].position;
        transform.eulerAngles = limbControllerList[_j].eulerAngles + new Vector3(0, 0, 0);
    }
}