//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Script that replace the "start" button by checking if all limbs are assigned and locked the launch a countdown to start the game
/// </summary>
public class TutorialLauncher : MonoBehaviour
{
    [SerializeField] private GameEvent onPlayerChangePanel;

    [SerializeField] private TMP_Text countDownText;
    [SerializeField] private List<GameObject> uiObjectToDispawnAtTutorial;
    [SerializeField] private GameObject tutorialBlocks;
    [SerializeField] private GameObject uiArrows;
    [SerializeField] private GameObject uiHintPartTwo;

    private float _countDown;
    private float _countDownToIntDisplay;
    private PlayerManager _playerManager;
    private bool _triggerOnceLaunchLevel;
    private bool _triggerOnceLaunchTutorialPartTwo;


    private void Awake()
    {
        _playerManager = GameObject.Find("GameManager").GetComponent<PlayerManager>();
    }

    private void Start() => Init();

    private void Init()
    {
        _countDown = 4;
        countDownText.gameObject.SetActive(false);
        foreach (GameObject obj in uiObjectToDispawnAtTutorial)
            obj.SetActive(true);

        tutorialBlocks.gameObject.SetActive(false);
        uiArrows.gameObject.SetActive(false);
        uiHintPartTwo.gameObject.SetActive(false);
    }

    private void Update()
    {
        TutorialPartOne();
        //if(_triggerOnceLaunchTutorialPartTwo) TutorialPartTwo();
    }

    private void TutorialPartOne()
    {
        if (_countDown < 0)
        {
            if (_triggerOnceLaunchLevel) return;
            _triggerOnceLaunchLevel = true;
            LaunchTutorial();
            return;
        }

        if (CheckForAllLimbsLock()) _countDown -= Time.deltaTime;
        else _countDown = 4;

        countDownText.gameObject.SetActive(CheckForAllLimbsLock());
        _countDownToIntDisplay = (int) _countDown;
        countDownText.text = _countDownToIntDisplay.ToString();
    }

    private void TutorialPartTwo()
    {
    }

    private bool CheckForAllLimbsLock()
    {
        return PlayerManager.Players.Count > 1 && PlayerManager.AllLimbsAssigned &&
               GameManager.UICanvaState == GameManager.UIStateEnum.Play;
    }

    private void LaunchTutorial()
    {
        onPlayerChangePanel.Raise(this, 5, null, null);
        _playerManager.StartGame();
        countDownText.gameObject.SetActive(false);
        StartCoroutine(DeactivateFallPanelAfterDelay());
    }

    IEnumerator DeactivateFallPanelAfterDelay()
    {
        yield return new WaitForSeconds(2);
        tutorialBlocks.gameObject.SetActive(true);
        foreach (GameObject obj in uiObjectToDispawnAtTutorial)
            obj.SetActive(false);
    }

    public void OnFirstTutorialPartAchieved(Component sender, object unUsed1, object playerID, object inputID)
    {
        if (playerID is not int) return;
        if (inputID is not int) return;
        if ((int) playerID != 0) return;
        if ((int) inputID != 0) return;

        uiArrows.gameObject.SetActive(true);
        onPlayerChangePanel.Raise(this, 6, null, null);
        _triggerOnceLaunchTutorialPartTwo = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.CompareTag("Player") && _triggerOnceLaunchTutorialPartTwo) return;
        uiArrows.gameObject.SetActive(false);
        uiHintPartTwo.gameObject.SetActive(true);
    }
}