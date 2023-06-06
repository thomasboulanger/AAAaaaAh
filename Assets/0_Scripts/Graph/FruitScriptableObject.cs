using UnityEngine;

[CreateAssetMenu(fileName = "Fruits", menuName = "ScriptableObjects/FruitScriptableObject", order = 1)]
public class FruitScriptableObject : ScriptableObject
{
    public string fruitName;
    public Material[] materials;
    public Mesh fruitMesh;
}