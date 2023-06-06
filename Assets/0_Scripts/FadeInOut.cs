//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using System.Collections;
using UnityEngine;

/// <summary>
/// This script simply fade in and out a canvas with a canvasGroup on it with modular delay
/// </summary>
public class FadeInOut : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float timeToFade;
    [SerializeField] private float waitingTimeBetweenFadeInAndOut;

    private bool _fadeIn;
    private bool _fadeOut;

    public void FadeScreen(Component sender, object data1, object unUsed1, object unUsed2)
    {
        if (data1 is not bool) return;
        _fadeIn = (bool) data1;
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        if (_fadeIn)
        {
            if (canvasGroup.alpha <= 1)
            {
                canvasGroup.alpha += deltaTime / timeToFade;
                if (canvasGroup.alpha >= 1)
                {
                    _fadeIn = false;
                    StartCoroutine(WaitBeforeFadeOut());
                }
            }
        }

        if (_fadeOut)
        {
            if (canvasGroup.alpha >= 0)
            {
                canvasGroup.alpha -= deltaTime / timeToFade;
                if (canvasGroup.alpha == 0) _fadeOut = false;
            }
        }
    }

    private IEnumerator WaitBeforeFadeOut()
    {
        yield return new WaitForSeconds(waitingTimeBetweenFadeInAndOut);
        _fadeOut = true;
    }
}