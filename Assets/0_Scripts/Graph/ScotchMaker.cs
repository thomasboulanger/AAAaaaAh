using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScotchMaker : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private List<Transform> scotchTransformPointsListToLink = new List<Transform>();
    [SerializeField] private List<Vector3> posesToLink = new List<Vector3>();
    [SerializeField] private bool useTransformArray;

    public List<Transform> objects = new List<Transform>();

    [SerializeField] private GameObject scotchPF;
    [SerializeField] private GameObject plot;
    [SerializeField] private float plotOffset = -0.2f;

    public List<Material> materials = new List<Material>();

    [SerializeField] private int selectedMat = 0;



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying) return;
        Gizmos.color = Color.black;

        if (useTransformArray)
        {
            foreach (Transform item in scotchTransformPointsListToLink)
            {
                DrawPoint(item.position);
            }
        }
        else
        {
            foreach (Vector3 item in posesToLink)
            {
                DrawPoint(item);
            }
        }

    }

    void DrawPoint(Vector3 item)
    {
        Gizmos.DrawWireCube(transform.TransformPoint(item), Vector3.one * 0.2f);
    }

    public void ActualizeScotch()
    {
        bool loop = posesToLink[posesToLink.Count - 1] == Vector3.zero;

        scotchPF.GetComponent<MeshRenderer>().material = materials[selectedMat];

        ClearObjects();

        SpawnPlot(transform.position);

        scotchPF.transform.localScale = new Vector3(posesToLink[0].magnitude * 500, scotchPF.transform.localScale.y, scotchPF.transform.localScale.z);
        scotchPF.transform.LookAt(transform.TransformPoint(posesToLink[0]), transform.up);
        scotchPF.transform.Rotate(Vector3.forward, 90f);scotchPF.transform.Rotate(Vector3.up, -90f);

        for (int i = 0; i < posesToLink.Count-1; i++) //eclu le dernier de la liste
        {
            Vector3 pos = transform.TransformPoint(posesToLink[i]);
            Vector3 pos1 = transform.TransformPoint(posesToLink[i + 1]);

            SpawnLine(pos,pos1);

            SpawnPlot(pos);
        }

        Vector3 lastpos = transform.TransformPoint(posesToLink[posesToLink.Count - 1]);

        if (!loop)
        {
            SpawnPlot(lastpos);
        }
    }

    void SpawnLine(Vector3 pos, Vector3 pos1)
    {

        GameObject scotchTemp = Instantiate(scotchPF, pos, transform.rotation, transform);
        Transform tempTransform = scotchTemp.transform;
        tempTransform.LookAt(pos1, transform.up);//regarder vers le prochain point
        tempTransform.transform.Rotate(Vector3.forward, 90f); tempTransform.transform.Rotate(Vector3.up, -90f);//bien tourner la merde
        tempTransform.localScale = new Vector3(Vector3.Distance(pos, pos1) * 500, tempTransform.localScale.y, tempTransform.localScale.z);//etirer ligne
        objects.Add(tempTransform);
    }

    void SpawnPlot(Vector3 pos)
    {
        GameObject plotTemp = Instantiate(plot, pos + new Vector3(0, plotOffset, 0), Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(-90, UnityEngine.Random.Range(0, 360), 0)), transform); //spawnPlot
        objects.Add(plotTemp.transform);
    }

    public void ClearObjects()
    {
        foreach (Transform item in objects)
        {
            try
            {
                DestroyImmediate(item.gameObject, true);
            }
            catch
            {
                continue;
            }
            
        }
        objects.Clear();
    }
}
