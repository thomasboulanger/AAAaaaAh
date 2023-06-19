//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using UnityEngine;

/// <summary>
/// call the dedicated event
/// </summary>
public class ReturnToLastCheckPoint : MonoBehaviour
{
   [SerializeField] private GameEvent onReturnToLastCheckPoint;
   
   private void OnCollisionEnter(Collision collision)
   {
      if (!collision.transform.CompareTag("UIInteractable")) return;
      onReturnToLastCheckPoint.Raise(this,null,null,null);
   }
}
