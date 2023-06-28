//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using TMPro;
using UnityEngine;

/// <summary>
/// Script that toggle trowel bounciness on/off
/// </summary>
public class TrowelBouncingToggle : MonoBehaviour
{
    [SerializeField] private GameEvent onChangeTrowelBounciness;
    
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("UIInteractable")) return;
        onChangeTrowelBounciness.Raise(this, null, null, null);
        transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text =
            "Trowel bouncing: " + (GameManager.TrowelBouncing ? "yes" : "no");
    }
}