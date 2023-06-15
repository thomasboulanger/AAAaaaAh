//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using UnityEngine;

/// <summary>
/// Call a function that "clean" all the truelles that are stuck in UI panel
/// </summary>
public class RecycleTruelle : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("UIInteractable")) return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, 25, 15);
        foreach (Collider collider in colliders)
        {
            if (collider.transform.GetComponent<UIHommingTruelle>())
                collider.transform.GetComponent<UIHommingTruelle>().isRecycling = true;
        }
    }
}