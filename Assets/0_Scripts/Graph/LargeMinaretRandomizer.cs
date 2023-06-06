using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeMinaretRandomizer : MonoBehaviour
{
    [SerializeField] private Mesh[] _minaretSupports;
    [SerializeField] private Mesh[] _minaretLargeDomes;
    [SerializeField] private Mesh[] _minaretOrnement;

    [SerializeField] private MeshFilter _minaretLargeDomeMr;
    [SerializeField] private MeshFilter _supportMr;
    [SerializeField] private MeshFilter _ornementMr;

    [SerializeField] private GameObject _pioche;
    [SerializeField] private GameObject _piocheSingle;
    [SerializeField] private GameObject _piocheDouble;


    [SerializeField] private bool forcePioche;
    [SerializeField] private bool doublePioche;



    public void Randomize()
    {
        bool _piocheEnabled = false;
        bool _piocheDoubleEnabled = false;
        if (Random.value<=0.1f || forcePioche)
        {
            if (Random.value <= 0.2f || doublePioche) _piocheDoubleEnabled = true;
            else _piocheEnabled = true;
        }


        _pioche.SetActive(_piocheEnabled || _piocheDoubleEnabled);

        _piocheSingle.SetActive(_piocheEnabled);
        _piocheDouble.SetActive(_piocheDoubleEnabled);

        if (Random.value >= 0.5f) _ornementMr.gameObject.SetActive(false);
        else
        {
            _ornementMr.gameObject.SetActive(true);
            _ornementMr.mesh = _minaretOrnement[Random.Range(0, _minaretOrnement.Length)];
        }

        _minaretLargeDomeMr.mesh = _minaretLargeDomes[Random.Range(0, _minaretLargeDomes.Length)];
        if (Random.value >= 0.5f)
        {
            _supportMr.gameObject.SetActive(false);
            _minaretLargeDomeMr.transform.localPosition = new Vector3(4.00066383e-05f, 0.000199999995f, 0.0368000008f);
        }
        else
        {
            _supportMr.gameObject.SetActive(true);
            _supportMr.mesh = _minaretSupports[Random.Range(0, _minaretSupports.Length)];
            _minaretLargeDomeMr.transform.localPosition = new Vector3(4.00066383e-05f, 0.000199999995f, 0.0438f);
        }
    }
}
