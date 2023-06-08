using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighResScreenshots : MonoBehaviour
{
    [SerializeField] private string path;
    [Range(0,5)]
    [SerializeField] private int size;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            path += "screenshot ";
            path += System.Guid.NewGuid().ToString() + ".png";

            ScreenCapture.CaptureScreenshot(path, size);
        }
    }
}
