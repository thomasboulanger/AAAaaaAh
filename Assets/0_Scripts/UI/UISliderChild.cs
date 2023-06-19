//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using UnityEngine;

/// <summary>
/// Volume child, call an event in parent collide with a truelle
/// </summary>
public class UISliderChild : MonoBehaviour
{
    [SerializeField] private int volumeValue;
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("UIInteractable")) return;
        transform.parent.GetComponent<UISliderParent>().UpdateVolumeValue(volumeValue);
        collision.transform.GetComponent<UIHommingTruelle>().TruelleHitButton();
    }
}