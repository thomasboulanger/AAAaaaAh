//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// This script receive player's input and move its dedicated sphere which move the limb attached to it (and do some other stuffs about grab etc)
/// </summary>
public class LimbController : MonoBehaviour
{
    private static bool[] _tutorialBlocksGrabbed = new bool[4];
    private static Renderer[] _tutorialBlock = new Renderer[4];

    [SerializeField] private GameEvent onLimbGrabEvent;
    [SerializeField] private GameEvent onLimbGrabShaderEvent;
    [SerializeField] private GameEvent onLimbGrabSoundEvent;
    [SerializeField] private GameEvent onLimbGrabValueEvent;
    [SerializeField] private GameEvent onFirstTutorialPartAchieved;

    [Header("The ID of limb that player with same ID will control")]
    [SerializeField] private int playerID;
    [SerializeField] private int limbID;
    [Space] [Header("The range that the player can move his limb")] 
    [SerializeField] private float limbLength;
    [SerializeField] private bool limbIsLeg;
    [SerializeField] private float limbSpeed = 10000;
    [SerializeField] private float playerInputSpeedDivider = 100;
    [SerializeField] private LayerMask grabInteractableLayerMask;
    [SerializeField] private float grabRadius;
    [SerializeField] private TMP_Text tutorialBlocksGrabCountText;

