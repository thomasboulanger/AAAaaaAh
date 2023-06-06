//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using UnityEngine;

/// <summary>
/// Script that call an event for fade screen in and out
/// </summary>
public class ScreenFadeEvent : MonoBehaviour
{
    [SerializeField] private GameEvent fadeOutEvent;

    public void CallScreenFade() => fadeOutEvent.Raise(this, true, null, null);
}