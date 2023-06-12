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
    [SerializeField] private GameEvent onPlayerChangePanel;
    [SerializeField] private TMP_Text countDownText;
    [SerializeField] private GameObject uiPanelToFall;
    [SerializeField] private GameObject tutorialBlocks;

    private float _countDown;
    private float _countDownToIntDisplay;
    private PlayerManager _playerManager;
    private Transform _initialTransform;
    private bool _triggerOnceLaunchLevel;

    private void Awake()
    {
        _playerManager = GameObject.Find("GameManager").GetComponent<PlayerManager>();
        _initialTransform = uiPanelToFall.transform;
    }

    private void Start() => Init();

    private void Init()
    {
        _countDown = 4;
        countDownText.gameObject.SetActive(false);
        uiPanelToFall.SetActive(true);
        uiPanelToFall.transform.position = _initialTransform.transform.position;
        uiPanelToFall.transform.rotation = _initialTransform.transform.rotation;
        uiPanelToFall.transform.localScale = _initialTransform.transform.localScale;
        tutorialBlocks.gameObject.SetActive(false);
    }

    private void Update()
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

    private void LaunchTutorial()
    {
        onPlayerChangePanel.Raise(this, 5, null, null);
        _playerManager.StartGame();
        countDownText.gameObject.SetActive(false);
        uiPanelToFall.GetComponent<Rigidbody>().isKinematic = false;
        StartCoroutine(DesactivateFallPanelAfterDelay());
    }

    IEnumerator DesactivateFallPanelAfterDelay()
    {
        yield return new WaitForSeconds(.5f);
        tutorialBlocks.gameObject.SetActive(true);
        uiPanelToFall.SetActive(false);
    }

    private bool CheckForAllLimbsLock()
    {
        return PlayerManager.Players.Count > 1 && PlayerManager.AllLimbsAssigned &&
               GameManager.UICanvaState == GameManager.UIStateEnum.Play;
    }
}