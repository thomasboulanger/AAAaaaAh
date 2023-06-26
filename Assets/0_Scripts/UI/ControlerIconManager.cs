using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ControlerIconManager : MonoBehaviour
{
    [SerializeField] private TextMeshPro playerIDText;
    [SerializeField] private MeshRenderer selfMeshRenderer;
    [SerializeField] private JoystickManager joystickPrefab;
    [SerializeField] private Transform spawnAnchorL;
    [SerializeField] private Transform spawnAnchorR;
    [SerializeField] private int playerID = -1;
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private List<SpriteRenderer> _fvxGrab = new();
    
    private Material _spriteMat;
    private JoystickManager _joystickL;
    private JoystickManager _joystickR;
    private GameObject[] _limbBoxArray;
    private bool _triggerOnce;

    private void Start() => Init();

    void Init()
    {
        foreach (ShockwaveAnimatorControler item in FindObjectsOfType<ShockwaveAnimatorControler>())
            _fvxGrab.Add(item.transform.GetChild(0).GetComponent<SpriteRenderer>());
        
        SpriteRenderer tempSR = _fvxGrab[1];
        SpriteRenderer tempSR4 = _fvxGrab[2];

        _fvxGrab[1] = tempSR4;
        _fvxGrab[2] = tempSR;

        selfMeshRenderer.sharedMaterial = new Material(selfMeshRenderer.sharedMaterial);
        _spriteMat = selfMeshRenderer.sharedMaterial;
        _triggerOnce = false;

        playerIDText.text = (playerID + 1).ToString();

        _limbBoxArray = GameObject.FindGameObjectsWithTag("LimbSelectorBox");

        foreach (GameObject element in _limbBoxArray)
            ResetOutline(element, Color.white);
    }

    private void Update()
    {
        if (GameManager.UICanvaState != GameManager.UIStateEnum.Play || _triggerOnce) return;
        _triggerOnce = true;
        StartCoroutine(coroutine());
    }

    IEnumerator coroutine()
    {
        yield return new WaitForSeconds(2);
        SpawnJoysticks(spawnAnchorR, true);
        SpawnJoysticks(spawnAnchorL, false);
    }

    private void SpawnJoysticks(Transform anchorTransform, bool isRightJoystick)
    {
        JoystickManager joystick;
        joystick = Instantiate(joystickPrefab, anchorTransform.position, Quaternion.identity);
        joystick.name = "Joystick P" + (playerID + 1) + " " + (isRightJoystick ? "R" : "L");
        joystick.controllerRef = this;
        joystick.SetIDs(playerID, isRightJoystick ? 0 : 1);

        if (isRightJoystick) _joystickR = joystick;
        else _joystickL = joystick;
    }

    public void SetPlayerColor(Color col, JoystickManager sender)
    {
        _spriteMat.SetColor("_Color", col);

        if (sender == _joystickL)
            _joystickR.SetPlayerColor(col, true);
        else
            _joystickL.SetPlayerColor(col, true);

        foreach (GameObject element in _limbBoxArray)
        {
            if (element.GetComponent<LimbSelectorCall>().currentPlayerIndexAssigned == playerID)
            {
                element.GetComponent<MeshRenderer>().material.SetColor("_BCTint", col);
                outlineMaterial.SetColor(element.GetComponent<LimbSelectorCall>().outlineNameStr, col);
                //_fvxGrab[element.GetComponent<LimbSelectorCall>().currentLimbIndex].color = col;
            }
        }
    }

    public void ResetOutline(GameObject objToReplace, Color color)
    {
        outlineMaterial.SetColor(objToReplace.GetComponent<LimbSelectorCall>().outlineNameStr, color);
        //_fvxGrab[objToReplace.GetComponent<LimbSelectorCall>().currentLimbIndex].color = color;
    }


}