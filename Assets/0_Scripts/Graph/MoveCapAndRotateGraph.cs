using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCapAndRotateGraph : MonoBehaviour
{
    [SerializeField] private Transform tiltTransform;
    public Transform rangeCenter;
    [SerializeField] private float range;
    [SerializeField] private float rangeGrab;

    //body
    [SerializeField] public Rigidbody _bodyRB;
    [SerializeField] private Rigidbody _selfRB;
    public bool isRelativeToBody;
    [SerializeField] private float _force = 10f;

    public Vector3 targetPos;
    public Vector3 targetPosChild;
    public Vector3 targetPosBody;

    private Vector3 _lastMousePos;
    private Vector3 _lastMousePosBody;

    [SerializeField] private float movementPower = 1f;
    [Range(0, 3)]
    [SerializeField] public int member;
    //[SerializeField] private IKControlGraphPasMain _ikMain;
    private int memberActual;

    public bool isGrabbingStatic;
    public bool isGrabbingMovable;

    [SerializeField]private bool _canGrabMovable;

    private MovableObject movableRef;

    public Vector3 addForceToBody;
    
    [HideInInspector] public Vector3 grabOffset;

    public LayerMask player;
    public LayerMask grabLayer;



    // Start is called before the first frame update
    void Start()
    {
        targetPos = transform.position;
        _lastMousePos = Input.mousePosition;
        _lastMousePosBody = Input.mousePosition;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //Vector3 posOffset = tiltTransform.TransformPoint(_basePos -tiltTransform.position);

        Vector3 inputWithTilt = targetPos;


        if (isGrabbingStatic)
        {
            inputWithTilt = targetPosBody;
            //_bodyRB.AddForce()
            //position centrale avec moyenne tt les membres
            Vector3 addedForce = (inputWithTilt - _bodyRB.position) * _force;


            addForceToBody = addedForce;

        }
        else
        {
            if (isRelativeToBody)
            {
                transform.position = new Vector3(inputWithTilt.x, inputWithTilt.y, transform.position.z) + new Vector3(rangeCenter.position.x, rangeCenter.position.y, 0);
            }
            else
            {
                transform.position = new Vector3(inputWithTilt.x, inputWithTilt.y, transform.position.z);
            }


        }


    }

    private void Update()
    {

        if (!Input.GetKey(KeyCode.Mouse0) && member == memberActual)
        {
            _lastMousePos = Input.mousePosition;
            _lastMousePosBody = Input.mousePosition;

        }


        if (Input.GetKeyDown(KeyCode.Keypad7)) memberActual = 0;
        if (Input.GetKeyDown(KeyCode.Keypad9)) memberActual = 1;
        if (Input.GetKeyDown(KeyCode.Keypad1)) memberActual = 2;
        if (Input.GetKeyDown(KeyCode.Keypad3)) memberActual = 3;


        if (Input.GetKeyDown(KeyCode.Mouse1) && member == memberActual)
        {
            if ((_canGrabMovable || isGrabbingMovable) && !isGrabbingStatic) //m
            {
                movableRef.Grab(_selfRB, isGrabbingMovable, this);
                if (!isGrabbingMovable)
                {
                    _selfRB.gameObject.layer = LayerMask.NameToLayer("Grabbing");
                }
                else
                {
                    _selfRB.gameObject.layer = LayerMask.NameToLayer("Player");
                }

                isGrabbingMovable = !isGrabbingMovable;
                return;
            }

            if (isGrabbingStatic)
            {
                Debug.Log("pafMoyenneRun");
                //_ikMain.AssignMoyenne(this);//eviter de jump quand on degrab
            }
            else
            {
                _bodyRB.useGravity = false;
                targetPosBody = _bodyRB.position;
            }

            isGrabbingStatic = !isGrabbingStatic;
            _lastMousePos = Input.mousePosition;
            _lastMousePosBody = Input.mousePosition;

        }

        if (!Input.GetKey(KeyCode.Mouse0) || member != memberActual)
        {
            Range();
            return;
        }

        MoveTargetPos(Vector3.one);

        Range();


    }

    void MoveTargetPos(Vector3 constraint)
    {
        if (isGrabbingStatic)
        {
            Vector3 addedPos = (Input.mousePosition - _lastMousePosBody) * movementPower * Time.deltaTime;
            addedPos = new Vector3(addedPos.x * constraint.x, addedPos.y * constraint.y, addedPos.z * constraint.z);
            targetPosBody += addedPos;

            _lastMousePosBody = Input.mousePosition;

        }
        else
        {
            Vector3 addedPos = (Input.mousePosition - _lastMousePos) * movementPower * Time.deltaTime + grabOffset;
            addedPos = new Vector3(addedPos.x * constraint.x, addedPos.y * constraint.y, addedPos.z * constraint.z);

            targetPos += addedPos;

            _lastMousePos = Input.mousePosition;
        }
    }

    public void TryToMoveConstrained(Vector3 constraint, List<int> members)
    {
        if (!members.Contains(memberActual)) return;
        if (member != memberActual && Input.GetKey(KeyCode.Mouse0))//c'est pas le membre (eviter de bouger 2 fois)
        {
            MoveTargetPos(constraint);
        }
    }


    void Range()
    {
        if (!isGrabbingStatic)
        {
            if (isRelativeToBody)
            {
                targetPos = Vector3.ClampMagnitude(targetPos, range); // comme c'est une pos relative osef
            }
            else
            {
                Vector3 relativeTargetPos;

                relativeTargetPos = rangeCenter.InverseTransformPoint(targetPos); // pos non relative, on la rend relative on clamp et on remet en world

                relativeTargetPos = Vector3.ClampMagnitude(relativeTargetPos, range);

                targetPos = rangeCenter.TransformPoint(relativeTargetPos);
            }
        }
        else
        {
            Vector3 relativeTargetPos;//pareil que au desus mais pour la val body quand on est grab

            relativeTargetPos = transform.InverseTransformPoint(targetPosBody);

            relativeTargetPos = Vector3.ClampMagnitude(relativeTargetPos, rangeGrab);

            targetPosBody = transform.TransformPoint(relativeTargetPos);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        MovableObject tempMO;
        other.TryGetComponent<MovableObject>(out tempMO);
        if (tempMO == null)
        {
            if (other.transform.parent!=null)
            {
                other.transform.parent.TryGetComponent<MovableObject>(out tempMO);
            }
        }

        if (tempMO != null)
        {
            movableRef = tempMO;
            _canGrabMovable = true;
        }

        //rajouter feedback de grab dispo ?

    }

    private void OnTriggerExit(Collider other)
    {
        MovableObject tempMO;
        other.TryGetComponent<MovableObject>(out tempMO);

        if (tempMO != null)
        {
            movableRef = tempMO;
            _canGrabMovable = false;
        }
        else
        {
            if (other.transform.parent != null)
            {
                other.transform.parent.TryGetComponent<MovableObject>(out tempMO);
                if (tempMO != null)
                {
                    movableRef = tempMO;
                    _canGrabMovable = false;
                }
            }
        }
    }

}
