//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Volume Parent, get called when a child collide with a truelle
/// </summary>
public class UISliderParent : MonoBehaviour
{ 
    [SerializeField] private GameEvent onPlayerUpdateVolume;
    [SerializeField] private Slider visualSlider ;
    public void UpdateVolumeValue(int childIndex)
    {
        onPlayerUpdateVolume.Raise(this, childIndex, null, null);
        visualSlider.value = childIndex * .01f;
    }
}