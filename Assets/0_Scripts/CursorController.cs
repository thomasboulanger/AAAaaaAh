//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// This script placed on "cursor" are used by player 1 to move cursor and interact with UI objects
/// </summary>
public class CursorController : MonoBehaviour
{
    [SerializeField] private GameEvent onPlayerThrowTruelleEvent;
    [SerializeField] private int cursorID;
    [SerializeField] private int cursorCanvasState;
    [SerializeField] private UIHommingTruelle truelleUIPrefab;


    private const float Speed = 5;
    private bool _inputPressed;
    private bool _triggerOnce;
    private float _objectWidth;
    private float _objectHeight;
    private Camera _mainCamera;
    private Vector4 _screenBounds;
    
    //truelle part
    private readonly float _unitSphereRandomRadius = 0.69f;
    private readonly float _startPosZ = -5;
    private readonly float _startPosRandom = 5;
    private Vector3 _truelleSpawnPosition;

    private void Start()
    {
        _mainCamera = Camera.main;
        _screenBounds = CalculateScreenBounds();
        _objectWidth = transform.localScale.x * GetComponent<SpriteRenderer>().bounds.extents.x;
        _objectHeight = transform.localScale.y * GetComponent<SpriteRenderer>().bounds.extents.y;
    }
    
    public void UpdateCursorPosition(Component sender, object data1, object playerID, object unUsed)
    {
        if((int)GameManager.UICanvaState != cursorCanvasState) return;
        if (playerID is not int) return;
        if (cursorID != (int) playerID) return;

        if (data1 is Vector2)
        {
            Vector2 limbVector = (Vector2) data1;
            Vector3 moveValue = new Vector3(limbVector.x, limbVector.y, 0);
            _screenBounds = CalculateScreenBounds();
            
            //clamp object to the size of the screen
            Vector3 clampedPosition = transform.position + moveValue * Time.deltaTime * Speed;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, _screenBounds.x, _screenBounds.y);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, _screenBounds.z, _screenBounds.w);
            transform.position = clampedPosition;
        }
        else if (data1 is float) _inputPressed = (float) data1 > .9f;
    }

    private void Update()
    {
        if((int)GameManager.UICanvaState != cursorCanvasState) return;
        if (!_inputPressed) _triggerOnce = false;
        if (!_inputPressed || _triggerOnce) return;

        _triggerOnce = true;
        onPlayerThrowTruelleEvent.Raise(this, null, null, null);

        //instantiate truelle and target our cursor
        Vector3 unitSphere = Random.insideUnitSphere * _unitSphereRandomRadius;
        _truelleSpawnPosition = transform.position + new Vector3
        (
            Random.Range(-_startPosRandom, _startPosRandom),
            Random.Range(-_startPosRandom, _startPosRandom),
            _startPosZ
        );

        UIHommingTruelle truelleGo = Instantiate(truelleUIPrefab, _truelleSpawnPosition, Quaternion.identity);
        truelleGo.Init(transform.position);
    }

    private Vector4 CalculateScreenBounds()
    {
        Vector4 bounds = new Vector4();
        
        float cameraDistance = transform.position.z - _mainCamera.transform.position.z;

        bounds.x = _mainCamera.ScreenToWorldPoint(new Vector3(0, 0, cameraDistance)).x + _objectWidth;
        bounds.y = _mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, cameraDistance)).x - _objectWidth;
        bounds.z = _mainCamera.ScreenToWorldPoint(new Vector3(0, 0, cameraDistance)).y + _objectHeight;
        bounds.w = _mainCamera.ScreenToWorldPoint(new Vector3(0, Screen.height, cameraDistance)).y - _objectHeight;

        return bounds;
    }
}