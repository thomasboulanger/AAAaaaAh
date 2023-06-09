//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using UnityEngine;

/// <summary>
/// Script simply call an event when the player pass through its collider
/// </summary>
public class PlayerReachEndOfLevel : MonoBehaviour
{
   [SerializeField] private GameEvent onPlayerReachEndOfLevel;

   private void OnTriggerEnter(Collider other)
   {
      if(!other.transform.CompareTag("Player"))return;
      onPlayerReachEndOfLevel.Raise(this,null,null,null);
   }
}
