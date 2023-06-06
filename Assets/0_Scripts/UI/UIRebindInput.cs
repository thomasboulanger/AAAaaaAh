using UnityEngine;
using UnityEngine.InputSystem.Samples.RebindUI;

public class UIRebindInput : MonoBehaviour
{
    [SerializeField] private int playerIndexToRebind;
    [SerializeField] private RebindActionUI rebindScript;
    
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("UIInteractable")) return;
        
        collision.transform.GetComponent<UIHommingTruelle>().TruelleHitButton();

        
        PlayerManager.Players[playerIndexToRebind].actions.Disable();
        rebindScript.StartInteractiveRebind();

        PlayerManager.Players[playerIndexToRebind].actions.Enable();

        //string toto = _playerInput.actions.SaveBindingOverridesAsJson();
        //_playerInput.actions.LoadBindingOverridesFromJson(toto);
    }
}