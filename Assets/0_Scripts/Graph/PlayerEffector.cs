using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffector : MonoBehaviour
{
    public void FreezePlayer(Component sender, object unUsed1, object unUsed2, object unUsed3)
    {
        GetComponent<Rigidbody>().isKinematic = true;
    }
}
