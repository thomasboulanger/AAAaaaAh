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

    [SerializeField] private GameObject level;
    [SerializeField] private GameObject limbSelectionPanel;
    [SerializeField] private GameObject tutorialCharacterCage;

    private void Start() => Init();

    private void Init()
    {
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

    public void PlayerChangePanel(Component sender, object data1, object unUsed1, object unUsed2)
    {
        if (data1 is not int) return;
        UICanvaState = (UIStateEnum) data1;
        Debug.Log("moved to panel " + (int) data1);
    }

    //event to launch the game for real (after the tutorial part)
    public void AllTutorialBlocksAreGrabbed(Component sender, object data1, object playerID, object limbID)
    {
        if (data1 is not bool) return;
        if (playerID is not int) return;
        if (limbID is not int) return;
        
        //test to execute it once as it will be called 4 times
        if ((int) playerID != 0) return;
        if ((int) limbID != 0) return;
        
        UICanvaState = UIStateEnum.Start;
        tutorialCharacterCage.SetActive(false);
    }

    public void PlayerHaveReachedTheEnd()
    {
        //player has passed the trigger box at the end of the level, now it has to seat and fire fruits on the demon
    }

    public void LoadLevel()
    {
        if (GameObject.FindGameObjectWithTag("LevelContainer"))
            Destroy(GameObject.FindGameObjectWithTag("LevelContainer"));
        GameObject go = Instantiate(level, transform.position, Quaternion.identity);
    }
}