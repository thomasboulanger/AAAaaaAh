//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using UnityEngine;

/// <summary>
/// Main Menu and UI script Manager used for UI / canvas / Button and settings
/// </summary>
public class MainMenu : MonoBehaviour
{
    #region OptionRegion

    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
    }

    public void SetResolution(float resolution)
    {
        switch ((int) resolution)
        {
            case 0:
                Screen.SetResolution(640, 360, Screen.fullScreen);
                break;
            case 1:
                Screen.SetResolution(800, 600, Screen.fullScreen);
                break;
            case 2:
                Screen.SetResolution(1280, 720, Screen.fullScreen);
                break;
            case 3:
                Screen.SetResolution(1600, 900, Screen.fullScreen);
                break;
            case 4:
                Screen.SetResolution(1920, 1080, Screen.fullScreen);
                break;
        }
    }

    #endregion
}