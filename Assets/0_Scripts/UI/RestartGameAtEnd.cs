//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using UnityEngine;

/// <summary>
/// Used to store data to change panel in game
/// </summary>
public class RestartGameAtEnd : MonoBehaviour
{
    [SerializeField] private GameEvent onRestartGame;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("UIInteractable")) return;
        onRestartGame.Raise(this, null, null, null);
        PlayerInputsScript.InGamePauseButton = false;
    }
}