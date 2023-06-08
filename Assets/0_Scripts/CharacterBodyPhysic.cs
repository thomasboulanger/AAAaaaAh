//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using System;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// This Script handle all the physic of the player character's body, independently of the rest ou the limbs
/// </summary>
public class CharacterBodyPhysic : MonoBehaviour
{
    public static Transform[] LimbsTransforms = new Transform[4];
    public static Transform[] LimbsCenterTransforms = new Transform[4];

    [SerializeField] private float force = 50;
    [SerializeField] private float range = 2;
    [SerializeField] private float xThreshold = 1;
    [SerializeField] private float GravityForce = -25;
    [SerializeField] private float yAxisMultiplier = 10;
    [SerializeField] private Transform[] virtualTransforms = new Transform[4];

    private RaycastHit _hit;
    private Rigidbody _charRb;
    private bool[] _limbsGrabbed = new bool[4];
    private bool[] _oldLimbsGrabbed = new bool[4];
    private Vector3[] _playersVectorInput = new Vector3[4];
    private Vector3[] _destinationTargetForBody = new Vector3[4];
    private bool _isFalling;
    private float _yAxisModifier = .6f;
    private float _goalAngle;
    private float _goalRotation;
    private Vector3 _gravityForce = -Vector3.up;
    private bool _hasInitializedCheckpointLogic;
    private float _lerpStrength = 4f;

    private void Awake() => _charRb = GetComponent<Rigidbody>();

    private void Start() => Init();

    private void Init()
    {
        _hasInitializedCheckpointLogic = false;
    }

    private void FixedUpdate()
    {
        //check the game state
        if (!GameManager.InGame) return;
        if (!_hasInitializedCheckpointLogic)
        {
            _hasInitializedCheckpointLogic = true;
            transform.GetComponent<CheckpointManager>().Init(LimbsTransforms, virtualTransforms);
        }

        //initialize variable
        Vector3 addForceToBody = Vector3.zero;
        int grabCount = _limbsGrabbed.Count(element => element);
        float deltatime = Time.deltaTime;

        //iterate on all limbs
        for (int i = 0; i < _limbsGrabbed.Length; i++)
        {
            //check if a limb is released (and was grab the last frame)
            if (!_limbsGrabbed[i] && _oldLimbsGrabbed[i])
                SetMedianDestionationTarget(i);

            //check if a limb wasn't grabbed the last frame and is now
            if (_limbsGrabbed[i] && !_oldLimbsGrabbed[i])
            {
                _playersVectorInput[i] = Vector3.zero;
                _destinationTargetForBody[i] = _charRb.position;
                virtualTransforms[i].position = LimbsTransforms[i].position;
                virtualTransforms[i].rotation = LimbsTransforms[i].rotation;
                virtualTransforms[i].localScale = LimbsTransforms[i].localScale;
            }
        }

        //iterate on all limbs if check if they are grabbing environement
        for (int i = 0; i < _limbsGrabbed.Length; i++)
        {
            //check if the current limb is higher of the center of our body
            bool isLimbUp = _charRb.position.y < LimbsTransforms[i].position.y;

            if (_limbsGrabbed[i])
            {
                //place the limb in his "lock" position in case it moved too far from his own range (in case of body rotation)
                LimbsTransforms[i].position = virtualTransforms[i].position;

                float xOffset = Mathf.Abs(LimbsTransforms[i].position.x - _charRb.position.x);
                if (grabCount == 1 && !isLimbUp)
                {
                    //passively move down when one limb is grab from 0.6 to 1
                    float yAxisModifierNormalized = (xOffset - _yAxisModifier) / (xThreshold - _yAxisModifier);
                    yAxisModifierNormalized = Mathf.Clamp(yAxisModifierNormalized, 0, 1);
                    yAxisModifierNormalized *= yAxisMultiplier;
                    _destinationTargetForBody[i] += Vector3.down * yAxisModifierNormalized;
                }

                //calculate, clamp and apply a force on the body from the player input
                _destinationTargetForBody[i] += _playersVectorInput[i];

                _destinationTargetForBody[i] = virtualTransforms[i].InverseTransformPoint(_destinationTargetForBody[i]);
                _destinationTargetForBody[i] = Vector3.ClampMagnitude(_destinationTargetForBody[i], range);
                _destinationTargetForBody[i] = virtualTransforms[i].TransformPoint(_destinationTargetForBody[i]);

                //check if several limbs are grabbed at once to clamp movement applied
                for (int j = 0; j < _limbsGrabbed.Length; j++)
                {
                    if (!_limbsGrabbed[j] || i == j) continue;
                    _destinationTargetForBody[i] =
                        virtualTransforms[j].InverseTransformPoint(_destinationTargetForBody[i]);
                    _destinationTargetForBody[i] = Vector3.ClampMagnitude(_destinationTargetForBody[i], range);
                    _destinationTargetForBody[i] = virtualTransforms[j].TransformPoint(_destinationTargetForBody[i]);
                }

                //generate force to add to rigidbody
                Vector3 addedForce = (_destinationTargetForBody[i] - LimbsCenterTransforms[i].position) * force;

                //adding the force to a sum
                addForceToBody += addedForce;
            }
        }

        //adding gravity to the force when condition are met and freeze/unfreeze Z rotation of character
        if (grabCount == 0)
        {
            _charRb.constraints = RigidbodyConstraints.FreezePositionZ |
                                  RigidbodyConstraints.FreezeRotationX |
                                  RigidbodyConstraints.FreezeRotationY;
            if (_gravityForce == Vector3.zero) _gravityForce = -Vector3.up;
            _gravityForce += new Vector3(0, GravityForce * Time.deltaTime, 0);
        }
        else
        {
            _charRb.constraints = RigidbodyConstraints.FreezePositionZ |
                                  RigidbodyConstraints.FreezeRotationX |
                                  RigidbodyConstraints.FreezeRotationY |
                                  RigidbodyConstraints.FreezeRotationZ;
            _gravityForce = Vector3.zero;
        }

        //applying the final force to the rigidbody
        _charRb.AddForce(addForceToBody);
        _charRb.AddForce(_gravityForce, ForceMode.Acceleration);

        //update last frame array
        for (int i = 0; i < _limbsGrabbed.Length; i++)
            _oldLimbsGrabbed[i] = _limbsGrabbed[i];


        Vector3 medianDirection = GetDirection();

        // Vector3 targetDirection = medianDirection - transform.position;
        // // Calculate the angle between the two vectors
        // float angle = Vector3.Angle(new Vector3(0f,180f,transform.position.z), targetDirection);
        //
        // if (grabCount < 1) return;
        // // Lerp towards the target rotation
        // transform.rotation = Quaternion.Lerp(transform.rotation,  Quaternion.Euler(targetDirection), 1f * Time.deltaTime);
        // //transform.rotation = Quaternion.Euler(0f,180f, transform.rotation.z/*Mathf.Clamp(angle, -75, 75)*/);


        Vector3 lookingDirection = (transform.position - medianDirection).normalized;
        float angle = Vector3.Angle(Vector3.right, lookingDirection);
        if (transform.position.y > medianDirection.y) angle = -angle;
        angle += 90;
        if (angle > 90) angle -= 90 - angle;
        if (angle < -90) angle += Mathf.Abs(angle) - 90;
        Quaternion targetRotation = Quaternion.Euler(0, 180, angle);
        if (grabCount < 1) return;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _lerpStrength * deltatime);

