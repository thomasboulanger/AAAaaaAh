//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using UnityEngine;

/// <summary>
/// Used to store data to change panel on UI
/// </summary>
public class UIButtonInfo : MonoBehaviour
{
    public int indexToMoveTo;
    [SerializeField] private GameEvent onPlayerChangePanel;
    [SerializeField] private GameEvent onPlayerPressPause;
    [SerializeField] private GameEvent onRestartGame;

    private void OnCollisionEnter(Collision collision)
    {
        if (GameManager.UICanvaState is GameManager.UIStateEnum.Play or GameManager.UIStateEnum.PreStart or GameManager.UIStateEnum.Start)
        {
            if(!collision.transform.name.Contains("Truelle")) return;
            onPlayerChangePanel.Raise(this, indexToMoveTo, null, null);
            
            if(PlayerInputsScript.InGamePauseButton) onRestartGame.Raise(this,null,null,null);
            
            onPlayerPressPause.Raise(this, 0, false, null);
            PlayerInputsScript.InGamePauseButton = false;
            return;
        }
        
        if (!collision.transform.CompareTag("UIInteractable")) return;
        
        onPlayerChangePanel.Raise(this, indexToMoveTo, null, null);
        collision.transform.GetComponent<UIHommingTruelle>().TruelleHitButton();
    }
}