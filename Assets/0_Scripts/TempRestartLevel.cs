using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// temp script for development only, to delete later
/// </summary>
public class TempRestartLevel : MonoBehaviour
{
    [SerializeField] private int sceneIndex;
    private void Start()
    {
        PlayerManager.RestartLevel(sceneIndex);
        foreach (PlayerInput player in PlayerManager.Players)
            player.user.UnpairDevices();
    }
}