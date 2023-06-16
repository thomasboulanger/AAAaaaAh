//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using Cinemachine;
using UnityEngine;

/// <summary>
/// camera manager, store all Vcam and change the priority between game state
/// </summary>
public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera[] VCams;

    private CinemachineBrain _camera;

    private void Start()
    {
        _camera = Camera.main.GetComponent<CinemachineBrain>();
        VCams[0].Priority = 20;
    }

    public void CameraPriorityChange(Component sender, object data1, object unUsed1, object unUsed2)
    {
        if ((int) data1 is 5 or 6 or 8 or 9) return;
        foreach (var vCam in VCams) vCam.Priority = 10;
        VCams[(int) data1].Priority = 20;
    }
}
