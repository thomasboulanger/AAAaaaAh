using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchEndAnimation : MonoBehaviour
{
   [SerializeField] private PoubelleVisualManager poubelleRef;
   [SerializeField] private GameEvent onScreenFade;

    [SerializeField] private float timeToFade = 0.5f;
    [SerializeField] private float timeToWaitbetweenFades = 0f;
    public void onLaunchEndAnimation(Component sender, object unUsed1, object unUsed2, object unUsed3)
    {
        Fire();
    }

    void Fire()
    {
        onScreenFade.Raise(this, true, timeToFade, timeToWaitbetweenFades);
        StartCoroutine(FireAnimation());
    }

    IEnumerator FireAnimation()
    {
        yield return new WaitForSeconds(timeToFade);
        poubelleRef.PrepareCinematic();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("fireCinematic");
            Fire();
        }
    }
}
