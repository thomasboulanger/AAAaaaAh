using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraCopy : MonoBehaviour
{
    [SerializeField] private Camera playerCam;
    [SerializeField] private Camera moucheCam;

    private float fov;
    void Update()
    {
        if (fov != playerCam.fieldOfView && playerCam!=null && moucheCam !=null)
        {
            fov = playerCam.fieldOfView;
            moucheCam.fieldOfView = fov;
        }
    }
}
