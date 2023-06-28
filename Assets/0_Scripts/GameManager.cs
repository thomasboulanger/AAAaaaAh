//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Game logic, states like win and lose, timer etc... 
/// </summary>
public class GameManager : MonoBehaviour
{
    public static bool InGame;
    public static bool TrowelBouncing;

    [SerializeField] private GameEvent onUpdateRebindVisual;
    [SerializeField] private GameEvent onFlyCanvaToggle;
    [SerializeField] private GameEvent onFadeScreen;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private CursorController cursor;
    [SerializeField] private GameObject level;
    [SerializeField] private float delayBeforeRelaunchCinematic;

    private float _currentDelayLeft;

    public enum UIStateEnum
    {
        PressStartToAddPlayers,
        MainMenu,
        Options,
        Credits,
        Play,
        PreStart,
        Start,
        RebindInputs,
        PlayerHaveReachEndOfLevel,
        Quit,
    }

    public static UIStateEnum UICanvaState = UIStateEnum.PressStartToAddPlayers;

    public enum Difficulty
    {
        Nofly,
        PeacefulFlies,
        AgressiveFliesNoFruitLoss,
        AgressiveFliesFruitLoss,
        Ganged
    }

    public static Difficulty CurrentDifficulty;

    private void Start() => Init();

    public void Init()
    {
        //fade out the screen at start (the screen should be black at start)
        onFadeScreen.Raise(this,false,null,null);

        InGame = false;
        TrowelBouncing = true;
        pauseMenu.SetActive(false);
        _currentDelayLeft = delayBeforeRelaunchCinematic;

        // 0 -> press start to add players
        // 1 -> main menu
        // 2 -> options
        // 3 -> credits
        // 4 -> play
        // 5 -> pre start
        // 6 -> start
        // 7 -> rebind inputs
        // 8 -> end of level
        // 9 -> quit
        // (int) UICanvaState > 3 or < 7 

        UICanvaState = UIStateEnum.PressStartToAddPlayers;
    }

    private void Update()
    {
        switch (UICanvaState)
        {
            case UIStateEnum.Quit:
                Application.Quit();
                break;
            case UIStateEnum.Play:
            case UIStateEnum.PreStart:
            case UIStateEnum.Start:
            case UIStateEnum.PlayerHaveReachEndOfLevel:
                return;
        }

        bool playersAFK = true;
        foreach (bool element in PlayerInputsScript.PlayersAreAFK)
            if (!element)
                playersAFK = false;

        if (!playersAFK) _currentDelayLeft = delayBeforeRelaunchCinematic;
        else _currentDelayLeft -= Time.deltaTime;
        if (_currentDelayLeft < 0) SceneManager.LoadScene(0);
    }

    public void PlayerChangePanel(Component sender, object data1, object unUsed1, object unUsed2)
    {
        if (data1 is not int) return;
        UICanvaState = (UIStateEnum) data1;

        if ((int) data1 is 7) onUpdateRebindVisual.Raise(this, null, null, null);
        if ((int) data1 is not 6) onFlyCanvaToggle.Raise(this, false, null, null);

        Debug.Log("moved to panel " + (int) data1);
    }

    public void PlayerPressPause(Component sender, object data1, object isActive, object unUsed2)
    {
        if (data1 is not int) return;
        pauseMenu.SetActive((bool) isActive);
        cursor.cursorID = (int) data1;
    }
    

    public void PlayerHasReachEndOfLevel(Component sender, object unUsed1, object unUsed2, object unUsed3)
    {
        PlayerChangePanel(this, 8, null, null);
        InGame = false;
    }

    public void ToggleTrowelBounciness(Component sender, object unUsed1, object unUsed2, object unUsed3) => TrowelBouncing = !TrowelBouncing;

    public void LoadLevel()
    {
        if (GameObject.FindGameObjectWithTag("LevelContainer"))
            Destroy(GameObject.FindGameObjectWithTag("LevelContainer"));
        Instantiate(level, transform.position, Quaternion.identity);
    }

    public void RestartGame(Component sender, object unUsed1, object unUsed2, object unUsed3)
    {
        // Disable all PlayerInput components in the scene
        PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
        foreach (PlayerInput player in playerInputs)
        {
            player.DeactivateInput();
            player.enabled = false;
        }

        //clear the playerInputs list
        PlayerManager.Players.Clear();
        
        onFadeScreen.Raise(this,true,null,null);
        StartCoroutine(RestartGameCoroutine());
    }

    IEnumerator RestartGameCoroutine()
    {
        yield return new WaitForSeconds(1);
        
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}