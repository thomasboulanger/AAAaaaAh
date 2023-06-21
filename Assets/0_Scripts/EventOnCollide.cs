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
    [SerializeField] private bool parameter1;
    [SerializeField] private object parameter2;
    [SerializeField] private object parameter3;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.CompareTag("Player")) return;
        Debug.Log("passed the end wall");
        eventTrigered.Raise(this, parameter1, parameter2, parameter3);
    }
}
