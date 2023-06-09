using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]

public class PithonsManager : MonoBehaviour
{
    [SerializeField] private GameObject pithonGO;
    [SerializeField] private GameObject ropeGO;

    [SerializeField] private Transform transformList;
    [SerializeField] private float gizmosSize = 0.1f;
    [SerializeField] private float distanceForwardShow = 3f;
    [SerializeField] private float automaticRegenDelay = 0f;
    [SerializeField] private float shapeKeyPower = 100f;
    public bool automaticRegen = false;

    public List<Transform> spawnedItems = new List<Transform>();

    public List<Transform> points = new List<Transform>();





    private float t = 0;

    // Update is called once per frame
    void Update()
    {
        if (Application.isPlaying || !automaticRegen) return;
        if (t < automaticRegenDelay)
        {
            t += Time.deltaTime;
        }
        else
        {
            t = 0;

            Generate();
        }
    }

    public void Generate()
    {
        GetPoints();
        DestroyPreviusAssets();
        SpawnAssets();
    }

    public void DestroyPreviusAssets()
    {
        foreach (Transform item in spawnedItems)
        {
            if (item != null)
            {
                DestroyImmediate(item.gameObject, true);
            }
        }
        spawnedItems.Clear();
    }

    void SpawnAssets()
    {
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 pos = points[i].position;
            Quaternion rotation = points[i].rotation;

            GameObject piton = Instantiate(pithonGO, pos, rotation, transformList);
            spawnedItems.Add(piton.transform);

            if (i == points.Count - 1) continue;

            Vector3 pos1 = points[i + 1].position;
            float distance = Vector3.Distance(pos, pos1);
            float parralel = (Mathf.Abs((Vector3.Dot((pos1 - pos).normalized, transform.up))) * shapeKeyPower) * -1;
            GameObject corde = Instantiate(ropeGO, pos, Quaternion.identity, transformList);
            corde.GetComponentInChildren<SkinnedMeshRenderer>().SetBlendShapeWeight(0, parralel);
            corde.transform.LookAt(pos1);
            corde.transform.Rotate(0, -90, 0, Space.Self);
            corde.transform.localEulerAngles = new Vector3(0, corde.transform.localEulerAngles.y, corde.transform.localEulerAngles.z);
            corde.transform.Rotate(90, 0, 0, Space.Self);

            corde.transform.localScale = new Vector3(distance * 5, 1, 1);
            spawnedItems.Add(corde.transform);
        }
    }

    void GetPoints()
    {
        points.Clear();
        foreach (Transform item in transform)
        {
            points.Add(item);
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 pos = points[i].position;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(pos, gizmosSize);
            //angle piton
            Gizmos.DrawLine(pos, points[i].forward * distanceForwardShow + pos);
            if (i == points.Count - 1) continue;
            Gizmos.color = Color.black;
            Gizmos.DrawLine(pos, points[i + 1].position);
        }
    }
}
