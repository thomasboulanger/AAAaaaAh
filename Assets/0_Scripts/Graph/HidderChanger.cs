using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidderChanger : MonoBehaviour
{
    [SerializeField] MeshFilter _mf;
    [SerializeField] Mesh[] _meshes;

    public void Randomize()
    {
        _mf.mesh = _meshes[Random.Range(0, _meshes.Length)];
    }
}
