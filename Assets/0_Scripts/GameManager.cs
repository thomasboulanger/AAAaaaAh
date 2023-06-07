//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using UnityEngine;

/// <summary>
/// Game logic, states like win and lose, timer etc... 
/// </summary>
public class GameManager : MonoBehaviour
{
    public static bool InGame;
    public static bool GameOver;
    public static bool HasWin;

    [SerializeField] private GameObject level;

    public enum UIStateEnum
    {
        PressStartToAddPlayers,
        MainMenu,
        Options,
        Credits,
        Play,
        PreStart,
        Start,
        RebindInputs
    }

    public static UIStateEnum UICanvaState = UIStateEnum.PressStartToAddPlayers;

    public float levelTimer;
    private float _currentTime;

    private void Start() => Init();

    private void Init()
    {
        _currentTime = levelTimer;
        InGame = false;
        GameOver = false;
        HasWin = false;

        // 0 -> press start to add players
        // 1 -> main menu
        // 2 -> options
        // 3 -> credits
        // 4 -> play
        // 5 -> pre start
        // 6 -> start
        // 7 -> rebind inputs

        UICanvaState = UIStateEnum.PressStartToAddPlayers;
    }

    private void Update()
    {
        if (!InGame) return;
        float deltatime = Time.deltaTime;

        _currentTime -= deltatime;
        if (_currentTime < 0) GameOver = true;
    }

    public void PlayerChangePanel(Component sender, object data1, object unUsed1, object unUsed2)
    {
        if (data1 is not int) return;
        UICanvaState = (UIStateEnum) data1;
        Debug.Log("moved to panel " + (int) data1);
    }

    public static void PlayerHaveReachedTheEnd()
    {
        //player has passed the trigger box at the end of the level, now it has to seat and fire fruits on the demon
    }

    public void LoadLevel()
    {
        if(GameObject.FindGameObjectWithTag("LevelContainer")) Destroy(GameObject.FindGameObjectWithTag("LevelContainer"));
        GameObject go = Instantiate(level, transform.position, Quaternion.identity);
    }
}