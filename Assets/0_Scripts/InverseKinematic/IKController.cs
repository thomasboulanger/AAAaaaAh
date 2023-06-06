using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class IKController : MonoBehaviour
{
    protected Animator animator;

    [SerializeField] private bool ikActive;
    [SerializeField] private bool physics;
    [SerializeField] private bool eyeTracking;
    [SerializeField] private float influence = 1f;
    [SerializeField] private float forcePower = 1000;
    [Space]
    [SerializeField] private Transform headTransform;
    [SerializeField] private Rigidbody headRB;
    
    [Header("List of transforms for member to snap on :\nRight Hand / Left Hand  / Right Foot / Left Foot")]
    public List<LimbController> trackedPositionList;
    public List<Transform> limbsPivot;

    [Header("Place here in the same order as precedent, the list of rigidBody")]
    [SerializeField] private List<Rigidbody> bonesRB;
            
    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.fireEvents = true;
        if (trackedPositionList.Count < 4) Debug.LogError("Error of list length, please provide all the necessary Objs.");
        for (int i = 0; i < trackedPositionList.Count; i++)
            trackedPositionList[i].SetLimbActionRadius(limbsPivot[i]);
    }

    private void FixedUpdate()
    {
        if (!physics) return;
        if (eyeTracking) headRB.transform.LookAt(headTransform);
        
        for (int i = 0; i < trackedPositionList.Count; i++)
        {
            Vector3 direction = trackedPositionList[i].transform.position - bonesRB[i].position;
            bonesRB[i].AddForce(direction * forcePower);
        }
    }
    
    private void OnAnimatorIK(int layerIndex)
    {
        if (!animator && physics) return;
        
        if (trackedPositionList.Count > 0 && ikActive)
        {
            animator.SetLookAtWeight(1);
            animator.SetLookAtPosition(headTransform.position);
            
            SetIK(trackedPositionList[0].transform, AvatarIKGoal.RightHand, influence,0);
            SetIK(trackedPositionList[1].transform, AvatarIKGoal.LeftHand, influence,1);
            SetIK(trackedPositionList[2].transform, AvatarIKGoal.RightFoot, influence,2);
            SetIK(trackedPositionList[3].transform, AvatarIKGoal.LeftFoot, influence,3);
        }
        else
        {
            SetIK(trackedPositionList[0].transform, AvatarIKGoal.RightHand, 0f,0);
            SetIK(trackedPositionList[1].transform, AvatarIKGoal.LeftHand, 0f,1);
            SetIK(trackedPositionList[2].transform, AvatarIKGoal.RightFoot, 0f,2);
            SetIK(trackedPositionList[3].transform, AvatarIKGoal.LeftFoot, 0f,3);
            animator.SetLookAtWeight(0);
        }
        
    }

    private void SetIK(Transform target, AvatarIKGoal ikGoal, float influence, int index)
    {
        if (target is null) return;
        Vector3 angle = index < 2 ? new Vector3(0,90,90) :  new Vector3(0,180,0);
        
        animator.SetIKPositionWeight(ikGoal, influence);
        animator.SetIKRotationWeight(ikGoal, influence);
        if (influence <= 0f) return;
        animator.SetIKPosition(ikGoal, target.position);
        animator.SetIKRotation(ikGoal, target.rotation  * Quaternion.Euler(angle));
    }
}