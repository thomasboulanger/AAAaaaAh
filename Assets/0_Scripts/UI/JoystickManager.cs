//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

/// <summary>
/// This script is used to handle all the interaction with the cursor (lock/unlock, change limb color, throw truelles etc..)
/// </summary>
public class JoystickManager : MonoBehaviour
{
    [SerializeField] private GameEvent onPlayerAssignLimb;
    [SerializeField] private GameEvent onCheckCursorOverrideLimbSelector;

    [SerializeField] public GameObject background;
    [SerializeField] private TextMeshPro RLText;
    [SerializeField] private TextMeshPro playerIdText;
    [SerializeField] private HommingTruelle truelle;
    [HideInInspector] public ControlerIconManager controllerRef;

    [SerializeField] private bool isLocked;
    [SerializeField] private Transform blackStick;
    [SerializeField] private List<Color> colorsReference = new();
    [Range(.1f, 100f)] [SerializeField] private float unitSphereRandomRadius = 0.69f;
    [SerializeField] private float startPosZ = -5;
    [SerializeField] private float startPosRandom = 7.41f;
    [SerializeField] private GameObject cadenaGo;
    [SerializeField] private float rotationLerpSpeed = 5;

    [SerializeField] private int cursorID;
    [SerializeField] private int inputID;
    [SerializeField] private int cursorCanvasState;

    [SerializeField] private float speedLerp = 2f;
    [SerializeField] private AnimationCurve animationCurveSnap;


    private MeshRenderer _backgroundMeshRenderer;
    private Material _material;
    private Rigidbody _cadenasRb;
    private Rigidbody _rb;
    private bool _colorChange;
    private Vector3 _lastMousePos;
    private Vector3 _pos;
    private Animator _joystickAnim;
    private Vector3 _currentMoveMouseDelta;
    private Color _actualColor;
    private Vector3 _truelleTargetPoint;
    private Vector3 _truelleApparitionPoint;
    private static int[] _colorsUsedByPLayers = new int[4];

    private const float Speed = 5;
    private bool _colliding;
    private bool _inputTrigger;
    private bool _triggerOnce;
    private bool _isLimbAssigned;
    private GameObject _collidingObjRef;
    private GameObject _assignedObjRef;

    private float _objectWidth;
    private float _objectHeight;
    private Camera _mainCamera;
    private Vector4 _screenBounds;

    private float _tLerp = 1.2f;
    private Vector3 _targetPos;
    private Vector3 _basePos;

    public void SetIDs(int playerID, int inputID)
    {
        cursorID = playerID;
        this.inputID = inputID;
    }

