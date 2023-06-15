//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This script receive and read inputs pressed by players then call events with the values
/// </summary>
public class PlayerInputsScript : MonoBehaviour
{
    private GameEvent onPlayerInputUpdate;
    private GameEvent onPlayerUpdateCursor;
    private GameEvent onPlayerUpdateSingleCursor;
    private GameEvent onPlayerGrabAfterEndOfLevel;
    
    //DEBUG ONLY
    private GameEvent onPlayerReturnToLastCheckPoint;

    //reference to the PlayerInput component
    private PlayerInput _playerInput;

    //information of player index and input script (this)
    private int _playerID;
    public int inputScriptID = 5;

    //dynamic name of input actions
    private string _moveInputStr;
    private string _grabInputStr;
    private string _colorButtonStr;

    //receiver variable of input actions
    private Vector2 _limbVector2D;
    private float _grabValue;
    private bool _colorChangeButton;
    
    private PlayerInputsScript[] _playerInputArray;
    
    private void Awake() => _playerInput = GetComponent<PlayerInput>();
    private void Start() => _playerInputArray = GetComponents<PlayerInputsScript>();
    
    private void Update()
    {
        //temp solution//
        if (GameManager.InGame && _playerInputArray[0].inputScriptID == _playerInputArray[1].inputScriptID)
            _playerInputArray[1].inputScriptID = 5;
        
        //get value of player inputs
        _limbVector2D = _playerInput.actions[_moveInputStr].ReadValue<Vector2>();
        _grabValue = _playerInput.actions[_grabInputStr].ReadValue<float>();
        if (_playerInput.actions[_colorButtonStr].WasPressedThisFrame()) _colorChangeButton = !_colorChangeButton;
        
        //check game state to know where to call event with player's inputs
        if (GameManager.InGame)
        {
            onPlayerInputUpdate.Raise(this, _limbVector2D, _playerID, inputScriptID);
            onPlayerInputUpdate.Raise(this, _grabValue, _playerID, inputScriptID);
        }
        else if(GameManager.UICanvaState == GameManager.UIStateEnum.Play)
        {
            onPlayerUpdateCursor.Raise(this, _limbVector2D, _playerID, inputScriptID);
            onPlayerUpdateCursor.Raise(this, _grabValue, _playerID, inputScriptID);
            onPlayerUpdateCursor.Raise(this, _colorChangeButton, _playerID, inputScriptID);
            
            //DEBUG ONLY
            if(_playerInput.actions["TmpRestart"].WasPressedThisFrame() && _playerInputArray[0] == this) 
                onPlayerReturnToLastCheckPoint.Raise(this,null,null,null);
        }
        else if(_playerID == 0 && this == _playerInputArray[0])
        {
            onPlayerUpdateSingleCursor.Raise(this, _limbVector2D, _playerID, null);
            onPlayerUpdateSingleCursor.Raise(this, _grabValue, _playerID, null);
        }
        else if (GameManager.UICanvaState == GameManager.UIStateEnum.PlayerHaveReachEndOfLevel)
            onPlayerGrabAfterEndOfLevel.Raise(this, _grabValue, _playerID, null);
        
    }

    public void Init(string moveInputStr, string grabInputStr, string colorChangeButtonStr, int playerID, GameEvent inputUpdateEvent,
        GameEvent playerMoveCursor, GameEvent playerUpdateSingleCursor, GameEvent playerReturnToLastCheckPoint, GameEvent PlayerGrabAfterEndOfLevel)
    {
        _moveInputStr = moveInputStr;
        _grabInputStr = grabInputStr;
        _playerID = playerID;
        onPlayerInputUpdate = inputUpdateEvent;
        onPlayerUpdateCursor = playerMoveCursor;
        onPlayerUpdateSingleCursor = playerUpdateSingleCursor;
        _colorButtonStr = colorChangeButtonStr;
        onPlayerReturnToLastCheckPoint = onPlayerReturnToLastCheckPoint;
        onPlayerGrabAfterEndOfLevel = PlayerGrabAfterEndOfLevel;
    }

    public void AssignInputID(int inputID) => inputScriptID = inputID;
}