        // //handle body rotation
        // if (grabCount < 1) return;
        //
        // _goalRotation = Vector3.Angle(transform.position, sumOfBodyRotationValues / grabCount);
        // _directionAngle = Mathf.Lerp(_directionAngle, _goalRotation, .4f);
        //
        // transform.rotation = Quaternion.Euler(0, -180, Mathf.Clamp(_directionAngle, -75, 75));
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = Vector3.zero;
        int count = 0;
        for (int i = 0; i < _limbsGrabbed.Length; i++)
        {
            if (!_limbsGrabbed[i]) continue;
            direction += virtualTransforms[i].position;
            count++;
        }

        if (count == 0) return transform.position;
        return direction / count;
    }

    private void SetMedianDestionationTarget(int lastFrameGrabLimbIndex)
    {
        Vector3 moyenne = Vector3.zero;
        moyenne += _destinationTargetForBody[lastFrameGrabLimbIndex];
        int counter = 1;

        for (int i = 0; i < _limbsGrabbed.Length; i++)
        {
            if (_limbsGrabbed[i])
            {
                moyenne += _destinationTargetForBody[i];
                counter++;
            }
        }

        moyenne /= counter;
        for (int i = 0; i < _destinationTargetForBody.Length; i++)
            _destinationTargetForBody[i] = moyenne;
    }

    //event that receive all information from players inputs
    public void UpdateLimbsGrab(Component sender, object data1, object playerID, object limbID)
    {
        if (playerID is not int) return;
        if (limbID is not int) return;
        int currentLimbID = (int) limbID;

        if (data1 is Vector3)
            _playersVectorInput[currentLimbID] = (Vector3) data1;
        else if (data1 is bool)
            _limbsGrabbed[currentLimbID] = (bool) data1;
        else
            Debug.LogError("type mismatch in UpdateLimbsGrab " + data1);
    }
}