using TMPro;
using UnityEngine;

public class ChangePlayerToRebind : MonoBehaviour
{
    [SerializeField] private UIRebindInput[] uiRebindInputs;
    [SerializeField] private TMP_Text text;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("UIInteractable")) return;
        collision.transform.GetComponent<UIHommingTruelle>().TruelleHitButton();

        foreach (UIRebindInput element in uiRebindInputs)
        {
            element.playerIndexToRebind++;
            if (element.playerIndexToRebind > PlayerManager.Players.Count - 1) element.playerIndexToRebind = 0;

            text.text = "Player : " + (element.playerIndexToRebind + 1);
            element.UpdateRebindVisual(this,null,null,null);
        }
    }
}
