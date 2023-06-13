using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeColliderGenerator : MonoBehaviour
{
    [SerializeField] private GameObject ropeCollider;
    [SerializeField] private AnimationCurve ropeProfile;
    [SerializeField] private SkinnedMeshRenderer _skinedMR;

    [SerializeField] private float _maxDistanceY = 0.356f;
    [SerializeField] private float _maxDistanceX = 0.2f;

    [SerializeField] private List<Transform> spawnedBoxes = new List<Transform>();


    public void SetRopeProfile(float bend)
    {
        GameObject container = GameObject.Find("RopesColliderContainer");
        if (container == null || !container.activeInHierarchy)
        {
            container = new GameObject("RopesColliderContainer");
            container.name = "RopesColliderContainer";
        }

        GameObject tempGO = new GameObject("tempGO");

        DestroyColliders();

        int boxes = Mathf.RoundToInt(transform.localScale.magnitude)/2;
        

        for (int i = 0; i < boxes; i++)
        {
            Vector3 position = new Vector3(i* _maxDistanceX/boxes,0, ropeProfile.Evaluate((float)i/boxes) * _maxDistanceY* (-bend/100));
            Vector3 positionNext = new Vector3((i + 1) * _maxDistanceX / boxes, 0, ropeProfile.Evaluate((float)(i + 1) / boxes) * _maxDistanceY*(-bend/100));

            tempGO.transform.position = transform.TransformPoint(positionNext);

            GameObject instantiatedCollider = Instantiate(ropeCollider, transform.position, transform.rotation, container.transform);

            instantiatedCollider.transform.position = transform.TransformPoint(position);

            instantiatedCollider.transform.LookAt(tempGO.transform);

            spawnedBoxes.Add(instantiatedCollider.transform);
        }

        DestroyImmediate(tempGO, true);
        _skinedMR.SetBlendShapeWeight(0, bend);
    }

    public void DestroyColliders()
    {
        foreach (Transform item in spawnedBoxes)
        {
            if (spawnedBoxes != null)
            {
                DestroyImmediate(item.gameObject, true);
            }
        }
        spawnedBoxes.Clear();
    }
}