    public void UpdateCursorPosition(Component sender, object data1, object playerID, object limbID)
    {
        if(cursorCanvasState != GameManager.UICanvaState) return;
        if (playerID is not int) return;
        if (limbID is not int) return;
        if (cursorID != (int) playerID) return;
        if (inputID != (int) limbID) return;

        if (data1 is Vector2)
        {
            Vector2 limbVector = (Vector2) data1;
            Vector3 moveValue = new Vector3(limbVector.x, limbVector.y, 0);

            if (!_colorChange && !isLocked)
            {
                _screenBounds = CalculateScreenBounds();

                //clamp object to the size of the screen
                Vector3 clampedPosition = transform.position + moveValue * Time.deltaTime * Speed;
                clampedPosition.x = Mathf.Clamp(clampedPosition.x, _screenBounds.x, _screenBounds.y);
                clampedPosition.y = Mathf.Clamp(clampedPosition.y, _screenBounds.z, _screenBounds.w);
                transform.position = clampedPosition;
            }
            else if (_colorChange) ColorSelect(moveValue); //move the pointer on color wheel
        }
        else if (data1 is float)
        {
            _inputTrigger = (float) data1 > .9f;
            if ((float) data1 < .1f) _triggerOnce = true;
        }
        else if (data1 is bool)
        {
            _colorChange = (bool) data1;
            _joystickAnim.SetBool("isActive", _colorChange);
        }
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

    void Start()
    {
        _cadenasRb = cadenaGo.GetComponent<Rigidbody>();
        _joystickAnim = GetComponent<Animator>();
        _backgroundMeshRenderer = background.GetComponent<MeshRenderer>();
        _backgroundMeshRenderer.sharedMaterial = new Material(_backgroundMeshRenderer.sharedMaterial);
        _material = _backgroundMeshRenderer.sharedMaterial;
        _rb = GetComponent<Rigidbody>();

        _mainCamera = Camera.main;
        _screenBounds = CalculateScreenBounds();
        _objectWidth = transform.localScale.x * _backgroundMeshRenderer.bounds.extents.x;
        _objectHeight = transform.localScale.y * _backgroundMeshRenderer.bounds.extents.y;

        _colorsUsedByPLayers[cursorID] = cursorID * 3;
        SetPlayerColor(colorsReference[cursorID * 3], false);
        SetSide();
    }

    private void Update()
    {
        if (_tLerp < 1.1f && isLocked) Transition();

        if(cursorCanvasState != GameManager.UICanvaState) return;
        if (_inputTrigger && _triggerOnce)
        {
            _triggerOnce = false;

            //instantiate truelle and target our cursor
            Vector3 unitSphere = Random.insideUnitSphere * unitSphereRandomRadius;

            _truelleTargetPoint = new Vector3(unitSphere.x, unitSphere.y, 0);

            _truelleApparitionPoint = transform.position + new Vector3(Random.Range(-startPosRandom, startPosRandom),
                Random.Range(-startPosRandom, startPosRandom), startPosZ);

            HommingTruelle truelleObj = Instantiate(truelle, _truelleApparitionPoint, Quaternion.identity);

            truelleObj.Init(_truelleTargetPoint + transform.position, _actualColor);
        }
    }

    private void Transition()
    {
       transform.position = Vector3.LerpUnclamped(_basePos, _targetPos, animationCurveSnap.Evaluate(_tLerp));
       _tLerp += Time.deltaTime * speedLerp;
        if (_tLerp >= 1f) transform.position = _targetPos;
    }


    private void FixedUpdate()
    {
        if(cursorCanvasState != GameManager.UICanvaState) return;
        if (_rb.angularVelocity.magnitude < 2f)
            _rb.rotation = Quaternion.Lerp(_rb.rotation, Quaternion.Euler(Vector3.zero),
                Time.deltaTime * rotationLerpSpeed);
    }

    public void UpdateLimbSelectionedList(Component sender, object playerIDToCompare, object goOverrided,
        object inputIndex)
    {
        if (playerIDToCompare is not int) return;
        if (goOverrided is not GameObject) return;
        if (inputIndex is not int) return;
        if (cursorID != (int) playerIDToCompare) return;

        if (_assignedObjRef == (GameObject) goOverrided)
            ResetObjAssigned((GameObject) goOverrided);
    }

    private void ResetObjAssigned(GameObject objToReplace)
    {
        controllerRef.ResetOutline(objToReplace, Color.white);
        onPlayerAssignLimb.Raise(this, objToReplace.GetComponent<LimbSelectorCall>().currentLimbIndex, 5, 5);
        objToReplace.GetComponent<LimbSelectorCall>().currentPlayerIDAssigned = 5;
        objToReplace.GetComponent<MeshRenderer>().material.SetColor("_BCTint", Color.white);
    }

    private void AssignOneLimb(int currentLimbID, LimbSelectorCall currentObj)
    {
        onPlayerAssignLimb.Raise(this, currentLimbID, cursorID, inputID);
        currentObj.currentPlayerIDAssigned = cursorID;
        currentObj.GetComponent<MeshRenderer>().material.SetColor("_BCTint", _actualColor);

        //set joystick position to the center of the check box (init timer)
        _basePos = transform.position;
        _targetPos = new Vector3(currentObj.transform.position.x, currentObj.transform.position.y,transform.position.z);
        _tLerp = 0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("LimbSelectorBox")) return;
        _colliding = true;
        _collidingObjRef = collision.gameObject;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.transform.CompareTag("LimbSelectorBox")) return;
        _colliding = false;
        _collidingObjRef = null;
    }

    public void HitByTruelle(HommingTruelle attacker)
    {
        _rb.angularVelocity *= attacker.GetComponent<Rigidbody>().velocity.magnitude;
        isLocked = !isLocked;
        _cadenasRb.isKinematic = !isLocked;
        _joystickAnim.SetBool("status", isLocked);

        attacker.InitiateDiscontrol();

        if (!_colliding) return;

        if (isLocked)
        {
            LimbSelectorCall currentObj = _collidingObjRef.GetComponent<LimbSelectorCall>();

            //check if a selected limb is already selected by another player
            if (currentObj.currentPlayerIDAssigned != 5)
                onCheckCursorOverrideLimbSelector.Raise(this, currentObj.currentPlayerIDAssigned, _collidingObjRef,
                    inputID);

            //check if a limb is already assigned of that controller
            if (_isLimbAssigned) ResetObjAssigned(_assignedObjRef);

            //assign the selected limb to the current player
            AssignOneLimb(currentObj.currentLimbIndex, currentObj);
            _assignedObjRef = _collidingObjRef;
            _isLimbAssigned = true;
            SetPlayerColor(_actualColor, false);
        }
        else ResetObjAssigned(_collidingObjRef);
    }

    public void SetSide()
    {
        playerIdText.text = (cursorID + 1).ToString();
        float angle;

        if (inputID == 0)
        {
            angle = 90f;
            RLText.text = "R";
        }
        else
        {
            angle = -90f;
            RLText.text = "L";
        }

        background.transform.rotation = Quaternion.Euler(new Vector3(background.transform.rotation.eulerAngles.x,
            background.transform.rotation.eulerAngles.y, angle));
    }

    public void SetPlayerColor(Color col, bool casted)
    {
        _actualColor = col;
        if (_material) _material.SetColor("_Color", col);
        if (controllerRef != null && !casted) controllerRef.SetPlayerColor(col, this);
    }

    void ColorSelect(Vector3 joystickValue)
    {
        int index;

        _pos += joystickValue;
        _pos = transform.InverseTransformPoint(_pos);
        _pos = Vector3.ClampMagnitude(_pos, 1f);
        _pos += transform.position;

        if (joystickValue.magnitude > 0)
        {
            float angle = Vector3.SignedAngle(Vector3.up, _pos - transform.position, Vector3.back);
            Quaternion rot = blackStick.transform.rotation;
            blackStick.transform.rotation = Quaternion.Euler(rot.eulerAngles.x, rot.eulerAngles.y, -angle);

            if (angle < 0) angle = 360 + angle; //passer de 180/-180 a 0/360
            index = Mathf.RoundToInt(angle / 30);
            if (index == 0) index = 1; //pour eviter le out of bounds
            SetPlayerColor(colorsReference[index - 1], false);
        }
    }
}