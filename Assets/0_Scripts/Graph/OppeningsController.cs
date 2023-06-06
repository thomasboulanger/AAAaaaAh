using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OppeningsController : MonoBehaviour
{
    [SerializeField] private Transform vollet;
    [SerializeField] private MeshRenderer volletMr;
    [SerializeField] private MeshRenderer porteMr;
    [SerializeField] private Transform porte;
    [SerializeField] private Vector2 minMax = new Vector2(0,-117);

    //previewOnly
    [SerializeField] private Transform previewMin;
    [SerializeField] private Transform previewMax;
    [SerializeField] private Transform min;
    [SerializeField] private Transform max;

    [SerializeField] private GameObject disco;

    [SerializeField] private Material[] windowMat;
    [SerializeField] private Material[] vfxMats;
    [SerializeField] private Material trimMat;

    [SerializeField] private bool forceDoor;

    private bool isDoor;

    public void Randomize()
    {
        if (Random.value >= 0.5f) transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        if (Random.value >= 0.5f &&!forceDoor)//volletouporte
        {
            isDoor = false;
            porte.gameObject.SetActive(false);vollet.gameObject.SetActive(true);volletMr.enabled = true;
            if (Random.value >= 0.2f)//ouvert
            {
                vollet.localRotation = Quaternion.Euler(new Vector3(Random.Range(minMax.x,minMax.y), vollet.localRotation.eulerAngles.y, vollet.localRotation.eulerAngles.z));
            }
            else
            {
                vollet.localRotation = Quaternion.Euler(new Vector3(0, 45, 0));//vollet ferme
            }
        }
        else
        {
            isDoor = true;
            porte.gameObject.SetActive(true); vollet.gameObject.SetActive(false); volletMr.enabled = false;
        }


    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying || forceDoor || isDoor) return;
        previewMin.localRotation = Quaternion.Euler(minMax.x, 45, 0);
        previewMax.localRotation = Quaternion.Euler(minMax.y, 45, 0);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(min.position, 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(max.position, 0.1f);
        Gizmos.color = Color.black;
        Gizmos.DrawLine(min.position, max.position);
    }

    public void Disco(bool status, int state)
    {
        if (isDoor)
        {
            porteMr.sharedMaterial = status? windowMat[state]:trimMat;
        }
        else
        {
            Material[] mats = volletMr.sharedMaterials;
            mats[1] = windowMat[state];
            volletMr.sharedMaterials = mats;
        }
        //faire truc pour les portes

        disco.SetActive(status);

        if (!status) return;
        disco.GetComponent<MeshRenderer>().sharedMaterial = vfxMats[state-1];//0 c lights base 1 c disco
    }
}
