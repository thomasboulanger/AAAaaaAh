using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicEventLauncher : MonoBehaviour
{
    [SerializeField] Selection choice;
    enum Selection
    {
        windowToggle,
        lauchAnimation
    }
    [SerializeField] private List<OppeningsController> windowTargets = new List<OppeningsController>();

    //[Header("0 normal, 1 lights, 2 disco")]

    public void Activate(int action)
    {
        switch (choice)
        {
            case Selection.windowToggle:
                WindowToggle(action);
                break;
            case Selection.lauchAnimation:
                break;
            default:
                break;
        }

    }

    void WindowToggle(int action)
    {
        foreach (OppeningsController item in windowTargets)
        {
            switch (action)
            {
                case 0:
                    item.Disco(false, 0);
                    break;
                case 1:
                    item.Disco(true, 1);
                    break;
                case 2:
                    item.Disco(true, 2);
                    break;
                default:
                    break;
            }
        }
    }
}
