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
    [SerializeField] private GameEvent onUpdateRebindVisual;

    [SerializeField] private GameObject pauseMenu;

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
        Quit
    }

    public static UIStateEnum UICanvaState = UIStateEnum.PressStartToAddPlayers;

    [SerializeField] private GameObject level;
    [SerializeField] private GameObject limbSelectionPanel;
    [SerializeField] private GameObject tutorialCharacterCage;

    private void Start() => Init();

    private void Init()
    {
        InGame = false;
        pauseMenu.SetActive(false);

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
        if (UICanvaState == UIStateEnum.Quit) Application.Quit();
    }

    public void PlayerChangePanel(Component sender, object data1, object unUsed1, object unUsed2)
    {
        if (data1 is not int) return;
        if (UICanvaState == UIStateEnum.Play && (int) data1 < 4) InGame = false;
        
        UICanvaState = (UIStateEnum) data1;
        if ((int) data1 == 7) onUpdateRebindVisual.Raise(this, null, null, null);
        Debug.Log("moved to panel " + (int) data1);
    }

    public void PlayerPressPause(Component sender, object data1, object isActive, object unUsed2)
    {
        if(data1 is not int) return;
        pauseMenu.SetActive((bool) isActive);
        if((bool) isActive) pauseMenu.transform.GetChild(0).GetComponent<CursorController>().cursorID = (int) data1;
    }
    
    public void PlayerHasReachEndOfLevel(Component sender, object unUsed1, object unUsed2, object unUsed3)
    {
        PlayerChangePanel(this, 8, null, null);
        InGame = false;
    }

    public void LoadLevel()
    {
        if (GameObject.FindGameObjectWithTag("LevelContainer"))
            Destroy(GameObject.FindGameObjectWithTag("LevelContainer"));
        Instantiate(level, transform.position, Quaternion.identity);
    }
}