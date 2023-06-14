//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// This Script is the logic around fruits when they spawn, when they're grabbed and when they're stored in bag
/// </summary>
public class FruitSelector : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public bool animating;


    [SerializeField] private FruitScriptableObject[] fruitPool;

    private Renderer _renderer;
    private MeshFilter _meshFilter;
    private MeshCollider _collider;
    private Animator _animator;
    private GameObject _shader;
    private int _chosenFruitIndex;
    private bool _triggerOnce;
    private GameObject _currentParent;
    private bool _fruitStored;

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

        _currentParent = null;
    }

    private void Update()
    {
        if (!_triggerOnce) return;

        if (IsFruitGrabbed())
        {
            rb.isKinematic = true;
            _collider.isTrigger = true;

            if (animating) return;
            transform.position = _currentParent.transform.position;
        }
        else
        {
            rb.isKinematic = false;
            _collider.isTrigger = false;
        }
    }

    public void GrabFruit(GameObject parent)
    {
        _animator.SetTrigger("Pickup");
        _currentParent = parent;

        if (_triggerOnce) return;
        _triggerOnce = true;
        _shader.SetActive(false);
    }

    public void ReleaseFruit()
    {
        _currentParent = null;
        _fruitStored = false;
        if (_fruitStored) gameObject.SetActive(true);
    }

    public bool IsFruitGrabbed()
    {
        return _currentParent != null;
    }
}