//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Script that replace the "start" button by checking if all limbs are assigned and locked the launch a countdown to start the game
/// </summary>
public class TimerBeforeStart : MonoBehaviour
{
    [SerializeField] private TMP_Text countDownText;
    [SerializeField] private TMP_Text titleTimerText;
    [SerializeField] private GameObject uiPanelToFall;

    private bool _allLimbsLock;
    private float _countDown;
    private float _countDownToIntDisplay;
    private PlayerManager _playerManager;

    private void Awake() => _playerManager = GameObject.Find("GameManager").GetComponent<PlayerManager>();
    private void Start() => Init();

    private void Init()
    {
        _countDown = 4;
        titleTimerText.gameObject.SetActive(false);
        countDownText.gameObject.SetActive(false);
        uiPanelToFall.SetActive(true);
    }

    private void Update()
    {
        if (_countDown < 0)
        {
            LaunchTutorial();
            return;
        }

        CheckForAllLimbsLock();
        if (_allLimbsLock) _countDown -= Time.deltaTime;
        else _countDown = 4;

        titleTimerText.gameObject.SetActive(_allLimbsLock);
        countDownText.gameObject.SetActive(_allLimbsLock);
        _countDownToIntDisplay = (int) _countDown;
        countDownText.text = _countDownToIntDisplay.ToString();
    }

    private void LaunchTutorial()
    {
        _playerManager.StartGame();
        countDownText.gameObject.SetActive(false);
        titleTimerText.gameObject.SetActive(false);
        uiPanelToFall.GetComponent<Rigidbody>().isKinematic = false;
        StartCoroutine(DesactivateFallPanelAfterDelay());
    }

    IEnumerator DesactivateFallPanelAfterDelay()
    {
        yield return new WaitForSeconds(5);
        uiPanelToFall.SetActive(false);
    }

    private void CheckForAllLimbsLock() => _allLimbsLock =
        PlayerManager.Players.Count > 1 && PlayerManager.AllLimbsAssigned && !GameManager.InGame;
}