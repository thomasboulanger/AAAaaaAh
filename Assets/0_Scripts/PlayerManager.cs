//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Player part of the GameManager handling the spawn and the setup of players
/// </summary>
public class PlayerManager : MonoBehaviour
{
    public static List<PlayerInput> Players = new();

    public static bool AllLimbsAssigned;
    private static int[] _referenceTableLimbsToPlayerID = new int[4];
    private static int[] _referenceTableLimbsToInputID = new int[4];

    [SerializeField] private AudioManager audioManager;

    [Header("place here the scriptableObj of input event")] [SerializeField]
    private GameEvent playerInputUpdate;

    [SerializeField] private GameEvent onPlayerMoveCursor;
    [SerializeField] private GameEvent onPlayerJoinUiEvent;
    [SerializeField] private GameEvent onPlayerUpdateSingleCursor;
    [SerializeField] private GameEvent onPlayerReturnToLastCheckPoint;
    [SerializeField] private GameEvent onPlayerGrabAfterEndOfLevel;


    [Space] [Header("Name of Input Actions for Player Controller")] [SerializeField]
    private string moveLimbRight;

    [SerializeField] private string grabRight;
    [SerializeField] private string moveLimbLeft;
    [SerializeField] private string grabLeft;
    [SerializeField] private string colorChangeButtonRight;
    [SerializeField] private string colorChangeButtonLeft;
    [SerializeField] private GameObject[] limbControllerList;
    //[SerializeField] private TMP_Text[] readyWhenPlayerJoinTexts = new TMP_Text[4];

    private PlayerInputManager _playerInputManager;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        _playerInputManager = GetComponent<PlayerInputManager>();
        Init();
    }

    private void Init()
    {
        for (int i = 0; i < _referenceTableLimbsToPlayerID.Length; i++)
        {
            _referenceTableLimbsToPlayerID[i] = 5;
            _referenceTableLimbsToInputID[i] = 5;
        }

       // foreach (TMP_Text element in readyWhenPlayerJoinTexts) element.text = "";
        _playerInputManager.EnableJoining();

        //initialize / reset sounds
        audioManager.InitializeMenuSounds();
    }

    private void Update()
    {
        ////make player unable to join after this step
        //if (GameManager.UICanvaState != GameManager.UIStateEnum.PressStartToAddPlayers)
        //{
        //    if (_playerInputManager.joiningEnabled)
        //        _playerInputManager.DisableJoining();
        //}
        //else _playerInputManager.EnableJoining();
    }

    public void AddPlayer(PlayerInput player)
    {
        //add player to list of all players
        Players.Add(player);
        string Json = player.actions.ToJson();
        InputActionAsset inputActionAsset = ScriptableObject.CreateInstance<InputActionAsset>();
        inputActionAsset.LoadFromJson(Json);
        player.actions = inputActionAsset;

        //call an event to show player connected in UI
        onPlayerJoinUiEvent.Raise(this, Players.Count - 1, null, null);

        //add 2 script component to the player for input controls
        player.transform.AddComponent<PlayerInputsScript>();
        player.transform.AddComponent<PlayerInputsScript>();

        //set an array of those 2 components
        PlayerInputsScript[] playerInputArray = player.GetComponents<PlayerInputsScript>();

        //initialize the 2 different scripts with values
        playerInputArray[0].Init(moveLimbRight, grabRight, colorChangeButtonRight, Players.Count - 1, playerInputUpdate,
            onPlayerMoveCursor, onPlayerUpdateSingleCursor, onPlayerReturnToLastCheckPoint,onPlayerGrabAfterEndOfLevel);
        playerInputArray[1].Init(moveLimbLeft, grabLeft, colorChangeButtonLeft, Players.Count - 1, playerInputUpdate,
            onPlayerMoveCursor, onPlayerUpdateSingleCursor, onPlayerReturnToLastCheckPoint,onPlayerGrabAfterEndOfLevel);
        playerInputArray[0].AssignInputID(0);
        playerInputArray[1].AssignInputID(1);

        //set the text on UI when player has joined
        //readyWhenPlayerJoinTexts[Players.Count - 1].text = "Ready";
    }

    public void UpdateAssignedLimbs(Component sender, object limbIndex, object playerID, object inputID)
    {
        if (limbIndex is not int) return;
        if (playerID is not int) return;
        if (inputID is not int) return;

        bool tempBoolAllLimbsAreAssigned = true;

        _referenceTableLimbsToPlayerID[(int) limbIndex] = (int) playerID;
        _referenceTableLimbsToInputID[(int) limbIndex] = (int) inputID;

        foreach (int element in _referenceTableLimbsToPlayerID)
            if (element == 5)
                tempBoolAllLimbsAreAssigned = false;

        AllLimbsAssigned = tempBoolAllLimbsAreAssigned;
    }

    public void StartGame()
    {
        GameManager.InGame = true;
        GetComponent<GameManager>().LoadLevel();

        //iterate on through all players
        for (int j = 0; j < Players.Count; j++)
        {
            //get an array of the PlayerInputsScript components on them
            PlayerInputsScript[] playerInputArray = Players[j].GetComponents<PlayerInputsScript>();

            //iterate through _referenceTableLimbsToPlayerID to find which is equal to our current player
            for (int i = 0; i < _referenceTableLimbsToPlayerID.Length; i++)
            {
                if (_referenceTableLimbsToPlayerID[i] != j) continue;
                //get the script index (0 or 1) then assign the limb it control
                playerInputArray[_referenceTableLimbsToInputID[i]].AssignInputID(i);
                limbControllerList[i].GetComponent<LimbController>().UpdatePlayerID(j);
            }
        }

        //sound part setup
        audioManager.SetupLevelMucsic();
        //launch ambiant sounds
        audioManager.LaunchAmbianSounds();
    }

    //temp method for development only, to delete later
    public static void RestartLevel(int sceneIndex)
    {
        if (Players.Count > 0)
            foreach (PlayerInput player in Players)
                Destroy(player);
        Players.Clear();
        SceneManager.LoadScene(sceneIndex);

        //trash code
        //if(sceneIndex == 0)   Init();
    }

    private void OnEnable() => _playerInputManager.onPlayerJoined += AddPlayer;
    private void OnDisable() => _playerInputManager.onPlayerLeft -= AddPlayer;
}