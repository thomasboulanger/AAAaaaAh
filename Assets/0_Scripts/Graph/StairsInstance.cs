using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsInstance : MonoBehaviour
{
    [SerializeField] private Vector3 _offsetPos;
    [Range(0.1f, 2)]
    [SerializeField] private float _lengh = 1;
    [SerializeField] private List<Mesh> _meshes = new List<Mesh>();
    [SerializeField] private Material _baseMat;
    [SerializeField] private bool moveDynamic;
    [SerializeField] private float _scaleMultiplier = 100f;

    [HideInInspector] public List<GameObject> toDestroy = new List<GameObject>();

    private Vector3 _tempoffset = new Vector3(0, 1, 0);
    private List<Vector3> _posArray = new List<Vector3>();
    public List<GameObject> steps = new List<GameObject>();

    Vector3 GetDirectionVector()
    {
        return ((transform.position + _offsetPos) - transform.position).normalized;
    }

    public void PlaceSteps(bool onvalidate)
    {
        RemoveSteps(onvalidate);
        GenerateArray(false);
        SpawnSteps();
    }

    void SpawnSteps()
    {
        foreach (Vector3 item in _posArray)
        {
            GameObject go = new GameObject();
            go.transform.position = item;
            go.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 180, 0) * Random.Range(0, 1));
            go.transform.localScale = transform.localScale * _scaleMultiplier;
            go.name = "marche";
            MeshRenderer _mr = go.AddComponent<MeshRenderer>();
            _mr.sharedMaterial = _baseMat;
            MeshFilter _mf = go.AddComponent<MeshFilter>();
            _mf.sharedMesh = _meshes[Random.Range(0, _meshes.Count - 1)];
            steps.Add(go);

            go.transform.parent = transform;
        }
    }

    public void RemoveSteps(bool onvalidate)
    {
        foreach (GameObject item in steps)
        {
            if (onvalidate) toDestroy.Add(item);


            else {
                DestroyImmediate(item, true);
                if (toDestroy.Count>0)
                {
                    foreach (GameObject forgottenSteps in toDestroy)
                    {
                        DestroyImmediate(forgottenSteps, true);
                    }
                    toDestroy.Clear();
                }
            } 

        }
        steps.Clear();
    }

    void GenerateArray(bool gizmo)
    {
        _posArray.Clear();
        Vector3 pos = transform.position + _offsetPos;

        if (gizmo)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(pos, 0.2f);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, pos);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_tempoffset + transform.position, _tempoffset + transform.position + GetDirectionVector());
        }


        float distance = Vector3.Distance(transform.position, pos);
        Vector3 directionVector = GetDirectionVector();
        if (gizmo) Gizmos.color = Color.white;
        for (float split = _lengh; split < distance; split += _lengh)
        {
            Vector3 localPos = (split * directionVector) + transform.position;
            if (gizmo)
            {
                Gizmos.DrawWireSphere(localPos, 0.1f);
                Gizmos.DrawWireSphere(transform.position, 0.1f);
            }

            _posArray.Add(localPos);
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isEditor)
        {
            GenerateArray(true);
        }
    }
    /* infamie qui fait des erreurs
    private void OnValidate()
    {
        if (!moveDynamic) return;
        PlaceSteps(true);
    }*/
}
