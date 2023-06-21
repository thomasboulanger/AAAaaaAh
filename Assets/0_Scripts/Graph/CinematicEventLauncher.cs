using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CinematicEventLauncher : MonoBehaviour
{
    [SerializeField] Selection choice;


    private bool textIsVisible;

    [SerializeField]private TMP_Text text;

    private float _t;

    [SerializeField] private float lerpSpeed=10f;
    [SerializeField] private float timeToWait=3f;



    enum Selection
    {
        windowToggle,
        lauchAnimation,
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

    public void LoadScene()
    {
        SceneManager.LoadScene(1);
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            if (textIsVisible) LoadScene();
            else ShowText();
        }
        float dt = Time.deltaTime;

        text.color = Color.Lerp(text.color, textIsVisible ? Color.white : Color.clear, dt * lerpSpeed);

        if (textIsVisible && _t< timeToWait)
        {
            _t += dt;
        }
        else
        {
            textIsVisible = false;
            _t = 0;
        }
    }

    void ShowText()
    {
        textIsVisible = true;
        text.enabled = true;
    }

    private void Start()
    {
        text.enabled = false;
    }


}
