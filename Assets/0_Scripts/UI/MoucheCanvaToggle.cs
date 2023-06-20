using UnityEngine;

public class MoucheCanvaToggle : MonoBehaviour
{
    public void OnFlyCanvaToggle(Component sender, object data1, object data2, object data3)
    {
        if (data1 is not bool) return;
        transform.GetChild(0).gameObject.SetActive((bool)data1);
    }
}
