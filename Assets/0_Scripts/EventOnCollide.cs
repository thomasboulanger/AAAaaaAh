//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using UnityEngine;

/// <summary>
/// This Script detect when the player pass through it then call an event
/// </summary>
public class EventOnCollide : MonoBehaviour
{
    [SerializeField] private GameEvent eventTrigered;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.CompareTag("Player")) return;
        eventTrigered.Raise(this, null, null, null);
    }
}
