using UnityEngine;
using UnityEngine.InputSystem.Samples.RebindUI;

public class UIResetInput : MonoBehaviour
{
    [SerializeField] private RebindActionUI rebindScript;
    
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("UIInteractable")) return;
        rebindScript.ResetToDefault();
        collision.transform.GetComponent<UIHommingTruelle>().TruelleHitButton();
    }
}