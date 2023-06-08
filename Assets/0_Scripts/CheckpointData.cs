//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using UnityEngine;

/// <summary>
/// Script that store the data on checkpoints, place one on each checkpoints along the level
/// </summary>
public class CheckpointData : MonoBehaviour
{
   public int checkpointNumber;

   private void OnTriggerEnter(Collider other)
   {
      if(!other.transform.CompareTag("Player")) return;
      other.GetComponent<CheckpointManager>().SetNewCheckpoint(transform.position, checkpointNumber);
   }
}