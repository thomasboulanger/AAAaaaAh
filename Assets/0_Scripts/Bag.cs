//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains all the logic of the bag, the grab logic, the fruit storing etc...
/// </summary>
public class Bag : MonoBehaviour
{
    // [Header("Wiggle variables part")] [SerializeField]
    // private GameEvent onWiggleLimb;

    [SerializeField] private Material characterMaterialForFall;
    [SerializeField] private string[] limbsMatName = new string[4];
    [SerializeField] private List<GameObject> fruitsStoredList;
    [SerializeField] private float limbLength = 1.1f;
    [SerializeField] private float gravityForce = 50;

    private Rigidbody _rb;
    private FixedJoint _fixedJoint;
    private float _currentHoldingTimer;
    private static LimbController[] _limbControllers = new LimbController[4];
    private static Transform[] _limbsCenterTransforms = new Transform[4];
    private Vector3[] _inputVectorOnGrab = new Vector3[4];
    private bool[] _limbsGrabbingBag = new bool[4];
    private float[] _wiggleLimbsValue = new float[4];
    private float[] _offsetDistance = new float[4];
    private Vector3[] _offsetVector = new Vector3[4];
    private bool _isBagGrabbed;
    private int _limbGrabNumber;
    private Quaternion _currentRot;
    private Vector3 _gravityForce = Vector3.zero;

    private void Awake() => _rb = GetComponent<Rigidbody>();

    private void Update()
    {
        if (!GameManager.InGame) return;


        float deltaTime = Time.deltaTime;
        Vector3 goalPosition = Vector3.zero;

        for (int i = 0; i < _limbsGrabbingBag.Length; i++)
        {
            if (!_limbsGrabbingBag[i]) _wiggleLimbsValue[i] -= .1f * deltaTime;
            else
            {
                goalPosition += _inputVectorOnGrab[i];
                _wiggleLimbsValue[i] += .1f * deltaTime;
            }

            _wiggleLimbsValue[i] = Mathf.Clamp(_wiggleLimbsValue[i], 0, 1);
            characterMaterialForFall.SetFloat(limbsMatName[i], _wiggleLimbsValue[i]);

            //onWiggleLimb.Raise(this, _wiggleLimbsValue[i], null, null);
            //onWiggleLimb.Raise(this, 0f, null, null);
        }


        if (_limbGrabNumber < 1)
        {
            _gravityForce += new Vector3(0, -gravityForce * Time.deltaTime, 0);
            _rb.AddForce(_gravityForce, ForceMode.Acceleration);
            return;
        }

        _gravityForce = Vector3.zero;

        goalPosition /= _limbGrabNumber;
        _rb.AddForce(goalPosition);
    }

    private void LateUpdate()
    {
        if (!GameManager.InGame) return;
        if (_limbGrabNumber < 1) return;
        //if the position is out of the farest limb radius, clamp the transform to the limb radius
        Transform limbCenterTransform = null;
        float tmpVec = float.MaxValue;
        int index = 0;

        //return the farrest limb center
        for (int i = 0; i < _limbsGrabbingBag.Length; i++)
        {
            if (!_limbsGrabbingBag[i]) continue;
            if (!(Vector3.Distance(_limbsCenterTransforms[i].position, _limbControllers[i].transform.position) <
                  tmpVec)) continue;

            tmpVec = Vector3.Distance(_limbsCenterTransforms[i].position, _limbControllers[i].transform.position);
            limbCenterTransform = _limbsCenterTransforms[i];
            index = i;
        }

        if (Vector3.Distance(transform.position, limbCenterTransform.position) > limbLength + _offsetDistance[index])
        {
            Vector3 direction = (transform.position - limbCenterTransform.position).normalized;
            transform.position = limbCenterTransform.position + direction * (limbLength + _offsetDistance[index]);
        }
    }

    public void OnGrabBag(int limbID)
    {
        _limbsGrabbingBag[limbID] = true;
        _limbGrabNumber++;
        _offsetDistance[limbID] = Vector3.Distance(transform.position, _limbControllers[limbID].transform.position);
        _offsetVector[limbID] = _limbControllers[limbID].transform.position - transform.position;
        if (_limbGrabNumber != 1) return;

        _currentRot = transform.rotation;
    }

    public void ReleaseBag(int limbID)
    {
        _limbsGrabbingBag[limbID] = false;
        _limbGrabNumber--;
        if (_limbGrabNumber > 0) return;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.CompareTag("Fruit")) return;
        FruitSelector fruit = other.transform.GetComponentInParent<FruitSelector>();
        if (fruit.IsFruitGrabbed()) fruit.ReleaseFruit(false);
        //fruit.FruitStoredInBag(gameObject);
        fruitsStoredList.Add(other.gameObject);
    }

    public void SetPositionValue(Vector3 moveValue, int limbID) =>
        _inputVectorOnGrab[limbID] = moveValue;

    public void SetLimbAndCenterTransforms(Transform limbCenterTransform, LimbController limbController, int limbID)
    {
        _limbsCenterTransforms[limbID] = limbCenterTransform;
        _limbControllers[limbID] = limbController;
    }
}