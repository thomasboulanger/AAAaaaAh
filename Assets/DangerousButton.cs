using UnityEngine;

public class DangerousButton : MonoBehaviour
{
    private void Update()
    {
        if(GameManager.UICanvaState is GameManager.UIStateEnum.Start) gameObject.SetActive(false);
    }
}