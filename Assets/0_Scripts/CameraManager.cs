//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using Cinemachine;
using UnityEngine;
using UnityEngine.PlayerLoop;

/// <summary>
/// camera manager, store all Vcam and change the priority between game state
/// </summary>
public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera[] vCams;
    [SerializeField] private CursorController[] cursors;

    private CinemachineBrain _camera;

    private void Start() => Init();

    public void Init()
    {
        _camera = Camera.main.GetComponent<CinemachineBrain>();
        vCams[0].Priority = 11;
    }

    public void CameraPriorityChange(Component sender, object data1, object unUsed1, object unUsed2)
    {
        if ((int) data1 is 5 or 6 or 9) return;

        foreach (var vCam in vCams) vCam.Priority = 10;
        vCams[(int) data1].Priority = 11;

        foreach (var cursor in cursors) cursor.gameObject.SetActive(false);
        cursors[(int) data1].gameObject.SetActive(true);
    }
}