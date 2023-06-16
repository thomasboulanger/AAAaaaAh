//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net
//Bah du coup c fau y  aussi moi ptn + ratio en fait stop écrire votre nom partout pls

using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

/// <summary>
/// This Script is the logic around fruits when they spawn, when they're grabbed and when they're stored in bag
/// </summary>
public class FruitSelector : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public bool animating;
    [HideInInspector] public GameObject currentParent;

    [SerializeField] private FruitScriptableObject[] fruitPool;

    private Renderer _renderer;
    private MeshFilter _meshFilter;
    private MeshCollider _collider;
    private Animator _animator;
    private GameObject _shader;
    private int _chosenFruitIndex;
    private bool _triggerOnce;
    private bool _fruitStored;

    private PoubelleCollider _pcRef;
    private bool canBeStored;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _renderer = transform.GetChild(0).GetComponent<Renderer>();
        _meshFilter = transform.GetChild(0).GetComponent<MeshFilter>();
        _collider = transform.GetChild(0).GetComponent<MeshCollider>();
        _shader = transform.GetChild(1).gameObject;
        _animator = GetComponent<Animator>();

        _chosenFruitIndex = Random.Range(0, fruitPool.Length - 1);
        _meshFilter.sharedMesh = fruitPool[_chosenFruitIndex].fruitMesh;
        _collider.sharedMesh = fruitPool[_chosenFruitIndex].fruitMesh;
        _renderer.materials = fruitPool[_chosenFruitIndex].materials;

        currentParent = null;
    }

    private void Update()
    {
        if (!_triggerOnce || _fruitStored) return;

        if (IsFruitGrabbed())
        {
            rb.isKinematic = true;
            _collider.isTrigger = true;

            if (animating) return;
            transform.position = currentParent.transform.position;
        }
        else
        {
            rb.isKinematic = false;
            _collider.isTrigger = false;
        }
    }

    public void GrabFruit(GameObject parent)
    {
        currentParent = parent;

        DisableAnim();
    }

    public void ReleaseFruit(bool gotOut)
    {
        currentParent = null;

        if (canBeStored && _pcRef != null && !gotOut)
        {
            _pcRef.Store(this);
            _fruitStored = true;
        }
        else
        {
            _fruitStored = false;
        }


        DisableAnim();
    }

    public bool IsFruitGrabbed()
    {
        return currentParent != null;
    }

    public void CanBeStrored(PoubelleCollider pcReference, bool status)
    {
        if (_pcRef == null) _pcRef = pcReference;
        canBeStored = status;
    }

    public void FakePossessesion(PoubelleCollider pcReference)
    {
        canBeStored = true;
        _pcRef = pcReference;

        DisableAnim();
    }

    void DisableAnim()
    {
        _animator.SetTrigger("Pickup");
        if (_triggerOnce) return;
        _triggerOnce = true;
        _shader.SetActive(false);
    }
}