using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : MonoBehaviour
{
    public List<FixedJoint> fixedJoints = new List<FixedJoint>();

    private List<Rigidbody> _rbs = new List<Rigidbody>();
    [SerializeField]private List<int> _membersInt = new List<int>();
    [SerializeField] private List<MoveCapAndRotateGraph> _limbs = new List<MoveCapAndRotateGraph>();

    [SerializeField]
    private List<Vector3> _initialPositions = new List<Vector3>();


    private Rigidbody _selfRB;

    [SerializeField] private bool _useCustomGravity;
    [SerializeField] private float _customgravity = 5f;

    private void Start()
    {
        _selfRB = GetComponent<Rigidbody>();
        _selfRB.useGravity = !_useCustomGravity;

    }

    public void Grab(Rigidbody target, bool ungrab, MoveCapAndRotateGraph reference)
    {

        if (ungrab)
        {
            FixedJoint tJoint = null;
            foreach (FixedJoint item in fixedJoints)
            {
                if (item.connectedBody == target)
                {
                    tJoint = item;
                }
            }
            if (tJoint == null) return;

            fixedJoints.Remove(tJoint);

            Destroy(tJoint); // supression du joint qui a le target

            int index = _rbs.IndexOf(target);

            _rbs.Remove(target);

            _initialPositions.RemoveAt(index);
            _limbs.ToArray()[index].grabOffset = Vector3.zero;
            _limbs.RemoveAt(index);
            _membersInt.RemoveAt(index);

        }
        else
        {
            FixedJoint tFixed = gameObject.AddComponent(typeof(FixedJoint)) as FixedJoint;
            tFixed.connectedBody = target;
            tFixed.anchor = target.position;
            fixedJoints.Add(tFixed); // ajout joint

            _rbs.Add(target);

            _initialPositions.Add(transform.InverseTransformPoint(target.position));
            _limbs.Add(reference);
            _membersInt.Add(reference.member);
        }

        if (fixedJoints.Count == 1)
        {
            _selfRB.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;// on empeche de rotate quand il y a que 1 joint
        }
        if (fixedJoints.Count == 0)
        {
            _selfRB.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        }
        if (fixedJoints.Count > 1)
        {
            _selfRB.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY; // rotate autorisee au dela
        }

    }

    private void FixedUpdate()
    {
        foreach (MoveCapAndRotateGraph limbs in _limbs)
        {
            limbs.TryToMoveConstrained(new Vector3(1, 0, 0), _membersInt);// bouger les mains que sur x
        }


        if (!_useCustomGravity) return;

        _selfRB.AddForce(Vector3.down * _customgravity * _selfRB.mass);
    }
}
