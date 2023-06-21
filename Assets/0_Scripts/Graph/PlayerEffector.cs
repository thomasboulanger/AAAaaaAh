using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffector : MonoBehaviour
{

    [SerializeField] List<GameObject> playersParts = new();
    public void FreezePlayerAndHidePlayer(Component sender, object unUsed1, object unUsed2, object unUsed3)
    {
        GetComponent<Rigidbody>().isKinematic = true;

        foreach (GameObject item in playersParts)
        {
            item.SetActive(false);
        }
    }
}
