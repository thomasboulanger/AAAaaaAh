//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using UnityEngine;

/// <summary>
/// Resume button function on in-game pause menu
/// </summary>
public class ResumeButton : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("UIInteractable")) return;
        Destroy(collision.gameObject);
        transform.parent.gameObject.SetActive(false);
    }
}
