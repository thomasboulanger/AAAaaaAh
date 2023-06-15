using UnityEngine;

public class EjectFruitsAtEndLevel : MonoBehaviour
{
    private bool _isGrabbing;
    private bool _triggerOnce;
    
    public void OnPlayerGrabAfterEndOfLevel(Component sender, object data1, object unUsed1, object unUsed2)
    {
        if (data1 is not float) return;
        _isGrabbing = (float) data1 > .9f;

        if (_isGrabbing && _triggerOnce)
        {
            //vire tout ce qui touche au triggerOnce si tu veux que les gens rafalent...
            _triggerOnce = false;

            //fait ton code ici
        }
        else
        {
            _triggerOnce = true;
        }
    }
}