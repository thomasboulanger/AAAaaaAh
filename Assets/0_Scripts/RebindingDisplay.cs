//Script made by Thomas "elootam" Boulanger for a school project
//see more on my work:
//https://elootam.itch.io/
//https://www.linkedin.com/in/thomas-boulanger-49379b195/
//You can contact me by email:
//thomas.boulanger.auditeur@lecnam.net

using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Rebinding input script
/// </summary>
public class RebindingDisplay : MonoBehaviour
{
    [SerializeField] private InputActionReference moveL;
    [SerializeField] private InputActionReference moveR;
    [SerializeField] private InputActionReference grabL;
    [SerializeField] private InputActionReference grabR;
    [SerializeField] private PlayerInput playerInput;

    [Header("Text in button")] 
    [SerializeField] private TMP_Text[] bindingDisplayNameText;
    [Header("Place there all buttons and Text used to rebind for each bind for each players")]
    [SerializeField] private GameObject[] startRebindButton;
    [SerializeField] private GameObject[] waitingForInputText;

    private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;

    public void StartRebinding()
    {
        startRebindButton[0].SetActive(false);
        waitingForInputText[0].SetActive(true);

        playerInput.SwitchCurrentActionMap("Player");

        _rebindingOperation = moveL.action.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .WithControlsExcluding("Start")
            .WithControlsExcluding("Menu")
            .OnMatchWaitForAnother(.1f)
            .OnComplete(operation => EndRebinding())
            .Start();
    }

    private void EndRebinding()
    {
        int bindingIndex = moveL.action.GetBindingIndexForControl(moveL.action.controls[0]);
        bindingDisplayNameText[0].text = InputControlPath.ToHumanReadableString(moveL.action.bindings[0].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
        _rebindingOperation.Dispose();
        startRebindButton[0].SetActive(true);
        waitingForInputText[0].SetActive(false);

        playerInput.SwitchCurrentActionMap("UI");
    }
}