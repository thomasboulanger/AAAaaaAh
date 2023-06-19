//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using TMPro;
using UnityEngine;

/// <summary>
/// Script placed on button to enable/disable interaction with
/// </summary>
public class ButtonOpacityAndInteractibility : MonoBehaviour
{
    [SerializeField] private Collider collider;
    [SerializeField] private TMP_Text text;
    void Update()
    {
        if(GameManager.UICanvaState != GameManager.UIStateEnum.PressStartToAddPlayers) return;
        collider.isTrigger = PlayerManager.Players.Count > 1;
        text.alpha = PlayerManager.Players.Count > 1 ? 255 : 180;
    }
}
