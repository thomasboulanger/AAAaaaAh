//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using UnityEngine;

/// <summary>
/// Script placed on in-game pause panel, set his position to player 
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    
    private GameObject _player;
    private void Start() =>_player = GameObject.Find("Parpaing").gameObject;
    private void Update() => transform.position = _player.transform.position + offset;
    
}
