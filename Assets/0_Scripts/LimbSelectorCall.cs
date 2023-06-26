//Script made by Thomas "elootam" Boulanger for a school project
//see more of my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using UnityEngine;

/// <summary>
/// This script just have an ID that is used when a cursor "click" on it
/// </summary>
public class LimbSelectorCall : MonoBehaviour
{
    public int currentLimbIndex;
    public int currentPlayerIndexAssigned = 5;
    public string outlineNameStr = "";

    private void Start() => currentPlayerIndexAssigned = 5;
}