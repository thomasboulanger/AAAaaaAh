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
    [SerializeField] private GameObject tutorialCage;
    [SerializeField] private GameObject uiHintToMoveJoysticks;
    [SerializeField] private GameObject uiArrows;
    [SerializeField] private GameObject uiHintPartTwo;

    private float _countDown;
    private float _countDownToIntDisplay;
    private PlayerManager _playerManager;
    private bool _triggerOnceLaunchLevel;
    private GameObject[] _joysticks = new GameObject[8];


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

        tutorialBlocks.SetActive(false);
        tutorialCage.SetActive(true);
        uiHintToMoveJoysticks.SetActive(false);
        uiArrows.SetActive(false);
        uiHintPartTwo.SetActive(false);
    }

    private void Update()
    {
        if (GameManager.UICanvaState == GameManager.UIStateEnum.Play) uiHintToMoveJoysticks.SetActive(true);
        else uiHintToMoveJoysticks.SetActive(false);

        if(!_triggerOnceLaunchLevel) TutorialPartOne();
        //if(_triggerOnceLaunchTutorialPartTwo) TutorialPartTwo();
    }

    private void TutorialPartOne()
    {
        if (_countDown < 0)
        {
            _triggerOnceLaunchLevel = true;
            LaunchTutorial();
            return;
        }

        if (CheckForAllLimbsLock()) _countDown -= Time.deltaTime;
        else _countDown = 4;

        uiHintToMoveJoysticks.SetActive(!CheckForAllLimbsLock());
        countDownText.gameObject.SetActive(CheckForAllLimbsLock());
        countDownText.text = ((int) _countDown).ToString();
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
        
        //can be a coroutine from there
        tutorialBlocks.gameObject.SetActive(true);
        foreach (GameObject obj in uiObjectToDispawnAtTutorial)
            obj.SetActive(false);

        _joysticks = GameObject.FindGameObjectsWithTag("Cursor");
        foreach (GameObject obj in _joysticks)
            if (!obj.transform.name.Contains("Cursor"))
                obj.SetActive(false);
    }

    public void OnFirstTutorialPartAchieved(Component sender, object unUsed1, object playerID, object inputID)
    {
        tutorialCage.SetActive(false);
        uiArrows.gameObject.SetActive(true);
        onPlayerChangePanel.Raise(this, 6, null, null);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.CompareTag("Player")) return;
        uiArrows.gameObject.SetActive(false);
        uiHintPartTwo.gameObject.SetActive(true);
    }
}