    private Transform _limbCenterTransform;
    private Vector3 _moveValue = Vector3.zero;
    private Vector2 _limbVector = Vector2.zero;
    private bool _isGrabbing;
    private bool _isGrabbingEnvironment;
    private bool _isGrabbingFruit;
    private Rigidbody _rb;
    private Vector3 _stackableMoveValue;
    private bool _triggerGrabOnce;
    private Vector3 _initialPos;
    private GameObject _fruit;
    private Color _tutorialBlockColor;
    private bool _triggerSetInitialPosOnce;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _initialPos = transform.position;
        tutorialBlocksGrabCountText.gameObject.SetActive(false);
    }

    private void Start()
    {
        CharacterBodyPhysic.LimbsTransforms[limbID] = transform;
        CharacterBodyPhysic.LimbsCenterTransforms[limbID] = _limbCenterTransform;
    }

    public void UpdatePlayerID(int playerID) => this.playerID = playerID;
    public void SetLimbActionRadius(Transform limbCenterTransform) => _limbCenterTransform = limbCenterTransform;

    public void UpdateLimbs(Component sender, object data1, object playerID, object inputID)
    {
        if (playerID is not int) return;
        if (inputID is not int) return;
        if (this.playerID != (int) playerID) return;
        if (limbID != (int) inputID) return;

        if (data1 is Vector2)
        {
            _limbVector = (Vector2) data1;
            _moveValue = new Vector3(_limbVector.x, _limbVector.y, 0) / playerInputSpeedDivider;
            Vector3 lookingDirection = (transform.position - _limbCenterTransform.position).normalized;

            if (_isGrabbingEnvironment)
            {
                if (!_rb.isKinematic) _rb.isKinematic = true;
                return;
            }

            //rotate the sphere that the limb on character rotate his dedicated limb in the right direction
            float angle;
            angle = Vector3.Angle(limbIsLeg ? Vector3.down : Vector3.right, lookingDirection);
            if (limbIsLeg
                    ? transform.position.x < _limbCenterTransform.position.x
                    : transform.position.y < _limbCenterTransform.position.y) angle = -angle;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            if (_rb.isKinematic) _rb.isKinematic = false;

            //stop the limb doing shit with his position at frame one
            if (!_triggerSetInitialPosOnce)
            {
                _stackableMoveValue = _initialPos;
                _triggerSetInitialPosOnce = true;
            }
            
            _stackableMoveValue += _moveValue;
            _stackableMoveValue = Vector3.ClampMagnitude(_stackableMoveValue, limbLength);
            Vector3 localLimbPos = _stackableMoveValue + _limbCenterTransform.position;

            //move transform with the player input value
            Vector3 goalPos = (localLimbPos - transform.position) * limbSpeed * Time.deltaTime;
            _rb.AddForce(goalPos);
        }
        else if (data1 is float)
        {
            _isGrabbing = (float) data1 > .9f;
            onLimbGrabValueEvent.Raise(this, (float) data1, this.playerID, limbID);

            if (!_isGrabbing)
            {
                //was grabbing a fruit
                if (_isGrabbingFruit) _fruit.transform.GetComponentInParent<FruitSelector>().ReleaseFruit();
                
                //was grabbing environment (check if the environment Obj was a tutorial block)
                if (_tutorialBlocksGrabbed[limbID])
                {
                    _tutorialBlock[limbID].material.color = _tutorialBlockColor;
                    _tutorialBlocksGrabbed[limbID] = false;
                }

                onLimbGrabShaderEvent.Raise(this, false, (float) data1, null);

                _triggerGrabOnce = false;
                _isGrabbingEnvironment = false;
                _isGrabbingFruit = false;
            }
            else if (!_triggerGrabOnce)
            {
                _triggerGrabOnce = true;
                Collider[] objectsInRange =
                    Physics.OverlapSphere(transform.position, grabRadius, grabInteractableLayerMask);

                if (objectsInRange.Length < 1) return;
                GameObject closestObj;
                if (objectsInRange.Length == 1)
                    closestObj = objectsInRange[0].gameObject;
                else
                    closestObj = objectsInRange
                        .OrderBy(element => (transform.position - element.transform.position).sqrMagnitude).First()
                        .gameObject;

                if (closestObj.CompareTag("Fruit"))
                {
                    _isGrabbingFruit = true;
                    _fruit = closestObj;
                    _fruit.GetComponentInParent<FruitSelector>().GrabFruit(gameObject);
                }
                else if (closestObj.CompareTag("Environment"))
                {
                    _isGrabbingEnvironment = true;
                    if (closestObj.name.Contains("Tutorial"))
                    {
                        _tutorialBlocksGrabbed[limbID] = true;
                        _tutorialBlock[limbID] = closestObj.GetComponent<Renderer>();
                        _tutorialBlockColor = _tutorialBlock[limbID].material.color;
                        _tutorialBlock[limbID].material.color = Color.green;
                    }
                }

                if (_isGrabbingFruit || _isGrabbingEnvironment)
                {
                    onLimbGrabShaderEvent.Raise(this, true, (float) data1, null);
                    onLimbGrabSoundEvent.Raise(this, _isGrabbingEnvironment ? 0 : _isGrabbingFruit ? 1 : 2, limbID,
                        null);
                }
            }

            onLimbGrabEvent.Raise(this, _isGrabbingEnvironment, this.playerID, limbID);
            onLimbGrabEvent.Raise(this, _moveValue, this.playerID, limbID);
        }
    }

    public void OnOverrideGrab(Component sender, object data, object unUsed2, object unUsed3)
    {
        // if(data is not bool) return;
        // _isGrabbing = (bool) data;
        // onLimbGrabEvent.Raise(this, _isGrabbing, playerID, limbID);
    }
    
    private void LateUpdate()
    {
        //if the position is out of the limb radius, clamp the transform to the limb radius
        if (Mathf.Abs((transform.position - _limbCenterTransform.position).sqrMagnitude) > limbLength * limbLength)
        {
            Vector3 direction = (transform.position - _limbCenterTransform.position).normalized;
            transform.position = _limbCenterTransform.position + direction * limbLength;
        }

        if(GameManager.UICanvaState != GameManager.UIStateEnum.PreStart) return;
        CheckForAllTutorialBlockGrabbed();
    }

    private void CheckForAllTutorialBlockGrabbed()
    {
        bool tmpComparator = false;
        int counter = _tutorialBlocksGrabbed.Count(element => element);

        foreach (bool element in _tutorialBlocksGrabbed)
            if (!element)
                tmpComparator = true;

        tutorialBlocksGrabCountText.gameObject.SetActive(true);
        tutorialBlocksGrabCountText.text = counter + "/4";

        if (tmpComparator) return;
        tutorialBlocksGrabCountText.gameObject.SetActive(false);
        for (int i = 0; i < _tutorialBlock.Length; i++)
        {
            _tutorialBlock[i].material.color = _tutorialBlockColor;
            _tutorialBlock[i].gameObject.SetActive(false);
            _tutorialBlocksGrabbed[i] = false;
        }
        onFirstTutorialPartAchieved.Raise(this,null,playerID,limbID);
    }
}