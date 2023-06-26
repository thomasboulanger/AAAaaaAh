
using UnityEngine;

public class DangerousButton : MonoBehaviour
{
   private void Update()
   {
      gameObject.SetActive(
         GameManager.UICanvaState is not (not GameManager.UIStateEnum.Play or GameManager.UIStateEnum.PreStart));
   }
}
