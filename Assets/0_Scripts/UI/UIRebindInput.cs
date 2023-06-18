using UnityEngine;
using UnityEngine.InputSystem.Samples.RebindUI;

public class UIRebindInput : MonoBehaviour
{
    public int playerIndexToRebind;
    [SerializeField] private RebindActionUI rebindScript;
    [SerializeField] private string rebingActionString;
    
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("UIInteractable")) return;
        
        collision.transform.GetComponent<UIHommingTruelle>().TruelleHitButton();
        
        PlayerManager.Players[playerIndexToRebind].actions.Disable();
        rebindScript.StartInteractiveRebind(playerIndexToRebind);
        PlayerManager.Players[playerIndexToRebind].actions.Enable();
    }

    public void UpdateRebindVisual(Component component, object unUsed1, object unUsed2, object unUsed3)
    {
        rebindScript.actionToRebind ??= PlayerManager.Players[playerIndexToRebind].actions.FindAction(rebingActionString);
        rebindScript.UpdateBindingDisplay();
    }
}
