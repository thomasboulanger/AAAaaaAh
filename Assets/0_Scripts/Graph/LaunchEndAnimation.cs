using System.Collections;
using UnityEngine;
using Cinemachine;

public class LaunchEndAnimation : MonoBehaviour
{
    [SerializeField] private PoubelleVisualManager poubelleRef;
    [SerializeField] private GameEvent onScreenFade;

    [SerializeField] private float timeToFade = 0.5f;
    [SerializeField] private float timeToWaitBetweenFades;

    [SerializeField] private CinemachineVirtualCamera virtualCamFinal;

    public void onLaunchEndAnimation(Component sender, object unUsed1, object unUsed2, object unUsed3)
    {
        onScreenFade.Raise(this, true, timeToFade, timeToWaitBetweenFades);
        StartCoroutine(FireAnimation());
    }

    IEnumerator FireAnimation()
    {
        virtualCamFinal.Priority = 11;
        yield return new WaitForSeconds(timeToFade);
        poubelleRef.PrepareCinematic();
    }

    // void Fire()
    // {
    //     onScreenFade.Raise(this, true, timeToFade, timeToWaitbetweenFades);
    //     StartCoroutine(FireAnimation());
    // }
    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.C))
    //     {
    //         Debug.Log("fireCinematic");
    //         Fire();
    //     }
    // }
}